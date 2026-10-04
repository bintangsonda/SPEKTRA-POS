


/**
*  Form Wizard (Refactored jQuery version)
*  Clean, modular, and reusable
*/

'use strict';

$(function () {
    if ($("#FormAction").val() == 2) {
        
        //Add By Renaldi 2 August 2024
        if ($('#PageEdit').val() == 0) {
            $('input, button, select, textarea, fieldset, optgroup, option').prop('disabled', true);
            $('#btnClose').prop('disabled', false);
        }
        $('#cboGender').val($('#Genders').val()).select2();
        $('#cboContactGender').val($('#GendersFamily').val()).select2();
        if (parseInt($('#AppStatus').val()) == 0) {
            Swal.fire("This Customer data waiting to be Approved!");
            //SweetAlertCustom("Info", "This Customer data waiting to be Approved!","question");

        }
        if ($('#MaritalStatus').val() == 1) {
            $('input[name="inlineRadioOptions"][value="kawinRadio"]').prop('checked', true).trigger('change');
        }
        else if ($('#MaritalStatus').val() == 2)
        {
            $('input[name="inlineRadioOptions"][value="blmKawinRadio"]').prop('checked', true).trigger('change');
        }
        else {
            $('input[name="inlineRadioOptions"][value="ceraiRadio"]').prop('checked', true).trigger('change');
        }
        //$('input[name="inlineRadioOptions"][value="B"]').prop('checked', true);
        //MaritalStatus: $('input[name="inlineRadioOptions"]:checked').val() == "kawinRadio" ? 1 : $('input[name="inlineRadioOptions"]:checked').val() == "blmKawinRadio" ? 2 : 3,

        
    }



    $("#IDNo").on("change focusout", function () {
        const formAction = $("#FormAction").val();
        let idNo = $("#IDNo").val().replace(/[\.-]/g, "");
        let msg = '';
        let errorCount = 0;

      

        // 🧩 Step 2: Validasi kecocokan dengan tanggal lahir
        const tempDOB = $("#LesseeDOB").val();
        if (tempDOB) {
            const tempDOBArr = tempDOB.split('/');
            const monthMap = {
                "Jan": "01", "Feb": "02", "Mar": "03", "Apr": "04",
                "May": "05", "Jun": "06", "Jul": "07", "Aug": "08",
                "Sep": "09", "Oct": "10", "Nov": "11", "Dec": "12"
            };
            const day = tempDOBArr[0];
            const month = monthMap[tempDOBArr[1]];
            const year = tempDOBArr[2].substring(2);
            const DOBdigits = day + month + year;

            const part2ID = idNo.substring(6, 12);
            const part3ID = idNo.substring(12, 16);

            if (part2ID !== DOBdigits || part3ID === "0000") {
                Swal.fire({
                    icon: 'info',
                    title: "Invalid ID Format",
                    text: "The entered ID number does not match the standard ID numbering format. Please check it again.",
                    confirmButtonText: 'OK',
                    customClass: { confirmButton: 'order-2' }
                });
                return false;
            }
        }

        // 🧩 Step 3: Cek duplikasi IDNo ke server (hanya untuk create mode)
        if (formAction == 1) {
            $.ajax({
                type: "POST",
                url: '/LeaseTable/Customer/CekIDNo', // Ganti kalau pakai @Url.Action di Razor
                dataType: 'json',
                data: { IDNo: idNo },
                success: function (respond) {
                    if (respond == 1) {
                        Swal.fire({
                            icon: 'info',
                            title: "Save Can't Be Process!!!",
                            text: "Customer with this IDNo already exists. Please check your data to avoid duplicates.",
                            confirmButtonText: 'OK',
                            customClass: { confirmButton: 'order-2' }
                        });
                        $("#IDNo").val('');
                    }
                },
                error: function () {
                    alert('Something is wrong, please contact your administrator.');
                }
            });
        }
    });
    // #region OnKeyUp Number Only
    $('#GrossIncomePerYear,#LegalRT,#LegalRW,#ResidenceRT,#ResidenceRW,#BillingRT,#BillingRW,#AreaCode,#WorkZipCode,#txtBillingZipCode,#LegalPhone1,#LegalPhone2,#LegalZipCode,#ResidenceZipCode,#Handphone1,#Handphone2,#Tahun,#Bulan,#FamilyPhoneNo,#FamilyHandPhoneNo,#WorkingPeriodPerYear,#JumlahTanggungan')
        .on('keyup', function () {
            var value = $(this).val();
            value = value.replace(/\D/g, ''); // Hapus karakter non-angka
            $(this).val(value);
        });
    // #endregion


    // #region Money Format
    $('#GrossIncomePerYear, #GrossLivingCostPerYear, #CreditLimit')
        .on('focusin', function () {
            $(this).val(RemoveAllComma($(this).val()));
        })
        .on('focusout', function () {
            $(this).val(AddCommaDecimal($(this).val()));
        });
    // #endregion
    // =========================
    // 🔧 1. Initialization Setup
    // =========================

    if (window.Helpers && typeof window.Helpers.initCustomOptionCheck === 'function') {
        window.Helpers.initCustomOptionCheck();
    }

    $('.select2').select2({
        placeholder: "",
        allowClear: true,
        width: 'resolve'
    });

    if ($('.flatpickr').length) {
        $('.flatpickr').flatpickr();
    }

    $('input[name="inlineRadioOptions"]').on('change', function () {
        toggleSpouseFields();
    });

    initPhoneMask();
    initTagify();
    initToggleSections();
    initAddressDropdowns();
    initBOBranchDropdown();
    initWizardForm();
    toggleSpouseFields();

});

