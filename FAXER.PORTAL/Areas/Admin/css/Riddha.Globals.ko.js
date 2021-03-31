
var Riddha = {
    init: function (menuCode) {
        //Riddha.global.permission.action(menuCode);
    },
    ko: {
        global: {
            init: function () {

            },
            sum: function (array, field) {
                var sum = 0;
                var maped = ko.utils.arrayForEach(ko.toJS(array), function (item) {
                    sum += parseFloat(item[field]);
                });
                return sum;
            },
            arrayMap: function (data, Model) {
                var datas = ko.utils.arrayMap(data, function (item) {
                    return new Model(item);
                });
                return datas;
            },
            arrayMapCheckbox: function (data) {
                var datas = ko.utils.arrayMap(data, function (item) {
                    return new checkBoxModel(item);
                });
                return datas;
            },
            find: function (array, id) {
                var maped = ko.utils.arrayFirst(array(), function (item) {
                    return item.Id() == id();
                });
                return maped
            },
            filter: function (array, value, key) {
                key = key || 'Name'
                if (typeof (value) == "function") {
                    value = value();
                }
                var filtered = ko.utils.arrayFilter(array(), function (item) {
                    return item[key]().toLowerCase().indexOf(value.toLowerCase()) >= 0;
                });
                return filtered;
            },
            Compare: function (array, field, value) {
                if (typeof (value) == "function") {
                    value = value();
                }

                //var maped = ko.utils.arrayFirst(array(), function (item) {
                //    return item[field]() == value;
                //});
                var maped = ko.utils.arrayFilter(array(), function (item) {
                    return item[field]() == value;
                });
                return maped;

            }
        }

    },
    config: function () {
        var self = this;
        var getCookie = function (c_name) {
            var EhajiriCookie = document.cookie.split(';');
            for (var i = 0; i < EhajiriCookie.length; i++) {
                if (EhajiriCookie[i].indexOf('Ehajiri') > -1) {
                    var cookie = EhajiriCookie[i].substr(EhajiriCookie[i].indexOf("Ehajiri") + 8, document.cookie.length);
                    var i, x, y, ARRcookies = cookie.split("&");

                    for (var i = 0; i < ARRcookies.length; i++) {
                        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
                        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
                        x = x.replace(/^\s+|\s+$/g, "");
                        if (x == c_name) {
                            return unescape(y);
                        }
                    }
                }
            }

        }
        var getContextCookie = function (c_name) {
            var EhajiriCookie = document.cookie.split(';');
            for (var i = 0; i < EhajiriCookie.length; i++) {
                if (EhajiriCookie[i].indexOf('Context') > -1) {
                    var cookie = EhajiriCookie[i].substr(EhajiriCookie[i].indexOf("Context") + 8, document.cookie.length);
                    var i, x, y, ARRcookies = cookie.split("&");

                    for (var i = 0; i < ARRcookies.length; i++) {
                        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
                        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
                        x = x.replace(/^\s+|\s+$/g, "");
                        if (x == c_name) {
                            return unescape(y);
                        }
                    }
                }
            }

        }
        self.CurrentToken = getContextCookie("Token");
        self.CurrentLanguage = getCookie("Language");
        self.CurrentOperationDate = getCookie("OperationDate"); //getCookie("OperationDate");
        self.UserId = getCookie("UserId");
        self.UserType = getCookie("UserType");
        self.CurDate = getCookie("CurDate");
        self.PackageId = getCookie("PackageId");
        self.CompanyDpId = getCookie("CompanyDPID");
        return self;
    },
    ajax: {
        get: function (url, data) {
            return $.ajax({
                method: "GET",
                url: url,

            });
        },
        post: function (url, data) {
            var urls = url.split('/');
            var count = 0;
            for (var i = 0; i < urls.length; i++) {
                if (urls[i] != "") {
                    count++;
                }
            }
            if (count > 3)
                url = url;
            else {
                url = url + "/post"
            }

            return $.ajax({
                method: "POST",
                url: url,
                data: data
            });
        },
        put: function (url, data) {
            return $.ajax({
                method: "PUT",
                url: url + "/put",
                data: data
            });
        },
        delete: function (url, id) {
            //api/apiController/id
            var strings = url.split('/');
            return $.ajax({
                method: "DELETE",
                url: '/' + strings[1] + '/' + strings[2] + '/delete/' + strings[3],
                data: id
            });

        }
    },
    util:
    {
        delayExecute: function (callBack, time) {
            time = time || 100;
            setTimeout(function () {
                callBack();
            }, time);
        },
        padLeft: function (codeValue, PADLEN) {

            var strlength1 = (codeValue || "").length;
            codeValue = "00000" + codeValue;
            var strlength2 = codeValue.length;
            codeValue = codeValue.substring(strlength2 - PADLEN, strlength2);
            return codeValue;
        },
        right: function (codeValue, len) {
            codeValue = codeValue + "";
            var strlength = codeValue.length;
            codeValue = codeValue.substring(strlength - len, strlength);
            return codeValue;
        },
        left: function (codeValue, len) {
            codeValue = codeValue + "";
            codeValue = codeValue.substring(0, len);
            return codeValue;
        },
        localize: {
            Required: function (field) {
                Riddha.ajax.get("/Api/CommonApi/GetLocaliseStringRequired/?field=" + field)
                    .done(function (result) {
                        return Riddha.UI.Toast(result, Riddha.CRM.Global.ResultStatus.processError)
                    });
            },
            ToLacalize: function (field) {
                Riddha.ajax.get("/Api/CommonApi/GetLocaliseString/?field=" + field)
                    .done(function (result) {
                        return result;
                    });
            }
        }, formatDate: function (date) {
            if (date != 'Invalid Date') {
                var curdate = new Date(date);
                var day = Riddha.util.padLeft(curdate.getDate(), 2);
                var monthIndex = Riddha.util.padLeft(curdate.getMonth() + 1, 2);
                var year = curdate.getFullYear();


                return year + '/' + (monthIndex) + '/' + day;
            }
            else {
                return 'NaN/aN/aN';
            }
        }, getYear: function (date) {

            var formattedDate = Riddha.util.formatDate(new Date(date));
            if (Riddha.config().CurrentOperationDate == "ne") {
                formattedDate = AD2BS(formattedDate);
            }
            return formattedDate.substring(0, 4);
        }, getEngYear: function (date) {

            var formattedDate = Riddha.util.formatDate(new Date(date));
            if (Riddha.config().CurrentOperationDate == "ne") {
                formattedDate = BS2AD(formattedDate);
            }
            return formattedDate.substring(0, 4);
        }, getMonth: function (date) {
            var formattedDate = Riddha.util.formatDate(new Date(date));
            if (Riddha.config().CurrentOperationDate == "ne") {
                formattedDate = AD2BS(formattedDate);
            }
            return formattedDate.substring(5, 7);
        }, getDayName: function (dateString) {
            var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
            var d = new Date(dateString);
            var dayName = days[d.getDay()];
            return dayName;
        },
        viewBase64: function (base64Content) {
            var base = base64Content.split(',');
            var byteString = atob(base[1]);
            var type = base[0].split(':')[1].split(';')[0];

            // Convert that text into a byte array.
            var ab = new ArrayBuffer(byteString.length);
            var ia = new Uint8Array(ab);
            for (var i = 0; i < byteString.length; i++) {
                ia[i] = byteString.charCodeAt(i);
            }
            // Blob for saving.
            var blob = new Blob([ia], { type: type });
            window.open(window.URL.createObjectURL(blob), "__self");
        },
        getTimeDiff: function (a, b) {
            var fromArray = a.split(':');
            var toArray = b.split(':');
            var sh = fromArray[0];
            var sm = fromArray[1];

            var eh = toArray[0];
            var em = toArray[1];

            //result
            var rm = "";
            var rh = "";
            /*
            0=hour
            1=minute
            2=sec
            //neglect second
        
            */
            //check minute
            if (em > sm) {
                rm = 60 + Number(sm) - Number(em);
                rh = Number(sh) - Number(eh) - 1;
            }
            else {
                rm = Number(sm) - Number(em);
                rh = Number(sh) - Number(eh);
            }
            rm = Riddha.util.padLeft(rm, 2);
            rh = Riddha.util.padLeft(rh, 2);

            return rh + ':' + rm;


        },
        getTimeAdd: function (a, b) {
            var fromArray = a.split(':');
            var toArray = b.split(':');
            var sh = fromArray[0];
            var sm = fromArray[1];

            var eh = toArray[0];
            var em = toArray[1];

            //result
            var rm = "";
            var rh = "";
            /*
            0=hour
            1=minute
            2=sec
            //neglect second
        
            */
            //check minute
            if ((Number(sm) + Number(em)) >= 60) {
                rm = (Number(sm) + Number(em)) - 60;
                rh = Number(sh) + Number(eh) + 1;
            }
            else {
                rm = Number(sm) + Number(em);
                rh = Number(sh) + Number(eh);
            }
            rm = Riddha.util.padLeft(rm, 2);
            rh = Riddha.util.padLeft(rh, 2);

            return rh + ':' + rm;


        },
        GetNepaliMonths: function () {
            var lang = Riddha.config().CurrentLanguage;
            var result = new Array();
            if (lang == "ne") {
                result = ["श्रावण", "भदौ", "असोज", "कार्तिक", "मंसिर", "पुष", "माघ", "फाल्गुन", "चैत्र", "बैशाख", "जेठ ", "असार"];
            }
            else {
                result = ["Shrawan", "Bhadra", "Asoj", "Kartik", "Mangshir", "Poush", "Magh", "Falgun", "Chaitra", "Baisakh", "Jestha", "Ashad"];
            }

            return result;
        },
        GetEnglishMonths: function () {
            var lang = Riddha.config().CurrentLanguage;
            var result = new Array();
            if (lang == "ne") {
                result = ["जनवरी", "फेब्रुअरी", "मार्च", "अप्रिल", "मे", "जून", "जुलाई", "अगस्ट", "सेप्टेम्बर", "अक्टोबर", "नोभेम्बर", "डिसेम्बर"];
            }
            else {
                result = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            }

            return result;
        },
        FormatMoney: function (number, decPlaces, decSep, thouSep) {
            var value = parseFloat(number).toFixed(decPlaces || 2);
            var numberOnly = value.split('.')[0];
            var temp = "", result = "";
            var j = 0;
            for (var i = numberOnly.length - 1; i >= 0; i--) {
                if (j == 3 || j == 5 || j == 7 || j == 9 || j == 11) {
                    temp += thouSep || ","
                }
                temp += numberOnly[i];
                j++;
            }
            for (var i = temp.length - 1; i >= 0; i--) {

                result += temp[i];

            }
            return result + '.' + value.split('.')[1];
        }

    },
    CRM: {
        Global: {
            CustomerType: {
                Doctor: 1,
                Stockist: 2,
                Chemist: 3
            },
            ResultStatus: {
                processError: 0,
                dataBaseError: 1,
                ComError: 2,
                unHandeledError: 3,
                Ok: 4
            }
        }
    },
    UI: {
        Tabs: {
            selectTab: function (selector, index) {
                $(selector + " li:eq(" + index + ") a").click();
            }
        },
        Loading: {
            show: function () {
                //$("#loaderModal").modal('show');
            },
            hide: function () {
                //$("#loaderModal").modal('hide');
            }
        },
        groupArray: function (outArray, groupByArray, detailArray, detailModel, sumFields) {
            outArray([]);
            ko.utils.arrayForEach(ko.toJS(groupByArray.list()), function (lot) {

                var maped = ko.utils.arrayFirst(ko.toJS(detailArray.list), function (rpItem) {
                    return rpItem[detailArray.key] == lot[groupByArray.key];
                });
                if (maped) {
                    var details = ko.utils.arrayFilter((detailArray.list()), function (rpItem) {
                        return rpItem[detailArray.key]() == lot[groupByArray.key];
                    });
                    maped = new detailModel(ko.toJS(maped));
                    if (sumFields.length > 0) {
                        ko.utils.arrayForEach(sumFields, function (item, index) {
                            maped[item](Riddha.ko.global.sum(details, item));
                        });
                    }
                    //ko.utils.arrayForEach(sumFields, function (item, index) {
                    //    maped[item](Riddha.ko.global.sum(details, item));
                    //});
                    outArray.push({ header: maped, detail: ko.observableArray(details) });
                }


            });
        },
        groupArrayDynimic: function (outArray, groupByArray, detailArray, detailModel, sumFields) {
            outArray([]);
            ko.utils.arrayForEach(ko.toJS(groupByArray.list()), function (lot) {

                var maped = ko.utils.arrayFirst(ko.toJS(detailArray.list), function (rpItem) {
                    return rpItem[detailArray.key] == lot[groupByArray.key];
                });
                if (maped) {
                    var details = ko.utils.arrayFilter((detailArray.list()), function (rpItem) {
                        return rpItem[detailArray.key] == lot[groupByArray.key];
                    });
                    maped = new detailModel(ko.toJS(maped));
                    if (sumFields.length > 0) {
                        ko.utils.arrayForEach(sumFields, function (item, index) {
                            maped[item](Riddha.ko.global.sum(details, item));
                        });
                    }
                    //ko.utils.arrayForEach(sumFields, function (item, index) {
                    //    maped[item](Riddha.ko.global.sum(details, item));
                    //});

                    outArray.push({ header: maped, detail: ko.observableArray(details) });
                }


            });
        },
        Message: function (message, status, flashEnable, flashTime) {
            switch (status) {
                case 0:
                    $('#message').removeAttr("class").addClass("text-red");
                    break;
                case 4:
                    $('#message').addClass("text-green");
                    break;
                default:

            }
            $('#message').text(message);
            if (flashEnable) {
                Riddha.util.delayExecute(function () {
                    $('#message').text('');
                }, flashTime);
            }

        },

        Toast: function (message, status, title, autoClose) {
            var msgObj = {
                title: title,
                msg: message,
                timeout: (autoClose || false) ? 0 : 3000,
                showClose: true,
                clickToClose: true
            };
            //toastr config
            toastr.options.closeButton = true;
            toastr.options.closeMethod = 'fadeOut';
            toastr.options.closeDuration = 300;
            toastr.options.showDuration = 300;
            toastr.options.showEasing = "swing";
            toastr.options.hideEasing = "linear";
            toastr.options.progressBar = true;
            toastr.options.showMethod = "fadeIn";
            toastr.options.hideMethod = "fadeOut";
            //toastr.options.showEasing = 'easeOutBounce';
            //toastr.options.hideEasing = 'easeInBack';
            //toastr.options.closeEasing = 'easeInBack';
            toastr.options.newestOnTop = true;
            toastr.options.positionClass = "toast-top-right";// "toast-top-full-width";
            //toastr.options.positionClass = 'toast-top-center';

            switch (status) {
                case Riddha.CRM.Global.ResultStatus.ComError:
                    msgObj.title = msgObj.title || "Error!";

                    toastr.error(msgObj.msg);
                    break;
                case Riddha.CRM.Global.ResultStatus.unHandeledError:
                    msgObj.title = msgObj.title || "Error!";
                    toastr.error(msgObj.msg);
                    break;
                case Riddha.CRM.Global.ResultStatus.dataBaseError:
                    msgObj.title = msgObj.title || "Error!";
                    toastr.error(msgObj.msg);
                    break;
                case Riddha.CRM.Global.ResultStatus.processError:

                    msgObj.title = msgObj.title || "Warning!";
                    toastr.warning(msgObj.msg);
                    break;
                case Riddha.CRM.Global.ResultStatus.Ok:
                    msgObj.title = msgObj.title || "Success!";
                    toastr.success(msgObj.msg);
                    break;
                default:
                    msgObj.title = msgObj.title || "Info!";
                    toastr.info(msgObj.msg);
                    break;

            }
        },

        SuccessPopUp: function (message, title, MType) {
            var type = MType;
            switch (type || 0) {
                case 0: type = 'success';
                    break;
                case 1: type = 'warning';
                    break;
                default:
            }

            Swal.fire({
                title: title,
                text: message,
                type: type
            }).then((result) => {
                window.location.reload();
            });


        },

        Alert: function (message, title, status) {

            switch (status) {
                case Riddha.CRM.Global.ResultStatus.ComError:
                    title = "Error!";
                    type = "error";
                    break;
                case Riddha.CRM.Global.ResultStatus.unHandeledError:
                    title = "Error!";
                    type = "error";
                    break;
                case Riddha.CRM.Global.ResultStatus.dataBaseError:
                    title = "Error!";
                    type = "error";
                    break;
                case Riddha.CRM.Global.ResultStatus.processError:
                    title = "Warning!";
                    type = "warning";
                    break;
                case Riddha.CRM.Global.ResultStatus.Ok:
                    title = "Success!";
                    type = "success";

                    break;
                default:
                    title = "Info!";
                    type = "info";

                    break;

            }

            Swal.fire({
                title: title,
                text: message,
                type: type,
            })
        },

        Confirm: function (message, callback, Cancelcallback, title, confirmButtonText, type) {

            var config = new Riddha.config();
            Swal.fire({
                title: title,
                text: message,
                type: type || 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: confirmButtonText || 'Yes !',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.value) {
                    callback();
                } else {
                    Cancelcallback();
                }
            });

            //BootstrapDialog 
            //BootstrapDialog.confirm({
            //    title: title || (config.CurrentLanguage == 'ne' ? 'पक्का गर्नुहोस' : 'Confirm'),
            //    //size: BootstrapDialog.SIZE_SMALL,
            //    message: getConfirmMessage(config, message),
            //    type: BootstrapDialog.TYPE_PRIMARY, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
            //    closable: true, // <-- Default value is false
            //    //draggable: true, // <-- Default value is false
            //    btnCancelLabel: config.CurrentLanguage == 'ne' ? 'होइन' : 'No', // <-- Default value is 'Cancel',
            //    btnOKLabel: config.CurrentLanguage == 'ne' ? 'हो' : 'Yes', // <-- Default value is 'OK',
            //    //btnOKClass: 'btn-info', // <-- If you didn't specify it, dialog type will be used,
            //    callback: function (result) {
            //        // result will be true if button was click, while it will be false if users close the dialog directly.
            //        if (result) {
            //            callback();
            //        } else {
            //            // alert('Nope.');
            //        }
            //    }
            //});


            function getConfirmMessage(config, message) {
                var result = "";
                if (config.CurrentLanguage == 'ne') {
                    switch (message.toLowerCase()) {
                        case 'deleteconfirm':
                            result = 'हटाउन को लागी पक्का गर्नुहोस ?';
                            break;
                        case 'approveconfirm':
                            result = "प्रमणित को लागी पक्का गर्नुहोस ? ";
                            break;
                        case 'settingconfirm':
                            result = "तपाईंको छनोट लागु गर्न पेज रिलोड गर्नुहोस । ";
                            break;
                        case 'deactivateconfirm':
                            result = "लगिन निस्क्रिय पक्का गर्नुहोस् । ";
                            break;
                        case 'pullconfirm':
                            result = "डिफल्ट डाटा ल्याउन पक्का गर्नुहोस ।";
                            break;
                        case 'exceluploadconfirm':
                            result = "एक्सल अपलोड पक्का गर्नुहोस ।";
                            break;
                        case 'pulldeviceemployeeconfirm':
                            result = "हाजिरी मेसिनमा कर्मचारी ल्याउन पक्का गर्नुहोस ।";
                            break;
                        default:
                            result = message;
                            break;
                    }
                }
                else {
                    switch (message.toLowerCase()) {
                        case 'deleteconfirm':
                            result = 'Confirm to Delete?';
                            break;
                        case 'approveconfirm':
                            result = "Confirm to Approve ? ";
                            break;
                        case 'settingconfirm':
                            result = "Confirm to Reload page to get the effect that you have just changed";
                            break;
                        case 'deactivateconfirm':
                            result = "Confirm to Deactivate";
                            break;
                        case 'pullconfirm':
                            result = "Confirm to pull default data?";
                            break;
                        case 'exceluploadconfirm':
                            result = "Confirm to Upload Excel data?";
                            break;
                        case 'pulldeviceemployeeconfirm':
                            result = "Confirm to Pull Device Employee data?";
                            break;
                        default:

                            result = message;
                            break;
                    }
                }
                return result;
            }
        },
        getOptionCaption: function () {
            var config = new Riddha.config();
            var SelectValue = config.CurrentLanguage == "en" ? "select" : "छान्नुहोस";
            return SelectValue;
        }

    },
    global: {
        permission: {
            action: function (menuCode) {
                return Riddha.ajax.get("/api/menuapi/getActions?menucode=" + menuCode);

            },
            validateAction: function (actionCode) {
                return Riddha.global.permission.actions.indexOf(actionCode) > -1;
            },
            actions: ['0000']

        },
        getMaxDaysInMonth: function (year, monthid) {
            var deffered = new $.Deferred();
            if (Riddha.config().CurrentOperationDate == 'en') {
                deffered.resolve(new Date(year, monthid, 0).getDate());
            }
            else {
                switch (monthid) {
                    case 12:
                        year = parseInt(year) + 1;
                        monthid = 1;
                        break;
                    default:
                        monthid = monthid + 1;
                        break;

                }

                var date = '' + year + '/' + Riddha.util.padLeft(monthid, 2) + '/00';
                var engDate = BS2AD(date);
                var nepDate = AD2BS(engDate);
                var day = parseInt(nepDate.split('/')[2]);
                deffered.resolve(day);
                //Riddha.ajax.get("/api/date/getMaxDaysInNepMonth?id=" + monthid)
                //    .done(function (response) {
                //        deffered.resolve(response.Data);
                //    });
            }

            return deffered.promise();
        }



    },
    MIS: {
        GetNewSequence: function (sequencefor) {
            var deferred = new $.Deferred();
            Riddha.ajax.get("/api/DocFormatApi/GetNewSequence?sequencefor=" + sequencefor).done(function (data) {
                deferred.resolve(data.Data);
            });
            return deferred.promise();
        },
        SequenceFor: {
            PurchaseOrder: 0,
            GoodsReceive: 1,
            BooksCode: 2,
            MemberCode: 3,
            JVoucher: 4,
            Purchase: 5,
            Sales: 6,
            Payment: 7,
            AssetCode: 8,
            ExpenseTitle: 9,
            Receipt: 10,
            StorePurchaseOrder: 11,
            StoreGoodsReceive: 12,
            StoreGoodsRequest: 13,
            StoreGoodsIssue: 14,
            CreditNote: 15,
            DebitNote: 16,
            ContraEntry: 17,
            MemberConfig: 18,
            ItemGroup: 19,
            Item: 20,
            GoodsTransfer: 21,
            CharacterCertificate: 22,
            IncomingRecordNo: 23,
            OutgoingRecordNo: 24
        },
    }

};
ko.bindingHandlers.actionVisibility = {
    init: function (element, valueAccessor) {
        ko.bindingHandlers.actionVisibility.bind(element, valueAccessor);
    },
    update: function (element, valueAccessor) {

        ko.bindingHandlers.actionVisibility.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {

        if (Riddha.global.permission.validateAction(valueAccessor()))
            $(element).removeClass("hidden").show();
        else
            $(element).hide();

    }
}

ko.bindingHandlers.CascadeTo = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        var listObject = value.childs;
        var online = value.online;
        var parentId = value.id;
        var parentPropertyName = value.property;
        var ChildList = value.childBindList;
        var filteredArray = ko.utils.arrayFilter(listObject, function (item) {
            return item[Property]() == parentId();
        });

        ChildList(filteredArray);
    },
    update: function (element, valueAccessor, allBindings) {
        // First get the latest data that we're bound to
        var value = valueAccessor();
        var listObject = value.childs;
        var online = value.online;
        var parentId = value.id;
        var parentPropertyName = value.property;
        var ChildList = value.childBindList;
        var filteredArray = ko.utils.arrayFilter(listObject(), function (item) {
            return item[parentPropertyName]() == parentId();
        });

        ChildList(filteredArray);


    }
};
ko.bindingHandlers.img = {
    update: function (element, valueAccessor) {
        //grab the value of the parameters, making sure to unwrap anything that could be observable
        var value = ko.utils.unwrapObservable(valueAccessor()),
            src = ko.utils.unwrapObservable(value.src),
            fallback = ko.utils.unwrapObservable(value.fallback),
            $element = $(element);

        //now set the src attribute to either the bound or the fallback value
        if (src) {
            $element.attr("src", src);
        } else {
            $element.attr("src", fallback);
        }
    },
    init: function (element, valueAccessor) {
        var $element = $(element);

        //hook up error handling that will unwrap and set the fallback value
        $element.error(function () {
            var value = ko.utils.unwrapObservable(valueAccessor()),
                fallback = ko.utils.unwrapObservable(value.fallback);

            $element.attr("src", fallback);
        });
    },
};
ko.bindingHandlers.createUpdateText = {
    update: function (element, valueAccessor) {

        var config = new Riddha.config();
        if (config.CurrentLanguage == "en") {

            $(element).val(valueAccessor()());
        }
        else {
            if (valueAccessor()() == "Create")
                $(element).val("सुरक्षित गर्नुहोस");
            else {
                $(element).val("परिमार्जन सुरक्षित गर्नुहोस");
            }
        }

        //grab the value of the parameters, making sure to unwrap anything that could be observable

    },
    init: function (element, valueAccessor) {

    },
};
ko.bindingHandlers.kendoGrid = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.kendoGrid.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {

    },
    bind: function (element, valueAccessor) {
        var lang = new Riddha.config().CurrentLanguage;
        var param = valueAccessor();

        if (param.open) {
            param.open(loadGrid);
            if (param.autoOpen) {
                loadGrid();
            }

        }
        else {

            loadGrid();
        }

        param["totalItem"] = function () {
            return $(param.target).data("kendoGrid").dataSource._total;
        }
        function loadGrid() {
            if (param.multiSelect) {
                //param.columns.splice(0, 0, { template: "<input type='checkbox' disabled class='checkbox' />", width: 20 });
            }
            var data = {};
            if (param.paramData) {
                if (typeof (param.paramData) == "function") {
                    data = param.paramData();
                }
                else {
                    data = param.paramData;
                }
            }
            var ds = {
                type: "json",
                serverPaging: true,
                serverSorting: false,
                serverFiltering: true,
                pageSize: 30,
                transport: {
                    read: {
                        type: "POST",
                        url: param.url,
                        dataType: "json",
                        data: data || {}
                    },
                    parameterMap: function (options, operation) {
                        if (operation == "read") {
                            return options;
                        }
                    },
                },
                schema: { data: "Data", total: "TotalCount" },
            }
            if (param.groupParam)
                ds["group"] = param.groupParam;
            var kGrid = $(param.target).kendoGrid({
                dataSource: ds,
                height: param.height || '100%',
                reorderable: false,
                resizable: true,
                scrollable: true,
                navigatable: true,
                sortable: true,
                autoSync: false,
                change: function (e) {
                    var rows = e.sender.select();
                    var SelectedItems = [];
                    $('tr').find('[type=checkbox]').prop('checked', false);
                    $('tr.k-state-selected').find('[type=checkbox]').prop('checked', true);
                    rows.each(function (e) {//absentWindow
                        var grid = $(param.target).data("kendoGrid");
                        var dataItem = grid.dataItem(this);
                        SelectedItems.push(dataItem);
                        param.SelectedItem(dataItem);
                    });
                    param.SelectedItems(SelectedItems);
                },
                dataBound: function (e) {
                    var grid = this;
                    grid.tbody.find("tr").dblclick(function (e) {

                    });
                },
                selectable: (param.multiSelect || false) ? "multiple, row" : true,
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            startswith: lang == "ne" ? "सुरुको " : "Starts with",
                            eq: lang == "ne" ? "बाराबर" : "Is equal to"
                        }
                    }
                },
                groupable: param.group || false,
                columnMenu: false,
                pageable: {
                    pageSizes: ['ALL', 50, 100, 150, 200],
                    buttonCount: 10,
                    refresh: true,
                    messages: {
                        empty: "No Record Found",
                        display: lang == "ne" ? "जम्मा {2}डाटा आईटम बाट देखाइएको {0}-{1}" : "Showing {0}-{1} from {2} data items",
                        itemsPerPage: "Items Per Page"
                    }
                },
                dataBinding: function () {
                    record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
                },
                columns: param.columns || [
                    { field: "Code", title: "Code" },
                    { field: "Name", title: "Name" },
                ]
            });
            //  param.grid = $(param.target).data("kendoGrid");

        }
    }
};



