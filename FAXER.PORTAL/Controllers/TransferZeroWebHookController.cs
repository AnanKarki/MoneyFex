using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using TransferZero.Sdk.Api;
using TransferZero.Sdk.Client;
using TransferZero.Sdk.Model;
using static TransferZero.Sdk.Model.Sender;

namespace FAXER.PORTAL.Controllers
{
    public class TransferZeroWebHookController : Controller
    {
        // GET: TransferZeroWebHook
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetRates(string SenderId, string Cur = "GBP")
        {

            TransferZeroApi bankDepositApi = new TransferZeroApi();

            //bankDepositApi.CreateSender();
            var result = bankDepositApi.GetCurrencyOut("b6648ba3-1c7b-4f59-8580-684899c84a12");

            var rates = result.Object.Where(x => x.Code == Cur).Select(x => x.Opposites).ToList();

            return Json(new
            {
                Data = rates
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CallBack(string hook)
        {

            return View();
        }

        public JsonResult CancelTransaction(string id = "3ea479dc-e9c7-4a3d-a6c9-2f91c63e2af1")
        {

            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            Guid? guid = Guid.Parse(id);
            TransactionResponse response = api.GetTransaction(guid);
            var recipientID = response.Object.Recipients[0].Id;

            var apiInstance = new RecipientsApi(configuration);
            //var recipientID = new Guid?(); // Guid? | ID of recipient to cancel.  Example: `/v1/recipients/9d4d7b73-a94c-4979-ab57-09074fd55d33`
            //recipientID = Guid.Parse(receiptId);

            try
            {
                // Cancelling a recipient
                RecipientResponse result = apiInstance.DeleteRecipient(recipientID);
                Debug.WriteLine(result);
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    RecipientResponse result = e.ParseObject<RecipientResponse>();
                    Debug.WriteLine("There was a validation error while processing!");
                    Debug.WriteLine(result);
                }
                else
                {
                    Debug.Print("Exception when calling RecipientsApi.DeleteRecipient: " + e.Message);
                }
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult CancelTransactionByReceiptNo(string receiptNo)
        {
            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            //Guid? guid = Guid.Parse(id);
            //TransactionResponse response = api.GetTransactionStatus(receiptNo);
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            var transaction = transferZeroApi.GetTransactionStatus(receiptNo);

            try
            {
                if (transaction != null && transaction.Recipients != null)
                {
                    var recipientID = transaction.Recipients[0].Id;

                    var apiInstance = new RecipientsApi(configuration);
                    //var recipientID = new Guid?(); // Guid? | ID of recipient to cancel.  Example: `/v1/recipients/9d4d7b73-a94c-4979-ab57-09074fd55d33`
                    //recipientID = Guid.Parse(receiptId);
                    // Cancelling a recipient
                    RecipientResponse result = apiInstance.DeleteRecipient(recipientID);
                    Debug.WriteLine(result);
                    var bankDetails = senderBankAccountDepositServices.GetBankDepositInfoByReceiptNo(receiptNo);
                    //sent transaction cancelled Email

                    senderBankAccountDepositServices.TransactionCancelled(bankDetails);
                    return Json(new { Data = true, Message = "cancelled Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //cancelled transaction and sent transaction cancelled Emai
                    senderBankAccountDepositServices.CancelTransaction(receiptNo);

                    var bankDetails = senderBankAccountDepositServices.GetBankDepositInfoByReceiptNo(receiptNo);
                    senderBankAccountDepositServices.TransactionCancelledByMoneyFex(bankDetails);

                    return Json(new { Data = true, Message = "cancelled Successfully" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    RecipientResponse result = e.ParseObject<RecipientResponse>();
                    Debug.WriteLine("There was a validation error while processing!");
                    Debug.WriteLine(result);
                }
                else
                {
                    Debug.Print("Exception when calling RecipientsApi.DeleteRecipient: " + e.Message);
                }

                return Json(new { Data = false, Message = "Cannot Cancel " }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { Data = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelCashPickUpTransactionByReceiptNo(string receiptNo)
        {
            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            //Guid? guid = Guid.Parse(id);
            //TransactionResponse response = api.GetTransactionStatus(receiptNo);
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            SSenderCashPickUp _senderCashpickUpServices = new SSenderCashPickUp();
            var transaction = transferZeroApi.GetTransactionStatus(receiptNo);

            try
            {
                if (transaction != null && transaction.Recipients != null)
                {
                    var recipientID = transaction.Recipients[0].Id;

                    var apiInstance = new RecipientsApi(configuration);
                    //var recipientID = new Guid?(); // Guid? | ID of recipient to cancel.  Example: `/v1/recipients/9d4d7b73-a94c-4979-ab57-09074fd55d33`
                    //recipientID = Guid.Parse(receiptId);
                    // Cancelling a recipient
                    RecipientResponse result = apiInstance.DeleteRecipient(recipientID);
                    Debug.WriteLine(result);
                    var CashPickUpDetails = _senderCashpickUpServices.GetCashPickUpInfoByReceiptNo(receiptNo);
                    //sent transaction cancelled Email
                    _senderCashpickUpServices.TransactionCancelled(CashPickUpDetails);
                    return Json(new { Data = true, Message = "cancelled Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //cancelled transaction 
                    _senderCashpickUpServices.CancelCashPickUpTransaction(receiptNo);

                    var cashPickUpDetails = _senderCashpickUpServices.GetCashPickUpInfoByReceiptNo(receiptNo);
                    //send transaction cancelled Emai
                    _senderCashpickUpServices.TransactionCancelledByMoneyFex(cashPickUpDetails);

                    return Json(new { Data = true, Message = "cancelled Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    RecipientResponse result = e.ParseObject<RecipientResponse>();
                    Debug.WriteLine("There was a validation error while processing!");
                    Debug.WriteLine(result);
                }
                else
                {
                    Debug.Print("Exception when calling RecipientsApi.DeleteRecipient: " + e.Message);
                }

                return Json(new { Data = false, Message = "Cannot Cancel " }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { Data = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelOtherWalletTransactionByReceiptNo(string receiptNo)
        {
            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            //Guid? guid = Guid.Parse(id);
            //TransactionResponse response = api.GetTransactionStatus(receiptNo);
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            SSenderMobileMoneyTransfer _senderMobileMoneyTransaferServices = new SSenderMobileMoneyTransfer();
            var transaction = transferZeroApi.GetTransactionStatus(receiptNo);

            try
            {
                if (transaction != null && transaction.Recipients != null)
                {
                    var recipientID = transaction.Recipients[0].Id;

                    var apiInstance = new RecipientsApi(configuration);
                    //var recipientID = new Guid?(); // Guid? | ID of recipient to cancel.  Example: `/v1/recipients/9d4d7b73-a94c-4979-ab57-09074fd55d33`
                    //recipientID = Guid.Parse(receiptId);
                    // Cancelling a recipient
                    RecipientResponse result = apiInstance.DeleteRecipient(recipientID);
                    Debug.WriteLine(result);
                    var otherWalletDetails = _senderMobileMoneyTransaferServices.GetOtherWalletInfoByReceiptNo(receiptNo);
                    //sent transaction cancelled Email
                    _senderMobileMoneyTransaferServices.TransactionCancelledEmail(otherWalletDetails);
                    return Json(new { Data = true, Message = "cancelled Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //cancelled transaction 
                    _senderMobileMoneyTransaferServices.CancelOtherWalletTransaction(receiptNo);

                    var otherWalletDetails = _senderMobileMoneyTransaferServices.GetOtherWalletInfoByReceiptNo(receiptNo);
                    //send transaction cancelled Emai
                    _senderMobileMoneyTransaferServices.TransactionCancelledByMoneyFexEmail(otherWalletDetails);

                    return Json(new { Data = true, Message = "cancelled Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    RecipientResponse result = e.ParseObject<RecipientResponse>();
                    Debug.WriteLine("There was a validation error while processing!");
                    Debug.WriteLine(result);
                }
                else
                {
                    Debug.Print("Exception when calling RecipientsApi.DeleteRecipient: " + e.Message);
                }

                return Json(new { Data = false, Message = "Cannot Cancel " }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { Data = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SetTransationToReIntialization(string receiptNo)
        {
            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            //Guid? guid = Guid.Parse(id);
            //TransactionResponse response = api.GetTransactionStatus(receiptNo);
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            var transaction = transferZeroApi.GetTransactionStatus(receiptNo);

            var NewReceiptNo = "";
            try
            {
                if (transaction.Recipients != null)
                {
                    var recipientID = transaction.Recipients[0].Id;

                    var apiInstance = new RecipientsApi(configuration);
                    //var recipientID = new Guid?(); // Guid? | ID of recipient to cancel.  Example: `/v1/recipients/9d4d7b73-a94c-4979-ab57-09074fd55d33`
                    //recipientID = Guid.Parse(receiptId);
                    // Cancelling a recipient
                    RecipientResponse result = apiInstance.DeleteRecipient(recipientID);
                    Debug.WriteLine(result);
                    var bankDetails = senderBankAccountDepositServices.GetBankDepositInfoByReceiptNo(receiptNo);

                    NewReceiptNo = Common.Common.GenerateBankAccountDepositReceiptNo(6);
                    bankDetails.ReceiptNo = NewReceiptNo;
                    bankDetails.Status = DB.BankDepositStatus.UnHold;
                    bankDetails.IsComplianceNeededForTrans = true;
                    bankDetails.IsComplianceApproved = false;
                    senderBankAccountDepositServices.Update(bankDetails);
                    senderBankAccountDepositServices.AddReIntializedTrans(new DB.ReinitializeTransaction()
                    {
                        Date = DateTime.Now,
                        NewReceiptNo = NewReceiptNo,
                        ReceiptNo = receiptNo
                    });
                    //sent transaction cancelled Email

                    return Json(new { Data = NewReceiptNo, Message = "Successfully Set to Reinitialization Point" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    //cancelled transaction and sent transaction cancelled Emai
                    //senderBankAccountDepositServices.CancelTransaction(receiptNo);
                    // return Json(new { Data = true, Message = "cancelled Successfully" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    RecipientResponse result = e.ParseObject<RecipientResponse>();
                    Debug.WriteLine("There was a validation error while processing!");
                    Debug.WriteLine(result);
                }
                else
                {
                    Debug.Print("Exception when calling RecipientsApi.DeleteRecipient: " + e.Message);
                }
            }


            return Json(new { Data = NewReceiptNo, Message = "Cannot Set to reinitialization " }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateReceipentDetials(string id = "3ea479dc-e9c7-4a3d-a6c9-2f91c63e2af1", string accountNo = "",
            string BankCode = "", string name = "")

        {

            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            Guid? guid = Guid.Parse(id);
            TransactionResponse response = api.GetTransaction(guid);
            var recipientID = response.Object.Recipients[0].Id;

            var receipent = response.Object.Recipients[0];
            receipent.PayoutMethod.Details.BankAccount = accountNo;
            if (!string.IsNullOrEmpty(name))
            {
                string FirstName = "";
                string LastName = "";
                string[] nameArr = name.Split(' ');
                switch (name.Split(' ').Length)
                {
                    case 1:
                        FirstName = nameArr[0];
                        break;
                    case 2:

                        FirstName = nameArr[0];
                        LastName = nameArr[1];
                        break;
                    case 3:

                        FirstName = nameArr[0];
                        LastName = nameArr[1] + " " + nameArr[2];
                        break;
                    case 4:
                        FirstName = nameArr[0];
                        LastName = nameArr[1] + " " + nameArr[2] + " " + nameArr[3];
                        break;
                    default:
                        break;
                }

                receipent.PayoutMethod.Details.FirstName = FirstName;
                receipent.PayoutMethod.Details.LastName = LastName;


            }
            var apiInstance = new RecipientsApi(configuration);

            RecipientRequest recipientRequest = new RecipientRequest();
            recipientRequest.Recipient = receipent;
            //var recipientID = new Guid?(); // Guid? | ID of recipient to cancel.  Example: `/v1/recipients/9d4d7b73-a94c-4979-ab57-09074fd55d33`
            //recipientID = Guid.Parse(receiptId);

            try
            {
                // Cancelling a recipient
                RecipientResponse result = apiInstance.PatchRecipient(recipientID, recipientRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    RecipientResponse result = e.ParseObject<RecipientResponse>();
                    Debug.WriteLine("There was a validation error while processing!");
                    Debug.WriteLine(result);
                }
                else
                {
                    Debug.Print("Exception when calling RecipientsApi.DeleteRecipient: " + e.Message);
                }
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CancelTransactionResult(string id = "a4e0540d-6a62-47d3-b6a7-1be9ec82dbb5")
        {
            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            //configuration.ApiKey = "IammYSjfAkddiNFPN4gA9Mu+cZYSYBQjJwGkzIcvKulwHOvdv/cS6MKnA19yiHGD8K/ASm0iIuA5zcyn43nSpQ==";
            //configuration.ApiSecret = "5fuBhR64H+hoPk4MS7glNh6zKSvPHUmEhm6dI7uo/ROUAr+H6F36fgtzSpsvGhsI8oJNVwjY4yFRSw2P+D1hZg==";
            //configuration.BasePath = "https://api.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            var apiInstance = new RecipientsApi(configuration);
            var recipientID = new Guid?(); // Guid? | ID of recipient to cancel.  Example: `/v1/recipients/9d4d7b73-a94c-4979-ab57-09074fd55d33`
            recipientID = Guid.Parse(id);

            try
            {
                // Cancelling a recipient
                RecipientResponse result = apiInstance.DeleteRecipient(recipientID);
                Debug.WriteLine(result);
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    RecipientResponse result = e.ParseObject<RecipientResponse>();
                    Debug.WriteLine("There was a validation error while processing!");
                    Debug.WriteLine(result);
                }
                else
                {
                    Debug.Print("Exception when calling RecipientsApi.DeleteRecipient: " + e.Message);
                }
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }



        //public void CancelTransaction(string id = "a4e0540d-6a62-47d3-b6a7-1be9ec82dbb5")
        //{

        //    TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();

        //    configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
        //    configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
        //    configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
        //    var apiInstance = new RecipientsApi(configuration);
        //    var recipientID = new Guid?(); // Guid? | ID of recipient to cancel.  Example: `/v1/recipients/9d4d7b73-a94c-4979-ab57-09074fd55d33`
        //    recipientID = Guid.Parse(id);

        //    try
        //    {
        //        // Cancelling a recipient
        //        RecipientResponse result = apiInstance.DeleteRecipient(recipientID);
        //        Debug.WriteLine(result);
        //    }
        //    catch (ApiException e)
        //    {
        //        if (e.IsValidationError)
        //        {
        //            // In case there was a validation error, obtain the object
        //            RecipientResponse result = e.ParseObject<RecipientResponse>();
        //            Debug.WriteLine("There was a validation error while processing!");
        //            Debug.WriteLine(result);
        //        }
        //        else
        //        {
        //            Debug.Print("Exception when calling RecipientsApi.DeleteRecipient: " + e.Message);
        //        }
        //    }
        //}

        public JsonResult GetStatusByTransactionId(string id)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();

            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            string[] receipt = id.Split(',');
            List<StatusModel> data = new List<StatusModel>();
            foreach (var item in receipt)
            {
                try
                {


                    Guid guid = Guid.Parse(item);
                    TransactionResponse response = api.GetTransaction(guid);

                    //var result = transferZeroApi.GetTransactionStatus(item);
                    data.Add(new StatusModel()
                    {
                        ReceiptNo = response.Object.ExternalId,
                        Status = Enum.GetName(typeof(TransactionState), response.Object.State),
                        Reference = item
                    });

                }
                catch (Exception ex)
                {

                    data.Add(new StatusModel()
                    {
                        // ReceiptNo = response.Object.ExternalId,
                        Reference = item,
                        Status = "Not Paid"
                    });
                }


            }

            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);

            // return Json(new { response.Object }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetTransferZeroIdAndSenderDetails(string receiptNo)
        {

            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            string[] receipt = receiptNo.Split(',');
            List<ReceiptNoAndSenderDetails> data = new List<ReceiptNoAndSenderDetails>();

            foreach (var item in receipt)
            {
                try
                {
                    var bankDetails = dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == item).FirstOrDefault();
                    var senderDetails = SenderDetails(bankDetails.SenderId);

                    var result = transferZeroApi.GetTransactionStatus(item);

                    data.Add(new ReceiptNoAndSenderDetails()
                    {
                        ReceiptNo = item,
                        ReferenceId = result.Id,
                        SenderName = senderDetails.FirstName + "" + senderDetails.MiddleName + " " + senderDetails.LastName,
                        SenderEmailAddress = senderDetails.Email,
                        TelephoneNo = senderDetails.PhoneNumber,

                    });
                }
                catch
                {
                    var result = transferZeroApi.GetTransactionStatus(item);
                    data.Add(new ReceiptNoAndSenderDetails()
                    {
                        // ReceiptNo = response.Object.ExternalId,
                        ReceiptNo = item,
                        ReferenceId = result.Id,
                    });
                }
            }


            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetTransactionAmountDetails(string fromDate, string toDate)
        {

            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            TransferZeroApi transferZeroApi = new TransferZeroApi();

            List<TransactionAmountDetails> data = new List<TransactionAmountDetails>();
            var FromDate = DateTime.Parse(fromDate);
            var ToDate = DateTime.Parse(toDate);
            try
            {
                var bankDeposit = dbContext.BankAccountDeposit.Where(x => x.TransactionDate >= FromDate && x.TransactionDate <= ToDate).ToList();

                foreach (var item in bankDeposit)
                {
                    var result = transferZeroApi.GetTransactionStatus(item.ReceiptNo);
                    try
                    {
                        data.Add(new TransactionAmountDetails()
                        {
                            Currency = result.InputCurrency,
                            Localamount = result.InputAmount,
                            MFRef = item.ReceiptNo,
                            payoutamount = result.PaidAmount,
                            TransactionDate = item.TransactionDate.ToString("dd/MM/yyyy"),
                            TZRef = result.Id,

                        });
                    }
                    catch (Exception)
                    {
                    }


                }

            }
            catch (Exception ex)
            {
            }

            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetStatusAndSenderDetailsByTransactionId(string id)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();

            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            string[] receipt = id.Split(',');
            List<ReceiptNoAndSenderDetails> data = new List<ReceiptNoAndSenderDetails>();
            foreach (var item in receipt)
            {
                try
                {


                    Guid guid = Guid.Parse(item);
                    TransactionResponse response = api.GetTransaction(guid);

                    var bankDetails = dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == response.Object.ExternalId).FirstOrDefault();
                    var senderDetails = SenderDetails(bankDetails.SenderId);

                    //var result = transferZeroApi.GetTransactionStatus(item);
                    data.Add(new ReceiptNoAndSenderDetails()
                    {
                        ReceiptNo = response.Object.ExternalId,
                        BankAccountNumber = bankDetails.ReceiverAccountNo,
                        BankName = BankNAme(bankDetails.BankId),
                        Currency = Common.Common.GetCountryCurrency(bankDetails.SendingCountry),
                        ReceivingAmount = bankDetails.ReceivingAmount,
                        RecipientName = bankDetails.ReceiverName,
                        SenderEmailAddress = senderDetails.Email,
                        SenderName = senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName,
                        SenderTelephone = senderDetails.PhoneNumber,
                        SendingAmount = bankDetails.SendingAmount,
                        TransactionDate = bankDetails.TransactionDate.ToString(),
                        Status = Enum.GetName(typeof(TransactionState), response.Object.State),
                        Reference = item,

                    });
                }
                catch (Exception ex)
                {

                    data.Add(new ReceiptNoAndSenderDetails()
                    {
                        // ReceiptNo = response.Object.ExternalId,
                        Reference = item,
                        Status = "Not Paid"
                    });
                }


            }

            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);

            // return Json(new { response.Object }, JsonRequestBehavior.AllowGet);
        }

        public string BankNAme(int BankId)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var bank = dbContext.Bank.Where(x => x.Id == BankId).FirstOrDefault();
            return bank.Name;
        }
        public FaxerInformation SenderDetails(int senderId)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var senderDetails = dbContext.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();
            return senderDetails;
        }

        public JsonResult UpdateStatus(string Date)
        {

            DB.FAXEREntities dbContext = new DB.FAXEREntities();

            var data = dbContext.BankAccountDeposit.Where(x => x.Status == DB.BankDepositStatus.Incomplete).ToList();
            foreach (var item in data)
            {

                TransferZeroApi transferZeroApi = new TransferZeroApi();
                var result = transferZeroApi.GetTransactionStatus(item.ReceiptNo);
                if (result != null)
                {
                    switch (result.State)
                    {
                        case TransactionState.Initial:
                            break;
                        case TransactionState.Approved:
                            break;
                        case TransactionState.Pending:
                            item.Status = DB.BankDepositStatus.Incomplete;
                            break;
                        case TransactionState.Received:
                            item.Status = DB.BankDepositStatus.Incomplete;
                            break;
                        case TransactionState.Mispaid:
                            break;
                        case TransactionState.Manual:
                            break;
                        case TransactionState.Paid:
                            item.Status = DB.BankDepositStatus.Confirm;
                            break;
                        case TransactionState.Canceled:
                            item.Status = DB.BankDepositStatus.Cancel;
                            break;
                        case TransactionState.Refunded:
                            item.Status = DB.BankDepositStatus.Cancel;
                            break;
                        case TransactionState.Processing:
                            break;
                        case TransactionState.Exception:
                            break;
                        default:
                            break;
                    }
                    dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }

            }

            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult GetStatus(string receiptNo)
        {

            TransferZeroApi transferZeroApi = new TransferZeroApi();

            var result = transferZeroApi.GetTransactionStatus(receiptNo);

            return Json(new
            {
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetArrayStatus(string receiptNo)
        {

            string[] receipt = receiptNo.Split(',');
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            List<StatusModel> data = new List<StatusModel>();
            foreach (var item in receipt)
            {
                try
                {

                    var result = transferZeroApi.GetTransactionStatus(item);
                    data.Add(new StatusModel()
                    {
                        ReceiptNo = item,
                        Status = Enum.GetName(typeof(TransactionState), result.State)
                    });
                }
                catch (Exception)
                {

                    data.Add(new StatusModel()
                    {
                        ReceiptNo = item,
                        Status = "Not Paid"
                    });
                }


            }

            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetZenithTransferStatus(string receiptNO)
        {

            ZenithApi ZenithApi = new ZenithApi();

            var statusResponse = ZenithApi.GetTransactionStatus(new ZenithTransferTransactionStatusModel()
            {
                Reference = "MF-" + receiptNO
            });

            return Json(new
            {
                Data = statusResponse

            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult TrasactionCreated([Bind(Include = WebhookCallbackModel.BindProperty)]WebhookCallbackModel data)
        {

            //Log.Write("Web Hook Started" + data.Object.external_id);

            string webhookContent = CommonExtension.SerializeObject<WebhookCallbackModel>(data);
            string url = Request.Url.GetLeftPart(UriPartial.Authority) + "/TransferZeroWebHook/TrasactionCreated";
            ////Dictionary<string, string> headers = new Dictionary<string, string>();
            ////headers.Add("Authorization-Nonce", AuthorizationNonce);
            ////headers.Add("Authorization-Signature", AuthorizationSignature);
            ////headers.Add("Authorization-Key", AuthorizationKey);

            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            Webhook Webhook = configuration.ParseString<Webhook>(webhookContent);
            string content = CommonExtension.SerializeObject<Webhook>(Webhook);


            try
            {
                //IsValid = configuration.ValidWebhookRequest(url, content, headers);


                Log.Write("Webhook Initiated Transaction" + data.Object.external_id + " Transaction Id " + data.Object.id);

                Webhook webhook = configuration.ParseString<Webhook>(webhookContent);
                TransactionWebhook transactionWebhook = configuration.ParseString<TransactionWebhook>(webhookContent);
                Guid transactionId = (Guid)transactionWebhook.Object.Id;
                string externalId = transactionWebhook.Object.ExternalId;
                string recipentId = "";
                try
                {
                    recipentId = transactionWebhook.Object.Recipients[0].Id.ToString();

                }
                catch (Exception)
                {

                }
                if (webhook.Event.StartsWith("transaction"))
                {
                    SBankDepositResponseStatus bankDepositResponseStatus = new SBankDepositResponseStatus();

                    var paymenttype = transactionWebhook.Object.Recipients[0].PayoutMethod.Type;
                    if (paymenttype.Contains("Bank"))
                    {

                        var bankDepositData = bankDepositResponseStatus.GetBankTransactionReponse(externalId);
                        TransactionState transactionState = TransactionState.Initial;

                        if (webhook.Event == "transaction.paid_in")
                        {
                            transactionState = TransactionState.Received;
                            if (bankDepositData != null)
                            {
                                //bankDepositData.transactionStatus = ;
                                
                            }

                            Log.Write("Bank Webhook Initiated paid_in");
                        }
                        else if (webhook.Event == "transaction.paid_out")
                        {

                            transactionState = TransactionState.Paid;
                            //var bankDepositData = bankDepositResponseStatus.GetBankTransactionReponse(externalId);
                            //if (bankDepositData != null)
                            //{
                            //    //bankDepositData.transactionStatus = (int)TransactionState.Paid;
                            //    //bankDepositResponseStatus.UpdateTransferZeroTransactionStatus(bankDepositData);
                            //    SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
                            //    //var bankAccountDeposit = _senderBankAccountDepositServices.List().Data.
                            //    //    Where(x => x.Id == bankDepositData.BankDepositResponseStatus.TransactionId).FirstOrDefault();

                            //    bankDepositResponseStatus.UpdateTransferZeroTransactionStatus(bankDepositData.Id, TransactionState.Paid);

                            //   // _senderBankAccountDepositServices.SendEmailAndSms(bankDepositData);

                            //}

                            Log.Write("Bank Webhook Initiated paid_out");
                        }
                        else if (webhook.Event == "transaction.canceled")
                        {
                            transactionState = TransactionState.Canceled;
                            //var bankDepositData = bankDepositResponseStatus.GetBankTransactionReponse(externalId);
                            //if (bankDepositData != null)
                            //{
                            //    //bankDepositData.transactionStatus = (int)TransactionState.Canceled;
                            //    bankDepositResponseStatus.UpdateTransferZeroTransactionStatus(bankDepositData.Id , TransactionState.Canceled);
                                
                            //}

                            Log.Write("Bank Webhook Initiated canceled");
                        }
                        else if (webhook.Event == "transaction.refunded")
                        {
                            transactionState = TransactionState.Canceled;
                            //var bankDepositData = bankDepositResponseStatus.GetBankTransactionReponse(externalId);
                            //if (bankDepositData != null)
                            //{
                            //    //bankDepositData.transactionStatus = (int)TransactionState.Canceled;
                            //    bankDepositResponseStatus.UpdateTransferZeroTransactionStatus(bankDepositData.Id , TransactionState.Canceled);
                            //}

                            Log.Write("Bank Webhook Initiated refunded");
                        }
                        else if (webhook.Event == "transaction.exception")
                        {
                            CancelTransaction(transactionId.ToString());
                            transactionState = TransactionState.Canceled;
                            //var bankDepositData = bankDepositResponseStatus.GetBankTransactionReponse(externalId);
                            //if (bankDepositData != null)
                            //{
                            //    //bankDepositData.transactionStatus = (int)TransactionState.Canceled;
                            //    bankDepositResponseStatus.UpdateTransferZeroTransactionStatus(bankDepositData.Id , TransactionState.Canceled);
                            //}

                            Log.Write("Bank Webhook Initiated exception");
                        }
                        bankDepositResponseStatus.UpdateTransferZeroTransactionStatus(bankDepositData.Id, transactionState);

                    }
                    else if (paymenttype.Contains("Cash"))
                    {
                        SSenderCashPickUp _cashPickUpServices = new SSenderCashPickUp();

                        if (webhook.Event == "transaction.paid_in")
                        {
                            var cashPickUpData = _cashPickUpServices.GetCashPickupTransactionReponse(externalId);
                            if (cashPickUpData != null)
                            {
                                cashPickUpData.transactionStatus = (int)TransactionState.Received;
                                _cashPickUpServices.UpdateTransferZeroTransactionStatus(cashPickUpData);
                            }

                            Log.Write("Cash Webhook Initiated paid_in");
                        }
                        else if (webhook.Event == "transaction.paid_out")
                        {
                            var cashPickUpData = _cashPickUpServices.GetCashPickupTransactionReponse(externalId);

                            if (cashPickUpData != null)
                            {
                                cashPickUpData.transactionStatus = (int)TransactionState.Paid;
                                _cashPickUpServices.UpdateTransferZeroTransactionStatus(cashPickUpData);

                                var cashPickUp = _cashPickUpServices.List().Data.
                                    Where(x => x.Id == cashPickUpData.CashPickUpResponseStatus.TransactionId).FirstOrDefault();

                                _cashPickUpServices.SendEmailAndSms(cashPickUp);

                            }

                            Log.Write("Cash Webhook Initiated paid_out");
                        }
                        else if (webhook.Event == "transaction.canceled")
                        {

                            var cashPickUpData = _cashPickUpServices.GetCashPickupTransactionReponse(externalId);
                            if (cashPickUpData != null)
                            {
                                cashPickUpData.transactionStatus = (int)TransactionState.Canceled;
                                _cashPickUpServices.UpdateTransferZeroTransactionStatus(cashPickUpData);
                                SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
                                var cashPickUp = _cashPickUpServices.List().Data.
                                     Where(x => x.Id == cashPickUpData.CashPickUpResponseStatus.TransactionId).FirstOrDefault();
                                _cashPickUpServices.SendEmailAndSms(cashPickUp);
                            }

                            Log.Write("Cash Webhook Initiated canceled");
                        }
                        else if (webhook.Event == "transaction.refunded")
                        {

                            var cashPickUpData = _cashPickUpServices.GetCashPickupTransactionReponse(externalId);
                            if (cashPickUpData != null)
                            {
                                cashPickUpData.transactionStatus = (int)TransactionState.Canceled;
                                _cashPickUpServices.UpdateTransferZeroTransactionStatus(cashPickUpData);
                            }

                            Log.Write("Cash Webhook Initiated refunded");
                        }
                        else if (webhook.Event == "transaction.exception")
                        {
                            CancelTransaction(transactionId.ToString());
                            var cashPickUpData = _cashPickUpServices.GetCashPickupTransactionReponse(externalId);
                            if (cashPickUpData != null)
                            {
                                cashPickUpData.transactionStatus = (int)TransactionState.Canceled;
                                _cashPickUpServices.UpdateTransferZeroTransactionStatus(cashPickUpData);
                            }

                            Log.Write("Cash Webhook Initiated exception");
                        }
                    }
                    else
                    {
                        SMobileMoneyTransferResopnseStatus sMobileMoneyTransferResopnse = new SMobileMoneyTransferResopnseStatus();
                        if (webhook.Event == "transaction.paid_in")
                        {
                            var result = sMobileMoneyTransferResopnse.GetMobileReponseLog(externalId);
                            if (result != null)
                            {
                                result.Status = TransactionState.Received.ToString();
                                sMobileMoneyTransferResopnse.UpdateTransferZeroMobileTransactionStatus(result, (int)TransactionState.Paid);

                            }

                            Log.Write("mobile Webhook Initiated paid_in");
                        }
                        else if (webhook.Event == "transaction.paid_out")
                        {

                            var result = sMobileMoneyTransferResopnse.GetMobileReponseLog(externalId);
                            result.Status = TransactionState.Paid.ToString();
                            sMobileMoneyTransferResopnse.UpdateTransferZeroMobileTransactionStatus(result, (int)TransactionState.Paid);

                            SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
                            var MobileMoneyTransferData = _senderMobileMoneyTransferServices.list().Data.Where(x => x.Id == result.TransactionId).FirstOrDefault();
                            _senderMobileMoneyTransferServices.SendEmailAndSms(MobileMoneyTransferData);
                            Log.Write("mobile Webhook Initiated paid_out");
                        }
                        else if (webhook.Event == "transaction.canceled")
                        {

                            var result = sMobileMoneyTransferResopnse.GetMobileReponseLog(externalId);
                            result.Status = TransactionState.Canceled.ToString();
                            sMobileMoneyTransferResopnse.UpdateTransferZeroMobileTransactionStatus(result, (int)TransactionState.Canceled);

                            Log.Write("mobile Webhook Initiated canceled");
                        }
                        else if (webhook.Event == "transaction.refunded")
                        {
                            var result = sMobileMoneyTransferResopnse.GetMobileReponseLog(externalId);
                            result.Status = TransactionState.Canceled.ToString();
                            sMobileMoneyTransferResopnse.UpdateTransferZeroMobileTransactionStatus(result, (int)TransactionState.Canceled);

                            Log.Write("mobile Webhook Initiated refunded");

                        }
                        else if (webhook.Event == "transaction.exception")
                        {
                            CancelTransaction(transactionId.ToString());
                            var result = sMobileMoneyTransferResopnse.GetMobileReponseLog(externalId);
                            result.Status = TransactionState.Canceled.ToString();
                            sMobileMoneyTransferResopnse.UpdateTransferZeroMobileTransactionStatus(result, (int)TransactionState.Canceled);
                            Log.Write("mobile Webhook Initiated refunded");
                        }
                    }
                }
                else if (webhook.Event.StartsWith("sender"))
                {
                    SenderWebhook senderWebhook = configuration.ParseString<SenderWebhook>(webhookContent);
                    // handle sender events
                }
                else if (webhook.Event.StartsWith("recipient"))
                {

                    Log.Write("recipient Webhook Initiated");
                    var paymenttype = transactionWebhook.Object.Recipients[0].PayoutMethod.Type;
                    if (paymenttype.Contains("Bank"))
                    {
                        SBankDepositResponseStatus bankDepositResponseStatus = new SBankDepositResponseStatus();
                        TransactionState transactionState = TransactionState.Pending;
                        var bankDepositData = bankDepositResponseStatus.GetBankTransactionReponse(externalId);
                        if (webhook.Event == "recipient.error")
                        {
                            CancelTransaction(transactionId.ToString());
                            //SBankDepositResponseStatus bankDepositResponseStatus = new SBankDepositResponseStatus();
                            //var bankDepositData = bankDepositResponseStatus.GetBankTransactionReponse(externalId);
                            //if (bankDepositData != null)
                            //{
                            //    bankDepositData.transactionStatus = (int)TransactionState.Canceled;
                            //    bankDepositResponseStatus.UpdateTransferZeroTransactionStatus(bankDepositData);
                            //}
                            transactionState = TransactionState.Canceled;

                            Log.Write("recipient Webhook error");
                        }
                        else if (webhook.Event == "recipient.refunded")
                        {
                            transactionState = TransactionState.Canceled;
                            Log.Write("recipient Webhook refunded");
                        }
                        else if (webhook.Event == "recipient.paid_out")
                        {

                            //SBankDepositResponseStatus bankDepositResponseStatus = new SBankDepositResponseStatus();
                            //var bankDepositData = bankDepositResponseStatus.GetBankTransactionReponse(externalId);
                            //if (bankDepositData != null)
                            //{
                            //    bankDepositData.transactionStatus = (int)TransactionState.Paid;
                            //    bankDepositResponseStatus.UpdateTransferZeroTransactionStatus(bankDepositData);
                            //    SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
                            //    var bankAccountDeposit = _senderBankAccountDepositServices.List().Data.
                            //        Where(x => x.Id == bankDepositData.BankDepositResponseStatus.TransactionId).FirstOrDefault();
                            //    _senderBankAccountDepositServices.SendEmailAndSms(bankAccountDeposit);
                            //}
                            transactionState = TransactionState.Paid;
                            Log.Write("recipient Webhook paid_out");
                        }
                        bankDepositResponseStatus.UpdateTransferZeroTransactionStatus(bankDepositData.Id, transactionState);


                    }
                    else
                    {
                        SMobileMoneyTransferResopnseStatus sMobileMoneyTransferResopnse = new SMobileMoneyTransferResopnseStatus();
                        if (webhook.Event == "recipient.error")
                        {

                            CancelTransaction(transactionId.ToString());
                            var result = sMobileMoneyTransferResopnse.GetMobileReponseLog(externalId);
                            if (result != null)
                            {
                                result.Status = TransactionState.Received.ToString();
                                sMobileMoneyTransferResopnse.UpdateTransferZeroMobileTransactionStatus(result, (int)TransactionState.Canceled);
                            }

                            Log.Write("recipient Webhook cancel");
                        }
                        else if (webhook.Event == "recipient.refunded")
                        {

                            var result = sMobileMoneyTransferResopnse.GetMobileReponseLog(externalId);
                            result.Status = TransactionState.Paid.ToString();
                            sMobileMoneyTransferResopnse.UpdateTransferZeroMobileTransactionStatus(result, (int)TransactionState.Canceled);
                            Log.Write("recipient Webhook refunded");
                        }
                        else if (webhook.Event == "recipient.paid_out")
                        {

                            var result = sMobileMoneyTransferResopnse.GetMobileReponseLog(externalId);
                            result.Status = TransactionState.Canceled.ToString();
                            sMobileMoneyTransferResopnse.UpdateTransferZeroMobileTransactionStatus(result, (int)TransactionState.Paid);

                            SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
                            var MobileMoneyTransferData = _senderMobileMoneyTransferServices.list().Data.Where(x => x.Id == result.TransactionId).FirstOrDefault();
                            _senderMobileMoneyTransferServices.SendEmailAndSms(MobileMoneyTransferData);
                            Log.Write("recipient Webhook paid-out");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return Json(new { Data = data });
        }

        public JsonResult UpdateMobileTransactionStatus()
        {

            SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            var mobileTransfer = _senderMobileMoneyTransferServices.list().Data.Where(x => x.Apiservice == DB.Apiservice.TransferZero).ToList();

            TransferZeroApi transferZeroApi = new TransferZeroApi();
            foreach (var item in mobileTransfer)
            {
                try
                {

                    var result = transferZeroApi.GetTransactionStatus(item.ReceiptNo);

                    switch (result.State)
                    {
                        case TransactionState.Initial:
                            break;
                        case TransactionState.Approved:
                            break;
                        case TransactionState.Pending:
                            break;
                        case TransactionState.Received:
                            break;
                        case TransactionState.Mispaid:
                            break;
                        case TransactionState.Manual:
                            break;
                        case TransactionState.Paid:
                            item.Status = DB.MobileMoneyTransferStatus.Paid;
                            Log.Write("Transaction Status need to be updated");
                            Log.Write(item.ReceiptNo);

                            break;
                        case TransactionState.Canceled:
                            break;
                        case TransactionState.Refunded:
                            item.Status = DB.MobileMoneyTransferStatus.Cancel;
                            break;
                        case TransactionState.Processing:
                            break;
                        case TransactionState.Exception:
                            break;
                        default:
                            break;
                    }
                    _senderMobileMoneyTransferServices.Update(item);

                }
                catch (Exception)
                {

                }

            }
            return new JsonResult()
            {

            };
        }
    }

    public class Response
    {


        public string webhook { get; set; }
        [DataMember(Name = "event", EmitDefaultValue = false)]
        public string Event { get; set; }

        [DataMember(Name = "object", EmitDefaultValue = false)]
        public WebhookTransaction Object { get; set; }
    }

    public class WebhookTransaction
    {

        public WebhookTransaction()
        {

        }
        [DataMember(Name = "", EmitDefaultValue = false)]
        public DateTime? expires_at { get; set; }
        [DataMember(Name = "created_at", EmitDefaultValue = false)]
        public DateTime? created_at { get; set; }
        [DataMember(Name = "due_amount", EmitDefaultValue = false)]
        public decimal? due_amount { get; set; }
        [DataMember(Name = "paid_amount", EmitDefaultValue = false)]
        public decimal? paid_amount { get; set; }
        [DataMember(Name = "payin_reference", EmitDefaultValue = false)]
        public string payin_reference { get; set; }
        [DataMember(Name = "input_amount", EmitDefaultValue = false)]
        public decimal? input_amount { get; set; }
        [DataMember(Name = "state", EmitDefaultValue = false)]
        public TransactionState state { get; set; }

        [DataMember(Name = "traits", EmitDefaultValue = false)]
        public TransactionTraits traits { get; set; }

        //[DataMember(Name = "recipients", EmitDefaultValue = false)]
        public List<WebHookReceipent> recipients { get; set; }
        [DataMember(Name = "sender", EmitDefaultValue = false)]
        public WebHookSender sender { get; set; }
        [DataMember(Name = "metadata", EmitDefaultValue = false)]
        public object metadata { get; set; }
        [DataMember(Name = "payin_methods", EmitDefaultValue = false)]
        public List<PayIn> payin_methods { get; set; }
        [DataMember(Name = "input_currency", EmitDefaultValue = false)]

        public string input_currency { get; set; }
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public Guid? id { get; }
    }

    public class PayIn
    {
        [DataMember(Name = "type", EmitDefaultValue = false)]
        public string type { get; set; }
        [DataMember(Name = "in_details", EmitDefaultValue = false)]
        public PayInDetials in_details { get; set; }
        [DataMember(Name = "out_details", EmitDefaultValue = false)]
        public object out_details { get; }
        [DataMember(Name = "instructions", EmitDefaultValue = false)]
        public object instructions { get; }
        [DataMember(Name = "provider", EmitDefaultValue = false)]
        public string provider
        {
            get; set;
        }
    }

    public class PayInDetials
    {
        [DataMember(Name = "payment_method", EmitDefaultValue = false)]
        public string payment_method { get; set; }
        [DataMember(Name = "redirect_url", EmitDefaultValue = false)]
        public string redirect_url { get; set; }
        [DataMember(Name = "phone_number", EmitDefaultValue = false)]
        public string phone_number { get; set; }
        [DataMember(Name = "send_instructions", EmitDefaultValue = false)]
        public bool? send_instructions { get; set; }
        [DataMember(Name = "refund_address", EmitDefaultValue = false)]
        public string refund_address { get; set; }

    }

    public class WebHookReceipent
    {
        [DataMember(Name = "output_currency", EmitDefaultValue = false)]
        public string output_currency { get; }
        [DataMember(Name = "output_amount", EmitDefaultValue = false)]
        public decimal? output_amount { get; }
        [DataMember(Name = "input_currency", EmitDefaultValue = false)]
        public string input_currency { get; }
        [DataMember(Name = "input_amount", EmitDefaultValue = false)]
        public decimal? input_amount { get; }
        [DataMember(Name = "fee_fractional", EmitDefaultValue = false)]
        public decimal? fee_fractional { get; }
        [DataMember(Name = "exchange_rate", EmitDefaultValue = false)]
        public decimal? exchange_rate { get; }
        [DataMember(Name = "transaction_state", EmitDefaultValue = false)]
        public TransactionState transaction_state { get; set; }
        [DataMember(Name = "transaction_id", EmitDefaultValue = false)]
        public string transaction_id { get; }
        [DataMember(Name = "state", EmitDefaultValue = false)]
        public RecipientState state { get; set; }
        [DataMember(Name = "state_reason", EmitDefaultValue = false)]
        public string state_reason { get; }
        [DataMember(Name = "may_cancel", EmitDefaultValue = false)]
        public bool? may_cancel { get; }
        [DataMember(Name = "input_usd_amount", EmitDefaultValue = false)]
        public decimal? input_usd_amount { get; }
        [DataMember(Name = "retriable", EmitDefaultValue = false)]
        public bool? retriable { get; }
        [DataMember(Name = "editable", EmitDefaultValue = false)]
        public bool? editable { get; }
        [DataMember(Name = "created_at", EmitDefaultValue = false)]
        public DateTime? created_at { get; }
        [DataMember(Name = "metadata", EmitDefaultValue = false)]
        public object metadata { get; set; }
        [DataMember(Name = "payout_method", EmitDefaultValue = false)]
        public Payouts payout_method { get; set; }

        [DataMember(Name = "requested_currency", EmitDefaultValue = false)]
        public string requested_currency { get; set; }
        [DataMember(Name = "requested_amount", EmitDefaultValue = false)]
        public decimal? requested_amount { get; set; }
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public Guid? id { get; }
    }


    public class Payouts
    {


        [DataMember(Name = "type", EmitDefaultValue = false)]
        public string Type { get; set; }
        [DataMember(Name = "details", EmitDefaultValue = false)]
        public PayoutDetials Details { get; set; }
        [DataMember(Name = "metadata", EmitDefaultValue = false)]
        public object Metadata { get; set; }
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public Guid? Id { get; set; }
        //[DataMember(Name = "errors", EmitDefaultValue = false)]
        //public Dictionary<string, List<ValidationErrorDescription>> Errors { get; set; }
        //[DataMember(Name = "fields", EmitDefaultValue = false)]
        //public Dictionary<string, FieldDescription> Fields
        //{
        //    get; set;
        //}
    }
    public class PayoutDetials
    {

        [DataMember(Name = "reference", EmitDefaultValue = false)]
        public string Reference { get; set; }
        [DataMember(Name = "identity_card_id", EmitDefaultValue = false)]
        public string identity_card_id { get; set; }
        [DataMember(Name = "identity_card_type", EmitDefaultValue = false)]
        public PayoutMethodIdentityCardTypeEnum identity_card_type { get; set; }
        [DataMember(Name = "reason", EmitDefaultValue = false)]
        public string reason { get; set; }
        [DataMember(Name = "sender_gender", EmitDefaultValue = false)]
        public PayoutMethodGenderEnum sender_gender { get; set; }
        [DataMember(Name = "sender_country_of_birth", EmitDefaultValue = false)]
        public string sender_country_of_birth { get; set; }
        [DataMember(Name = "sender_city_of_birth", EmitDefaultValue = false)]
        public string sender_city_of_birth { get; set; }
        [DataMember(Name = "sender_identity_card_id", EmitDefaultValue = false)]
        public string sender_identity_card_id { get; set; }
        [DataMember(Name = "sender_identity_card_type", EmitDefaultValue = false)]
        public PayoutMethodIdentityCardTypeEnum sender_identity_card_type { get; set; }
        [DataMember(Name = "bic", EmitDefaultValue = false)]
        public string Bic { get; set; }
        [DataMember(Name = "bank_country", EmitDefaultValue = false)]
        public string bank_country { get; set; }
        [DataMember(Name = "bank_name", EmitDefaultValue = false)]
        public string bank_name { get; set; }
        [DataMember(Name = "iban", EmitDefaultValue = false)]
        public string Iban { get; set; }
        [DataMember(Name = "mobile_provider", EmitDefaultValue = false)]
        public PayoutMethodMobileProviderEnum mobile_provider { get; set; }
        [DataMember(Name = "phone_number", EmitDefaultValue = false)]
        public string phone_number { get; set; }
        [DataMember(Name = "bank_account_type", EmitDefaultValue = false)]
        public PayoutMethodBankAccountTypeEnum bank_account_type { get; set; }
        [DataMember(Name = "bank_account", EmitDefaultValue = false)]
        public string bank_account { get; set; }
        [DataMember(Name = "bank_code", EmitDefaultValue = false)]
        public string bank_code { get; set; }
        [DataMember(Name = "last_name", EmitDefaultValue = false)]
        public string last_name { get; set; }
        [DataMember(Name = "first_name", EmitDefaultValue = false)]
        public string first_name { get; set; }
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Name = "address", EmitDefaultValue = false)]
        public string Address { get; set; }
    }
    public class WebHookSender
    {

        [DataMember(Name = "registration_date", EmitDefaultValue = false)]
        public string registration_date { get; set; }
        [DataMember(Name = "registration_number", EmitDefaultValue = false)]
        public string registration_number { get; set; }
        [DataMember(Name = "nature_of_business", EmitDefaultValue = false)]
        public string nature_of_business { get; set; }
        [DataMember(Name = "source_of_funds", EmitDefaultValue = false)]
        public string source_of_funds { get; set; }
        [DataMember(Name = "core_business_activities", EmitDefaultValue = false)]
        public string core_business_activities { get; set; }
        [DataMember(Name = "purpose_of_opening_account", EmitDefaultValue = false)]
        public string purpose_of_opening_account { get; set; }
        [DataMember(Name = "office_phone", EmitDefaultValue = false)]
        public string office_phone { get; set; }
        [DataMember(Name = "vat_registration_number", EmitDefaultValue = false)]
        public string vat_registration_number { get; set; }
        [DataMember(Name = "financial_regulator", EmitDefaultValue = false)]
        public string financial_regulator { get; set; }
        [DataMember(Name = "regulatory_licence_number", EmitDefaultValue = false)]
        public string regulatory_licence_number { get; set; }
        [DataMember(Name = "contact_person_email", EmitDefaultValue = false)]
        public string contact_person_email { get; set; }
        [DataMember(Name = "trading_country", EmitDefaultValue = false)]
        public string trading_country { get; set; }
        [DataMember(Name = "trading_address", EmitDefaultValue = false)]
        public string trading_address { get; set; }
        [DataMember(Name = "number_monthly_transactions", EmitDefaultValue = false)]
        public string number_monthly_transactions { get; set; }
        [DataMember(Name = "amount_monthly_transactions", EmitDefaultValue = false)]
        public string amount_monthly_transactions { get; set; }
        [DataMember(Name = "documents", EmitDefaultValue = false)]
        public List<Document> documents { get; set; }
        [DataMember(Name = "metadata", EmitDefaultValue = false)]
        public object metadata { get; set; }

        [DataMember(Name = "onboarding_status", EmitDefaultValue = false)]
        public string OnboardingStatus { get; set; }
        [DataMember(Name = "politically_exposed_people", EmitDefaultValue = false)]
        public List<PoliticallyExposedPerson> PoliticallyExposedPeople { get; set; }
        [DataMember(Name = "external_id", EmitDefaultValue = false)]
        public string ExternalId { get; set; }
        [DataMember(Name = "nationality", EmitDefaultValue = false)]
        public string Nationality { get; set; }
        [DataMember(Name = "occupation", EmitDefaultValue = false)]
        public string Occupation { get; set; }
        [DataMember(Name = "birth_date", EmitDefaultValue = false)]
        [JsonConverter(typeof(OpenAPIDateConverter))]
        public DateTime? BirthDate { get; set; }
        [DataMember(Name = "country", EmitDefaultValue = false)]
        public string Country { get; set; }
        [DataMember(Name = "type", EmitDefaultValue = false)]
        public TypeEnum? Type { get; set; }
        [DataMember(Name = "identification_type", EmitDefaultValue = false)]
        public IdentificationTypeEnum? IdentificationType { get; set; }
        [DataMember(Name = "legal_entity_type", EmitDefaultValue = false)]
        public LegalEntityTypeEnum? LegalEntityType { get; set; }
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public Guid? Id { get; set; }
        [DataMember(Name = "state", EmitDefaultValue = false)]
        public SenderState State { get; set; }
        [DataMember(Name = "last_name", EmitDefaultValue = false)]
        public string LastName { get; set; }
        [DataMember(Name = "street", EmitDefaultValue = false)]
        public string Street { get; set; }
        [DataMember(Name = "city", EmitDefaultValue = false)]
        public string City { get; set; }
        [DataMember(Name = "postal_code", EmitDefaultValue = false)]
        public string PostalCode { get; set; }
        [DataMember(Name = "phone_number", EmitDefaultValue = false)]
        public string PhoneNumber { get; set; }
        [DataMember(Name = "email", EmitDefaultValue = false)]
        public string Email { get; set; }
        [DataMember(Name = "ip", EmitDefaultValue = false)]
        public string Ip { get; set; }
        [DataMember(Name = "address_description", EmitDefaultValue = false)]
        public string AddressDescription { get; set; }
        [DataMember(Name = "identification_number", EmitDefaultValue = false)]
        public string IdentificationNumber { get; set; }
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Name = "first_name", EmitDefaultValue = false)]
        public string FirstName { get; set; }
        [DataMember(Name = "middle_name", EmitDefaultValue = false)]
        public string MiddleName { get; set; }
        [DataMember(Name = "phone_country", EmitDefaultValue = false)]
        public string PhoneCountry { get; set; }
    }

    public class StatusModel
    {

        public string ReceiptNo { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }

        public string NewReceiptNo { get; set; }
        public string Message { get; set; }

    }
    public class ReceiptNoAndSenderDetails
    {

        public string ReceiptNo { get; set; }
        public string TelephoneNo { get; set; }
        public string SenderEmailAddress { get; set; }
        public string SenderName { get; set; }
        public Guid? ReferenceId { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string Currency { get; set; }
        public string SenderTelephone { get; set; }
        public string RecipientName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public string TransactionDate { get; set; }

    }

    public class TransactionAmountDetails
    {
        public string TransactionDate { get; set; }
        public string Currency { get; set; }
        public decimal? payoutamount { get; set; }
        public decimal? Localamount { get; set; }
        public Guid? TZRef { get; set; }
        public string MFRef { get; set; }

    }
}