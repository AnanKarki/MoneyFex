using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.MoblieTransferApi.Models;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using TransferZero.Sdk.Model;
using Twilio.Rest.Api.V2010.Account.Recording;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class SenderDocumentationServices
    {

        DB.FAXEREntities dbContext = null;
        CommonServices _commonServices = null;
        private List<SenderDocumentationViewModel> senderDocuments;
        public SenderDocumentationServices()
        {
            dbContext = new DB.FAXEREntities();
            _commonServices = new CommonServices();
            senderDocuments = new List<SenderDocumentationViewModel>();
        }

        internal List<SenderDocumentationViewModel> GetSenderDocumentList(string country, string city,
            string SenderName, string AccountNo, int Status, string TelephoneNo, string Email = "",
            int pageNumber = 0, int pageSize = 0)
        {
            //Set sender Documents 

            var searchParam = new SenderDocumentSearchParamVm()
            {
                Country = country,
                City = city,
                AccountNo = AccountNo.Trim(),
                Email = Email.Trim(),
                SenderName = SenderName.Trim(),
                Status = Status,
                Telephone = TelephoneNo.Trim(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            GetSenderDocumentationDetials(searchParam);

            Log.Write(DateTime.Now.ToString(), ErrorType.UnSpecified, "Sender Document Time");
            return senderDocuments;
        }


        private void GetSenderDocumentationDetials(SenderDocumentSearchParamVm searchParam)
        {
            senderDocuments = dbContext.Sp_GetSenderDocumentDetails(searchParam);
        }
        private void SearchSenderDocumentByParam(SenderDocumentSearchParamVm searchParamVm)
        {

            if (!string.IsNullOrEmpty(searchParamVm.Country))
            {
                senderDocuments = senderDocuments.Where(x => x.Country == searchParamVm.Country).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.City))
            {
                senderDocuments = senderDocuments.Where(x => x.City == searchParamVm.City).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.AccountNo))
            {
                senderDocuments = senderDocuments.Where(x => x.AccountNo.Contains(searchParamVm.AccountNo)).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.Email))
            {
                senderDocuments = senderDocuments.Where(x => x.SenderEmailAddress.Contains(searchParamVm.Email)).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.Telephone))
            {
                senderDocuments = senderDocuments.Where(x => x.TelephoneNo.Contains(searchParamVm.Telephone)).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.SenderName))
            {
                senderDocuments = senderDocuments.Where(x => x.SenderFullName.Contains(searchParamVm.SenderName)).ToList();
            }
            if (searchParamVm.Status != 9)
            {
                senderDocuments = senderDocuments.Where(x => x.Status == (DocumentApprovalStatus)searchParamVm.Status).ToList();
            }
        }

        private int GetNoteCount(int senderId)
        {
            var noteCount = dbContext.TransactionStatementNote.Where(x => x.SenderId == senderId && x.IsRead == false).ToList().Count();
            return noteCount;
        }

        internal void Delete(int id)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == id).FirstOrDefault();
            dbContext.SenderBusinessDocumentation.Remove(data);
            dbContext.SaveChanges();
        }

        internal SenderDocumentationViewModel GetUploadedDocumentInfo(int id)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == id);
            var result = (from c in data
                          join d in dbContext.FaxerInformation on c.SenderId equals d.Id

                          select new SenderDocumentationViewModel()
                          {
                              Country = c.Country,
                              CreatedDate = c.CreatedDate,
                              AccountNo = c.AccountNo,
                              Id = c.Id,
                              SenderId = c.SenderId,
                              City = c.City,
                              CreatedBy = c.CreatedBy,
                              DocumentExpires = c.DocumentExpires,
                              DocumentName = c.DocumentName,
                              ExpiryDate = c.ExpiryDate,
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentPhotoUrlTwo = c.DocumentPhotoUrlTwo,
                              DocumentType = c.DocumentType,
                              IssuingCountry = c.IssuingCountry,
                              Status = c.Status,
                              SenderFirstName = d.FirstName,
                              SenderLastName = d.LastName,
                              StaffMiddleName = d.MiddleName,
                          }).FirstOrDefault();

            return result;
        }

        internal void SaveNotes(SenderDocumentationAndSenderNote vm)
        {
            TransactionStatementNote model = new TransactionStatementNote()
            {
                TransactionId = vm.TransactionStatementNote.TransactionId,
                Note = vm.TransactionStatementNote.Note,
                CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                CreatedDateAndTime = DateTime.Now,
                NoteType = vm.TransactionStatementNote.NoteType,
                SenderId = vm.TransactionStatementNote.SenderId
            };
            dbContext.TransactionStatementNote.Add(model);
            dbContext.SaveChanges();
        }

        internal void SaveDocumentationNote(TransactionStatementNoteViewModel vm)
        {
            TransactionStatementNote model = new TransactionStatementNote()
            {
                TransactionId = vm.TransactionId,
                Note = vm.Note,
                CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                CreatedDateAndTime = DateTime.Now,
                NoteType = vm.NoteType,
                SenderId = vm.SenderId
            };
            dbContext.TransactionStatementNote.Add(model);
            dbContext.SaveChanges();
        }

        internal List<DropDownViewModel> GetSender(string country, string city)
        {
            var result = (from c in dbContext.FaxerInformation.Where(x => x.IsBusiness == false
                          && x.Country == country).ToList()
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                          }).ToList();
            return result;
        }

        public string GetDocumentPhotoUrl(int id)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == id).Select(x => x.DocumentPhotoUrl).FirstOrDefault();
            return data;
        }

        public SenderBusinessDocumentation GetDocumentDetails(int id)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == id).FirstOrDefault();
            return data;
        }

        public List<DropDownViewModel> GetSender()
        {
            var result = (from c in dbContext.FaxerInformation.Where(x => x.IsBusiness == false).ToList()
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                          }).ToList();
            return result;

        }
        public List<DropDownViewModel> GetFilteredSenderList(string country, string city)
        {
            var result = (from c in dbContext.FaxerInformation.Where(x => (x.Country == country) && ((x.City.Trim()).ToLower() == city))
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }
        internal void UploadDocument(SenderDocumentationViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff == null ? 0 : Common.StaffSession.LoggedStaff.StaffId;


            SenderBusinessDocumentation model = new SenderBusinessDocumentation()
            {
                AccountNo = vm.AccountNo,
                City = vm.City,
                Country = vm.Country,
                CreatedBy = StaffId,
                CreatedDate = DateTime.Now,
                DocumentExpires = vm.DocumentExpires,
                DocumentName = vm.DocumentName,
                DocumentPhotoUrl = vm.DocumentPhotoUrl,
                DocumentPhotoUrlTwo = vm.DocumentPhotoUrlTwo,
                DocumentType = vm.DocumentType,
                ExpiryDate = vm.ExpiryDate,
                SenderId = vm.SenderId,
                IssuingCountry = vm.IssuingCountry,
                Status = (DocumentApprovalStatus)vm.Status,
                IsUploadedFromSenderPortal = vm.IsUploadedFromSenderPortal,
                IsUploadedFromAgentPortal = vm.IsUploadedFromAgentPortal,
                AgentId = vm.AgentId,
                IdentificationTypeId = vm.IdentificationTypeId,
                IdentityNumber = vm.IdentityNumber,
                ReasonForDisApprovalByAdmin = vm.ReasonForDisApprovalByAdmin,
                ReasonForDisApproval = vm.ReasonForDisApproval,
                ComplianceApprovalStatus = vm.ComplianceApprovalStatus

            };

            dbContext.SenderBusinessDocumentation.Add(model);
            dbContext.SaveChanges();
        }

        internal void UpdateTransactionStatementNote(int senderId)
        {
            var data = dbContext.TransactionStatementNote.Where(x => x.SenderId == senderId).ToList();
            foreach (var item in data)
            {
                item.IsRead = true;
                dbContext.Entry<TransactionStatementNote>(item).State = EntityState.Modified;
                dbContext.SaveChanges();

            }
        }

        internal List<TransactionStatementNoteViewModel> TransactionStatementNote(int senderId)
        {
            var data = dbContext.TransactionStatementNote.Where(x => x.SenderId == senderId &&
            x.NoteType == NoteType.SenderDocumentation).ToList();
            var result = (from c in data
                          join d in dbContext.StaffInformation on c.CreatedBy equals d.Id
                          select new TransactionStatementNoteViewModel()
                          {
                              Id = c.Id,
                              TransactionId = c.TransactionId,
                              CreatedBy = c.CreatedBy,
                              CreatedByName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              TransactionMethod = c.TransactionMethod,
                              CreatedDate = c.CreatedDateAndTime.ToFormatedString(),
                              CreatedTime = c.CreatedDateAndTime.ToString("HH:mm"),
                              Note = c.Note,
                              TransactionMethodName = c.TransactionMethod.ToString(),
                              IsRead = c.IsRead,
                              SenderId = c.SenderId,
                              NoteType = c.NoteType
                          }).ToList();
            return result;

        }

        internal void UpdateDocument(SenderDocumentationViewModel vm)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == vm.Id).FirstOrDefault();
            data.SenderId = vm.SenderId;
            data.ExpiryDate = vm.ExpiryDate;
            data.DocumentType = vm.DocumentType;
            data.DocumentPhotoUrl = vm.DocumentPhotoUrl;
            data.DocumentPhotoUrlTwo = vm.DocumentPhotoUrlTwo;
            data.DocumentName = vm.DocumentName;
            data.DocumentExpires = vm.DocumentExpires;
            data.Country = vm.Country;
            data.City = vm.City;
            data.AccountNo = vm.AccountNo;
            data.IssuingCountry = vm.IssuingCountry;
            data.Status = (DocumentApprovalStatus)vm.Status;
            data.ReasonForDisApproval = vm.ReasonForDisApproval;
            data.ReasonForDisApprovalByAdmin = vm.ReasonForDisApprovalByAdmin;
            data.ComplianceApprovalStatus = vm.ComplianceApprovalStatus;
            dbContext.Entry<SenderBusinessDocumentation>(data).State = EntityState.Modified;
            dbContext.SaveChanges();

            if (vm.Status == DocumentApprovalStatus.Approved)
            {
                ReinitialAllTransaction(data.SenderId);
            }
        }

        internal void ReinitialAllTransaction(int senderId)
        {

            var bankdepositdata = dbContext.BankAccountDeposit.Where(x => x.SenderId == senderId
            && x.Status == BankDepositStatus.IdCheckInProgress &&
            (x.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
            || x.SenderPaymentMode == Models.SenderPaymentMode.SavedDebitCreditCard)).ToList();
            foreach (var item in bankdepositdata)
            {
                Log.Write("Transaction Intiated" + item.ReceiptNo);
                ReInitialBankDepositTransaction(item);
            }

            var mobileTransferdata = dbContext.MobileMoneyTransfer.Where(x => x.SenderId == senderId && x.Status ==
            MobileMoneyTransferStatus.IdCheckInProgress && (x.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
            || x.SenderPaymentMode == Models.SenderPaymentMode.SavedDebitCreditCard)).ToList();
            foreach (var item in mobileTransferdata)
            {
                ReInitialMobileDepositTransaction(item);
            }

            var mobileTransaction = (from c in dbContext.MobileMoneyTransfer.Where(x => x.SenderId == senderId &&
                                     x.Status == MobileMoneyTransferStatus.InProgress &&
                                     (x.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
               || x.SenderPaymentMode == Models.SenderPaymentMode.SavedDebitCreditCard))
                                     where !(from o in dbContext.MobileMoneyTransferResposeStatus
                                             select o.TransactionId)
                                           .Contains(c.Id)
                                     select c).ToList();
            foreach (var item in mobileTransaction)
            {
                ReInitialMobileDepositTransaction(item);
            }

            var CashPickUpData = dbContext.FaxingNonCardTransaction.Where(x => x.SenderId == senderId
                                      && x.FaxingStatus == FaxingStatus.IdCheckInProgress &&
                                      (x.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
                                      || x.SenderPaymentMode == Models.SenderPaymentMode.SavedDebitCreditCard)).ToList();
            foreach (var item in CashPickUpData)
            {
                Log.Write("Transaction Intiated" + item.ReceiptNumber);
                if (item.ReceivingCountry == "MA")
                {
                    ReInitialCashPickUpTransaction(item);
                }
            }
        }

        public void ReInitialBankDepositTransaction(BankAccountDeposit item)
        {

            var ApiService = Common.Common.GetApiservice(item.SendingCountry,
             item.ReceiverCountry, item.SendingAmount, TransactionTransferMethod.BankDeposit, TransactionTransferType.Online);

            var exchangeRateType = ExchangeRateType.TransactionExchangeRate;
            try
            {
                exchangeRateType = Common.Common.SystemExchangeRateType(item.SendingCountry, item.ReceivingCountry, TransactionTransferMethod.BankDeposit);

            }
            catch (Exception)
            {

            }

            if (exchangeRateType == ExchangeRateType.CurrentExchangeRate)
            {

                var newExchange = Common.Common.GetModifedAmount(item.SendingCountry, item.ReceivingCountry,
                    TransactionTransferMethod.BankDeposit, item.SendingAmount, TransactionTransferType.Admin);

                item.ReceivingAmount = newExchange.ReceivingAmount;
                item.ExchangeRate = newExchange.ExchangeRate;
                item.Fee = newExchange.Fee;
                item.TotalAmount = newExchange.TotalAmount;

            }
            if (item.IsComplianceNeededForTrans == true)
            {

                item.Status = BankDepositStatus.Held;
                dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                SBankDepositResponseStatus sBankDepositResponseStatus = new SBankDepositResponseStatus();


                var bankdepositTransactionResult = new BankDepositResponseVm();

                SSenderBankAccountDeposit senderBankAccountDeposit = new SSenderBankAccountDeposit();
                var transResponse = senderBankAccountDeposit.CreateBankTransactionToApi(item);
                item.Status = transResponse.BankAccountDeposit.Status;
                bankdepositTransactionResult = transResponse.BankDepositApiResponseVm;
                dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();
                sBankDepositResponseStatus.AddLog(bankdepositTransactionResult, item.Id);
                senderBankAccountDeposit.SendEmailAndSms(item);
            }
        }

        public void ReInitialMobileDepositTransaction(MobileMoneyTransfer item)
        {

            Log.Write(item.ReceiptNo + "Mobile Transaction Api Initiated");
            var ApiService = Common.Common.GetApiservice(item.SendingCountry,
                  item.ReceivingCountry, item.SendingAmount, TransactionTransferMethod.OtherWallet, TransactionTransferType.Online);



            var exchangeRateType = ExchangeRateType.TransactionExchangeRate;
            try
            {
                exchangeRateType = Common.Common.SystemExchangeRateType(item.SendingCountry, item.ReceivingCountry, TransactionTransferMethod.OtherWallet);

            }
            catch (Exception)
            {

            }

            if (exchangeRateType == ExchangeRateType.CurrentExchangeRate)
            {

                var newExchange = Common.Common.GetModifedAmount(item.SendingCountry, item.ReceivingCountry,
                    TransactionTransferMethod.OtherWallet, item.SendingAmount, TransactionTransferType.Admin);
                item.ReceivingAmount = newExchange.ReceivingAmount;
                item.ExchangeRate = newExchange.ExchangeRate;
                item.Fee = newExchange.Fee;
                item.TotalAmount = newExchange.TotalAmount;

            }

            #region API Call

            if (item.IsComplianceNeededForTrans == true)
            {

                item.Status = MobileMoneyTransferStatus.Held;
                dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                Log.Write(item.ReceiptNo + "Mobile Transaction Api Call ");
                string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(item.ReceivingCountry);
                var MobileMoneyTransactionResult = new MTNCameroonResponseParamVm();
                SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
                var transferApiResponse = _senderMobileMoneyTransferServices.CreateTransactionToApi(item, TransactionTransferType.Online);
                Log.Write(item.ReceiptNo + "Mobile Transaction Api Success ");

                item.Status = transferApiResponse.status;
                MobileMoneyTransactionResult = transferApiResponse.response;

                dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();
                #region   Create Transaction Log 


                SMobileMoneyTransferResopnseStatus _sMobileMoneyResposeStatus = new SMobileMoneyTransferResopnseStatus();
                _sMobileMoneyResposeStatus.AddLog(MobileMoneyTransactionResult, item.Id);
                Log.Write(item.ReceiptNo + "Mobile Transaction Api Successful ");


                #endregion


                #region Send Sms and email

                _senderMobileMoneyTransferServices.SendEmailAndSms(item);
                #endregion
            }
            #endregion
        }


        public void ReInitialCashPickUpTransaction(FaxingNonCardTransaction item)
        {

            var ApiService = Common.Common.GetApiservice(item.SendingCountry,
             item.ReceivingCountry, item.FaxingAmount, TransactionTransferMethod.CashPickUp, TransactionTransferType.Online);

            var exchangeRateType = ExchangeRateType.TransactionExchangeRate;
            try
            {
                exchangeRateType = Common.Common.SystemExchangeRateType(item.SendingCountry, item.ReceivingCountry, TransactionTransferMethod.BankDeposit);
            }
            catch (Exception)
            {
            }

            if (exchangeRateType == ExchangeRateType.CurrentExchangeRate)
            {

                var newExchange = Common.Common.GetModifedAmount(item.SendingCountry, item.ReceivingCountry,
                    TransactionTransferMethod.CashPickUp, item.FaxingAmount, TransactionTransferType.Admin);

                item.ReceivingAmount = newExchange.ReceivingAmount;
                item.ExchangeRate = newExchange.ExchangeRate;
                item.FaxingFee = newExchange.Fee;
                item.TotalAmount = newExchange.TotalAmount;

            }
            if (item.IsComplianceNeededForTrans == true)
            {

                item.FaxingStatus = FaxingStatus.Hold;
                dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                SBankDepositResponseStatus sBankDepositResponseStatus = new SBankDepositResponseStatus();


                var bankdepositTransactionResult = new BankDepositResponseVm();

                SSenderCashPickUp _senderCashPickUpService = new SSenderCashPickUp();
                var transResponse = _senderCashPickUpService.CreateCashPickTransactionToApi(item);
                item.FaxingStatus = transResponse.CashPickUp.FaxingStatus;

                bankdepositTransactionResult = transResponse.BankDepositApiResponseVm;
                dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();

                _senderCashPickUpService.AddResponseLog(bankdepositTransactionResult, item.Id);
                _senderCashPickUpService.SendEmailAndSms(item);
            }
        }




        public List<SenderListDropDown> GetSenderList(string City = "", string Country = "")
        {

            var data = dbContext.FaxerInformation.Where(x => x.IsBusiness == false).ToList();

            if (!string.IsNullOrEmpty(Country))
            {

                data = data.Where(x => x.Country == Country).ToList();

            }
            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.City.ToLower() == City.ToLower()).ToList();
            }

            var senders = (from c in data
                           select new SenderListDropDown()
                           {
                               senderId = c.Id,
                               senderName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                               Country = c.Country
                           }).ToList();
            return senders;

        }

        public void SendIdentiVerificationInProgressEmail(int senderId)
        {
            var senderInfo = _commonServices.GetSenderInfo(senderId);
            string email = senderInfo.Email;
            string senderFirstname = senderInfo.FirstName;
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = "";

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/IdentityVerificationInProgress/Index?senderFirstname=" + senderFirstname);
            mail.SendMail(email, "Identity Verification in PROGRESS", body);

        }
        public void SendIdentiVerificationCompletedEmail(int senderId)
        {
            var senderInfo = _commonServices.GetSenderInfo(senderId);
            string email = senderInfo.Email;
            string senderFirstname = senderInfo.FirstName;
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = "";

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/IdentityVerificationCompleted/Index?senderFirstname=" + senderFirstname);
            mail.SendMail(email, "Identity Verification COMPLETED", body);

        }
        public void SendIdentiVerificationFailedEmail(int senderId, string Reason)
        {
            var senderInfo = _commonServices.GetSenderInfo(senderId);
            string email = senderInfo.Email;
            string senderFirstname = senderInfo.FirstName;
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = "";

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/IdentityVerificationFailed/Index?senderFirstname=" + senderFirstname + "&Reason=" + Reason);
            mail.SendMail(email, "Identity Verification FAILED", body);

        }

    }

}