//ko.bindingHandlers.ArchitectUIGrid = {
//    update: function (element, valueAccessor) {
//        ko.bindingHandlers.kendoGrid.bind(element, valueAccessor);
//    },
//    init: function (element, valueAccessor) {

//    },
//    bind: function (element, valueAccessor) {
//        var lang = new Riddha.config().CurrentLanguage;
//        var param = valueAccessor();

//        if (param.open) {
//            param.open(loadGrid);
//            if (param.autoOpen) {
//                loadGrid();
//            }

//        }
//        else {

//            loadGrid();
//        }

//        param["totalItem"] = function () {
//            return $(param.target).data("ArchitectUIGrid").dataSource._total;
//        }
//        function loadGrid() {
//            if (param.multiSelect) {
//                //param.columns.splice(0, 0, { template: "<input type='checkbox' disabled class='checkbox' />", width: 20 });
//            }
//            var data = {};
//            if (param.paramData) {
//                if (typeof (param.paramData) == "function") {
//                    data = param.paramData();
//                }
//                else {
//                    data = param.paramData;
//                }
//            }
//            var ds = {
//                type: "json",
//                serverPaging: true,
//                serverSorting: false,
//                serverFiltering: true,
//                pageSize: 30,
//                transport: {
//                    read: {
//                        type: "POST",
//                        url: param.url,
//                        dataType: "json",
//                        data: data || {}
//                    },
//                    parameterMap: function (options, operation) {
//                        if (operation == "read") {
//                            return options;
//                        }
//                    },
//                },
//                schema: { data: "Data", total: "TotalCount" },
//            }
//            if (param.groupParam)
//                ds["group"] = param.groupParam;
//            var kGrid = $(param.target).kendoGrid({
//                dataSource: ds,
//                height: param.height || '100%',
//                reorderable: false,
//                resizable: true,
//                scrollable: true,
//                navigatable: true,
//                sortable: true,
//                autoSync: false,
//                change: function (e) {
//                    var rows = e.sender.select();
//                    var SelectedItems = [];
//                    $('tr').find('[type=checkbox]').prop('checked', false);
//                    $('tr.k-state-selected').find('[type=checkbox]').prop('checked', true);
//                    rows.each(function (e) {//absentWindow
//                        var grid = $(param.target).data("kendoGrid");
//                        var dataItem = grid.dataItem(this);
//                        SelectedItems.push(dataItem);
//                        param.SelectedItem(dataItem);
//                    });
//                    param.SelectedItems(SelectedItems);
//                },
//                dataBound: function (e) {
//                    var grid = this;
//                    grid.tbody.find("tr").dblclick(function (e) {