// 🔹 Ambil semua field pasangan
const $spouseFields = $([
    'input[name="SpouseName"]',
    'input[name="SpouseIDNo"]',
    'input[name="SpouseDOB2"]',
    '#cboPisahHarta'
].join(',')).closest('.form-control-validation');

// 🔹 Fungsi toggle field pasangan
function toggleSpouseFields() {
    const selected = $('input[name="inlineRadioOptions"]:checked').val();

    if (selected === 'kawinRadio') {
        $spouseFields.show();
    } else {
        $spouseFields.hide();
        // Kosongkan field biar tidak terkirim
        $spouseFields.find('input').val('');
    }
}

// Jalankan saat halaman pertama kali dimuat

// Jalankan tiap kali radio berubah

// =============================
// 📞 2. Phone Input Formatter
// =============================
function initPhoneMask() {
    const $phoneMask = $('.contact-number-mask');
    if (!$phoneMask.length) return;

    $phoneMask.on('input', function () {
        const cleanValue = $(this).val().replace(/\D/g, '');
        $(this).val(
            formatGeneral(cleanValue, {
                blocks: [3, 3, 4],
                delimiters: [' ', ' ']
            })
        );
    });

    if (typeof registerCursorTracker === 'function') {
        registerCursorTracker({ input: $phoneMask[0], delimiter: ' ' });
    }
}


// =============================
// 🏷️ 3. Tagify Initialization
// =============================
function initTagify() {
    const $plFurnishingDetails = $('#plFurnishingDetails');
    if (!$plFurnishingDetails.length) return;

    const furnishingList = [
        'Fridge', 'TV', 'AC', 'WiFi', 'RO',
        'Washing Machine', 'Sofa', 'Bed',
        'Dining Table', 'Microwave', 'Cupboard'
    ];

    new Tagify($plFurnishingDetails[0], {
        whitelist: furnishingList,
        maxTags: 10,
        dropdown: {
            maxItems: 20,
            classname: 'tags-inline',
            enabled: 0,
            closeOnSelect: false
        }
    });
}


// ==================================
// 🧩 4. Show/Hide Sections (Dom/Bill)
// ==================================
function initToggleSections() {
    $('#checkDom').on('change', function () {
        $('.domAdd').toggleClass('d-none', $(this).is(':checked'));
    }).trigger('change');

    $('#checkBill').on('change', function () {
        $('.billAdd').toggleClass('d-none', $(this).is(':checked'));
    }).trigger('change');
}


