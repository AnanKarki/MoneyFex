using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class PaymentSelectionServices
    {
        DB.FAXEREntities dbContext = null;
        public PaymentSelectionServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<PaymentSelection> List()
        {
            var data = dbContext.PaymentSelection.ToList();
            return data;
        }

        public List<PaymentSelectionViewModel> GetPaymentSelectionList(string sendingCountry, string receivingCountry)
        {
            var data = List();
            if (!string.IsNullOrEmpty(sendingCountry))
            {
                data = data.Where(x => x.SendingCountry == sendingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(receivingCountry))
            {
                data = data.Where(x => x.ReceivingCountry == receivingCountry).ToList();
            }
            var result = (from c in data
                          select new PaymentSelectionViewModel()
                          {
                              Id = c.Id,
                              ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
                              PaymentMethodName = Common.Common.GetEnumDescription(c.PaymentMethod),
                              PaymentMethod = c.PaymentMethod,
                              ReceivingCountryFlag = c.ReceivingCountry.ToLower(),
                              SendingCountryFlag = c.SendingCountry.ToLower()
                          }).ToList();

            return result;
        }

        internal void Delete(int id)
        {
            var data = List().Where(x => x.Id == id).FirstOrDefault();
            dbContext.PaymentSelection.Remove(data);
            dbContext.SaveChanges();
        }

        internal PaymentSelectionViewModel PaymentSelection(int id)
        {
            var data = List().Where(x => x.Id == id);

            var result = (from c in data
                          select new PaymentSelectionViewModel()
                          {
                              Id = c.Id,
                              ReceivingCountry = c.ReceivingCountry,
                              SendingCountry = c.SendingCountry,
                              PaymentMethod = c.PaymentMethod
                          }).FirstOrDefault();

            return result;
        }

        internal void AddPaymentSelection(PaymentSelectionViewModel vm)
        {
            PaymentSelection model = new PaymentSelection()
            {
                ReceivingCountry = vm.ReceivingCountry,
                SendingCountry = vm.SendingCountry,
                PaymentMethod = vm.PaymentMethod
            };
            dbContext.PaymentSelection.Add(model);
            dbContext.SaveChanges();

        }

        internal void UpdatePaymentSelection(PaymentSelectionViewModel vm)
        {
            var data = List().Where(x => x.Id == vm.Id).FirstOrDefault();
            data.ReceivingCountry = vm.ReceivingCountry;
            data.SendingCountry = vm.SendingCountry;
            data.PaymentMethod = vm.PaymentMethod;

            dbContext.Entry<PaymentSelection>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
    }
}