//                    });
//                },
//                selectable: (param.multiSelect || false) ? "multiple, row" : true,
//                filterable: {
//                    extra: false,
//                    operators: {
//                        string: {
//                            startswith: lang == "ne" ? "सुरुको " : "Starts with",
//                            eq: lang == "ne" ? "बाराबर" : "Is equal to"
//                        }
//                    }
//                },
//                groupable: param.group || false,
//                columnMenu: false,
//                pageable: {
//                    pageSizes: ['ALL', 50, 100, 150, 200],
//                    buttonCount: 10,
//                    refresh: true,
//                    messages: {
//                        empty: "No Record Found",
//                        display: lang == "ne" ? "जम्मा {2}डाटा आईटम बाट देखाइएको {0}-{1}" : "Showing {0}-{1} from {2} data items",
//                        itemsPerPage: "Items Per Page"
//                    }
//                },
//                dataBinding: function () {
//                    record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
//                },
//                columns: param.columns || [
//                    { field: "Code", title: "Code" },
//                    { field: "Name", title: "Name" },
//                ]
//            });
//        }
//    }
//};


ko.bindingHandlers.kendoWindowGrid = {
    update: function (element, valueAccessor) {
        //$(valueAccessor().target).find('div').eq(0).remove();
        ko.bindingHandlers.kendoWindowGrid.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
        //ko.bindingHandlers.kendoWindow.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {
        //var options = localStorage["kendo-grid-options"];

        var param = valueAccessor();
        //var dataWindow = $(param.target).kendoWindow({
        //    title: param.title,
        //    width: param.width || 1120,
        //    height: param.height || 650,
        //    visible: false,
        //    actions: param.actions || [
        //                "Minimize", "Maximize", "Close"
        //    ],
        //    scrollable: true,
        //    resizable: true,
        //    draggable: true,
        //    open: function () {
        //        loadGrid();
        //    }
        //}).data("kendoWindow");



        $(element).off('click').on('click', function () {
            var dataDailog = $(param.target).kendoDialog({
                width: param.width || $(window).width() - 10,
                height: param.height - 2,
                animation: {
                    close: {
                        effects: "fade:out"
                    },
                    open: {
                        effects: "fade:in"
                    }
                },
                title: param.title,
                visible: false,
                closable: true,
                modal: true,
                actions: [
                    //{
                    //    text: lang == "ne" ? "बन्द गर्नुहोस" : 'Close',
                    //    primary: false,
                    //    action: onCancelClick
                    //},
                    //{
                    //    text: lang == "ne" ? "अनुमती बनाउनुस" : 'Create',
                    //    primary: true,
                    //    action: onOkClick
                    //}
                ],
                open: function () {
                    //  loadGrid();
                }
                //close: onClose
            }).data("kendoDialog");
            dataDailog.open();
            loadGrid();
            if (param.maximize) {
                //dataWindow.open().maximize()
            }
            else {
                //dataWindow.open().element.closest(".k-window").css({
                //    top: 105,
                //    left: 230
                //});
            }
        });
        function loadGrid() {
            if (param.multiSelect) {
                param.columns.splice(0, 0, { template: "<input type='checkbox' disabled class='checkbox' />" });
            }
            var ds = {
                type: "json",
                serverPaging: true,
                serverSorting: false,
                serverFiltering: true,
                pageSize: param.pageSize || 30,
                transport: {
                    read: {
                        type: "POST",
                        url: param.url,
                        dataType: "json",
                        data: param.paramData || {}
                    },
                    parameterMap: function (options, operation) {
                        if (operation == "read") {
                            return options;
                        }
                    },
                },
                schema: {
                    data: "Data", total: "TotalCount", fields: {
                        WorkDate: { type: "date" }
                    }
                },
                group: param.groupParam,
                sort: param.sort || {}
            }
            if (param.groupParam)
                ds["group"] = param.groupParam;
            $(param.target).find('div').eq(0).html('');


            /* pdf content template*/
            var contentTemplatePdf = ' <div class="page-template">                                                          ' +
                '     <img class="logo" src="' + localStorage.companyLogo + '">                                  ' +
                '     </img>                                                                                 ' +
                '     <div class="header">                                                            ' +
                '      <span class="company-name">   ' + localStorage.companyName + '</span><br />                                                             ' +

                '      <span class="company-address">   ' + localStorage.companyAddress + '</span><br />                                                             ' +
                '      <span class="report-title">   ' + param.title + '</span><br />                 ' +
                '     </div>                                                                          ' +
                '     <div class="footer">                                                            ' +
                '         <div style="float: right">Page #: pageNum # of #: totalPages #</div>        ' +
                '     </div>                                                                          ' +
                ' </div>';
            /**/

            var pdfConfig = {
                allPages: true,
                paperSize: "A4",
                margin: { top: "2.5cm", right: "0.5cm", bottom: "0.5cm", left: "0.5cm" },
                landscape: param.landscape || true,
                template: contentTemplatePdf,
                scale: param.scale || 0.6
            };
            var toolbarConfig = [];

            if (param.hidePdf) {
                toolbarConfig = ["excel", {
                    id: "ColumnMenu",
                    template: "<a id='columnMenuButton" + param.target.substring(1, param.target.length - 1) + "'></a>",
                    overflow: "never"
                }];
                pdfConfig = false;
            }
            else {
                toolbarConfig = ["pdf", "excel", {
                    id: "ColumnMenu",
                    template: "<a id='columnMenuButton" + param.target.substring(1, param.target.length - 1) + "'></a>",
                    overflow: "never"
                }];
            }
            var kGrid = $(param.target).find('div').eq(0).kendoGrid({
                toolbar: toolbarConfig,
                pdf: pdfConfig,
                excelExport: function (e) {
                    var borderColor = "#bfbdbd";
                    var sheet = e.workbook.sheets[0];
                    sheet.freezePane = [{ rowSplit: 0 }];
                    var colLength = 0;

                    for (var rowIndex = 0; rowIndex < sheet.rows.length; rowIndex++) {
                        var rowBorder = sheet.rows[rowIndex];

                        if (rowBorder.cells.length > colLength)
                            colLength = rowBorder.cells.length;
                        console.log(rowBorder.type);
                        for (var cellIndex = 0; cellIndex < rowBorder.cells.length; cellIndex++) {
                            var c = rowBorder.cells[cellIndex];
                            if (rowBorder.type == "header") {
                                var cell = rowBorder.cells[cellIndex];
                                console.log(cell);
                                cell.background = "#dddddd";
                                cell.borderLeft = "#000";
                                cell.borderRight = "#000"
                                cell.borderTop = "#000"
                                cell.borderBottom = "#000"
                                cell.color = "#000";
                                // rowBorder.cells[cellIndex] = { background: borderColor, color: "black", borderRight: { color: borderColor }, borderLeft: { color: borderColor }, borderTop: { color: borderColor }, borderBottom: { color: borderColor } };
                            }
                            else if (rowBorder.type == "group-header") {
                                var cell = rowBorder.cells[cellIndex];
                                console.log(cell);
                                cell.background = borderColor;
                            }
                            else {
                                rowBorder.cells[cellIndex] = { color: "black", value: c.value, colSpan: 1, rowSpan: 1, borderRight: { color: borderColor }, borderLeft: { color: borderColor }, borderBottom: { color: borderColor }, borderTop: { color: borderColor } };
                            }
                        }
                    }

                    for (var rowIndex = 2; rowIndex < sheet.rows.length; rowIndex++) {
                        var row = sheet.rows[rowIndex];


                        if (row.type != "group-header") {
                            if (rowIndex % 2 == 0) {
                                for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                                    row.cells[cellIndex].background = "#efefef";    //"#aabbcc";
                                }
                            }
                            else {
                                for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                                    row.cells[cellIndex].background = "#fff";    //"#aabbcc";

                                }
                            }
                        }
                    }

                    sheet.rows.unshift({
                        cells: [{ value: param.title, colSpan: colLength, borderLeft: { color: borderColor }, borderRight: { color: borderColor }, textAlign: "center", verticalAlign: "top", fontSize: 12, bold: true, background: "#fff" }],
                        height: 30
                    });
                    sheet.rows.unshift({
                        cells: [{ value: localStorage.companyAddress, borderLeft: { color: borderColor }, borderRight: { color: borderColor }, colSpan: colLength, textAlign: "center", verticalAlign: "center", fontSize: 12, bold: true, background: "#fff", border: "#000" }],
                        height: 14
                    });
                    sheet.rows.unshift({
                        cells: [{ value: localStorage.companyName, borderTop: { color: borderColor, size: 3 }, borderLeft: { color: borderColor }, borderRight: { color: borderColor }, colSpan: colLength, textAlign: "center", verticalAlign: "bottom", fontSize: 14, bold: true, background: "#fff" }],
                        height: 25
                    });


                },
                dataSource: ds,
                height: '100%',
                reorderable: true,
                resizable: true,
                scrollable: true,
                navigatable: true,
                sortable: param.sortable == undefined ? true : param.sortable,
                autoSync: false,
                change: function (e) {
                    var rows = e.sender.select();
                    var SelectedItems = [];
                    $('tr').find('[type=checkbox]').prop('checked', false);
                    $('tr.k-state-selected').find('[type=checkbox]').prop('checked', true);
                    rows.each(function (e) {//absentWindow
                        var grid = $(param.target).find('div').eq(0).data("kendoGrid");
                        console.log({ $this: this });

                        var dataItem = grid.dataItem(this);
                        param.getSelectedItem(dataItem);
                        SelectedItems.push(dataItem);
                    });
                    param.SelectedItems(SelectedItems);
                },
                selectable: false,// (param.multiSelect || false) ? "multiple, row" : true,
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            startswith: "Starts with",
                            eq: "Is equal to"
                        }
                    }
                },
                groupable: param.group || true,
                columnMenu: false,
                pageable: {
                    pageSizes: true,
                    buttonCount: 10,
                    pageSizes: [50, 100, 150, 200, 500, 800, 1000],
                    refresh: true,
                    messages: {
                        empty: "No Record Found",
                        display: "Showing {0}-{1} from {2} data items",
                        itemsPerPage: "Items Per Page"
                    }
                },
                dataBinding: function () {
                    record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
                },
                columns: getColumns(),
                columnMenuInit: function (e) {
                    var items = e.container.find(".k-item");
                }
            });
            $("#columnMenuButton" + param.target.substring(1, param.target.length - 1)).kendoColumnMenu({
                filterable: false,
                sortable: false,
                dataSource: kGrid.data("kendoGrid").dataSource,
                columns: true,
                owner: kGrid.getKendoGrid(),
                init: function (e) {
                    var grid = kGrid.data("kendoGrid");
                    var menu = e.container.find(".k-menu").data("kendoMenu");
                    var popup = e.container.data().kendoPopup;
                    if (param.target.toLowerCase() == "#kendomonthlysummaryreportwindowdaywise") {
                        menu.append("<li id='savegridstate' class='k-item'><span><img class='k-image' alt='' src='{0}'>save grid state</span></li>");
                        menu.bind("select", function (e) {
                            if (e.item.id === "savegridstate") {
                                if (param.url.toLowerCase() == "/Api/MonthlyEmployeeSummaryReportApi/GenerateReportLeaveWise".toLowerCase()) {
                                    localStorage["summary-leave-column-options"] = kendo.stringify(grid.getOptions().columns);
                                }
                                else {
                                    localStorage["summary-column-options"] = kendo.stringify(grid.getOptions().columns);
                                }
                                menu.close();
                                popup.close();
                            }

                        });
                    }
                }
            });
            function getColumns() {
                if (typeof (param.columns) == "function")
                    return param.columns();
                else {
                    return param.columns || [
                        { field: "Code", title: "Code" },
                        { field: "Name", title: "Name" },
                    ];
                }
            }
        }
    }
}

