using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class CustomerPaymentFeeServices
    {
        DB.FAXEREntities dbContext = null;
        public CustomerPaymentFeeServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<CustomerPaymentFee> List()
        {
            var data = dbContext.CustomerPaymentFee.ToList();
            return data;
        }

        public List<CustomerPaymentFeeViewModel> GetCoustmerPaymentFeeList(string country)
        {
            var data = List();
            if (!string.IsNullOrEmpty(country))
            {
                data = data.Where(x => x.Country == country).ToList();
            }
            var result = (from c in data
                          select new CustomerPaymentFeeViewModel()
                          {
                              BankTransfer = c.BankTransfer,
                              Country = Common.Common.GetCountryName(c.Country),
                              CreditCard = c.CreditCard,
                              DebitCard = c.DebitCard,
                              KiiPayWallet = c.KiiPayWallet,
                              Id = c.Id,
                              CountryFlag = c.Country.ToLower(),
                          }).ToList();
            return result;


        }

        public void DeleteCustomerPaymentFee(int id)
        {
            var data = List().Where(x => x.Id == id).FirstOrDefault();
            dbContext.CustomerPaymentFee.Remove(data);
            dbContext.SaveChanges();

        }

        public CustomerPaymentFeeViewModel GetCoustmerPaymentFee(int id, string country)
        {
            var data = List();
            if (id != 0)
            {
                data = data.Where(x => x.Id == id).ToList();
            }
            if (!string.IsNullOrEmpty(country))
            {
                data = data.Where(x => x.Country == country).ToList();
            }
            var result = (from c in data
                          select new CustomerPaymentFeeViewModel()
                          {
                              BankTransfer = c.BankTransfer,
                              Country = c.Country,
                              CreditCard = c.CreditCard,
                              DebitCard = c.DebitCard,
                              KiiPayWallet = c.KiiPayWallet,
                              Id = c.Id
                          }).FirstOrDefault();

            return result;


        }

        internal void Update(CustomerPaymentFeeViewModel vm)
        {
            var data = List().Where(x => x.Id == vm.Id).FirstOrDefault();
            data.KiiPayWallet = vm.KiiPayWallet;
            data.BankTransfer = vm.BankTransfer;
            data.CreditCard = vm.CreditCard;
            data.DebitCard = vm.DebitCard;
            data.Country = vm.Country;

            dbContext.Entry<CustomerPaymentFee>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        internal void Add(CustomerPaymentFeeViewModel vm)
        {
            CustomerPaymentFee model = new CustomerPaymentFee()
            {

                Country = vm.Country,
                BankTransfer = vm.BankTransfer,
                CreditCard = vm.CreditCard,
                KiiPayWallet = vm.KiiPayWallet,
                DebitCard = vm.DebitCard,
            };
            dbContext.CustomerPaymentFee.Add(model);
            dbContext.SaveChanges();
        }
    }
}