// ====================================
// 🏙️ 5. Cascading Address Dropdowns
// ====================================
function setupAddressDropdown(prefix) {
    const $prov = $(`#cbo${prefix}Province`);
    const $city = $(`#cbo${prefix}City`);
    const $kec = $(`#cbo${prefix}Kecamatan`);
    const $kel = $(`#cbo${prefix}Kelurahan`);

    // Province → City
    $prov.on('change', function () {
        const val = $(this).val();
        resetDropdown([$city, $kec, $kel]);
        if (!val) return;

        loadDropdown(window.apiUrls.getCity, { Province: val }, $city);
    });

    // City → Kecamatan
    $city.on('change', function () {
        const val = $(this).val();
        resetDropdown([$kec, $kel]);
        if (!val) return;

        loadDropdown(window.apiUrls.getKecamatan, { City: val }, $kec);
    });

    // Kecamatan → Kelurahan
    $kec.on('change', function () {
        const val = $(this).val();
        resetDropdown([$kel]);
        if (!val) return;

        loadDropdown(window.apiUrls.getKelurahan, { Kecamatan: val }, $kel);
    });
}

function loadDropdown(url, data, $target) {
    $.ajax({
        type: "POST",
        url,
        dataType: 'json',
        data,
        success: function (res) {
            $.each(res, function (_, item) {
                $target.append(`<option value="${item.Value}">${item.Text}</option>`);
            });
        },
        error: function () {
            alert('Terjadi kesalahan saat memuat data. Hubungi administrator.');
        }
    });
}

function resetDropdown(list) {
    list.forEach($el => $el.empty().append('<option value=""></option>'));
}

function initAddressDropdowns() {
    ['Legal', 'Residence', 'Billing', 'Work'].forEach(setupAddressDropdown);
}


// ==================================
// 🏢 6. Branch Office Dropdown
// ==================================
function initBOBranchDropdown() {
    $('#cboBOName').on('change', function () {
        $.ajax({
            type: "POST",
            url: '@Url.Action("GetBoBranch", "Customer", new { Area = "LeaseTable" })',
            dataType: 'json',
            data: { Name: $(this).val() },
            success: function (res) {
                const $branch = $('#cboBOBranch').empty().append('<option value=""></option>');
                $.each(res, function (_, item) {
                    $branch.append(`<option value="${item.Value}">${item.Text}</option>`);
                });
            },
            error: function () {
                alert('Gagal memuat data cabang BO.');
            }
        });
    });
}


// =====================================
// 🧭 7. Wizard Form + Validation Steps
// =====================================
function initWizardForm() {
    const $wizard = $('#wizard-property-listing');
    if (!$wizard.length) return;

    const $form = $wizard.find('#wizard-property-listing-form');
    const stepElems = [
        '#personal-details', '#property-details',
        '#property-features', '#property-area',
        '#price-details', '#others-details'
    ].map(id => $form.find(id)[0]);

    const stepper = new Stepper($wizard[0], { linear: true });
    const $btnNext = $form.find('.btn-next');
    const $btnPrev = $form.find('.btn-prev');

    const validators = [
        initValidationStep1(stepElems[0], stepper),
        initValidationStep2(stepElems[1], stepper),
        initValidationStep3(stepElems[2], stepper),
        initValidationStep4(stepElems[3], stepper),
        initValidationStep5(stepElems[4], stepper),
        initFinalStep(stepElems[5], stepper)
    ];

    // Navigation
    $btnNext.on('click', () => validators[stepper._currentIndex].validate());
    $btnPrev.on('click', () => stepper.previous());
}


