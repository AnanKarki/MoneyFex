using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewRegisteredFaxersServices
    {
        FAXEREntities dbContext = null;
        Services.CommonServices CommonService = new Services.CommonServices();
        private List<ViewRegisteredFaxersViewModel> senderRegisetered;
        public ViewRegisteredFaxersServices()
        {
            dbContext = new FAXEREntities();
            senderRegisetered = new List<ViewRegisteredFaxersViewModel>();
        }

        public List<ViewModels.ViewRegisteredFaxersViewModel> getFaxerInformationList()
        {

            var result = (from c in dbContext.FaxerInformation.Where(x => x.IsDeleted == false).ToList()
                          join d in dbContext.FaxerLogin on c.Id equals d.FaxerId
                           into j
                          from joined in j.DefaultIfEmpty()
                          select new ViewRegisteredFaxersViewModel()
                          {
                              Id = c.Id,
                              FirstName = c.FirstName,
                              MiddleName = c.MiddleName,
                              LastName = c.LastName,
                              City = c.City,
                              Country = c.Country,
                              Address1 = c.Address1,
                              Address2 = c.Address2,
                              DateOfBirth = c.DateOfBirth,
                              GenderName = Enum.GetName(typeof(Gender), c.GGender),
                              IDCardExpDate = c.IdCardExpiringDate,
                              IDCardNumber = c.IdCardNumber,
                              IDCardType = c.IdCardType,
                              IssuingCountry = c.IssuingCountry,
                              Phone = c.PhoneNumber,
                              PostalCode = c.PostalCode,
                              State = c.State,
                              UsernameEmail = c.Email,
                              LoginFailedCount = joined == null ? 0 : joined.LoginFailedCount,
                              AccountStatus = joined == null ? true : joined.IsActive
                          }
                          ).ToList();
            return result;
        }

        internal List<ViewModels.ViewRegisteredFaxersViewModel> GetFilteredFaxerList(string countryCode, string city)
        {

            var result = (from c in dbContext.FaxerInformation.Where(x => x.IsDeleted == false).ToList()
                          join d in dbContext.FaxerLogin on c.Id equals d.FaxerId
                           into j
                          from joined in j.DefaultIfEmpty()
                          select new ViewRegisteredFaxersViewModel()
                          {
                              Id = c.Id,
                              FirstName = c.FirstName,
                              MiddleName = c.MiddleName,
                              LastName = c.LastName,
                              City = c.City,
                              Country = c.Country,
                              Address1 = c.Address1,
                              Address2 = c.Address2,
                              DateOfBirth = c.DateOfBirth,
                              GenderName = Enum.GetName(typeof(Gender), c.GGender),
                              IDCardExpDate = c.IdCardExpiringDate,
                              IDCardNumber = c.IdCardNumber,
                              IDCardType = c.IdCardType,
                              IssuingCountry = c.IssuingCountry,
                              Phone = c.PhoneNumber,
                              PostalCode = c.PostalCode,
                              State = c.State,
                              UsernameEmail = c.Email,
                              LoginFailedCount = joined == null ? 0 : joined.LoginFailedCount,
                              AccountStatus = joined == null ? true : joined.IsActive
                          }).ToList();
            if (!string.IsNullOrEmpty(countryCode))
            {
                result = result.Where(x => x.Country == countryCode).ToList();
            }
            if (!string.IsNullOrEmpty(city))
            {
                result = result.Where(x => x.City == city).ToList();
            }
            return result;
        }

        public ViewModels.ViewRegisteredFaxersViewModel getFaxerInformation(int FaxerId)
        {
            var data = dbContext.FaxerInformation.Where(x => x.Id == FaxerId).FirstOrDefault();

            var result = (from c in dbContext.FaxerInformation.Where(x => x.IsDeleted == false && x.Id == FaxerId)
                          join e in dbContext.Country on c.Country equals e.CountryCode
                          join d in dbContext.FaxerLogin.Where(x => x.FaxerId == FaxerId) on c.Id equals d.FaxerId into j
                          from d in j.DefaultIfEmpty()
                          select new ViewRegisteredFaxersViewModel()
                          {
                              Id = c.Id,
                              FirstName = c.FirstName,
                              MiddleName = c.MiddleName,
                              LastName = c.LastName,
                              City = c.City,
                              CountryCode = c.Country,
                              Country = e.CountryName,
                              Address1 = c.Address1,
                              Address2 = c.Address2,
                              DateOfBirth = c.DateOfBirth,
                              Gender = (Gender)c.GGender,
                              IDCardExpDate = c.IdCardExpiringDate,
                              IDCardNumber = c.IdCardNumber,
                              IDCardType = c.IdCardType,
                              IDCardImage = c.CardUrl,
                              IssuingCountry = e.CountryName,
                              Phone = c.PhoneNumber,
                              PostalCode = c.PostalCode,
                              State = c.State,
                              UsernameEmail = c.Email,
                              MFAccountNo = c.AccountNo,
                              LoginFailedCount = d == null ? 0 : d.LoginFailedCount,
                              AccountStatus = d == null ? true : d.IsActive
                          }
                          ).FirstOrDefault();
            return result;
        }

        public bool RegisterFaxer(ViewRegisteredFaxersViewModel model)
        {

            SFaxerSignUp faxerSignUpService = new SFaxerSignUp();
            string accountNo = faxerSignUpService.GetNewAccount(10);
            var data = new FaxerInformation()
            {
                Email = model.UsernameEmail,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                GGender = (int)model.Gender,
                IdCardType = model.IDCardType,
                IdCardNumber = model.IDCardNumber,
                IdCardExpiringDate = model.IDCardExpDate,
                IssuingCountry = model.IssuingCountry,
                CardUrl = model.IDCardImage,
                CheckAmount = model.TransactionOver1000,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                State = model.State,
                Country = model.Country,
                PostalCode = model.PostalCode,
                PhoneNumber = model.Phone,
                AccountNo = accountNo
            };
            //var guId = Guid.NewGuid().ToString();
            //DB.FaxerLogin login = new DB.FaxerLogin()
            //{
            //    FaxerId = data.Id,
            //    UserName = data.Email,
            //    Password = "",
            //    ActivationCode = guId,
            //    IsActive = false
            //};
            dbContext.FaxerInformation.Add(data);
            dbContext.SaveChanges();
            SCity.Save(model.City, model.Country, DB.Module.Faxer);


            //MailCommon mail = new MailCommon();
            //var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            //var link = string.Format("{0}/FaxerAccount/Activate/{1}", baseUrl, guId);

            //string body = "";
            //body = Common.Common.GetTemplate(baseUrl + "/emailtemplate/FaxerActivationEmail?guid=" + guId + "&faxername=" + data.FirstName + " " + data.LastName);

            //mail.SendMail(data.Email, "Welcome to MoneyFax", body);

            return true;
        }

        public bool UpdateFaxerInformation(ViewRegisteredFaxersViewModel model)
        {
            var faxerInformation = dbContext.FaxerInformation.Where(x => (x.Id == model.Id) && (x.IsDeleted == false)).FirstOrDefault();
            var faxerLogin = dbContext.FaxerLogin.Where(x => x.FaxerId == model.Id).FirstOrDefault(); // for password later. after asking binod dai.
            if (faxerInformation != null)
            {
                faxerInformation.Id = model.Id;
                faxerInformation.Email = model.UsernameEmail;
                faxerInformation.Address1 = model.Address1;
                faxerInformation.Address2 = model.Address2;
                faxerInformation.City = model.City;
                faxerInformation.State = model.State;
                faxerInformation.PostalCode = model.PostalCode;
                faxerInformation.PhoneNumber = model.Phone;
                faxerInformation.GGender = (int)model.Gender;
                faxerInformation.DateOfBirth = model.DateOfBirth;
                faxerInformation.FirstName = model.FirstName;
                faxerInformation.MiddleName = model.MiddleName;
                faxerInformation.LastName = model.LastName;
                dbContext.Entry(faxerInformation).State = EntityState.Modified;
                dbContext.SaveChanges();
                SCity.Save(model.City, model.Country, DB.Module.Faxer);

                if (faxerLogin != null)
                {
                    faxerLogin.UserName = model.UsernameEmail;
                    faxerLogin.MobileNo = model.Phone;
                    dbContext.Entry(faxerLogin).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
                return true;
            }

            return false;

        }

        // Activate and deativate Faxer Account Status
        public bool AccountStatus(int id, bool AccountStatus)
        {
            var data = dbContext.FaxerLogin.Where(x => x.FaxerId == id).FirstOrDefault();
            if (data != null)
            {
                // If Activate
                if (AccountStatus == true)
                {

                    data.IsActive = true;
                }
                // if deactivate
                else
                {
                    data.IsActive = false;
                }
                // update agent status
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;

        }

        public bool DeleteFaxerInformation(int id)
        {
            if (id != 0)
            {
                var data = dbContext.FaxerInformation.Find(id);
                data.IsDeleted = true;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();

                var faxerLogin = dbContext.FaxerLogin.Where(x => x.FaxerId == data.Id).FirstOrDefault();
                faxerLogin.IsActive = false;
                dbContext.Entry(faxerLogin).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
        public List<FaxerInformation> List()
        {
            var data = dbContext.FaxerInformation.Where(x => x.IsBusiness == false).ToList();
            return data;
        }

        public List<FaxerLogin> ListOfActiveSender()
        {
            var data = dbContext.FaxerLogin.Where(x => x.IsActive == true && x.Faxer.IsBusiness == false).ToList();
            return data;
        }
        public List<FaxerLogin> ListOfInActiveSender()
        {
            var data = dbContext.FaxerLogin.Where(x => x.IsActive == false && x.Faxer.IsBusiness == false).ToList();
            return data;
        }

        private void GetRegisteredSenderDetials(SenderRegisteredSearchParamVm searchParam)
        {
            senderRegisetered = dbContext.Sp_GetRegisteredSenderDetails(searchParam);
        }

        public List<ViewModels.ViewRegisteredFaxersViewModel> GetFilteredFaxerList(string countryCode = "", string city = "",
            string SenderName = "", string AccountNo = "", string Address = "", string Telephone = "", string Email = "", string SenderStatus = ""
            , int pageNumber = 0, int PageSize = 0, string dateRange = "")
        {
            //set Registered Details
            var searchParam = new SenderRegisteredSearchParamVm()
            {
                Country = countryCode,
                City = city,
                AccountNo = AccountNo,
                Email = Email,
                SenderName = SenderName,
                Status = SenderStatus,
                Telephone = Telephone,
                Address = Address,
                DateRange = dateRange,
                ToDate = "",
                FromDate = "",
                PageSize = PageSize,
                PageNumber = pageNumber
            };
            GetRegisteredSenderDetials(searchParam);
            return senderRegisetered;
        }
        public List<SenderDetailsForExpotExcelViewModel> GetSenderDetailsForExcelFile(string dateRange = "")
        {
            DateTime FromDate = DateTime.Now;
            DateTime ToDate = DateTime.Now;
            if (!string.IsNullOrEmpty(dateRange))
            {
                var Date = dateRange.Split('-');
                FromDate = Date[0].ToDateTime();
                ToDate = Date[1].ToDateTime();
            }
            var searchParam = new SenderRegisteredSearchParamVm()
            {
                Country = "",
                City = "",
                AccountNo = "",
                Email = "",
                SenderName = "",
                Status = "",
                Telephone = "",
                Address = "",
                DateRange = dateRange,
                FromDate = FromDate.ToString(),
                ToDate = ToDate.ToString()
            };
            GetRegisteredSenderDetials(searchParam);
            var senderDetailsForExcelFile = (from c in senderRegisetered
                                             select new SenderDetailsForExpotExcelViewModel()
                                             {
                                                 SenderName = c.FullName,
                                                 Email = c.UsernameEmail,
                                                 Address1 = c.Address1,
                                                 Address2 = c.Address2,
                                                 City = c.City,
                                                 Country = c.Country,
                                                 DOB = c.DateOfBirth.HasValue ? c.DateOfBirth.ToFormatedString("dd/MM/yyyy") : "",
                                                 Gender = c.GGenderName,
                                                 IsActive = c.AccountStatus == true ? "Yes" : "No",
                                                 MFAccountNo = c.MFAccountNo,
                                                 PostalCode = c.PostalCode,
                                                 State = c.State
                                             }).ToList();
            return senderDetailsForExcelFile;
        }

        private void SearchRegisteresSenderDetailsByParam(SenderRegisteredSearchParamVm searchParamVm)
        {
            if (!string.IsNullOrEmpty(searchParamVm.Country))
            {
                senderRegisetered = senderRegisetered.Where(x => x.Country == searchParamVm.Country).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.City))
            {
                senderRegisetered = senderRegisetered.Where(x => x.City.ToLower() == searchParamVm.City.ToLower()).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.SenderName))
            {
                var SenderName = searchParamVm.SenderName.Trim();
                string Name = "";
                string[] name = SenderName.Split(' ');
                for (int i = 0; i < name.Length; i++)
                {
                    if (!string.IsNullOrEmpty(name[i]))
                    {
                        Name = Name + name[i].Trim() + " ";
                    }
                }
                Name = Name.Trim();
                senderRegisetered = senderRegisetered.Where(x => x.FirstName.ToLower().Contains(Name.ToLower())
                           || x.MiddleName.ToLower().Contains(Name.ToLower())
                           || x.LastName.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.AccountNo))
            {
                searchParamVm.AccountNo = searchParamVm.AccountNo.Trim();
                senderRegisetered = senderRegisetered.Where(x => x.MFAccountNo.ToLower().Contains(searchParamVm.AccountNo.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.Address))
            {
                searchParamVm.Address = searchParamVm.Address.Trim();
                senderRegisetered = senderRegisetered.Where(x => x.Address1.ToLower().Contains(searchParamVm.Address.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.Telephone))
            {
                searchParamVm.Telephone = searchParamVm.Telephone.Trim();
                senderRegisetered = senderRegisetered.Where(x => x.Phone.Contains(searchParamVm.Telephone)).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.Status))
            {
                senderRegisetered = senderRegisetered.Where(x => x.AccountStatusName.ToLower() == searchParamVm.Status.ToLower()).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.DateRange))
            {

                var Date = searchParamVm.DateRange.Split('-');
                string[] startDate = Date[0].Split('/');
                string[] endDate = Date[1].Split('/');
                var FromDate = new DateTime(int.Parse(startDate[2]), int.Parse(startDate[0]), int.Parse(startDate[1]));
                var ToDate = new DateTime(int.Parse(endDate[2]), int.Parse(endDate[0]), int.Parse(endDate[1]));// Convert.ToDateTime(Date[1]);
                senderRegisetered = senderRegisetered.Where(x => x.CreatedDate >= FromDate &&
                                                                 x.CreatedDate <= ToDate).ToList();
            }
        }

        public NewSenderTransactionViewModel GetSenderTransaction(int faxerId = 0, int year = 0, string Transaction = "", int pageNumber = 0, int pageSize = 0)
        {
            NewSenderTransactionViewModel transactionHistory = new NewSenderTransactionViewModel();
            transactionHistory.TransactionList = GetMobileTransferDetails(faxerId, year).
                                                                   Concat(GetKiiPayWalletPayment(faxerId, year)).
                                                                   Concat(GetCashPickUpDetails(faxerId, year))
                                                                 .Concat(GetBankDepositDetails(faxerId, year)).ToPagedList(pageNumber, pageSize);
            if (!string.IsNullOrEmpty(Transaction))
            {
                Transaction = Transaction.Trim();
                //transactionHistory.TransactionList = transactionHistory.TransactionList.Where(x => x.TranasactionId==Transaction)).ToPagedList(pageNumber, pageSize);

            }
            transactionHistory.TransactionList = transactionHistory.TransactionList.OrderByDescending(x => x.DateTime).ToPagedList(pageNumber, pageSize);

            return transactionHistory;
        }
        public NewSenderTransactionViewModel GetSenderTransactionForDownlaod(int faxerId = 0, int year = 0)
        {
            NewSenderTransactionViewModel transactionHistory = new NewSenderTransactionViewModel();
            transactionHistory.TransactionListDownload = GetMobileTransferDetails(faxerId, year).
                                                                   Concat(GetKiiPayWalletPayment(faxerId, year)).
                                                                   Concat(GetCashPickUpDetails(faxerId, year))
                                                                 .Concat(GetBankDepositDetails(faxerId, year)).ToList();

            transactionHistory.TransactionListDownload = transactionHistory.TransactionListDownload.OrderByDescending(x => x.DateTime).ToList();

            return transactionHistory;
        }


        public NewSenderTransactionViewModel GetNewTransactionStatement(int senderId = 0, int year = 0, string receiptNo = "")
        {
            NewSenderTransactionViewModel transactionHistory = new NewSenderTransactionViewModel();


            var SecureTradingApiResponseTransactionLog = dbContext.SecureTradingApiResponseTransactionLog.OrderByDescending(x => x.Id)
                                                        .GroupBy(x => x.orderreference).Select(x => x.FirstOrDefault());

            var bankDepositData = dbContext.BankAccountDeposit.Where(x => x.SenderId == senderId);
            var cashPickUpData = dbContext.FaxingNonCardTransaction.Where(x => x.SenderId == senderId);
            var mobileWalletData = dbContext.MobileMoneyTransfer.Where(x => x.SenderId == senderId);
            if (!string.IsNullOrEmpty(receiptNo))
            {
                receiptNo = receiptNo.Trim();
                bankDepositData = bankDepositData.Where(x => x.ReceiptNo == receiptNo);
                cashPickUpData = cashPickUpData.Where(x => x.ReceiptNumber == receiptNo);
                mobileWalletData = mobileWalletData.Where(x => x.ReceiptNo == receiptNo);
            }
            var bankDeposit = (from c in bankDepositData.ToList()
                               join d in SecureTradingApiResponseTransactionLog on c.ReceiptNo
                               equals d.orderreference into joined
                               from d in joined.DefaultIfEmpty()
                               join e in dbContext.FaxerInformation on c.SenderId equals e.Id
                               join country in dbContext.Country on c.SendingCountry equals country.CountryCode
                               join receivingCountry in dbContext.Country on c.ReceivingCountry equals receivingCountry.CountryCode
                               select new NewSenderTransactionStatementListVm()
                               {
                                   Amount = c.TotalAmount,
                                   BillingAddress = e.Address1,
                                   CardIssuer = d == null ? "" : d.issuer,
                                   Last4Digits = d == null ? "" : d.maskedpan.Substring(d.maskedpan.Length - 4),
                                   TranasactionId = c.Id,
                                   Status = c.Status.ToString(),
                                   Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                   DateTime = c.TransactionDate, //d.transactionstartedtimestamp.ToDateTime(),
                                   CountryCurrency = c.SendingCurrency != null ? c.SendingCurrency : country.Currency,
                                   ReceivingCurrency = c.ReceivingCurrency != null ? c.ReceivingCurrency : receivingCountry.Currency,
                                   Id = c.Id,
                                   Receiver = c.ReceiverName,
                                   ReceiptNo = c.ReceiptNo,
                                   Fee = c.Fee
                               }).ToList();

            var casPickUp = (from c in cashPickUpData.ToList()
                             join d in SecureTradingApiResponseTransactionLog on
                             c.ReceiptNumber equals d.orderreference into joined
                             from d in joined.DefaultIfEmpty()
                             join e in dbContext.FaxerInformation on c.SenderId equals e.Id
                             join country in dbContext.Country on c.SendingCountry equals country.CountryCode
                             join receivingCountry in dbContext.Country on c.ReceivingCountry equals receivingCountry.CountryCode
                             select new NewSenderTransactionStatementListVm()
                             {
                                 Amount = c.TotalAmount,
                                 BillingAddress = e.Address1,
                                 CardIssuer = d == null ? "" : d.issuer,
                                 Last4Digits = d == null ? "" : d.maskedpan.Substring(d.maskedpan.Length - 4),
                                 TranasactionId = c.Id,
                                 Status = c.FaxingStatus.ToString(),
                                 Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                 DateTime = c.TransactionDate,
                                 CountryCurrency = c.SendingCurrency != null ? c.SendingCurrency : country.Currency,
                                 ReceivingCurrency = c.ReceivingCurrency != null ? c.ReceivingCurrency : receivingCountry.Currency,
                                 Id = c.Id,
                                 Receiver = c.NonCardReciever.FullName,
                                 Fee = c.FaxingFee,
                                 ReceiptNo = c.ReceiptNumber
                             }).ToList();
            var OtherWallet = (from c in mobileWalletData.ToList()
                               join d in SecureTradingApiResponseTransactionLog on
                               c.ReceiptNo equals d.orderreference into joined
                               from d in joined.DefaultIfEmpty()
                               join e in dbContext.FaxerInformation on c.SenderId equals e.Id
                               join country in dbContext.Country on c.SendingCountry equals country.CountryCode
                               join receivingCountry in dbContext.Country on c.ReceivingCountry equals receivingCountry.CountryCode
                               select new NewSenderTransactionStatementListVm()
                               {
                                   Amount = c.TotalAmount,
                                   BillingAddress = e.Address1,
                                   CardIssuer = d == null ? "" : d.issuer,
                                   Last4Digits = d == null ? "" : d.maskedpan.Substring(d.maskedpan.Length - 4),
                                   TranasactionId = c.Id,
                                   Status = c.Status.ToString(),
                                   Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                   DateTime = c.TransactionDate,
                                   CountryCurrency = c.SendingCurrency != null ? c.SendingCurrency : country.Currency,
                                   ReceivingCurrency = c.ReceivingCurrency != null ? c.ReceivingCurrency : receivingCountry.Currency,
                                   Id = c.Id,
                                   Receiver = c.ReceiverName,
                                   ReceiptNo = c.ReceiptNo,
                                   Fee = c.Fee
                               }).ToList();

            transactionHistory.TransactionListDownload = bankDeposit.Concat(casPickUp).Concat(OtherWallet).ToList();
            return transactionHistory;
        }

        public MonthlyTransactionMeter GetMonthlyTransactionMeter(List<NewSenderTransactionStatementListVm> result, int senderId)
        {
            string currency = CommonService.getCurrency(CommonService.GetSenderCountry(senderId));
            result = result.Where(x => x.Status.ToLower() == "paid" ||
                                       x.Status.ToLower() == "completed" ||
                                       x.Status.ToLower() == "received" ||
                                       x.Status.ToLower() == "confirm").ToList();
            MonthlyTransactionMeter monthy = new MonthlyTransactionMeter()
            {
                Jan = GetMonthlySum(result, 1, currency),
                Feb = GetMonthlySum(result, 2, currency),
                March = GetMonthlySum(result, 3, currency),
                April = GetMonthlySum(result, 4, currency),
                May = GetMonthlySum(result, 5, currency),
                Jun = GetMonthlySum(result, 6, currency),
                July = GetMonthlySum(result, 7, currency),
                Aug = GetMonthlySum(result, 8, currency),
                Sep = GetMonthlySum(result, 9, currency),
                Oct = GetMonthlySum(result, 10, currency),
                Nov = GetMonthlySum(result, 11, currency),
                Dec = GetMonthlySum(result, 12, currency),

            };
            return monthy;
        }
        private string GetMonthlySum(List<NewSenderTransactionStatementListVm> result, int Month, string Currency = "")
        {
            var Amount = result.Where(x => x.DateTime.Month == Month).ToList().Sum(x => x.Amount);

            if (Amount > 0)
            {

                return Amount + " " + Currency;
            }
            return "";
        }

        public List<NewSenderTransactionStatementListVm> GetBankDepositDetails(int SenderId, int year = 0)
        {
            List<NewSenderTransactionStatementListVm> result = new List<NewSenderTransactionStatementListVm>();

            var data = dbContext.BankAccountDeposit as IQueryable<BankAccountDeposit>;

            if (SenderId != 0)
            {

                data = data.Where(x => x.SenderId == SenderId);
            }
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year);
            }


            result = (from c in data.ToList()
                      join d in dbContext.FaxerInformation on c.SenderId equals d.Id
                      join SendingCountry in dbContext.Country on c.SendingCountry equals SendingCountry.CountryCode
                      select new NewSenderTransactionStatementListVm()
                      {
                          Id = c.Id,
                          TranasactionId = c.Id,
                          Date = c.TransactionDate.ToString("yyyy/MM/dd"),
                          DateTime = c.TransactionDate,
                          Last4Digits = "",
                          Amount = c.SendingAmount,
                          BillingAddress = "",
                          CardIssuer = "",
                          CountryCurrency = SendingCountry.CurrencySymbol,
                          Fee = c.Fee,
                          Receiver = c.ReceiverName,
                          Status = Common.Common.GetEnumDescription(c.Status),

                      }).ToList();
            return result;

        }
        public List<NewSenderTransactionStatementListVm> GetCashPickUpDetails(int senderId, int year = 0)
        {
            var data = dbContext.FaxingNonCardTransaction.ToList();

            if (senderId != 0)
            {
                data = data.Where(x => x.NonCardReciever.FaxerID == senderId).ToList();

            }
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year).ToList();
            }

            var result = (from c in data
                          join d in dbContext.FaxerInformation on c.NonCardReciever.FaxerID equals d.Id
                          join SendingCountry in dbContext.Country on c.SendingCountry equals SendingCountry.CountryCode
                          join receiver in dbContext.Recipients on c.RecipientId equals receiver.Id

                          select new NewSenderTransactionStatementListVm()
                          {
                              Id = c.Id,
                              TranasactionId = c.Id,
                              Date = c.TransactionDate.ToString("yyyy/MM/dd"),
                              DateTime = c.TransactionDate,
                              Last4Digits = "",
                              Amount = c.FaxingAmount,
                              BillingAddress = "",
                              CardIssuer = "",
                              CountryCurrency = SendingCountry.CurrencySymbol,
                              Fee = c.FaxingFee,
                              Receiver = receiver.ReceiverName,
                              Status = Common.Common.GetEnumDescription(c.FaxingStatus),

                          }).ToList();
            return result;

        }

        public List<NewSenderTransactionStatementListVm> GetKiiPayWalletPayment(int senderId, int year = 0)
        {
            var data = dbContext.TopUpSomeoneElseCardTransaction.ToList();

            if (senderId != 0)
            {
                data = data.Where(x => x.FaxerId == senderId).ToList();
            }
            if (year != 0)
            {

                data = data.Where(x => x.TransactionDate.Year == year).ToList();
            }

            var result = (from c in data
                          join d in dbContext.FaxerInformation on c.FaxerId equals d.Id
                          join SendingCountry in dbContext.Country on c.SendingCountry equals SendingCountry.CountryCode
                          join receiver in dbContext.Recipients on c.RecipientId equals receiver.Id

                          select new NewSenderTransactionStatementListVm()
                          {
                              Id = c.Id,
                              TranasactionId = c.Id,
                              Date = c.TransactionDate.ToString("yyyy/MM/dd"),
                              DateTime = c.TransactionDate,
                              Last4Digits = "",
                              Amount = c.FaxingAmount,
                              BillingAddress = "",
                              CardIssuer = "",
                              CountryCurrency = SendingCountry.CurrencySymbol,
                              Fee = c.FaxingFee,
                              Receiver = receiver.ReceiverName,
                              Status = Common.Common.GetEnumDescription(FaxingStatus.Completed),
                          }).ToList();

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            int SenderWalletId = 0;
            var senderWalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(senderId);

            if (senderWalletInfo != null)
            {
                SenderWalletId = senderWalletInfo.Id;
            }

            var kiipaywallet = dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.ToList();

            if (senderId != 0)
            {
                kiipaywallet = kiipaywallet.Where(x => x.SenderWalletId == SenderWalletId).ToList();
            }

            if (year != 0)
            {
                kiipaywallet = kiipaywallet.Where(x => x.TransactionDate.Year == year).ToList();
            }

            var transactionUsingKiipayWallet = (from c in kiipaywallet
                                                join d in dbContext.FaxerInformation on c.SenderId equals d.Id
                                                join SendingCountry in dbContext.Country on c.SendingCountry equals SendingCountry.CountryCode
                                                join wallet in dbContext.MobileWalletOperator on c.ReceiverWalletId equals wallet.Id into walletInfo
                                                from wallet in walletInfo.DefaultIfEmpty()
                                                select new NewSenderTransactionStatementListVm()
                                                {
                                                    Id = c.Id,
                                                    TranasactionId = c.Id,
                                                    Date = c.TransactionDate.ToString("yyyy/MM/dd"),
                                                    DateTime = c.TransactionDate,
                                                    Last4Digits = "",
                                                    Amount = c.FaxingAmount,
                                                    BillingAddress = "",
                                                    CardIssuer = "",
                                                    CountryCurrency = SendingCountry.CurrencySymbol,
                                                    Fee = c.FaxingFee,
                                                    Receiver = wallet.Name,
                                                    Status = Common.Common.GetEnumDescription(FaxingStatus.Completed),


                                                }).ToList();
            return result.Concat(transactionUsingKiipayWallet).ToList();

        }

        public List<NewSenderTransactionStatementListVm> GetMobileTransferDetails(int SenderId = 0, int year = 0)
        {

            List<NewSenderTransactionStatementListVm> result = new List<NewSenderTransactionStatementListVm>();

            var data = dbContext.MobileMoneyTransfer as IQueryable<MobileMoneyTransfer>;

            if (SenderId != 0)
            {

                data = data.Where(x => x.SenderId == SenderId);
            }
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year);
            }
            result = (from c in data.ToList()
                      join d in dbContext.FaxerInformation on c.SenderId equals d.Id
                      join SendingCountry in dbContext.Country on c.SendingCountry equals SendingCountry.CountryCode
                      join ReceivingCountry in dbContext.Country on c.ReceivingCountry equals ReceivingCountry.CountryCode
                      select new NewSenderTransactionStatementListVm()
                      {
                          Id = c.Id,
                          TranasactionId = c.Id,
                          Date = c.TransactionDate.ToString("yyyy/MM/dd"),
                          DateTime = c.TransactionDate,
                          Last4Digits = "",
                          Amount = c.SendingAmount,
                          BillingAddress = "",
                          CardIssuer = "",
                          CountryCurrency = SendingCountry.CurrencySymbol,
                          Fee = c.Fee,
                          Receiver = c.ReceiverName,
                          Status = Enum.GetName(typeof(MobileMoneyTransferStatus), c.Status)
                      }).ToList();
            return result;
        }

        public List<NewSenderTransactionStatementListVm> GetNewSenderTransactionStatement(int year)
        {
            var data = new List<DB.FaxerInformation>();

            data = List().Where(x => x.IsDeleted == false).ToList();
            var result = (from c in data
                          join d in dbContext.FaxerLogin on c.Id equals d.FaxerId
                           into j
                          from joined in j.DefaultIfEmpty()
                          join country in dbContext.Country on c.Country equals country.CountryCode
                          select new NewSenderTransactionStatementListVm()
                          {

                          }
                      ).ToList();
            return null;
        }
        public bool checkExistingEmail(string email)
        {
            email = email.ToLower();
            var data = dbContext.FaxerInformation.Where(x => x.Email.ToLower() == email).FirstOrDefault();
            if (data == null)
            {
                return true;
            }
            return false;
        }

        public bool AddNewNote(FaxerNote model)
        {

            dbContext.FaxerNote.Add(model);
            dbContext.SaveChanges();
            return true;
        }
        public List<FaxerNoteViewModel> GetFaxerNotes(int faxerId)
        {

            var result = (from c in dbContext.FaxerNote.Where(x => x.FaxerId == faxerId).OrderByDescending(x => x.CreatedDateTime).ToList()
                          select new FaxerNoteViewModel()
                          {
                              Date = c.CreatedDateTime.ToString("dd/MM/yyyy"),
                              Time = c.CreatedDateTime.ToString("HH:mm"),
                              Note = c.Note,
                              StaffName = c.CreatedByStaffName
                          }).ToList();
            return result;
        }




    }

    public class SenderRegisteredSearchParamVm
    {
        public string SenderName { get; set; }
        public string AccountNo { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string DateRange { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

}