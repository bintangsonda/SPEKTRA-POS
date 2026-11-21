/* ============================================================
   Bluetooth Thermal Printer (ESC/POS over BLE)
   Target: VSC TM-58V (58mm, ESC/POS, BLE 4.0)
   ============================================================

   Catatan penting:
   - Web Bluetooth HANYA jalan di HTTPS (atau localhost) + Chrome/Edge Android.
   - JANGAN pair printer di pengaturan Bluetooth HP (SPP). Cukup printer nyala,
     koneksi BLE dilakukan langsung oleh browser.
   - Klik pertama: muncul dialog pilih printer. Perangkat diingat selama
     halaman belum di-refresh. Setelah refresh, pilih ulang (kebijakan keamanan browser).
   ============================================================ */

(function () {
    'use strict';

    var PRINTER = {
        device: null,          // BluetoothDevice terpilih
        characteristic: null,  // characteristic untuk menulis data
        chunkSize: 20,         // aman untuk hampir semua printer 58mm
        chunkDelayMs: 10,
        // Service/characteristic yang umum dipakai printer thermal 58mm (inkl. TM-58V)
        optionalServices: [
            '0000ff00-0000-1000-8000-00805f9b34fb', // FF00 (paling umum, write di FF02)
            '000018f0-0000-1000-8000-00805f9b34fb', // 18F0
            '49535343-fe7d-4ae5-8fa9-9fafd205e455', // UART-like service
            'e7810c71-f73e-4f75-8203-cb1f1d24cce4',
            '0000ffff-0000-1000-8000-00805f9b34fb'
        ]
    };

    /* ================= KONEKSI ================= */

    function isSupported() {
        return !!navigator.bluetooth;
    }

    function connectPrinter() {
        if (!isSupported()) {
            return Promise.reject(new Error('Browser ini tidak mendukung Web Bluetooth. Gunakan Chrome/Edge di Android.'));
        }

        return navigator.bluetooth.requestDevice({
            acceptAllDevices: true,
            optionalServices: PRINTER.optionalServices
        }).then(function (device) {
            PRINTER.device = device;
            device.addEventListener('gattserverdisconnected', function () {
                PRINTER.characteristic = null;
            });
            return device.gatt.connect();
        }).then(function (server) {
            // Cari characteristic yang bisa ditulis, cek semua service yang diizinkan
            return server.getPrimaryServices().then(function (services) {
                var queue = Promise.resolve(null);
                var found = null;

                services.forEach(function (service) {
                    queue = queue.then(function (result) {
                        if (result) return result; // sudah ketemu
                        return service.getCharacteristics().then(function (chars) {
                            for (var i = 0; i < chars.length; i++) {
                                var p = chars[i].properties;
                                if (p.write || p.writeWithoutResponse) {
                                    found = chars[i];
                                    return found;
                                }
                            }
                            return null;
                        }).catch(function () { return null; });
                    });
                });

                return queue.then(function (char) {
                    if (!char) throw new Error('Characteristic printer tidak ditemukan. Pastikan printer tidak sedang ter-pair di Bluetooth HP.');
                    PRINTER.characteristic = char;
                    return char;
                });
            });
        });
    }

    function ensureConnected() {
        if (PRINTER.characteristic) return Promise.resolve(PRINTER.characteristic);

        if (PRINTER.device && PRINTER.device.gatt && PRINTER.device.gatt.connected) {
            // device masih connected tapi characteristic hilang -> cari ulang
            return PRINTER.device.gatt.getPrimaryServices().then(function () {
                return connectToExistingDevice();
            });
        }

        if (PRINTER.device) {
            // sudah pernah dipilih, reconnect tanpa dialog
            return PRINTER.device.gatt.connect().then(function () {
                return connectToExistingDevice();
            }).catch(function () {
                return connectPrinter(); // gagal reconnect, minta pilih ulang
            });
        }

        return connectPrinter();
    }

    function connectToExistingDevice() {
        var server = PRINTER.device.gatt;
        return server.getPrimaryServices().then(function (services) {
            var queue = Promise.resolve(null);
            services.forEach(function (service) {
                queue = queue.then(function (result) {
                    if (result) return result;
                    return service.getCharacteristics().then(function (chars) {
                        for (var i = 0; i < chars.length; i++) {
                            if (chars[i].properties.write || chars[i].properties.writeWithoutResponse) {
                                PRINTER.characteristic = chars[i];
                                return chars[i];
                            }
                        }
                        return null;
                    }).catch(function () { return null; });
                });
            });
            return queue.then(function (char) {
                if (!char) throw new Error('Characteristic printer tidak ditemukan.');
                return char;
            });
        });
    }

    /* ================= KIRIM DATA ================= */

    function writeBytes(bytes) {
        var char = PRINTER.characteristic;
        var useResponse = char.properties.write;

        var i = 0;
        function next() {
            if (i >= bytes.length) return Promise.resolve();
            var slice = bytes.subarray(i, Math.min(i + PRINTER.chunkSize, bytes.length));
            i += PRINTER.chunkSize;
            var p = useResponse
                ? char.writeValue(slice)
                : char.writeValueWithoutResponse(slice);
            return p.then(function () {
                return new Promise(function (r) { setTimeout(r, PRINTER.chunkDelayMs); });
            }).then(next);
        }
        return next();
    }

    /* ================= BANGUN STRUK (ESC/POS) ================= */

    // Kertas 58mm: 32 karakter per baris (font A 12x24)
    var WIDTH = 32;

    function esc() { return [0x1B]; }
    function gs() { return [0x1D]; }

    function bytesFromString(str) {
        var out = [];
        for (var i = 0; i < str.length; i++) {
            var c = str.charCodeAt(i);
            if (c > 126) {
                // karakter non-ASCII sederhanakan agar tidak kacau di printer
                var map = {
                    0x2013: '-', 0x2014: '-', 0x2018: "'", 0x2019: "'",
                    0x201C: '"', 0x201D: '"', 0x2026: '...', 0x00A0: ' ',
                    0x2192: '->', 0x00D7: 'x'
                };
                out.push((map[c] || '?').charCodeAt(0));
            } else {
                out.push(c);
            }
        }
        return out;
    }

    function line(left, right, width) {
        width = width || WIDTH;
        left = left || '';
        right = (right !== undefined && right !== null) ? String(right) : '';
        var space = width - left.length - right.length;
        if (space < 1) {
            // terlalu panjang: potong kiri
            left = left.substring(0, Math.max(0, width - right.length - 1));
            space = width - left.length - right.length;
        }
        return left + new Array(space + 1).join(' ') + right;
    }

    function center(text, width) {
        width = width || WIDTH;
        text = String(text);
        if (text.length >= width) return text;
        var pad = Math.floor((width - text.length) / 2);
        return new Array(pad + 1).join(' ') + text;
    }

    function wrapText(text, width) {
        var words = String(text).split(' ');
        var lines = [], current = '';
        words.forEach(function (w) {
            var test = current ? current + ' ' + w : w;
            if (test.length > width) {
                if (current) lines.push(current);
                // kata kepanjangan sendiri dipotong paksa
                while (w.length > width) {
                    lines.push(w.substring(0, width));
                    w = w.substring(width);
                }
                current = w;
            } else {
                current = test;
            }
        });
        if (current) lines.push(current);
        return lines;
    }

    function formatMoney(num) {
        var n = Math.round(Number(num) || 0);
        return n.toLocaleString('id-ID');
    }

    function buildReceipt(data) {
        var bytes = [];
        function push() {
            for (var i = 0; i < arguments.length; i++) {
                var a = arguments[i];
                if (Array.isArray(a)) bytes.push.apply(bytes, a);
                else if (typeof a === 'string') bytes.push.apply(bytes, bytesFromString(a));
                else bytes.push(a & 0xFF);
            }
        }

        // INIT printer + reset
        push(esc(), 0x40);

        // Header
        push(esc(), 0x61, 0x01);                       // align center
        push(esc(), 0x21, 0x30);                       // double height+width
        push(data.header + '\n');
        push(esc(), 0x21, 0x00);                       // normal size
        push('--------------------------------\n');
        push(data.transCode + '\n');
        push(data.dateTime + '\n');
        if (data.customer) {
            wrapText('Pelanggan: ' + data.customer, WIDTH).forEach(function (l) {
                push(l + '\n');
            });
        }
        push('--------------------------------\n');
        push(esc(), 0x61, 0x00);                       // align left

        // Items
        data.items.forEach(function (item) {
            var qty = (item.qty || 1);
            var price = item.price || 0;
            var discount = item.discount || 0;

            var nameLines = wrapText(item.name || '-', WIDTH);
            nameLines.forEach(function (l) { push(l + '\n'); });

            push(line('  ' + qty + ' x ' + formatMoney(price), formatMoney(qty * price)) + '\n');

            if (discount > 0) {
                push(line('  Diskon', '-' + formatMoney(discount)) + '\n');
            }

            // remark & varian
            if (item.remark) {
                wrapText('  (*' + item.remark + ')', WIDTH).forEach(function (l) {
                    push(l + '\n');
                });
            }
            if (item.variants && item.variants.length) {
                item.variants.forEach(function (v) {
                    wrapText('  + ' + v.value, WIDTH).forEach(function (l) {
                        push(l + '\n');
                    });
                });
            }

            // tipe pesanan & bungkus/makan di sini
            var salesText = '';
            if (item.salesType === 'on') salesText = 'Bungkus';
            else if (item.salesType === 'off') salesText = 'Makan di sini';
            var typeText = [item.orderType, salesText].filter(Boolean).join(' | ');
            if (typeText) {
                wrapText('  [' + typeText + ']', WIDTH).forEach(function (l) {
                    push(l + '\n');
                });
            }
        });

        push('--------------------------------\n');

        // Total (bold)
        push(esc(), 0x45, 0x01);                       // bold on
        push(line('TOTAL', 'Rp ' + formatMoney(data.total)) + '\n');
        push(esc(), 0x45, 0x00);                       // bold off

        push('--------------------------------\n');

        // Footer
        push(esc(), 0x61, 0x01);                       // center
        push(data.footer + '\n');

        // Feed + potong kertas
        push(esc(), 0x64, 0x04);                       // print & feed 4 baris
        push(gs(), 0x56, 0x42, 0x00);                  // GS V B 0 : potong kertas

        return new Uint8Array(bytes);
    }

    /* ================= API PUBLIK ================= */

    // Dipanggil dari tombol Print
    window.PrintThermalReceipt = function () {
        if (!window.isSecureContext) {
            alert('Print Bluetooth butuh HTTPS. Buka POS lewat alamat https://...');
            return;
        }
        if (!isSupported()) {
            alert('Browser tidak mendukung Web Bluetooth. Gunakan Chrome atau Edge di HP Android.');
            return;
        }
        if (typeof DataMenu === 'undefined' || !DataMenu.length) {
            alert('Order masih kosong');
            return;
        }

        var transCode = '';
        try { transCode = $('.TransactionCode').first().text().trim(); } catch (e) { }

        var total = 0;
        try { total = RemoveAllComma($('.Total').first().text().trim()) || 0; } catch (e) {
            DataMenu.forEach(function (i) { total += i.qty * i.price; });
        }

        var customer = '';
        try { customer = $('.CustomerName').first().text().trim(); } catch (e) { }
        if (customer === '-') customer = '';

        var now = new Date();
        var data = {
            header: 'RIBWRAP',
            transCode: 'No: ' + transCode,
            dateTime: now.toLocaleString('id-ID'),
            customer: customer,
            items: DataMenu,
            total: total,
            footer: '~ Terima Kasih ~'
        };

        ensureConnected()
            .then(function () {
                var bytes = buildReceipt(data);
                return writeBytes(bytes);
            })
            .then(function () {
                if (typeof SweetAlertSuccess === 'function') {
                    SweetAlertSuccess('Success', 'Struk terkirim ke printer');
                }
            })
            .catch(function (err) {
                PRINTER.characteristic = null;
                if (err && err.name === 'NotFoundError') {
                    // user menutup dialog pilih printer -> diam saja
                    return;
                }
                if (typeof SweetAlertError === 'function') {
                    SweetAlertError('Print Gagal', (err && err.message) ? err.message : String(err));
                } else {
                    alert('Print gagal: ' + ((err && err.message) || err));
                }
            });
    };

    // Status koneksi (bisa dipakai untuk indikator di UI)
    window.IsPrinterConnected = function () {
        return !!(PRINTER.characteristic && PRINTER.device && PRINTER.device.gatt.connected);
    };

})();
