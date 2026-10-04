function NoAccess() {
    alert("You dont have access to create. Please contact your administrator.");
    return false;
}

function RemoveAllComma(x) {
    
    if (x != null && x !== 'undefined') {
        return x.replace(/,/g, "");
    }
}

// Date Format MM/dd/yyyy
function AddYear(date, value) {
    var eDate = new Date(date);
    var eYear = parseFloat(eDate.getFullYear()) + parseFloat(value);
    var eFullDate = new Date((eDate.getMonth() + 1).toString() + "/" + eDate.getDate().toString() + "/" + eYear.toString());
}

function isNumeric(str) {
    if (typeof str != "string") return false;
    return !isNaN(str) && !isNaN(parseFloat(str));
}
function InputNumberMinus(e) {
    $("#" + e.id).keyup(function (event) {
        //if (event.which >= 37 && event.which <= 40) return;
        if (event.which >= 37 && event.which <= 40 && event.which != 46 && event.which != 45 && event.which != 46 &&
            !(event.which >= 48 && event.which <= 57)) return;
        $(this).val(function (index, value) {
            return value
                // Keep only digits and decimal points:
                .replace(/[^\d.-]/g, "")
                // Remove duplicated decimal point, if one exists:
                .replace(/^(\d*\.)(.*)\.(.*)$/, '$1$2$3')
                // Keep only two digits past the decimal point:
                .replace(/\.(\d{2})\d+/, '.$1')
                // Add thousands separators:
                .replace(/\B(?=(\d{3})+(?!\d))/g, ",")
        });
    });
}



//Untuk input hanya angka di event keypress
function isNumber(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (
        (charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
        (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function AddCommaDecimal(numb) {

    if (numb === null || numb === undefined || numb === "") {
        return "-";
    }

    let num = parseFloat(numb.toString().replace(/,/g, ""));

    if (isNaN(num)) return "-";

    // Jika bilangan bulat → tanpa decimal
    if (Number.isInteger(num)) {
        return num.toLocaleString("en-US");
    }

    // Jika ada pecahan → max 2 decimal, tanpa .00
    return num.toLocaleString("en-US", {
        minimumFractionDigits: 0,
        maximumFractionDigits: 2
    });
}
function InputNumber(el) {
    let value = el.value;

    value = value
        .replace(/[^\d.-]/g, "")
        .replace(/^(\d*\.)(.*)\.(.*)$/, '$1$2$3')
        .replace(/\.(\d{6})\d+/, '.$1')
        .replace(/\B(?=(\d{3})+(?!\d))/g, ",");

    el.value = value;
}

function OnlyNumber(e) {
    $("#" + e.id).keyup(function (event) {
        if (event.which >= 37 && event.which <= 40) return;
        $(this).val(function (index, value) {
            return value.replace(/\D/g, '');
        });
    });
}

function RemoveAllComma(value) {
    if (value === null || value === undefined || value === "") {
        return 0;
    }

    return value.toString().replace(/,/g, "");
}

function FormatGrandTotal(el) {

    // ambil nilai input
    let raw = el.value;

    if (!raw) {
        el.value = '';
        return;
    }

    // bersihkan dulu (angka & koma)
    raw = raw.replace(/,/g, '');

    // panggil fungsi lama
    let formatted = AddCommaDecimal(raw);

    // set balik ke input
    if (formatted !== "-") {
        el.value = formatted;
    }
}


