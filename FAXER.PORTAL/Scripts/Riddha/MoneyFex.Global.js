/// <reference path="riddha.globals.ko.js" />
function TransferSummaryRequestParamModel(item) {
    var self = this;
    item = item || {};
    self.SendingAmount = ko.observable(item.SendingAmount || 0);
    self.ReceivingAmount = ko.observable(item.ReceivingAmount || 0);
    self.SendingCountry = ko.observable(item.SendingCountry || '');
    self.ReceivingCountry = ko.observable(item.ReceivingCountry || '');
    self.SendingCurrency = ko.observable(item.SendingCurrency || '');
    self.ReceivingCurrency = ko.observable(item.ReceivingCurrency || '');
    self.IsReceivingAmount = ko.observable(item.IsReceivingAmount || false);
    self.TransferMethod = ko.observable(item.TransferMethod || undefined);
}

var MoneyFex = {

    init: function () {

    },
    param:
    {
        url: "",

    },
    // Model Should be EstimateTransferSummaryRequestModel type model
    EstimateTransferSummary: function (model)
    {
        debugger;
        Riddha.ajax.post("/EstimationSummary/GetTransferSummary", model)
            .done(function (result) {

                return result;

        });
    }


}