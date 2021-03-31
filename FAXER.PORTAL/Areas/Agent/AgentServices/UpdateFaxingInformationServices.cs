using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class UpdateFaxingInformationServices
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();
        public Models.UpdateFaxingInformationViewModel getFaxerInformation(string MFCN)
        {
            var result = (from noncardTransaction in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).ToList()
                          join receiver in dbContext.ReceiversDetails on noncardTransaction.NonCardRecieverId equals receiver.Id
                          join data in dbContext.FaxerInformation on receiver.FaxerID equals data.Id
                          join AgentStaffInfo in dbContext.AgentStaffInformation on noncardTransaction.PayingStaffId equals AgentStaffInfo.Id
                          join AgentDetails in dbContext.AgentInformation on AgentStaffInfo.AgentId equals AgentDetails.Id
                          join faxerCountry in dbContext.Country on data.Country equals faxerCountry.CountryCode
                          join receiverCountry in dbContext.Country on receiver.Country equals receiverCountry.CountryCode
                          select new Models.UpdateFaxingInformationViewModel()
                          {
                              FaxerId = data.Id,
                              FaxerFirstName = data.FirstName,
                              FaxerMiddleName = data.MiddleName,
                              FaxerLastName = data.LastName,
                              FaxerAddress = data.Address1,
                              FaxerTelephone = CommonService.getPhoneCodeFromCountry(data.Country) + " " + data.PhoneNumber,
                              FaxerCity = data.City,
                              FaxerCountryCode = data.Country,
                              FaxerCountry = faxerCountry.CountryName,
                              FaxerEmail = data.Email,
                              IdCardNumber = data.IdCardNumber,
                              IdCardType = data.IdCardType,
                              IdCardExpDate = data.IdCardExpiringDate,
                              ReceiverId = receiver.Id,
                              ReceiverFirstName = receiver.FirstName,
                              ReceiverMiddleName = receiver.MiddleName,
                              ReceiverLastName = receiver.LastName,
                              ReceiverCity = receiver.City,
                              ReceiverCountryCode = receiver.Country,
                              ReceiverCountry = receiverCountry.CountryName,
                              ReceiverTelephone = receiver.PhoneNumber,
                              ReceiverPhoneCode = CommonService.getPhoneCodeFromCountry(receiver.Country),
                              ReceiverEmailAddress = receiver.EmailAddress,
                              FaxingAmount = String.Format("{0:n}", noncardTransaction.FaxingAmount),
                              StatusOfFax = noncardTransaction.FaxingStatus,
                              StatusOfFaxName = Enum.GetName(typeof(FaxingStatus), noncardTransaction.FaxingStatus),
                              TransactionDate = noncardTransaction.TransactionDate,
                              TransactionId = noncardTransaction.Id,
                              MFCNNumber = noncardTransaction.MFCN,
                              PayingAgentName = AgentStaffInfo.FirstName  + " " + AgentStaffInfo.MiddleName + " " + AgentStaffInfo.LastName,
                              NameOfAgency = AgentDetails.Name,
                              AgencyMFSCode = AgentStaffInfo.AgentMFSCode,
                              FaxerCurrency = CommonService.getCurrencyCodeFromCountry(data.Country),
                              FaxerCurrencySymbol = CommonService.getCurrencySymbol(data.Country),
                              ReceiverCurrency = CommonService.getCurrencyCodeFromCountry(receiver.Country),
                              ReceiverCurrencySymbol = CommonService.getCurrencySymbol(receiver.Country),
                              AmountToBeReceived = noncardTransaction.ReceivingAmount.ToString(),
                              FaxingFee = noncardTransaction.FaxingFee.ToString(),
                              CurrentExchangeRate = noncardTransaction.ExchangeRate.ToString(),
                              TotalAmountincludingFee = (noncardTransaction.FaxingAmount + noncardTransaction.FaxingFee).ToString()


                          }).FirstOrDefault();

            return result;



        }

        public bool updateFaxMoneyInformation(Models.UpdateFaxingInformationViewModel vm)
        {

            var faxerdata = dbContext.FaxerInformation.Where(x => x.Id == vm.FaxerId).FirstOrDefault();
            if (faxerdata != null)
            {

                faxerdata.FirstName = vm.FaxerFirstName;
                faxerdata.LastName = vm.FaxerLastName;
                faxerdata.MiddleName = vm.FaxerMiddleName;
                faxerdata.IdCardExpiringDate = vm.IdCardExpDate;
                faxerdata.IdCardNumber = vm.IdCardNumber;
                faxerdata.IdCardType = vm.IdCardType;
                //faxerdata.IssuingCountry ;
                faxerdata.Address1 = vm.FaxerAddress;
                faxerdata.City = vm.FaxerCity;
                //faxerdata.Country = vm.FaxerCountry;
                faxerdata.Email = vm.FaxerEmail;
                dbContext.Entry(faxerdata).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            var receiverdata = dbContext.ReceiversDetails.Where(x => x.Id == vm.ReceiverId).FirstOrDefault();

            if (receiverdata != null)
            {
                receiverdata.FirstName = vm.ReceiverFirstName;
                receiverdata.MiddleName = vm.ReceiverMiddleName;
                receiverdata.LastName = vm.ReceiverLastName;
                receiverdata.EmailAddress = vm.ReceiverEmailAddress;
                receiverdata.PhoneNumber = vm.ReceiverTelephone;
                receiverdata.City = vm.ReceiverCity;
                dbContext.Entry(receiverdata).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

            }
            var nonCardTransactionData = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == vm.MFCNNumber).FirstOrDefault();
            if (nonCardTransactionData != null)
            {
                nonCardTransactionData.ReceivingAmount = Decimal.Parse(vm.AmountToBeReceived);
                nonCardTransactionData.FaxingAmount = Convert.ToDecimal(vm.FaxingAmount);
                dbContext.Entry(nonCardTransactionData).State = System.Data.Entity.EntityState.Modified;
                //dbContext.SaveChanges();

                FaxingUpdatedInformation updatedInfo = new FaxingUpdatedInformation()
                {
                    AgentId = vm.AgentId,
                    NonCardTransactionId = nonCardTransactionData.Id,
                    NameOfUpdatingAgent = vm.NameofUpdatingAgent,
                    Date = DateTime.Now,
                    UpdatingAgentAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId
                };
                dbContext.FaxingUpdatedInformation.Add(updatedInfo);
                dbContext.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }

        }
    }
}