ko.bindingHandlers.kendoInlineEditGridInDialog = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.kendoInlineEditGridInDialog.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        var dataDailog = $(param.target).kendoDialog({
            width: param.width,
            height: param.height,
            animation: {
                close: {
                    effects: "fade:out"
                },
                open: {
                    effects: "fade:in"
                }
            },
            title: param.title,
            visible: false,
            closable: true,
            modal: true,
            open: function () {
                loadGrid();
            }
        }).data("kendoDialog");

        $(element).off('click').on('click', function () {
            dataDailog.open();
        });
        function loadGrid() {
            var ds = {
                type: "json",
                serverPaging: true,
                serverSorting: false,
                serverFiltering: true,
                pageSize: param.pageSize || 30,
                transport: {
                    read: {
                        type: "POST",
                        url: param.url,
                        dataType: "json",
                        data: param.paramData || {}
                    },
                    update: {
                        url: "",
                        dataType: "jsonp"
                    },
                    parameterMap: function (options, operation) {
                        if (operation == "read") {
                            return options;
                        }
                    }
                },
                schema: {
                    data: "Data",
                    total: "TotalCount",
                    model: {
                        id: "ProductID",
                        fields: {
                            EmployeeId: { editable: false, nullable: false },
                            //IdCardNo: { validation: { required: true } },
                            EmployeeName: { editable: false, validation: { required: true } },
                            //DepartmentName: { validation: { required: true } },
                            //Section: { validation: { required: true } },
                            //UnitPrice: { type: "number", validation: { required: true, min: 1 } },
                            //Discontinued: { type: "boolean" },
                            //UnitsInStock: { type: "number", validation: { min: 0, required: true } }
                        }
                    }
                },
                group: param.groupParam,
                sort: param.sort || {}
            }
            if (param.groupParam)
                ds["group"] = param.groupParam;
            $(param.target).find('div').eq(0).html('');
            var kGrid = $(param.target).find('div').eq(0).kendoGrid({
                dataSource: ds,
                height: '100%',
                reorderable: true,
                resizable: true,
                scrollable: true,
                navigatable: true,
                sortable: param.sortable == undefined ? true : param.sortable,
                autoSync: false,
                change: function (e) {
                    var rows = e.sender.select();
                    console.log(e.sender);
                    var SelectedItems = [];
                    $('tr').find('[type=checkbox]').prop('checked', false);
                    $('tr.k-state-selected').find('[type=checkbox]').prop('checked', true);
                    rows.each(function (e) {//absentWindow
                        var grid = $(param.target).find('div').eq(0).data("kendoGrid");
                        console.log({ $this: this });

                        var dataItem = grid.dataItem(this);
                        param.getSelectedItem(dataItem);
                        SelectedItems.push(dataItem);
                    });
                    param.SelectedItems(SelectedItems);
                },
                selectable: false,
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            startswith: "Starts with",
                            eq: "Is equal to"
                        }
                    }
                },
                groupable: param.group || false,
                columnMenu: false,
                pageable: {
                    pageSizes: true,
                    buttonCount: 10,
                    pageSizes: [50, 100, 150, 200],
                    refresh: true,
                    messages: {
                        empty: "No Record Found",
                        display: "Showing {0}-{1} from {2} data items",
                        itemsPerPage: "Items Per Page"
                    }
                },
                columns: getColumns(),
                editable: "inline"
            });


            function getColumns() {

                if (typeof (param.columns) == "function")
                    return param.columns();
                else {
                    return param.columns || [
                        { field: "Code", title: "Code" },
                        { field: "Name", title: "Name" },
                    ];
                }
            }
        }
    }
}
ko.bindingHandlers.kendoGridExport = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.kendoGridExport.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
        ko.bindingHandlers.kendoGridExport.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor(),
            exportExcell = function () {
                $(param.target).data("kendoGrid")
                    .bind("excelExport", function (e) {
                        e.workbook.fileName = param.fileName + ".xlsx";
                    }).saveAsExcel();
            },
            exportPdf = function () {
                var grid = $(param.target).data("kendoGrid")
                grid.options.pdf.fileName = param.fileName + ".pdf";
                grid.saveAsPDF();
            }

        $(element).off("click").on("click", function () {
            switch (param.type) {
                case 'xlsx':
                    exportExcell();
                    break;
                default:
                    exportPdf();
                    break;

            }
        });
    }
}
ko.bindingHandlers.kendoGridPrint = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.kendoGridPrint.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
        ko.bindingHandlers.kendoGridPrint.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        function printGrid() {
            var gridElement = $(param),
                printableContent = '',
                win = window.open('', '', 'width=800, height=500, resizable=1, scrollbars=1'),
                doc = win.document.open();

            var htmlStart =
                '<!DOCTYPE html>' +
                '<html>' +
                '<head>' +
                '<meta charset="utf-8" />' +
                '<title>Kendo UI Grid</title>' +
                '<link href="http://kendo.cdn.telerik.com/' + kendo.version + '/styles/kendo.common.min.css" rel="stylesheet" /> ' +
                '<style>' +
                'html { font: 11pt sans-serif; }' +
                '.k-grid { border-top-width: 0; }' +
                '.k-grid, .k-grid-content { height: auto !important; }' +
                '.k-grid-content { overflow: visible !important; }' +
                'div.k-grid table { table-layout: auto; width: 100% !important; }' +
                '.k-grid .k-grid-header th { border-top: 1px solid; }' +
                '.k-grouping-header, .k-grid-toolbar, .k-grid-pager > .k-link { display: none; }' +
                // '.k-grid-pager { display: none; }' + // optional: hide the whole pager
                '</style>' +
                '</head>' +
                '<body>';

            var htmlEnd =
                '</body>' +
                '</html>';

            var gridHeader = gridElement.children('.k-grid-header');
            if (gridHeader[0]) {
                var thead = gridHeader.find('thead').clone().addClass('k-grid-header');
                printableContent = gridElement
                    .clone()
                    .children('.k-grid-header').remove()
                    .end()
                    .children('.k-grid-content')
                    .find('table')
                    .first()
                    .children('tbody').before(thead)
                    .end()
                    .end()
                    .end()
                    .end()[0].outerHTML;
            } else {
                printableContent = gridElement.clone()[0].outerHTML;
            }

            doc.write(htmlStart + printableContent + htmlEnd);
            doc.close();
            win.print();
        }
        $(element).off('click').on('click', function () {
            printGrid();
        });

    }
}
ko.bindingHandlers.kendoTreeViewInDialog = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.kendoTreeViewInDialog.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        var treeTarget = $(param.treeTarget);
        var ds = param.dataSource;

        var dialog = $(param.dialogTarget);
        dialog.kendoDialog({
            width: "400px",
            title: "Permission Management",
            visible: false,
            actions: [
                {
                    text: 'Cancel',
                    primary: false,
                    action: onCancelClick
                },
                {
                    text: 'Ok',
                    primary: true,
                    action: onOkClick
                }
            ],
            close: onClose
        }).data("kendoDialog");

        $(element).off('click').on('click', function () {
            dialog.data("kendoDialog").open();
            var kendoTree = $(treeTarget).kendoTreeView({
                checkboxes: {
                    checkChildren: true
                },
                dataSource: new kendo.data.HierarchicalDataSource({
                    data: ds
                }),
                check: onCheck,
                //expand: onExpand
            });
            kendoTree.data("kendoTreeView").dataSource.read();
        });

        function onCancelClick(e) {
            e.sender.close();
        }

        function onOkClick(e) {
            var checkedNodes = [];
            var treeView = $("#treeview").data("kendoTreeView");

            getCheckedNodes(treeView.dataSource.view(), checkedNodes);
            console.log({ checkedNodes: checkedNodes });
            // populateMultiSelect(checkedNodes);


            e.sender.close();
        }

        function onClose() {
            $("#openWindow").fadeIn();
        }
        function checkUncheckAllNodes(nodes, checked) {
            for (var i = 0; i < nodes.length; i++) {
                nodes[i].set("checked", checked);

                if (nodes[i].hasChildren) {
                    checkUncheckAllNodes(nodes[i].children.view(), checked);
                }
            }
        }

        function chbAllOnChange() {
            var checkedNodes = [];
            var treeView = $("#treeview").data("kendoTreeView");
            var isAllChecked = $('#chbAll').prop("checked");

            checkUncheckAllNodes(treeView.dataSource.view(), isAllChecked)

            if (isAllChecked) {
                //setMessage($('#treeview input[type="checkbox"]').length);
            }
            else {
                //setMessage(0);
            }
        }

        function getCheckedNodes(nodes, checkedNodes) {
            var node;

            for (var i = 0; i < nodes.length; i++) {
                node = nodes[i];

                if (node.checked) {
                    checkedNodes.push({ text: node.text, id: node.id });
                }

                if (node.hasChildren) {
                    getCheckedNodes(node.children.view(), checkedNodes);
                }
            }
        }

        function onCheck() {
            var checkedNodes = [];
            var treeView = $("#treeview").data("kendoTreeView");

            getCheckedNodes(treeView.dataSource.view(), checkedNodes);
            //setMessage(checkedNodes.length);
        }
    }
};
ko.bindingHandlers.kendoWindow = {
    update: function (element, valueAccessor) {

        ko.bindingHandlers.kendoWindow.bind(element, valueAccessor);

        //grab the value of the parameters, making sure to unwrap anything that could be observable

    },
    init: function (element, valueAccessor) {
        // ko.bindingHandlers.kendoWindow.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        //jquery specific func
        $(param.target).hide();
        var dataWindow = $(param.target).kendoWindow({
            title: param.title,
            width: param.width || 1120,
            height: param.height || 500,
            actions: param.actions || [
                "Minimize", "Maximize", "Close"
            ],
            scrollable: true,
            resizable: true,
            draggable: true,
            open: function () {
                if (param.open) {
                    param.open();
                }
            }
        }).data("kendoWindow");
        switch (param.position) {
            case 'center':
                dataWindow.center();
                break;
            case 'left':
                break;
            case 'right':
                break;
            default:
                dataWindow.center();

        }

        //dataWindow.element.closest(".k-window").css({
        //    top: 105,
        //    left: 230
        //});
        $(element).off('click').on('click', function () {

            //if (param.autoOpen) {
            dataWindow.open();
            //}

            if (param.maximize) {
                dataWindow.maximize();
            }
        });
    }
};
ko.bindingHandlers.kendoDropDownList = {
    update: function (element, valueAccessor) {

    },
    init: function (element, valueAccessor) {
        ko.bindingHandlers.kendoDropDownList.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        //var dataSource = param.data();
        var autoComplete = $(element).kendoDropDownList({
            filter: "startswith",
            autoBind: false,

            dataTextField: param.dataTextField || 'Name',
            dataValueField: param.dataValueField || 'Id',
            //value: param.value||,
            dataSource: {
                dataType: "json",
                transport: {

                    read: {
                        url: param.url,
                        dataType: "json"
                    }
                },
                schema: {
                    data: "Data"
                }
            },

            select: function (e) {
                //param.select(e.dataItem) || {};

            }

        });
        param['kendoObj'] = autoComplete;

        param['value'] = function (value) {
            if (value)
                autoComplete.data("kendoDropDownList").value(value);
            else {

                return autoComplete.data("kendoDropDownList").value();
            }
        }
    }
};
ko.bindingHandlers.kendoAutocomplete = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.kendoAutocomplete.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
        ko.bindingHandlers.kendoAutocomplete.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        var acInstance = {};
        function createAcInstance() {
            acInstance = $(element).kendoAutoComplete({
                dataTextField: "Name",
                minLength: 3,
                height: 300,
                placeholder: param.placeholder || 'Select',
                footerTemplate: 'Total #: instance.dataSource.total() # items found',
                template: '' +//<h4>#: data.Code #</h4>
                    '<span class="k-state-default"><p>#: data.Name #</p></span><div tabindex="-1" class="dropdown-divider"></div>',
                dataSource: {
                    type: "POST",
                    dataType: "json",
                    transport: {
                        read: {
                            type: "POST",
                            url: param.url,
                            dataType: "json",
                            data: param.paramData || {}
                        },
                        parameterMap: function (options, operation) {
                            if (operation == "read") {
                                return options;
                            }
                        },
                    },
                    serverFiltering: true,
                    schema: { data: "Data" }
                },
                popup: {
                    origin: "top left",
                    position: "bottom left"
                },
                animation: {
                    open: {
                        effects: "fadeIn:up"
                    }
                },
                select: function (e) {
                    param.select(e.dataItem) || {};

                }

            });
            acInstance.data("kendoAutoComplete");
        };

        createAcInstance();


    }
};

