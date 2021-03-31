using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class FormNonCardUserMoneyFaxedUpdateServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public FormNonCardUserMoneyFaxedUpdateViewModel getFaxerInfo (string MFCNNo)
        {
            if (string.IsNullOrEmpty(MFCNNo))
            {
                return null;
            }

            var data = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCNNo).FirstOrDefault();
            if (data == null) {

                return null;
            }

            FormNonCardUserMoneyFaxedUpdateViewModel vm = new FormNonCardUserMoneyFaxedUpdateViewModel();
            //string card = MFTCCard.Encrypt();

            vm.FormNonCardUserFaxerDetails = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCNNo).ToList()
                                              join d in dbContext.Country on c.NonCardReciever.FaxerInformation.Country equals d.CountryCode
                                              select new FormNonCardUserFaxerDetailsViewModel()
                                              {
                                                  FaxerId = c.NonCardReciever.FaxerInformation.Id,
                                                  FaxerFirstName = c.NonCardReciever.FaxerInformation.FirstName,
                                                  FaxerMiddleName = c.NonCardReciever.FaxerInformation.MiddleName,
                                                  FaxerLastName = c.NonCardReciever.FaxerInformation.LastName,
                                                  FaxerIDCardNumber = c.NonCardReciever.FaxerInformation.IdCardNumber,
                                                  FaxerIDCardType = c.NonCardReciever.FaxerInformation.IdCardType,
                                                  FaxerIDCardExpDate = c.NonCardReciever.FaxerInformation.IdCardExpiringDate.ToString("dd-MM-yyyy"),
                                                  FaxerAddress = c.NonCardReciever.FaxerInformation.Address1,
                                                  FaxerCity = c.NonCardReciever.FaxerInformation.City,
                                                  FaxerCountryCode = c.NonCardReciever.FaxerInformation.Country,
                                                  FaxerCountry = d.CountryName,
                                                  FaxerEmailAddress = c.NonCardReciever.FaxerInformation.Email,
                                                  FaxerTelephone = c.NonCardReciever.FaxerInformation.PhoneNumber,
                                                  FaxingStatus = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus), //Enum.GetName(typeof( FaxingStatus),c.FaxingStatus)
                                                  FaxingStatusEnum = c.FaxingStatus
                                                  
                                              }).FirstOrDefault();

            vm.FormNonCardUserReceiverDetails = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCNNo).ToList()
                                                 join d in dbContext.Country on c.NonCardReciever.Country equals d.CountryCode
                                                 select new FormNonCardUserReceiverDetailsViewModel()
                                                 {
                                                     ReceiverId = c.NonCardReciever.Id,
                                                     ReceiverFirstName = c.NonCardReciever.FirstName,
                                                     ReceiverMiddleName = c.NonCardReciever.MiddleName,
                                                     ReceiverLastName = c.NonCardReciever.LastName,
                                                     ReceiverAddress = c.NonCardReciever.EmailAddress,
                                                     ReceiverCountryCode = c.NonCardReciever.Country,
                                                     ReceiverCountry = c.NonCardReciever.Country, //d.CountryName,
                                                     ReceiverCity = c.NonCardReciever.City,
                                                     ReceieverTelephone = c.NonCardReciever.PhoneNumber
                                                 }).FirstOrDefault();

            var faxingData = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCNNo).FirstOrDefault();
            vm.FaxingNonCardTransactionId = faxingData.Id;

            //var faxingCalcSummary = getFaxingCalculationSummary(faxingData.FaxingAmount, vm.FormNonCardUserFaxerDetails.FaxerCountryCode, vm.FormNonCardUserReceiverDetails.ReceiverCountryCode);
            //decimal faxingFee = faxingCalcSummary.TopUpFees;
            //decimal totalAmount = faxingCalcSummary.TotalAmountIncludingFees;
            //decimal exchangeRate = faxingCalcSummary.ExchangeRate;




            vm.FormNonCardUserFaxAmount = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCNNo).ToList()
                                           join d in dbContext.Country on c.NonCardReciever.Country equals d.CountryCode
                                           select new FormNonCardUserFaxAmountViewModel()
                                           {
                                               FaxingAmount = c.FaxingAmount,
                                               FaxingFee = c.FaxingFee,
                                               TotalAmount = c.FaxingAmount + c.FaxingFee,
                                               ExchangeRate = c.ExchangeRate,
                                               ReceivingAmount = c.ReceivingAmount,
                                               ReceivingCountry = c.NonCardReciever.Country,//d.CountryName
                                               SendingCurrency = CommonService.getCurrency(vm.FormNonCardUserFaxerDetails.FaxerCountryCode),
                                               SendingCurrencySymbol = CommonService.getCurrencySymbol(vm.FormNonCardUserFaxerDetails.FaxerCountryCode),
                                               ReceivingCurrency = CommonService.getCurrency(vm.FormNonCardUserReceiverDetails.ReceiverCountryCode),
                                               ReceivingCurrencySymbol = CommonService.getCurrencySymbol(vm.FormNonCardUserReceiverDetails.ReceiverCountryCode)
                                           }).FirstOrDefault();
            vm.FormNonCardUserAdminDetails.NameOfUpdater = CommonService.getStaffName(Common.StaffSession.LoggedStaff.StaffId);
            return vm;
        }

        public CalculateFaxingFeeVm getFaxingCalculationSummary (decimal faxingAmount, string faxingCountryCode, string receivingCountryCode )
        {
            CalculateFaxingFeeVm vm = new CalculateFaxingFeeVm();
            var feeSummary = new EstimateFaxingFeeSummary();
            decimal exchangeRate = 0;
            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == faxingCountryCode && x.CountryCode2 == receivingCountryCode).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateObj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == receivingCountryCode && x.CountryCode2 == faxingCountryCode).FirstOrDefault();
                if (exchangeRateObj2 != null)
                {

                    exchangeRateObj = exchangeRateObj2;
                    exchangeRateObj.CountryRate1 = Math.Round(1 / exchangeRateObj2.CountryRate1, 6, MidpointRounding.AwayFromZero);
                }
            }
            exchangeRate = exchangeRateObj.CountryRate1;
            feeSummary = SEstimateFee.CalculateFaxingFee(faxingAmount, true, false, exchangeRate, SEstimateFee.GetFaxingCommision(faxingCountryCode));
            vm.ExchangeRate = feeSummary.ExchangeRate;
            vm.TopUpFees = feeSummary.FaxingFee;
            vm.TopUpAmount = feeSummary.FaxingAmount;
            vm.ReceivingAmount = feeSummary.ReceivingAmount;
            vm.TotalAmountIncludingFees = feeSummary.FaxingFee + feeSummary.FaxingAmount;
            return vm;

        }

        public bool saveNonCardFaxingInfo (FormNonCardUserMoneyFaxedUpdateViewModel model)
        {
            if (model != null)
            {
                //updating data into ReceiversDetails Table 
                //var data = new ReceiversDetails()
                var data = dbContext.ReceiversDetails.Where(x => x.Id == model.FormNonCardUserReceiverDetails.ReceiverId).FirstOrDefault();
                if (data != null)
                {
                    //FaxerID = model.FormNonCardUserFaxerDetails.FaxerId,
                    data.FirstName = model.FormNonCardUserReceiverDetails.ReceiverFirstName;
                    data.MiddleName = model.FormNonCardUserReceiverDetails.ReceiverMiddleName;
                    data.LastName = model.FormNonCardUserReceiverDetails.ReceiverLastName;
                    data.City = model.FormNonCardUserReceiverDetails.ReceiverCity;
                    //Country = model.FormNonCardUserReceiverDetails.ReceiverCountry,
                    data.PhoneNumber = model.FormNonCardUserReceiverDetails.ReceieverTelephone;
                    data.EmailAddress = model.FormNonCardUserReceiverDetails.ReceiverAddress;
                    //CreatedDate = DateTime.Now
                };
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();

                FaxingUpdatedInformationByAdmin info = new FaxingUpdatedInformationByAdmin()
                {
                    StaffId = Common.StaffSession.LoggedStaff.StaffId,
                    NonCardTransactionId = model.FaxingNonCardTransactionId,
                    NameOfUpdatingAdmin = CommonService.getStaffName(Common.StaffSession.LoggedStaff.StaffId),
                    Date = DateTime.Now
                };
                dbContext.FaxingUpdatedInformationByAdmin.Add(info);
                dbContext.SaveChanges();

                //inserting data into FaxingNonCardTransaction Table
                //var receiverInfo = dbContext.ReceiversDetails.Where(x => x.EmailAddress == model.FormNonCardUserReceiverDetails.ReceiverAddress).FirstOrDefault();
                //FAXER.PORTAL.Services.SFaxingNonCardTransaction getMFCN = new SFaxingNonCardTransaction();
                //var MFCN = getMFCN.GetNewMFCNToSave();
                //var receiptNumber = getMFCN.GetNewReceiptNumberToSave();
                //var data1 = new FaxingNonCardTransaction()
                //{
                //    NonCardRecieverId = receiverInfo.Id,
                //    ReceivingAmount = model.FormNonCardUserFaxAmount.ReceivingAmount,
                //    TransactionDate = DateTime.Now,
                //    FaxingAmount = model.FormNonCardUserFaxAmount.FaxingAmount,
                //    MFCN = MFCN,
                //    ReceiptNumber = receiptNumber
                //};
                //dbContext.FaxingNonCardTransaction.Add(data1);
                //dbContext.SaveChanges();

                return true;
            }
            return false;
        }
    }
}