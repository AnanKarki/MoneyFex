using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMFTCCardInterMoneyTransferServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<ViewMFTCCardInterMoneyTransferViewModel> getList (string CountryCode = "", string City = "")
        {
            var sendingUser = new List<DB.KiiPayPersonalWalletInformation>();

            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                sendingUser = dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false && x.CardUserCountry == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                sendingUser = dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false && x.CardUserCity.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                sendingUser = dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false && x.CardUserCity.ToLower() == City.ToLower() && x.CardUserCountry == CountryCode).ToList();
            }
            else
            {
                sendingUser = dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false ).ToList();
            }

            var result = (from c in dbContext.MFTCCardInterMoneyTransfered.ToList()
                          join receiver in dbContext.KiiPayPersonalWalletInformation on c.ReceivingKiiPayWalletId equals receiver.Id
                          //join sender in dbContext.MFTCCardInformation on c.SendingMFTCCardId equals sender.Id
                          join sender in sendingUser on c.SendingKiiPayWalletId equals sender.Id
                          select new ViewMFTCCardInterMoneyTransferViewModel()
                          {
                              Id = c.Id,
                              FaxerId = c.SenderId,
                              FaxerName = c.Sender.FirstName + " " + c.Sender.MiddleName + " " + c.Sender.LastName,
                              FaxerAddress = c.Sender.Address1 + " " + c.Sender.City,
                              FaxerCountry = CommonService.getCountryNameFromCode(c.Sender.Country),
                              FaxerTelephone = CommonService.getPhoneCodeFromCountry(c.Sender.Country) + c.Sender.PhoneNumber,
                              FaxerEmail = c.Sender.Email,
                              FaxingMFTCNumber = sender.MobileNo.Contains("MF") ? sender.MobileNo : sender.MobileNo.Decrypt() ,
                              FaxingMFTCName = sender.FirstName + " " + sender.MiddleName + " " + sender.LastName,
                              FaxingMFTCCountry = CommonService.getCountryNameFromCode(sender.CardUserCountry),
                              FaxingMFTCAmount = sender.CurrentBalance,
                              ReceivingMFTCNumber = receiver.MobileNo.Contains("MF") ? receiver.MobileNo : receiver.MobileNo.Decrypt(),
                              ReceivingMFTCName = receiver.FirstName + " " + receiver.MiddleName + " " + receiver.LastName,
                              ReceivingMFTCCountry = CommonService.getCountryNameFromCode(receiver.CardUserCountry),
                              ReceivingMFTCAmount = receiver.CurrentBalance,
                              Date = c.TransactionDate.ToFormatedString(),
                              Time = c.TransactionDate.ToString("HH:mm"),
                              AmountTransferred = c.AmountTransfered
                          }).OrderByDescending(x=>x.Date).ToList();
            return result;
        }



    }
}