ko.bindingHandlers.select2 = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.select2.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
        ko.bindingHandlers.select2.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        $(element).select2({
            width: '100%' // need to override the changed default
        });
    }
};

ko.bindingHandlers.select2Autocomplete = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.select2Autocomplete.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
        ko.bindingHandlers.select2Autocomplete.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        $(element).select2({

            minimumInputLength: 3,
            tags: [],
            ajax: {
                url: param.url,
                dataType: 'json',
                type: "POST",
                quietMillis: 50,
                data: function (term) {
                    return {
                        SearchParam: term.term
                    };
                },
                processResults: function (data) {
                    return {
                        results: ko.utils.arrayMap(data.Data, function (item) {
                            return {
                                text: item.Name,
                                slug: item.Code,
                                id: item.Id
                            }
                        })
                    };
                }
            }
        }).on('select2:select', function (e) {
            var data = e.params.data;
            param.select({ Id: data.id, Name: data.text, Code: data.slug });
            console.log(data);
        });

    }
};

ko.bindingHandlers.kendoMultiSelect = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.kendoMultiSelect.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        var ds = new kendo.data.DataSource({
            type: "POST",
            dataType: "json",
            transport: {
                read: {
                    type: "POST",
                    url: param.url,
                    dataType: "json",
                    data: param.paramData || {}
                },
                parameterMap: function (options, operation) {
                    if (operation == "read") {
                        return options;
                    }
                },
            },
            serverFiltering: true,
            schema: { data: "Data" }
        });
        if ($(element).data('kendoMultiSelect')) {
            $(element).data('kendoMultiSelect').destroy();
            $(element).unwrap('.k-multiselect').show().empty();
            $(".k-multiselect-wrap").remove();
        }
        param.multiSelect = $(element).kendoMultiSelect({
            dataValueField: "Id",
            dataTextField: "Name",
            autoBind: false,
            minLength: 3,
            dataSource: ds,
            select: function (e) {
                param.select(e.dataItem);
            },
            deselect: function (e) {
                param.deselect(e.dataItem);
            },
            value: param.value()
        }).data("kendoMultiSelect");

    }
};

