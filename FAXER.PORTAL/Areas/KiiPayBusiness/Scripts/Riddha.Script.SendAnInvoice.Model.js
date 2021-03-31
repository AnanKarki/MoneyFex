function InvoiceModel(item) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.Id || 0);
    self.InvoiceNo = ko.observable(item.InvoiceNo || '');
    self.InvoiceDate = ko.observable(item.InvoiceDate || '');
    self.ToInvoiceMobileNumber = ko.observable(item.ToInvoiceMobileNumber || '');
    self.ToCCInvoiceMobileNumber = ko.observable(item.ToCCInvoiceMobileNumber || '');
    self.NoteToReceipient = ko.observable(item.NoteToReceipient || '');
    self.Discount = ko.observable(item.Discount || '');
    //self.DiscountAmount = ko.observable(item.DiscountAmount || 0);
    self.DiscountMethodId = ko.observable(item.DiscountMethodId || 0);
    self.Subtotal = ko.observable(item.Subtotal || 0);
    self.Shipping = ko.observable(item.Shipping || 0);
    self.TotalAmount = ko.observable(item.TotalAmount || 0);
    self.CountryCode = ko.observable(item.CountryCode || '');
    self.AmountDue = ko.observable(item.AmountDue || 0);
    self.ReciverBusinessName = ko.observable(item.ReciverBusinessName || '');

    self.DiscountAmount = ko.computed(function () {

        if (self.DiscountMethodId() == 0) {

            var discountAmount = (self.Discount() / 100) * self.Subtotal();
            return discountAmount;
        } else {
            return self.Discount();

        }
    });
}

function InvoiceDetailModel(item) {
    var self = this;
    item = item || {};
    self.ItemName = ko.observable(item.ItemName || '');
    self.Quantity = ko.observable(item.Quantity || '');
    self.Price = ko.observable(item.Price || '');
    self.Amount = ko.observable(item.Amount || '');
    self.CurrencySymbol = ko.observable(item.CurrencySymbol || '');
}
function DropDown(item) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.Id || 0);
    self.Name = ko.observable(item.Name || '');

}