// ==========================================
// 🧱 8. FormValidation Configurations
// ==========================================
function initValidationStep1(stepElem, stepper) {
    return FormValidation.formValidation(stepElem, {
        fields: {
            LesseeName: {
                validators: {
                    notEmpty: {
                        message: 'Nama Sesuai Identitas harus diisi'
                    },
                    regexp: {
                        regexp: /^[A-Za-z\s]+$/,
                        message: 'Nama (sesuai ID) tidak boleh mengandung angka atau tanda baca'
                    }
                }
            },
            LesseeFullName: {
                validators: {
                    notEmpty: {
                        message: 'Nama tanpa gelar harus diisi'
                    },
                    regexp: {
                        regexp: /^[A-Za-z\s]+$/,
                        message: 'Nama (tanpa gelar) tidak boleh mengandung angka atau tanda baca'
                    }
                }
            },
            Email: {
                validators: {
                    regexp: {
                        regexp: /^\w+([\-+.']\w+)*@\w+([\-]\w+)*\.\w+([\-]\w+)*$/,
                        message: 'Format email tidak valid'
                    },
                    callback: {
                        message: 'Format email tidak valid',
                        callback: function (input) {
                            // Kalau kosong, dianggap valid (tidak wajib)
                            if (input.value.trim() === '') {
                                return true;
                            }
                            // Kalau ada isinya, cek dengan regex di atas
                            return /^\w+([\-+.']\w+)*@\w+([\-]\w+)*\.\w+([\-]\w+)*$/.test(input.value);
                        }
                    }
                }
            },
             PlaceOfBirth: {
                validators: { notEmpty: { message: 'Tempat lahir harus diisi' } }
             },
             Gender: {
                validators: { notEmpty: { message: 'Jenis Kelamin harus diisi' } }
            },
            NPWP: {
                validators: {
                    callback: {
                        message: 'NPWP harus 15 atau 16 digit jika diisi',
                        callback: function (input) {
                            const value = input.value.replace(/[\.-]/g, '');
                            return (value === '' || (value.length >= 15 && value.length <= 16));
                        }
                    }
                }
            },
            IDNo: {
                validators: {
                    notEmpty: {
                        message: 'No Identitas harus diisi'
                    },
                    stringLength: {
                        min: 16,
                        max: 16,
                        message: 'No Identitas harus 16 digit'
                    },
                    regexp: {
                        regexp: /^[0-9]+$/,
                        message: 'No Identitas hanya boleh berisi angka'
                    }
                }
            },
            PlaceOfBirth: {
                validators: {
                    notEmpty: {
                        message: 'Tempat Lahir tidak boleh kosong atau hanya spasi'
                    },
                    regexp: {
                        regexp: /^(?![\d\W]+$).*$/,
                        message: 'Tempat Lahir tidak boleh hanya berisi angka atau tanda baca'
                    },
                    stringLength: {
                        min: 3,
                        message: 'Tempat Lahir harus lebih dari 2 karakter'
                    }
                }
            },
            MotherName: {
                validators: {
                    notEmpty: {
                        message: 'Nama gadis ibu kandung harus diisi'
                    },
                    regexp: {
                        regexp: /^[A-Za-z\s]+$/,
                        message: 'Nama Ibu tidak boleh mengandung angka atau tanda baca'
                    }
                }
            },
            Pendidikan: {
                validators: { notEmpty: { message: 'Pendidikan harus diisi' } }
            },
            IncomeSource: {
                validators: { notEmpty: { message: 'Sumber penghasilan harus diisi' } }
            },
            Agama: {
                validators: { notEmpty: { message: 'Agama harus diisi' } }
            },
            AreaCode: {
                validators: { notEmpty: { message: 'Kode area harus diisi' } }
            },
            LegalPhone1: {
                validators: { notEmpty: { message: 'No Telp harus diisi' } }
            },
            Handphone1: {
                validators: { notEmpty: { message: 'No Hp harus diisi' } }
            },
        },
        plugins: defaultValidationPlugins()
    }).on('core.form.valid', () => stepper.next());
}

function initValidationStep2(stepElem, stepper) {
    return FormValidation.formValidation(stepElem, {
        fields: {
            LegalCountry: { validators: { notEmpty: { message: 'Negara sesuai identitas harus diisi' } } },
            LegalProvince: { validators: { notEmpty: { message: 'Provinsi sesuai identitas harus diisi' } } },
            LegalCity: { validators: { notEmpty: { message: 'Kota sesuai identitas harus diisi' } } },
            LegalKecamatan: { validators: { notEmpty: { message: 'Kecamatan sesuai identitas harus diisi' } } },
            LegalKelurahan: { validators: { notEmpty: { message: 'Kelurahan sesuai identitas harus diisi' } } },
            LegalZipCode: { validators: { notEmpty: { message: 'Kodepos harus diisi' } } },
            LegalRT: { validators: { notEmpty: { message: 'RT harus diisi' } } },
            LegalRW: { validators: { notEmpty: { message: 'RW harus diisi' } } },
            LegalAddress1: {
                validators: {
                    notEmpty: {
                        message: 'Alamat harus diisi'
                    },
                    regexp: {
                        regexp: /^(?![\d\W]+$).*$/,
                        message: 'Legal Address tidak boleh hanya berisi angka atau tanda baca'
                    },
                    stringLength: {
                        min: 3,
                        message: 'Legal Address harus lebih dari 2 karakter'
                    }
                }
            },
            LegalZipCode: {
                validators: {
                    notEmpty: {
                        message: 'Kode Pos Alamat Legal harus diisi'
                    },
                    regexp: {
                        regexp: /^[0-9]{5}$/,
                        message: 'Kode Pos Alamat Legal harus terdiri dari 5 angka'
                    }
                }
            },
            ResidenceStatus: { validators: { notEmpty: { message: 'Status tinggal harus diisi' } } },
            Tahun: { validators: { notEmpty: { message: 'Lama tahun tinggal harus diisi' } } },
            Bulan: { validators: { notEmpty: { message: 'Lama bulan tinggal harus diisi' } } },
        },
        plugins: defaultValidationPlugins()
    }).on('core.form.valid', () => stepper.next());
}

function initValidationStep3(stepElem, stepper) {
    return FormValidation.formValidation(stepElem, {
        fields: {
            CompanyName: {
                validators: {
                    notEmpty: {
                        message: 'Tempat Bekerja harus diisi'
                    },
                    stringLength: {
                        min: 2,
                        message: 'Nama Perusahaan harus lebih dari atau sama dengan 2 karakter'
                    }
                }
            },
            WorkingPeriodPerYear: { validators: { notEmpty: { message: 'Lama bekerja harus diisi' } } },
            WorkAddress: {
                validators: {
                    notEmpty: {
                        message: 'Alamat tempat kerja'
                    },
                    stringLength: {
                        min: 2,
                        message: 'Alamat Tempat Kerja harus lebih dari atau sama dengan 2 karakter'
                    }
                }
            },
            Pekerjaan: { validators: { notEmpty: { message: 'Pekerjaan harus diisi' } } },
            JobTitles: { validators: { notEmpty: { message: 'Jabatan harus diisi' } } },
            BusinessLine: { validators: { notEmpty: { message: 'Bidang usaha tempat kerja harus diisi' } } },
            KindOfCompany: { validators: { notEmpty: { message: 'Golongan debitur harus diisi' } } },
            GrossIncomePerYear: { validators: { notEmpty: { message: 'Penghasilan kotor per tahun harus diisi' } } },
            
        },
        plugins: defaultValidationPlugins()
    }).on('core.form.valid', () => stepper.next());
}

function initValidationStep5(stepElem, stepper) {
    return FormValidation.formValidation(stepElem, {
        fields: {
            "RelationshipWithReporter.Code": { validators: { notEmpty: { message: 'Hubungan dengan pelapor harus diisi' } } },
            RiskLevel: { validators: { notEmpty: { message: 'Level resiko harus diisi' } } },
          

        },
        plugins: defaultValidationPlugins()
    }).on('core.form.valid', () => stepper.next());
}
function initValidationStep4(stepElem, stepper) {
    const fv = FormValidation.formValidation(stepElem, {
        fields: {
            JumlahTanggungan: {
                validators: {
                    notEmpty: {
                        message: 'Jumlah Tanggungan harus diisi'
                    }
                }
            }
        },
        plugins: defaultValidationPlugins()
    }).on('core.form.valid', function () {
        // Validasi manual radio
        const selected = stepElem.querySelector('input[name="inlineRadioOptions"]:checked');
        if (!selected) {
            // Tampilkan pesan error manual
            const msg = stepElem.querySelector('#radioErrorMsg');
            if (msg) msg.textContent = 'Status perkawinan harus dipilih';
            return; // stop, jangan next
        }

        // Kalau sudah pilih salah satu, lanjut step berikutnya
        stepper.next();
    });

    // Tambahkan pesan error di bawah radio group (sekali aja)
    let errorMsg = stepElem.querySelector('#radioErrorMsg');
    if (!errorMsg) {
        const div = document.createElement('div');
        div.id = 'radioErrorMsg';
        div.className = 'text-danger mt-1';
        const group = stepElem.querySelector('input[name="inlineRadioOptions"]')?.closest('.form-check-inline')?.parentNode;
        if (group) group.appendChild(div);
    }

    // Hapus pesan kalau user ubah pilihan
    stepElem.querySelectorAll('input[name="inlineRadioOptions"]').forEach(radio => {
        radio.addEventListener('change', () => {
            const msg = stepElem.querySelector('#radioErrorMsg');
            if (msg) msg.textContent = '';
        });
    });

    return fv;
}

function initSimpleStep(stepElem, stepper) {
    return FormValidation.formValidation(stepElem, {
        fields: {},
        plugins: defaultValidationPlugins()
    }).on('core.form.valid', () => stepper.next());
}

function initFinalStep(stepElem) {
    return FormValidation.formValidation(stepElem, {
        fields: {},
        plugins: defaultValidationPlugins()
    }).on('core.form.valid', () => {
        SaveFormData();
    });
}

function defaultValidationPlugins() {
    return {
        trigger: new FormValidation.plugins.Trigger(),
        bootstrap5: new FormValidation.plugins.Bootstrap5({
            eleValidClass: '',
            rowSelector: '.form-control-validation'
        }),
        autoFocus: new FormValidation.plugins.AutoFocus(),
        submitButton: new FormValidation.plugins.SubmitButton()
    };
}

function getLesseesData() {
    const parseIntOrZero = (val) => val ? parseInt(val.replace(/,/g, '')) : 0;
    const clean = (val) => val ? val.replace(/[\.-]/g, '') : '';

    return {
        LesseeNo: $('#LesseeNo').val(),
        LesseeName: $('#LesseeName').val(),
        IsGuarantor: false,
        IsBeneficiaryOwner: false,
        LesseeFullName: $('#LesseeFullName').val(),
        BranchCode: $('#cboBranchCode').val(),
        LesseeTypeNew: $('#cboLesseeType').val(),
        TypeCode: $('#cboTypeCode').val(),
        StatusCode: $('#cboStatusCode').val(),
        GroupNo: $('#cboGroupNo').val(),
        IndustryCode: $('#cboSektorEkonomi').val(),
        BusinessLine: $('#cboBussinessField').val(),
        LegalAddress1: $('#LegalAddress1').val(),
        LegalKecamatan: $('#cboLegalKecamatan').val(),
        LegalKelurahan: $('#cboLegalKelurahan').val(),
        LegalCity: $('#cboLegalCity').val(),
        LegalCountry: $('#cboLegalCountry').val(),
        LegalProvince: $('#cboLegalProvince').val(),
        LegalRT: $('#LegalRT').val(),
        LegalRW: $('#LegalRW').val(),
        LegalZipCode: $('#LegalZipCode').val(),
        LegalPhone1: $('#LegalPhone1').val(),
        LegalPhone2: $('#LegalPhone2').val(),
        Email: $('#Email').val(),
        IntroducedBy: $('#IntroducedBy').val(),
        CurrCode: $('#cboCurrCode').val(),
        CreditLimit: parseIntOrZero($('#CreditLimit').val()),
        UsedCreditLimit: $('#UsedCreditLimit').val(),
        CreditLimitExpiredDate2: $('#CreditLimitExpiredDate2').val(),
        KindOfCompany: $('#cboKindOfCompany').val(),
        NPWP: $('#NPWP').val(),
        LastTDRTDPNo: $('#LastTDRTDPNo').val(),
        Remark: $('#Remark').val(),
        //OutPrinpical: $('#OutPrinpical').val().replace(/,/g, ''),
        //OutIncome: $('#OutIncome').val().replace(/,/g, ''),
        //OutLR: $('#OutLR').val().replace(/,/g, ''),
        Nationality: $('#cboNationality').val(),
        IDNo: clean($('#IDNo').val()),
        IDType: $('#cboIDType').val(),
        LesseeAge: $('#LesseeAge').val(),
        AccessCode: $('#AccessCode').val(),
        NextReviewDate2: $('#NextReviewDate2').val(),
        MSLeaseAgreement: $('#MSLeaseAgreement').val(),
        MSLeaseAgreementDate2: $('#MSLeaseAgreementDate2').val(),
        CreditRiskGrade: $('#CreditRiskGrade').val(),
        //OutPrincipal_Int: $('#OutPrinpical').val().replace(/,/g, ''),
        //OutIncome_Int: $('#OutIncome').val().replace(/,/g, ''),
        //OutLR_Int: $('#OutLR').val().replace(/,/g, ''),
        SignBy: $('#SignBy').val(),
        Title: $('#Title').val(),
        RelationShip: $('#cboRelationship').val(),
        AreaCode: $('#AreaCode').val(),
        LesseeDOB: $('#LesseeDOB').val(),
        Affiliation: $('#Affiliation').val(),
        IDExpireds: $('#IDExpireds').val(),
        IDExpired: $('#IDExpireds').val(),
        SIUP: $('#SIUP').val(),
        SIUPExpiredDate2: $('#SIUPExpiredDate2').val(),
        LastTDRTDPExpired2: $('#LastTDRTDPExpired2').val(),
        MotherName: $('#MotherName').val(),
        "Category.Code": $('#cboCategory').val(),
        "CategoryDetail.Code": $('#cboCategory').val() == "0" ? 0 : $('#cboCategory').val() == "1" ? 1 : $('#cboCategory').val() == "2" ? 2 : 3,
        PlaceOfBirth: $('#PlaceOfBirth').val(),
        Education: $('#cboEducation').val(),
        GenderMale: $('#cboGender').val() == 0 ? true : false,
        GenderFemale: $('#cboGender').val() == 0 ? false : true,
        ResidenceAddress1: $('#ResidenceAddress1').val(),
        ResidenceCountry: $('#cboResidenceCountry').val(),
        ResidenceProvince: $('#cboResidenceProvince').val(),
        ResidenceCity: $('#cboResidenceCity').val(),
        ResidenceKecamatan: $('#cboResidenceKecamatan').val(),
        ResidenceKelurahan: $('#cboResidenceKelurahan').val(),
        ResidenceZipCode: $('#ResidenceZipCode').val(),
        ResidenceRT: $('#ResidenceRT').val(),
        ResidenceRW: $('#ResidenceRW').val(),
        Pekerjaan: $('#cboPekerjaan').val(),
        CompanyName: $('#CompanyName').val(),
        WorkAddress: $('#WorkAddress').val(),
        WorkProvince: $('#cboWorkProvince').val(),
        WorkCity: $('#cboWorkCity').val(),
        WorkZipCode: $('#WorkZipCode').val(),
        WorkingPeriodPerYear: $('#WorkingPeriodPerYear').val(),
        GrossIncomePerYear: parseIntOrZero($('#GrossIncomePerYear').val()),
        GrossLivingCostPerYear: parseIntOrZero($('#GrossLivingCostPerYear').val()),
        IncomeSource: $('#cboIncomeSource').val(),
        JumlahTanggungan: $('#JumlahTanggungan').val(),
        "RelationshipWithReporter.Code": $('#cboRelationEFI').val(),
        MaritalStatus: $('input[name="inlineRadioOptions"]:checked').val() == "kawinRadio" ? 1 : $('input[name="inlineRadioOptions"]:checked').val() == "blmKawinRadio" ? 2 : 3 ,
        SpouseName: $('#SpouseName').val(),
        SpouseDOB2: $('#SpouseDOB2').val(),
        SpouseIDNo: clean($('#SpouseIDNo').val()),
        SpouseIDExpired2: $('#SpouseIDExpired2').val(),
        isPisahHartaNo: $('#cboPisahHarta').val() == 0 ? true : false, 
        isPisahHartaYa: $('#cboPisahHarta').val() == 0 ? false : true,
        Handphone1: $('#Handphone1').val(),
        Handphone2: $('#Handphone2').val(),
        CIFNo: $('#CIFNo').val(),
        RiskLevel: $('#cboRiskLevel').val(),
        RiskDescription: $('#RiskDescription').val(),
        FamilyName: $('#FamilyName').val(),
        FamilyGenderMale: $('#cboContactGender').val() == 0 ? true : false,
        FamilyGenderFemale: $('#cboContactGender').val() == 0 ? false : true,
        FamilyPhoneNo: $('#FamilyPhoneNo').val(),
        FamilyHandPhoneNo: $('#FamilyHandPhoneNo').val(),
        FamilyAddress: $('#FamilyAddress').val(),
        ResidenceStatus: $('#cboResidenceStatus').val(),
        Tahun: $('#Tahun').val(),
        Bulan: $('#Bulan').val(),
        BOName: $('#cboBOName').val(),
        BOBranch: $('#cboBOBranch').val(),
        LesseePEP: $('#cboLesseePEP').val(),
        KategoriUsahaKeuanganBerkelanjutan: $('#cboKategoriUsaha').val(),
        BillingAddress1: $('#BillingAddress1').val(),
        BillingCountry: $('#cboBillingCountry').val(),
        BillingCity: $('#cboBillingCity').val(),
        BillingKecamatan: $('#cboBillingKecamatan').val(),
        BillingKelurahan: $('#cboBillingKelurahan').val(),
        BillingProvince: $('#cboBillingProvince').val(),
        BillingZipCode: $('#txtBillingZipCode').val(),
        BillingRT: $('#BillingRT').val(),
        BillingRW: $('#BillingRW').val(),
        Religion: $('#cboReligion').val(),
        JobTitles: $('#cboPosition').val()
       
    };
}

function handleSaveSuccess(respond) {
    const branch = $("#cboBranchCode").val();
    const lesseeNo = $("#LesseeNo").val();

    const alertInfo = (msg) => Swal.fire({
        icon: 'info',
        title: msg,
        confirmButtonText: 'OK',
        customClass: { confirmButton: 'order-2' }
    });

    switch (respond.Message) {
        case "AGE":
            return alertInfo("Age Must Be Greater than 16, Please Change Customer DOB !");
        case "KOF":
            return alertInfo("Selected kind of company only use for company Customer. Please re-select others !");
        case "IDNO":
            return alertInfo("Customer with this IDNo is exists. Please check your data avoidin duplicate data !");
        case "BRANCH":
            return alertInfo(`Only branch ${branch} can update this Customer data!`);
        case "NOACCESS":
            return alertInfo("You don't have accesss to Create or Edit!");
        default:
            if ($("#FormAction").val() == 1) {
                alert(respond.Message);
                window.location = `${editUrl}?LesseeNo=${respond.Message}&Branchs=${branch}`;
            } else {
                alert(respond.Message);
                window.location = `${editUrl}?LesseeNo=${lesseeNo}&Branchs=${branch}`;
            }
    }
}
function SaveFormData() {
    if (parseInt($("#FormAction").val()) == 1) {
        /*$("#myModal").modal("show");*/
        SaveDataCustomer();
    }
    else {
        if (parseInt($('#AppStatus').val()) == 0) {
            alert("This Customer data waiting to be Approved!")
            return false;
        }
        console.log('masuk');
        $("#myModal").modal("show");
    }


}
function SaveDataCustomer(event) {
    if (parseInt($("#FormAction").val()) == 2 && $("#CancelReason").val() === "") {
        alert("Please Select One Reason!");
        return false;
    }

    //var validated = Validation?.() ?? true; // pastikan Validation() ada
    //if (validated === false) {
    //    if (!$('#formCreateEdit').valid()) {
    //        event?.preventDefault();
    //        return false;
    //    }
    //    return false;
    //}

    var UserComments = parseInt($("#FormAction").val()) == 1 ? 0 : $("#CancelReason").val();
    var Jawaban = confirm("are you sure want to continue ?");
    if (!Jawaban) {
        $("#btnSave").removeAttr("disabled", true);
        return false;
    }

    $("#btnSave").prop("disabled", true);

    $.ajax({
        type: "POST",
        url: saveDataUrl,
        data: {
            "__RequestVerificationToken": token,
            "FormAction": $("#FormAction").val(),
            "Lessees": getLesseesData(),
            "UserComment": UserComments,
            checkDom: $('#checkDom').is(':checked'),
            checkBill: $('#checkBill').is(':checked')
        },
        success: handleSaveSuccess,
        error: function () {
            alert('Something is wrong, please contact your administrator');
        }
    });
}



// Placeholder untuk fungsi yang kamu sudah punya di tempat lain
function RemoveAllComma(val) {
    return val ? val.replace(/,/g, '') : '';
}

function AddCommaDecimal(val) {
    if (!val) return '';
    val = val.toString().replace(/,/g, '');
    return val.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