ko.bindingHandlers.kendoTreeListGrid = {

    update: function (element, valueAccessor) {
        ko.bindingHandlers.kendoTreeListGrid.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {

    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        if (param.open) {
            param.open(loadGrid);
        }
        else {
            loadGrid();
        }
        function loadGrid() {
            var ds = new kendo.data.TreeListDataSource({
                type: "json",
                serverPaging: true,
                serverSorting: false,
                serverFiltering: true,
                pageSize: 10,
                transport: {
                    read: {
                        type: "POST",
                        url: param.url,
                        dataType: "json",
                        data: param.paramData || {}
                    },
                    parameterMap: function (options, operation) {
                        if (operation == "read") {
                            var d = new Date(options.datefld);
                            options.datefld = d.toString("yyyy/MM/dd");
                            return options;
                        }
                    },
                },
                schema: {
                    data: "Data",
                    total: "TotalCount",
                    model: {
                        id: param.Id || "Id",
                        parentId: "ParentId",
                        fields: {
                            Id: { type: "number", nullable: false },
                            ParentId: { field: param.parentId || "ParentId", nullable: true }
                        },
                        expanded: true
                    }
                },
            });
            if (param.parentId) {

            }

            if (param.groupParam)
                ds["group"] = param.groupParam;
            var kGrid = $(param.target).kendoTreeList({
                dataSource: ds,
                height: param.height || 450,
                reorderable: true,
                resizable: true,
                scrollable: true,
                navigatable: true,
                sortable: true,
                autoSync: false,
                selectable: (param.multiSelect || false) ? "multiple, row" : true,
                change: function (e) {
                    var rows = e.sender.select();
                    var SelectedItems = [];
                    $('tr').find('[type=checkbox]').prop('checked', false);
                    $('tr.k-state-selected').find('[type=checkbox]').prop('checked', true);
                    rows.each(function (e) {//absentWindow
                        var grid = $(param.target).data("kendoTreeList");
                        var dataItem = grid.dataItem(this);
                        SelectedItems.push(dataItem);
                        param.SelectedItem(dataItem);
                    });
                    param.SelectedItems(SelectedItems);
                },
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            startswith: "Starts with",
                            eq: "Is equal to"
                        }
                    }
                },
                groupable: param.group || false,
                columnMenu: false,
                pageable: {
                    pageSizes: true,
                    buttonCount: 5,
                    refresh: true,
                    messages: {
                        empty: "No Record Found",
                        display: "Showing {0}-{1} from {2} data items",
                        itemsPerPage: "Items Per Page"
                    }
                },
                columns: param.columns || [
                    { field: "Code", title: "Code" },
                    { field: "Name", title: "Name" },
                ]
            });
        }
    }
};





ko.bindingHandlers.kendoTreeGridInDialog = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.kendoTreeGridInDialog.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        var dataDailog = $(param.target).kendoDialog({
            width: param.width,
            height: param.height,
            animation: {
                close: {
                    effects: "fade:out"
                },
                open: {
                    effects: "fade:in"
                }
            },
            title: param.title,
            visible: false,
            closable: true,
            modal: true,
            actions: [
            ],
            open: function () {
                loadGrid();
            }
        }).data("kendoDialog");

        $(element).off('click').on('click', function () {
            dataDailog.open();
        });
        function loadGrid() {
            var ds = new kendo.data.TreeListDataSource({
                type: "json",
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
                pageSize: param.pageSize || 30,
                transport: {
                    read: {
                        type: "POST",
                        url: param.url,
                        dataType: "json",
                        data: param.paramData || {}
                    },
                    parameterMap: function (options, operation) {
                        if (operation == "read") {
                            return options;
                        }
                    },
                },
                schema: {
                    data: "Data",
                    total: "TotalCount",
                    model: {
                        id: "Code",
                        parentId: "ParentId",
                        fields: {
                            Code: { type: "string", nullable: false },
                            ParentId: { field: "ParentId", nullable: true }
                        },
                        expanded: true
                    }
                },
                aggregate: [
                    { field: "Debit", aggregate: "sum" },
                    { field: "Credit", aggregate: "sum" }
                ]
            })
            $(param.target).find('div').eq(0).html('');
            var kGrid = $(param.target).find('div').eq(0).kendoTreeList({
                toolbar: ["pdf", "excel"],
                pdf: {
                    allPages: true,
                    avoidLinks: true,
                    paperSize: "A4",
                    margin: { top: "5px", left: "5px", right: "5px", bottom: "5px" },
                    landscape: true,
                    repeatHeaders: true,
                    scale: 0.8
                },
                dataSource: ds,
                height: '100%',
                reorderable: true,
                resizable: true,
                scrollable: true,
                navigatable: true,
                sortable: param.sortable == undefined ? true : param.sortable,
                autoSync: false,
                selectable: false,
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            startswith: "Starts with",
                            eq: "Is equal to"
                        }
                    }
                },
                groupable: false,
                columnMenu: false,
                pageable: {
                    pageSizes: true,
                    buttonCount: 10,
                    pageSizes: [50, 100, 150, 200],
                    refresh: true,
                    messages: {
                        empty: "No Record Found",
                        display: "Showing {0}-{1} from {2} data items",
                        itemsPerPage: "Items Per Page"
                    }
                },
                columns: getColumns()
            });


            function getColumns() {

                if (typeof (param.columns) == "function")
                    return param.columns();
                else {
                    return param.columns || [
                        { field: "Code", title: "Code" },
                        { field: "Name", title: "Name" },
                    ];
                }
            }
        }
    }
}
//kendo pivot grid in dialog
ko.bindingHandlers.kendoPivotGridInDialog = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.kendoPivotGridInDialog.bind(element, valueAccessor);
    },
    init: function (element, valueAccessor) {
        ko.bindingHandlers.kendoPivotGridInDialog.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        $(element).off('click').on('click', function () {
            var dataDailog = $(param.target).kendoDialog({
                width: param.width || $(document).width() - 10,
                height: param.height - 2,
                animation: {
                    close: {
                        effects: "fade:out"
                    },
                    open: {
                        effects: "fade:in"
                    }
                },
                title: param.title,
                toolbar: ["Excel"],
                visible: false,
                closable: true,
                modal: true,
                open: function () {
                }
            }).data("kendoDialog");
            dataDailog.open();
            loadPivotGrid();
        });
        function loadPivotGrid() {
            var ds = {
                type: "json",
                serverPaging: true,
                serverSorting: false,
                serverFiltering: true,
                pageSize: param.pageSize || 30,
                transport: {
                    read: {
                        type: "POST",
                        url: param.url,
                        dataType: "json",
                        data: param.paramData || {}
                    },
                    parameterMap: function (options, operation) {
                        if (operation == "read") {
                            return options;
                        }
                    },
                },
                schema: {
                    data: "Data",
                    total: "TotalCount",
                    model: { fields: param.modelFields } || { fields: {} },
                    cube: {
                        dimensions: param.cubeDimensions || {}, measures: {
                            "Sum": { field: param.measureField, format: "{0}", aggregate: "sum" }
                        }
                    }
                },
                columns: param.columns || [],
                rows: param.rows || [],
                measures: ["Sum"]
            }
            try {
                $(param.target).data().kendoPivotGrid.destroy();
                $(param.target).empty();
            }
            catch (err) {
            }

            var kGrid = $(param.target).kendoPivotGrid({
                dataSource: ds,
                height: param.gridHeight || 570,
                reorderable: true,
                resizable: true,
                scrollable: true,
                navigatable: true,
                sortable: param.sortable == undefined ? true : param.sortable,
                autoSync: false,
                change: function (e) {
                    var rows = e.sender.select();
                    console.log(e.sender);
                    var SelectedItems = [];
                    $('tr').find('[type=checkbox]').prop('checked', false);
                    $('tr.k-state-selected').find('[type=checkbox]').prop('checked', true);
                    rows.each(function (e) {//absentWindow
                        var grid = $(param.target).find('div').eq(0).data("kendoGrid");
                        console.log({ $this: this });

                        var dataItem = grid.dataItem(this);
                        param.getSelectedItem(dataItem);
                        SelectedItems.push(dataItem);
                    });
                    param.SelectedItems(SelectedItems);
                },
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            startswith: "Starts with",
                            eq: "Is equal to"
                        }
                    }
                },
                groupable: param.group || true,
                columnMenu: false,
                pageable: {
                    pageSizes: true,
                    buttonCount: 10,
                    pageSizes: [50, 100, 150, 200],
                    refresh: true,
                    messages: {
                        empty: "No Record Found",
                        display: "Showing {0}-{1} from {2} data items",
                        itemsPerPage: "Items Per Page"
                    }
                }
            });
        }
    }
}
ko.bindingHandlers.viewBase64 = {
    init: function (element, valueAccessor) {
        $(element).click(function () {
            Riddha.util.viewBase64(valueAccessor());
        });
    },
    update: function (element, valueAccessor) {
        $(element).click(function () {
            Riddha.util.viewBase64(valueAccessor());
        });
    },
}

