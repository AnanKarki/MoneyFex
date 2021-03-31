/// <reference path="../../../scripts/riddha/riddha.globals.ko.js" />
/// <reference path="../../../scripts/riddha/knockout-min3.2.0.js" />
/// <reference path="../../../scripts/riddha/knockout-3.4.2.js" />
/// <reference path="../../../scripts/riddha/knockout-2.3.0.js" />
/// <reference path="riddha.script.sendaninvoice.model.js" />


function sendAnInvoiceController(Id) {
    var url = "/KiiPayBusiness/KiiPayBusinessSendAnInvoice";
    var self = this;
    var config = new Riddha.config();
    self.Invoice = ko.observable(new InvoiceModel());
    self.InvoiceDetail = ko.observable(new InvoiceDetailModel());
    self.InvoiceDetails = ko.observableArray([]);
    self.SelectedInvoiceDetail = ko.observable();
    self.Invoices = ko.observableArray([]);
    self.ModeOfButton = ko.observable('Create');
    GetDiscountMethod();

    function GetDiscountMethod() {
        Riddha.ajax.get(url + "/GetDiscountMethod").done(function (result) {
            self.DiscountMethods(result.Data);
        });
    }
    if (Id > 0) {
        GEtInvoiceInfo();
        self.ModeOfButton = ko.observable('Update');
    }
   
    function GEtInvoiceInfo() {
        Riddha.ajax.get(url + "/GetInvoiceInfo?Id=" + Id).done(function (result) {
            self.Invoice(new InvoiceModel(result.Data.InvoiceMaster));
            self.Invoice().InvoiceDate(result.Data.InvoiceMaster.InvoiceDateToString)
            var InvoiceDetails = Riddha.ko.global.arrayMap(result.Data.InvoiceDetails, InvoiceDetailModel);
            self.InvoiceDetails(InvoiceDetails);

        })
    }

    self.GetAmount = function () {

        self.InvoiceDetail().Amount(self.InvoiceDetail().Quantity() * self.InvoiceDetail().Price());
    }

    self.Invoice().TotalAmount = ko.computed(function () {

        return Number(self.Invoice().Subtotal()) - Number(self.Invoice().DiscountAmount()) + Number(self.Invoice().Shipping());

    });


    self.DiscountMethods = ko.observableArray([
        { Id: 0, Name: "%" },
        { Id: 1, Name: "£" },
    ]);

  
    self.ErrorMessage = ko.observable("");

    self.IsValidToInvoiceReceiverMobileNo = function (model) {
        IsValidReceiver(self.Invoice().ToInvoiceMobileNumber())
    }
    self.IsValidToInvoiceCCMobileNo = function (model) {
        IsValidReceiver(self.Invoice().ToCCInvoiceMobileNumber());
    }
       
    function IsValidReceiver(mobileNo) {
        Riddha.ajax.get(url + "/IsValidMobileNo?mobileNo=" + mobileNo).done(function (result) {

            if (result.Data == false) {

                self.ErrorMessage("Enter valid mobile no");
                $("#validationModel").modal('show');
                $("#SendInvoice").prop("disabled", "disabled");
                $("#InvoicePreview").prop("disabled", "disabled");
                return;
            } else {

                $("#SendInvoice").prop("disabled", false);
                $("#InvoicePreview").prop("disabled", false);

            }
        });
    }

    function ShowError(message) {

        self.ErrorMessage(message);
        $("#validationModel").modal('show');
    }
    self.CreateUpdateDetail = function (data) {
        if (self.InvoiceDetail().ItemName() == '') {
            ShowError("Enter Item");
            return;
        }
        if (self.InvoiceDetail().Quantity() == 0) {
            ShowError("Enter Quantity");
            return;
        }
        if (self.InvoiceDetail().Price() == 0) {
            ShowError("Enter Price");
            return;
        }
        self.InvoiceDetails.push(self.InvoiceDetail());
        self.Invoice().Subtotal(self.Invoice().Subtotal() + self.InvoiceDetail().Amount());
        self.ResetDetail();
    }
    self.ResetDetail = function () {
        self.InvoiceDetail(new InvoiceDetailModel());
    }




    self.SendInvoice = function () {
        if (self.Invoice().InvoiceNo() == "") {
            ShowError("Enter Invoice No");
            return;

        }
        if (self.Invoice().InvoiceDate() == "") {
            ShowError("Enter Invoice Date");
            return;

        }

        if (self.Invoice().ToInvoiceMobileNumber() == "") {
            ShowError("Enter Receiver Mobile No.");
            return;

        }

        if (self.InvoiceDetails().length == 0) {
            ShowError("Add Item Details ");
            return;
        }
      
        if (self.ModeOfButton() == 'Create') {
            var data = {};
            Riddha.ajax.post(url + "/SendInvoice", ko.toJS({ InvoiceMaster: self.Invoice(), InvoiceDetails: self.InvoiceDetails() }))
                .done(function (result) {
                    var Amount = result.Amount;
                    var ReceiverName = result.ReceiverName;
                    window.location.href = "/KiiPayBusiness/KiiPayBusinessSendAnInvoice/InvoiceCreateSuccess?Amount=" + Amount
                        + "&ReceiverName=" + ReceiverName;

                });
        }
        else {
            self.Invoice().Id(Id);
            Riddha.ajax.post(url + "/UpdateSendInvoice", ko.toJS({ InvoiceMaster: self.Invoice(), InvoiceDetails: self.InvoiceDetails() }))
                .done(function (result) {
                    var Amount = result.Amount;
                    var ReceiverName = result.ReceiverName;
                    window.location.href = "/KiiPayBusiness/KiiPayBusinessSendAnInvoice/InvoiceCreateSuccess?Amount=" + Amount
                        + "&ReceiverName=" + ReceiverName;

                });
        }

    };
    self.ShowInvoicePreviewModel = function () {
        $("#InvoicePreviewModel").modal('show');
    }
    self.CloseInvoicePreviewModel = function () {
        $("#InvoicePreviewModel").modal('hide');
    }

}