ko.bindingHandlers.fileUpload = {
    init: function (element, valueAccessor) {
        ko.bindingHandlers.fileUpload.bind(element, valueAccessor);
    },
    update: function (element, valueAccessor) {
        ko.bindingHandlers.fileUpload.bind(element, valueAccessor);
    },
    bind: function (element, valueAccessor) {
        var param = valueAccessor();
        switch (param.type || '') {
            case 'image':
                $(element).attr('accept', "image/*");
                break;
            case 'excel':
                $(element).attr('accept', ".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel");
                break;
            case 'pdf':
                $(element).attr('accept', "application/pdf");

            default:
                break;
        }
        $(element).off('change').on('change', function (e) {
            var file = e.target.files[0];
            var reader = new FileReader();

            //ajaxfile upload code
            //
            var data = new FormData();
            files = e.target.files;
            $.each(files, function (key, value) {
                data.append(key, value);
            });
            $.ajax({
                // Your server script to process the upload
                url: '/fileupload',
                type: 'POST',

                // Form data
                data: data,
                cache: false,
                dataType: 'json',
                processData: false, // Don't process the files
                contentType: false, // Set content type to false as jQuery will tell the server its a query string request
                success: function (data, textStatus, jqXHR) {
                    if (typeof data.error === 'undefined') {
                        // Success so call function to process the form
                        param.value(data);
                        //$(param.target()).attr('src', data);
                    }
                    else {
                        // Handle errors here
                        console.log('ERRORS: ' + data.error);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    // Handle errors here
                    console.log('ERRORS: ' + textStatus);
                    // STOP LOADING SPINNER
                }
            });
        });
    }
};
ko.bindingHandlers.FileChooser = {
    init: function (element, valueAccessor) {
        $(element).each(function () {
            var name = $(this).attr("name");
            if (valueAccessor() !== "") {
                var temp = valueAccessor().toLowerCase();
                if (temp.endsWith(".swf")) {
                    swfobject.embedSWF($(this).val(), "ckfinder-swf-preview-" + name, "100%", "100%", "9.0.0", false, false, { wmode: "transparent" }, false, false);
                    $("#ckfinder-swf-preview-" + name).removeClass("hidden");
                } else if (temp.endsWith(".jpg") || temp.endsWith(".jpeg") || temp.endsWith(".gif") || temp.endsWith(".png")) {
                    $("#ckfinder-img-preview-" + name).attr("src", $(this).val()).removeClass("hidden");
                } else {
                    $("#ckfinder-file-preview-" + name).attr("href", $(this).val()).removeClass("hidden");
                }
            }
        })
    },
    update: function (element, valueAccessor) {

    }
};


ko.bindingHandlers.RichEditor = {
    init: function (element, valueAccessor) {
        $(element).wysihtml5({
            events: {
                "change": function (value) {
                    //var observable;
                    //var content = ko.utils.unwrapObservable(valueAccessor()) || {};

                    //if (content.data != undefined) {
                    //    observable = valueAccessor().data;
                    //} else {
                    //    observable = valueAccessor();
                    //}

                    //observable(control.getValue());
                    valueAccessor()($('iframe').contents().find('.wysihtml5-editor').html());
                }
            }
        });
        //var editor = new wysihtml5();
        console.log('');

        $(element).change(function () {
            alert('found');
            valueAccessor().data('updated');
        });
    },
    update: function (element, valueAccessor) {
        $('iframe').contents().find('.wysihtml5-editor').html(valueAccessor()());
    }
};

ko.bindingHandlers.mask = {
    init: function (element, valueAccessor) {
        var type = valueAccessor();
        ko.bindingHandlers.mask.bindFormating(type, element);


    },
    update: function (element, valueAccessor) {
        var type = valueAccessor();
        ko.bindingHandlers.mask.bindFormating(type, element);
    },

    bindFormating: function (type, element) {
        switch (type.toLowerCase()) {
            case 'date': "";
                $(element).attr('placeholder', "yyyy/mm/dd");
                $(element).datepicker({
                    language: 'np', format: "yyyy/mm/dd",

                }).on('changeDate', function (e) {
                    var value = Riddha.util.formatDate(e.date);
                    var data = AD2BS(value);
                    $($(element).data('nepdate')).val(data);
                    // `e` here contains the extra attributes
                });//{ format: "yyyy/mm/dd", autoclose: true, showOnFocus: false }
                //$(".nepDate").nepaliDatePicker();
                $(element).inputmask("9999/99/99").attr('placeholder', "yyyy/mm/dd");

                break;
            case 'time': "";
                $(document).ready(function () {
                    $(element).inputmask("99:99", { "placeholder": "hh:mm" });
                });
            case 'ipaddress': "";
                //$(element).attr('placeholder', "___.___.___");
                //$(element).mask('0ZZ.0ZZ.0ZZ.0ZZ', { translation: { 'Z': { pattern: /[0-9]/, optional: true } } });
                break;
            case 'amount':
                $(element).on("keydown", function (event) {
                    if (event.shiftKey == true) {
                        event.preventDefault();
                    }
                    var code = parseInt(event.keyCode);
                    if (code >= 48 && code <= 57 || code >= 96 && code <= 105)
                        return true;
                    if (code == 8 || code == 9 || code == 46 || code == 9 || (code >= 37 && code <= 40))
                        return true;
                    if (code == 110 || code == 190) {
                        if ($(this).val().indexOf(".") >= 0)
                            event.preventDefault();
                        else
                            return true;
                    }
                    event.preventDefault();
                });
                break;
            case 'email':
                $(element).inputmask('email');
                break;
            case 'number':
                $(element).on("keydown", function (event) {
                    if (event.shiftKey == true) {
                        // event.preventDefault();
                    }
                    var code = parseInt(event.keyCode);
                    if (code == 13)
                        return true;
                    if (code >= 48 && code <= 57 || code >= 96 && code <= 105)
                        return true;
                    if (code == 8 || code == 46 || code == 9 || (code >= 37 && code <= 40))
                        return true;
                    if (code == 110 || code == 190) {
                        event.preventDefault();
                        return true;
                    }
                    event.preventDefault();
                });
                break;
            case 'url': break;
            default:

        }
    }
};
ko.bindingHandlers.dateNp = {
    init: function (element, valueAccessor) {
        ko.bindingHandlers.dateNp.bind(element, valueAccessor);
    },
    update: function (element, valueAccessor) {
        ko.bindingHandlers.dateNp.bind(element, valueAccessor);

    },
    bind: function (element, valueAccessor) {
        var engDateObj = valueAccessor();
        $(element).attr('placeholder', "yyyy/mm/dd");
        $(element).inputmask("9999/99/99").attr('placeholder', "yyyy/mm/dd");
        var eng = engDateObj.engDate;
        var nepToEngMode = false;
        //nep to eng
        $(element).nepaliDatePicker({
            changeYear: true, changeMonth: true,
            npdMonth: true,
            npdYear: true,
            npdYearCount: 200,
            onChange: function (d, e) {
                eng(Riddha.util.formatDate(new Date(BS2AD($(element).val()))));

            }
        });
        $(element).off('keyup').on('keyup', function () {
            var value = $(element).val();
            if (value.indexOf('_') == -1) {
                eng(Riddha.util.formatDate(new Date(BS2AD(value))));
            }
        });



        if (engDateObj.engDate().indexOf('N') == -1 || engDateObj.engDate().length != 0) {
            var dt = new Date(engDateObj.engDate());
            if (dt != 'Invalid Date') {
                var data = AD2BS(engDateObj.engDate());
                $(element).val(data);
            }
        }
        //eng to nep

    }
}

ko.extenders.date = function (target, overrideMessage) {
    var date = new Date(target());
    target(Riddha.util.formatDate(date));
}

ko.extenders.Percent = function (target, overrideMessage) {
    var value = target();
    target.subscribe(function (newValue) {
        if (newValue > 100 || newValue < 0) {
            return target(0)
        }
    });
    return target();
}

ko.extenders.dateJSON = function (target, overrideMessage) {
    var date = new Date(parseInt(target().substr(6)));
    var day = Riddha.util.padLeft(date.getDate(), 2);
    var monthIndex = Riddha.util.padLeft(date.getMonth() + 1, 2);
    var year = date.getFullYear();
    return day + '/' + (monthIndex) + '/' + year;
}
ko.extenders.required = function (target, overrideMessage) {
    //add some sub-observables to our observable
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();

    //define a function to do validation
    function validate(newValue) {
        target.hasError(newValue ? false : true);
        target.validationMessage(newValue ? "" : overrideMessage || "This field is required");
    }

    //initial validation
    validate(target());

    //validate whenever the value changes
    target.subscribe(validate);

    //return the original observable
    return target;
};

//for serial number in nepali font and english
ko.bindingHandlers.serialNo = {
    init: function (element, valueAccessor) {
        var currentLanguage = new Riddha.config().CurrentLanguage;
        if (currentLanguage == 'ne') {
            $(element).text(riddhaConfigToUnicode(valueAccessor().toString()))
        }
        else
            $(element).text(valueAccessor());

    },
    update: function (element, valueAccessor) {
    }

}


ko.extenders.isValidDate = function (target, overrideMessage) {
    //add some sub-observables to our observable
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();

    //define a function to do validation
    function validate(newValue) {
        target.hasError(ko.validation.utils.isEmptyVal(val) || moment(val, 'YYYYY/MM/DD').isValid());
        target.validationMessage(target.hasError() ? "" : overrideMessage || 'Invalid date');
    }

    //initial validation
    validate(target());

    //validate whenever the value changes
    target.subscribe(validate);

    //return the original observable
    return target;
};
//convert preeti to unicode
ko.bindingHandlers.PreetiToUnicode = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        $(element).keyup(function (event) {
            var result = convert_to_unicode($(element).val());
            $(element).val(result);
            value(result);
        });
    },
    update: function (element, valueAccessor, viewModel) {
        $(element).val(valueAccessor()());
    }
};
//update
function checkBoxModel(item) {
    var self = this;
    item = item || {};
    self.Checked = ko.observable(item.Checked || false);
    self.Id = ko.observable(item.Id || 0);
    self.Name = ko.observable(item.Name || '');
}
function GlobalDropdownModel(item) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.Id || 0);
    self.Name = ko.observable(item.Name || '');
    self.Code = ko.observable(item.Code || '');
    self.NameNp = ko.observable(item.NameNp || '');
}


Riddha.init();

function bindNepaliDate(element) {
    //nep to eng
    var $this = $(element)[0];
    $(element).nepaliDatePicker({
        changeYear: true, changeMonth: true,
        npdMonth: true,
        npdYear: true,

        onChange: function (d, e) {
            var data = ko.dataFor($this);
            nepToEngMode = true;
            data.DateTime(BS2AD($(element).val()));
        }
    });
}

function SuitableMoney(number) {
    if (config.CurrentLanguage == "ne") {
        return SuitableNumber(Riddha.util.FormatMoney(number));
    } else {
        return Riddha.util.FormatMoney(number);
    }
};
function SuitableDate(date) {
    if (date == "Not Approved") {
        return date;
    }
    var config = new Riddha.config();
    if (config.CurrentOperationDate == 'ne') {
        var convertedDate = AD2BS(date);
        if (config.CurrentLanguage == "ne") {
            return GetNepaliUnicodeNumber(convertedDate);
        }
        else {
            return convertedDate;
        }
    }
    if (config.CurrentLanguage == "ne") {
        return GetNepaliUnicodeNumber(date);
    }
    else {
        return date;
    }
}

function SuitableNumber(field) {
    var config = new Riddha.config();
    field = typeof (field) == "function" ? field() : field;
    if (config.CurrentLanguage == "ne") {

        return GetNepaliUnicodeNumber(field + "");
    }
    else {
        return field;
    }
}


function SuitableRemarks(field) {
    var config = new Riddha.config();
    field = typeof (field) == "function" ? field() : field;
    if (config.CurrentLanguage == "ne") {
        switch (field) {
            case "Absent":
                return field = "अनुपस्थित"
                break;
            case "Present":
                return field = "उपस्थित"
                break;
            case "Holiday":
                return field = "सार्वजनिक बिदा"
                break;
            case "Misc":
                return field = "विविध"
                break;
            case "Weekend":
                return field = "सप्ताह"
                break;
            case "Leave":
                return field = "बिदा"
                break;
            case "Office Visit":
                return field = "काज"
                break;
            default:
                field = field;

        }
        return field;
    }
    else {
        return field;
    }
}

function SuitableRemarksAlias(field) {
    var config = new Riddha.config();
    field = typeof (field) == "function" ? field() : field;
    if (config.CurrentLanguage == "ne") {
        switch (field) {
            case "A":
                return field = "अनु"
                break;
            case "P":
                return field = "उ"
                break;
            case "Holiday":
                return field = "सा"
                break;
            case "Misc":
                return field = "विवि"
                break;
            case "Weekend":
                return field = "स"
                break;
            case "Leave":
                return field = "बिदा"
                break;
            case "Office Visit":
                return field = "का"
                break;
            default:
                field = field;

        }
        return field;
    }
    else {
        return field;
    }
}
function DateVisibility(dateMode) {
    var config = new Riddha.config();
    //config.CurrentOperationDate = config.CurrentOperationDate || "en";
    return config.CurrentOperationDate == dateMode;
}
function getIsActiveStatus(value) {
    var config = new Riddha.config();
    var lang = config.CurrentLanguage;
    if (value == true) {
        if (lang == "ne")

            return '<span class="badge badge-success">हो</span>';
        else
            return '<span class="badge badge-success">YES</span>';
    }
    else {
        if (lang == "ne")
            return '<span class="badge badge-danger">होईन</span>';
        else
            return '<span class="badge badge-danger">NO</span>';
    }
}
function langVisibility(languageMode) {
    var config = new Riddha.config();
    //config.CurrentOperationDate = config.CurrentOperationDate || "en";
    return config.CurrentLanguage == languageMode;
}
function ExitToHome() {
    Riddha.ajax.get("/Home/Index")
        .done(function (result) {
            $("#partial-render").html(result);
        });
}
function GetNepaliUnicodeNumber(field) {
    field = field || "";
    var result = "";
    for (var i = 0; i < field.length; i++) {
        switch (field[i]) {
            case '0':
                result = result + "०";
                break;
            case '1':
                result = result + "१";
                break;
            case '2':
                result = result + "२";
                break;
            case '3':
                result = result + "३";
                break;
            case '4':
                result = result + "४";
                break;
            case '5':
                result = result + "५";
                break;
            case '6':
                result = result + "६";
                break;
            case '7':
                result = result + "७";
                break;
            case '8':
                result = result + "८";
                break;
            case '9':
                result = result + "९";
                break;

            default:
                result = result + field[i];
                break;
        }
    }
    return result;
}


function GetAmountInWords(price) {
    var sglDigit = ["Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine"],
        dblDigit = ["Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"],
        tensPlace = ["", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"],
        handle_tens = function (dgt, prevDgt) {
            return 0 == dgt ? "" : " " + (1 == dgt ? dblDigit[prevDgt] : tensPlace[dgt])
        },
        handle_utlc = function (dgt, nxtDgt, denom) {
            return (0 != dgt && 1 != nxtDgt ? " " + sglDigit[dgt] : "") + (0 != nxtDgt || dgt > 0 ? " " + denom : "")
        };

    var str = "",
        digitIdx = 0,
        digit = 0,
        nxtDigit = 0,
        words = [];
    if (price += "", isNaN(parseInt(price))) str = "";
    else if (parseInt(price) > 0 && price.length <= 10) {
        for (digitIdx = price.length - 1; digitIdx >= 0; digitIdx--) switch (digit = price[digitIdx] - 0, nxtDigit = digitIdx > 0 ? price[digitIdx - 1] - 0 : 0, price.length - digitIdx - 1) {
            case 0:
                words.push(handle_utlc(digit, nxtDigit, ""));
                break;
            case 1:
                words.push(handle_tens(digit, price[digitIdx + 1]));
                break;
            case 2:
                words.push(0 != digit ? " " + sglDigit[digit] + " Hundred" + (0 != price[digitIdx + 1] && 0 != price[digitIdx + 2] ? " and" : "") : "");
                break;
            case 3:
                words.push(handle_utlc(digit, nxtDigit, "Thousand"));
                break;
            case 4:
                words.push(handle_tens(digit, price[digitIdx + 1]));
                break;
            case 5:
                words.push(handle_utlc(digit, nxtDigit, "Lakh"));
                break;
            case 6:
                words.push(handle_tens(digit, price[digitIdx + 1]));
                break;
            case 7:
                words.push(handle_utlc(digit, nxtDigit, "Crore"));
                break;
            case 8:
                words.push(handle_tens(digit, price[digitIdx + 1]));
                break;
            case 9:
                words.push(0 != digit ? " " + sglDigit[digit] + " Hundred" + (0 != price[digitIdx + 1] || 0 != price[digitIdx + 2] ? " and" : " Crore") : "")
        }
        str = words.reverse().join("")
    } else str = "";
    return str + " only";


}

/*plugin jquery print start*/

(function ($) {
    "use strict";
    // A nice closure for our definitions
    function getjQueryObject(string) {
        // Make string a vaild jQuery thing
        var jqObj = $("");
        try {
            jqObj = $(string)
                .clone();
        } catch (e) {
            jqObj = $("<span />")
                .html(string);
        }
        return jqObj;
    }

    function printFrame(frameWindow, content, options) {
        // Print the selected window/iframe
        var def = $.Deferred();
        try {
            frameWindow = frameWindow.contentWindow || frameWindow.contentDocument || frameWindow;
            var wdoc = frameWindow.document || frameWindow.contentDocument || frameWindow;
            if (options.doctype) {
                wdoc.write(options.doctype);
            }
            wdoc.write(content);
            wdoc.close();
            var printed = false;
            var callPrint = function () {
                if (printed) {
                    return;
                }
                // Fix for IE : Allow it to render the iframe
                frameWindow.focus();
                try {
                    // Fix for IE11 - printng the whole page instead of the iframe content
                    if (!frameWindow.document.execCommand('print', false, null)) {
                        // document.execCommand returns false if it failed -http://stackoverflow.com/a/21336448/937891
                        frameWindow.print();
                    }
                    // focus body as it is losing focus in iPad and content not getting printed
                    $('body').focus();
                } catch (e) {
                    frameWindow.print();
                }
                frameWindow.close();
                printed = true;
                def.resolve();
            }
            // Print once the frame window loads - seems to work for the new-window option but unreliable for the iframe
            $(frameWindow).on("load", callPrint);
            // Fallback to printing directly if the frame doesn't fire the load event for whatever reason
            setTimeout(callPrint, options.timeout);
        } catch (err) {
            def.reject(err);
        }
        return def;
    }

    function printContentInIFrame(content, options) {
        var $iframe = $(options.iframe + "");
        var iframeCount = $iframe.length;
        if (iframeCount === 0) {
            // Create a new iFrame if none is given
            $iframe = $('<iframe height="0" width="0" border="0" wmode="Opaque"/>')
                .prependTo('body')
                .css({
                    "position": "absolute",
                    "top": -999,
                    "left": -999
                });
        }
        var frameWindow = $iframe.get(0);
        return printFrame(frameWindow, content, options)
            .done(function () {
                // Success
                setTimeout(function () {
                    // Wait for IE
                    if (iframeCount === 0) {
                        // Destroy the iframe if created here
                        $iframe.remove();
                    }
                }, 1000);
            })
            .fail(function (err) {
                // Use the pop-up method if iframe fails for some reason
                console.error("Failed to print from iframe", err);
                printContentInNewWindow(content, options);
            })
            .always(function () {
                try {
                    options.deferred.resolve();
                } catch (err) {
                    console.warn('Error notifying deferred', err);
                }
            });
    }

    function printContentInNewWindow(content, options) {
        // Open a new window and print selected content
        var frameWindow = window.open();
        return printFrame(frameWindow, content, options)
            .always(function () {
                try {
                    options.deferred.resolve();
                } catch (err) {
                    console.warn('Error notifying deferred', err);
                }
            });
    }

    function isNode(o) {
        /* http://stackoverflow.com/a/384380/937891 */
        return !!(typeof Node === "object" ? o instanceof Node : o && typeof o === "object" && typeof o.nodeType === "number" && typeof o.nodeName === "string");
    }
    $.print = $.fn.print = function () {
        // Print a given set of elements
        var options, $this, self = this;
        // console.log("Printing", this, arguments);
        if (self instanceof $) {
            // Get the node if it is a jQuery object
            self = self.get(0);
        }
        if (isNode(self)) {
            // If `this` is a HTML element, i.e. for
            // $(selector).print()
            $this = $(self);
            if (arguments.length > 0) {
                options = arguments[0];
            }
        } else {
            if (arguments.length > 0) {
                // $.print(selector,options)
                $this = $(arguments[0]);
                if (isNode($this[0])) {
                    if (arguments.length > 1) {
                        options = arguments[1];
                    }
                } else {
                    // $.print(options)
                    options = arguments[0];
                    $this = $("html");
                }
            } else {
                // $.print()
                $this = $("html");
            }
        }
        // Default options
        var defaults = {
            globalStyles: true,
            mediaPrint: false,
            stylesheet: null,
            noPrintSelector: ".no-print",
            iframe: true,
            append: null,
            prepend: null,
            manuallyCopyFormValues: true,
            deferred: $.Deferred(),
            timeout: 750,
            title: null,
            doctype: '<!doctype html>'
        };
        // Merge with user-options
        options = $.extend({}, defaults, (options || {}));
        var $styles = $("");
        if (options.globalStyles) {
            // Apply the stlyes from the current sheet to the printed page
            $styles = $("style, link, meta, base, title");
        } else if (options.mediaPrint) {
            // Apply the media-print stylesheet
            $styles = $("link[media=print]");
        }
        if (options.stylesheet) {
            // Add a custom stylesheet if given

            $styles = $.merge($styles, $('<link rel="stylesheet" href="' + options.stylesheet + '">'));
        }
        // Create a copy of the element to print
        var copy = $this.clone();
        // Wrap it in a span to get the HTML markup string
        copy = $("<span/>")
            .append(copy);
        // Remove unwanted elements
        copy.find(options.noPrintSelector)
            .remove();
        // Add in the styles
        copy.append($styles.clone());
        // Update title
        if (options.title) {
            var title = $("title", copy);
            if (title.length === 0) {
                title = $("<title />");
                copy.append(title);
            }
            title.text(options.title);
        }
        // Appedned content
        copy.append(getjQueryObject(options.append));
        // Prepended content
        copy.prepend(getjQueryObject(options.prepend));
        if (options.manuallyCopyFormValues) {
            // Manually copy form values into the HTML for printing user-modified input fields
            // http://stackoverflow.com/a/26707753
            copy.find("input")
                .each(function () {
                    var $field = $(this);
                    if ($field.is("[type='radio']") || $field.is("[type='checkbox']")) {
                        if ($field.prop("checked")) {
                            $field.attr("checked", "checked");
                        }
                    } else {
                        $field.attr("value", $field.val());
                    }
                });
            copy.find("select").each(function () {
                var $field = $(this);
                $field.find(":selected").attr("selected", "selected");
            });
            copy.find("textarea").each(function () {
                // Fix for https://github.com/DoersGuild/jQuery.print/issues/18#issuecomment-96451589
                var $field = $(this);
                $field.text($field.val());
            });
        }
        // Get the HTML markup string
        var content = copy.html();
        // Notify with generated markup & cloned elements - useful for logging, etc
        try {
            options.deferred.notify('generated_markup', content, copy);
        } catch (err) {
            console.warn('Error notifying deferred', err);
        }
        // Destroy the copy
        copy.remove();
        if (options.iframe) {
            // Use an iframe for printing
            try {
                printContentInIFrame(content, options);
            } catch (e) {
                // Use the pop-up method if iframe fails for some reason
                console.error("Failed to print from iframe", e.stack, e.message);
                printContentInNewWindow(content, options);
            }
        } else {
            // Use a new window for printing
            printContentInNewWindow(content, options);
        }
        return this;
    };
})(jQuery);


/*plugin jquery print end*/