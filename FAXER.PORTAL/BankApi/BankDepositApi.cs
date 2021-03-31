using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using TransferZero.Sdk.Api;
using TransferZero.Sdk.Client;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.BankApi
{
    public class BankDepositApi
    {

        public async Task<T> Post<T>(string model, Models.AccessTokenVM accessTokenVM)
        {

            // Test Url 
            //var url = "http://jnfx.api.test.vggdev.com/api/Transactions/PostTransactionLocal";

            // Live Url
            //var url = "https://api.jnfx-emarkets.com/api/transactions/Posttransactionlocal";



            var url = Common.Common.GetAppSettingValue("VGNPostTransctionUrl").ToString();

            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Headers.Add("Authorization", accessTokenVM.TokenType + " " + accessTokenVM.AccessToken);
            http.Headers.Add("Cache-Control", "no-cache");
            http.Method = "POST";

            byte[] utf8bytes = Encoding.UTF8.GetBytes(model.ToString());

            byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);
            http.ContentType = "application/json";

            var content = new StringContent(model.ToString(), Encoding.UTF8, "application/json");



            http.ContentLength = model.Length;
            var sw = new StreamWriter(http.GetRequestStream());
            sw.Write(model);
            sw.Close();
            string body = "";
            try
            {
                var res = (HttpWebResponse)http.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                //Read the response into an xml document 

                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1"));
                body = reader.ReadToEnd();

            }
            catch (Exception ex)
            {

                throw;
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;


            //return await DeserializeObject<T>(responseString);


        }


        public Task<T> Login<T>()
        {

            // Test Url
            //var url = "http://jnfx.api.test.vggdev.com/api/account/GetUserToken";

            // Live Url
            //var url = "https://api.jnfx-emarkets.com/api/Account";

            var url = Common.Common.GetAppSettingValue("VGNLoginUrl").ToString();

            //var url = "http://jnfx.api.test.vggdev.com/api/account/GetUserToken";
            using (var client = new WebClient())
            {

                //client.Headers.Add("Postman-Token", "f1408656-4167-4cef-9e27-23a90bef4662");
                client.Headers.Add("Cache-Control", "no-cache");

                var payload = new NameValueCollection();

                // test key 
                payload.Add("ClientId", "EMPIRE_2_API1ARSYDG1GOY3");
                payload.Add("ClientSecret", "W0zTu7UumG3fWorN");
                payload.Add("userName", "top.py061@gmail.com");
                payload.Add("password", "Password1!");

                // Live key
                //payload.Add("ClientId", Common.Common.GetAppSettingValue("VGNClientId").ToString());
                //payload.Add("ClientSecret", Common.Common.GetAppSettingValue("VGNClientSecret").ToString());
                //payload.Add("userName", Common.Common.GetAppSettingValue("VGNuserName").ToString());
                //payload.Add("password", Common.Common.GetAppSettingValue("VGNpassword").ToString());

                var responseString = "";
                try
                {
                    var response = client.UploadValues(url, "POST", payload);
                    responseString = Encoding.Default.GetString(response);

                }
                catch (Exception ex)
                {

                    //write to  Log file 
                }


                var result = CommonExtension.DeserializeObject<T>(responseString);
                return result;

            }
        }



        public async Task<T> TransactionConfirmation<T>(Models.BankDepositResponseResult model, Models.AccessTokenVM accessTokenVM)
        {



            // Test url 
            //var url = "http://jnfx.api.test.vggdev.com/api/Transactions/Status";

            // Live url
            //var url = "https://api.jnfx-emarkets.com/api/Transactions/Status";

            var url = Common.Common.GetAppSettingValue("VGNTransactionConfirmationUrl").ToString();

            string body = "";
            // Create the web request

            HttpWebRequest request = WebRequest.Create(url + "?TransactionReference=" + model.transactionReference) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json; charset=iso-8859-1";
            request.Headers.Add("Authorization", accessTokenVM.TokenType + " " + accessTokenVM.AccessToken);
            request.ServicePoint.Expect100Continue = false;
            try
            {
                try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream
                        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                        body = reader.ReadToEnd();
                    }
                }
                catch (WebException wex)
                {
                    StreamReader reader = new StreamReader(wex.Response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                    body = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {

            }
            var result = await CommonExtension.DeserializeObject<T>(body);
            return result;


        }



        public async Task<T> ValidateAccountNo<T>(string bankcode, string accountNo, Models.AccessTokenVM accessTokenVM)
        {


            // Test Url
            //var url = "http://jnfx.api.test.vggdev.com/api/Transactions/AccountLookUp";

            // Live Url
            //var url = "https://api.jnfx-emarkets.com/api/Transactions/AccountLookUp";

            var url = Common.Common.GetAppSettingValue("VGNAccountLookUpUrl").ToString();

            string body = "";
            // Create the web request
            HttpWebRequest request = WebRequest.Create(url + "?bankCode=" + bankcode + "&accNumber=" + accountNo) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json; charset=iso-8859-1";


            request.Headers.Add("Authorization", accessTokenVM.TokenType + " " + accessTokenVM.AccessToken);
            request.ServicePoint.Expect100Continue = false;
            try
            {
                try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream
                        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                        body = reader.ReadToEnd();
                    }
                }
                catch (WebException wex)
                {
                    StreamReader reader = new StreamReader(wex.Response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                    body = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {

            }
            var result = await CommonExtension.DeserializeObject<T>(body);
            return result;
        }

        public async Task<T> TransactionConfirmationResult<T>(string refno, string PartnerRef, Models.AccessTokenVM accessTokenVM)
        {





            //var url = "http://jnfx.api.test.vggdev.com/api/Transactions/Status";
            var url = Common.Common.GetAppSettingValue("VGNTransactionConfirmationUrl").ToString();

            //var url = "https://api.jnfx-emarkets.com/api/Transactions/Status";


            string body = "";
            // Create the web request

            HttpWebRequest request = WebRequest.Create(url + "?TransactionReference=" + refno + "&PartnerReference=" + PartnerRef) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json; charset=iso-8859-1";
            request.Headers.Add("Authorization", accessTokenVM.TokenType + " " + accessTokenVM.AccessToken);
            request.ServicePoint.Expect100Continue = false;
            try
            {
                try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream
                        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                        body = reader.ReadToEnd();
                    }
                }
                catch (WebException wex)
                {
                    StreamReader reader = new StreamReader(wex.Response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                    body = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {

            }
            var result = await CommonExtension.DeserializeObject<T>(body);
            return result;


        }

        public void BankDepositGetStatusCallBack(string refno, string PartnerRef, bool isTerminatedTrans = false)
        {

            var accessToken = Login<AccessTokenVM>();

            var url = Common.Common.GetAppSettingValue("VGNTransactionConfirmationUrl").ToString();

            var result = new BankDepositResponseVm();
            if (isTerminatedTrans == false)
            {
                result = TransactionConfirmationResult<BankDepositResponseVm>(refno, PartnerRef, accessToken.Result).Result ?? new BankDepositResponseVm();
            }
            string body = "";

            //var result =   DeserializeObject<BankDepositResponseVm>(body);



            if (result.result == null || (result.result != null && result.result.transactionStatus == 3))
            {
                SBankDepositStatus sBankDeposit = new SBankDepositStatus();
                // Re-initial the transaction 

                var bankDepositTransdata = sBankDeposit.getTransactionDetail(PartnerRef);
                BankDepositLocalRequest vm = new BankDepositLocalRequest()
                {
                    partnerTransactionReference = bankDepositTransdata.ReceiptNo,
                    baseCurrencyCode = Common.Common.GetCountryCurrency(bankDepositTransdata.SendingCountry),
                    targetCurrencyCode = Common.Common.GetCountryCurrency(bankDepositTransdata.ReceiverCountry),
                    baseCurrencyAmount = bankDepositTransdata.SendingAmount,
                    targetCurrencyAmount = bankDepositTransdata.ReceivingAmount,
                    partnerCode = null,
                    purpose = "Payment " + bankDepositTransdata.ReceiverName,
                    accountNumber = bankDepositTransdata.ReceiverAccountNo,
                    bankCode = bankDepositTransdata.BankCode,
                    baseCountryCode = bankDepositTransdata.SendingCountry,
                    targetCountryCode = bankDepositTransdata.ReceiverCountry,
                    payerName = bankDepositTransdata.ReceiverName,
                    payermobile = ""
                };
                var transaction = Post<BankDepositResponseVm>(CommonExtension.SerializeObject<BankDepositLocalRequest>(vm), accessToken.Result);
                if (transaction.Result == null)
                {


                }
                else
                {


                    sBankDeposit.Update(transaction.Result);


                    //
                }

                //try
                //{

                //    result =  TransactionConfirmation<BankDepositResponseVm>(transaction.Result.result , accessToken.Result).Result;

                //}
                //catch (Exception)
                //{

                //    //throw;
                //}

            }
            // case of transaction pending , proccessing and successful
            else
            {

                SBankDepositStatus sBankDeposit = new SBankDepositStatus();
                sBankDeposit.Update(result);

                // Update the transaction status

            }


        }
        #region ATL Api Bank Transfer

        public async Task<T> PostATLTransaction<T>(string model, Models.AccessTokenVM accessToken)
        {


            var url = "https://www.atlmoneytransfer.com/api/transactions";

            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Headers.Add("Authorization", "Bearer" + " " + "sandbox_5e1328afc8e6d5e1328afc8f19");
            http.Headers.Add("Cache-Control", "no-cache");
            http.Method = "POST";

            byte[] utf8bytes = Encoding.UTF8.GetBytes(model.ToString());

            byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);
            http.ContentType = "application/json";

            var content = new StringContent(model.ToString(), Encoding.UTF8, "application/json");



            http.ContentLength = model.Length;
            var sw = new StreamWriter(http.GetRequestStream());
            sw.Write(model);
            sw.Close();
            string body = "";
            try
            {
                var res = (HttpWebResponse)http.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                //Read the response into an xml document 

                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1"));
                body = reader.ReadToEnd();

            }
            catch (Exception ex)
            {

                throw;
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;


            //return await DeserializeObject<T>(responseString);


        }
        public Task<T> LoginATLBank<T>()
        {

            var url = "https://www.atlmoneytransfer.com/api/Token";

            using (var client = new WebClient())
            {

                client.Headers.Add("Cache-Control", "no-cache");

                var payload = new NameValueCollection();

                // Live key
                payload.Add("userName", "sandbox_5e1328afc8e6d5e1328afc8f19z");
                payload.Add("password", "5e1328afc964d5e1328afc96f15e1328afc9795");

                var responseString = "";
                try
                {
                    var response = client.UploadValues(url, "POST", payload);
                    responseString = Encoding.Default.GetString(response);

                }
                catch (Exception ex)
                {

                    //write to  Log file 
                }


                var result = CommonExtension.DeserializeObject<T>(responseString);
                return result;

            }
        }
        public string SerializeObject<T>(T obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            return content;
        }
        public async Task<T> DeserializeObject<T>(string obj)
        {
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj);
            return items;

        }


        #endregion
    }
    public class EmergentApi
    {


        public async Task<T> CreateTransaction<T>(string model)
        {

            //var url = "https://testsrv.interpayafrica.com/v7/CashoutRest.svc/CreateCashoutTrans";
            var url = Common.Common.GetAppSettingValue("EmergentApiCreateTransUrl");

            var http = (HttpWebRequest)WebRequest.Create(url);
            //http.Headers.Add("Authorization", accessTokenVM.TokenType + " " + accessTokenVM.AccessToken);
            http.Headers.Add("Cache-Control", "no-cache");
            http.Method = "POST";

            byte[] utf8bytes = Encoding.UTF8.GetBytes(model.ToString());

            byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);
            http.ContentType = "application/json";

            var content = new StringContent(model.ToString(), Encoding.UTF8, "application/json");



            http.ContentLength = model.Length;
            var sw = new StreamWriter(http.GetRequestStream());
            sw.Write(model);
            sw.Close();
            string body = "";
            try
            {
                var res = (HttpWebResponse)http.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                //Read the response into an xml document 

                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1"));
                body = reader.ReadToEnd();

            }
            catch (Exception ex)
            {

                throw;
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;


            //return await DeserializeObject<T>(responseString);


        }

        public async Task<T> GetTransactionStatus<T>(string model)
        {
            var url = Common.Common.GetAppSettingValue("EmergentApiGetTransactionStatusUrl");

            var http = (HttpWebRequest)WebRequest.Create(url);
            //http.Headers.Add("Authorization", accessTokenVM.TokenType + " " + accessTokenVM.AccessToken);
            http.Headers.Add("Cache-Control", "no-cache");
            http.Method = "POST";

            byte[] utf8bytes = Encoding.UTF8.GetBytes(model.ToString());

            byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);
            http.ContentType = "application/json";

            var content = new StringContent(model.ToString(), Encoding.UTF8, "application/json");



            http.ContentLength = model.Length;
            var sw = new StreamWriter(http.GetRequestStream());
            sw.Write(model);
            sw.Close();
            string body = "";
            try
            {
                var res = (HttpWebResponse)http.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                //Read the response into an xml document 

                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1"));
                body = reader.ReadToEnd();

            }
            catch (Exception ex)
            {

                throw;
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;


            //return await DeserializeObject<T>(responseString);


        }

        public async Task<T> MobileMoneyCustomerCheck<T>(string model)
        {


            //var url = "https://testsrv.interpayafrica.com/v7/Interapi.svc/MMCustomerCheck";

            var url = Common.Common.GetAppSettingValue("EmergentApiMobileMoneyCustomerCheckUrl");

            var http = (HttpWebRequest)WebRequest.Create(url);
            //http.Headers.Add("Authorization", accessTokenVM.TokenType + " " + accessTokenVM.AccessToken);
            http.Headers.Add("Cache-Control", "no-cache");
            http.Method = "POST";

            byte[] utf8bytes = Encoding.UTF8.GetBytes(model.ToString());

            byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);
            http.ContentType = "application/json";

            var content = new StringContent(model.ToString(), Encoding.UTF8, "application/json");



            http.ContentLength = model.Length;
            var sw = new StreamWriter(http.GetRequestStream());
            sw.Write(model);
            sw.Close();
            string body = "";
            try
            {
                var res = (HttpWebResponse)http.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                //Read the response into an xml document 

                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1"));
                body = reader.ReadToEnd();

            }
            catch (Exception ex)
            {

                throw;
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;

        }

        public ServiceResult<bool> IsValidMobileCustomer(EmergentApiMobileMoneyCustomerCheck model)
        {

            var data = MobileMoneyCustomerCheck<EmergentApiMobileMoneyCustomerCheckResponse>(CommonExtension.SerializeObject<EmergentApiMobileMoneyCustomerCheck>(model)).Result;

            var result = new ServiceResult<bool>();
            try
            {

                result.Data = data.valid == "true" ? true : false;
                result.Message = data.status_message;
                result.Status = data.status_code == "1" ? ResultStatus.OK : ResultStatus.Warning;
            }
            catch (Exception)
            {
                result.Status = ResultStatus.Error;
            }

            return result;
        }
    }

    public class TransferZeroApi
    {
        public TransactionError transactionError = null;
        public TransferZeroApi()
        {

        }

        public TransactionResponse ValidateTransaction(TransactionRequest request)
        {

            //GET Transaction Status /v1/transactions?external_id=Transaction:NGN:17523


            Configuration configuration = new Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";
            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");


            TransactionsApi api = new TransactionsApi(configuration);

            TransactionResponse response = new TransactionResponse();
            try
            {
                response = api.CreateAndFundTransaction(request);
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    response = e.ParseObject<TransactionResponse>();
                    // Process validation error
                }

            }
            return response;


        }


        public TransactionResponse CreateTransaction(TransactionRequest request)
        {
            //GET Transaction Status /v1/transactions?external_id=Transaction:NGN:17523
            Configuration configuration = new Configuration();
            configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            configuration.BasePath = "https://api-sandbox.transferzero.com/v1";
            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            //Sender sender = new Sender(
            //         firstName: "Jane",
            //         lastName: "Doe",
            //         phoneCountry: "GB",
            //         phoneNumber: "07440395950",
            //         country: "GB",
            //         city: "Accra",
            //         street: "1 La Rd",
            //         postalCode: "GA100",
            //         addressDescription: "",
            //         birthDate: DateTime.Parse("1974-12-24"),
            //         // you can usually use your company's contact email address here
            //         email: "info@transferzero.com",
            //         //gender: "M",
            //         externalId: Guid.NewGuid().ToString(),
            //         // you'll need to set these fields but usually you can leave them the default
            //         ip: "127.0.0.1",
            //         documents: new List<Document>()
            //);
            //PayoutMethodDetails details = new PayoutMethodDetails(
            //     firstName: "John",
            //     lastName: "Deo",
            //     //mobileProvider: PayoutMethodMobileProviderEnum.Orange,
            //     phoneNumber: "212537718685"
            //     //bankAccount: "123456789",
            //     //bankCode: "030100",
            //     //bankAccountType: PayoutMethodBankAccountTypeEnum._20
            //     //               "reason" => "Remittance payment",
            //     //// Optional; Default value is 'Remittance payment'
            //     //"identity_card_type" => "NI",
            //     //// Optional; Values: "PP": Passport, "NI": National ID
            //     //"identity_card_id" => 'AB12345678'
            //     );
            //details.SenderCountryOfBirth = "GB";
            //details.SenderGender = PayoutMethodGenderEnum.M;
            //details.SenderCityOfBirth = "London";
            //details.SenderIdentityCardId = "AB12345678";
            //details.SenderIdentityCardType = PayoutMethodIdentityCardTypeEnum.NI;
            //details.IdentityCardId = "AB12345678";
            //details.IdentityCardType = PayoutMethodIdentityCardTypeEnum.NI;

            //PayoutMethod payout = new PayoutMethod(
            //  type: "MAD::Cash",
            //  details: details);

            //Recipient recipient = new Recipient(
            //  requestedAmount: 10,
            //  requestedCurrency: "MAD",
            //  payoutMethod: payout);


            //Transaction transaction = new Transaction(
            //  sender: sender,
            //  recipients: new List<Recipient>() { recipient },
            //  inputCurrency: "MAD",
            //  externalId: Guid.NewGuid().ToString());
            //TransactionRequest aa = new TransactionRequest(
            //    transaction: transaction);

            TransactionsApi api = new TransactionsApi(configuration);

            TransactionResponse response = new TransactionResponse();
            TransactionErrorServices _transactionErrorServices = new TransactionErrorServices();
            try
            {
                response = api.CreateAndFundTransaction(request);
                if (transactionError != null)
                {
                    var transactionErrorByTransactionIdAndTransferMethod = _transactionErrorServices.GetTransactionErrorByTransactionIdAndTransferMethod(transactionError.TransactionId,
                                                                                                                                                         transactionError.TransferMethod);
                    if (transactionErrorByTransactionIdAndTransferMethod != null)
                    {
                        transactionError.TransactionErrorStatus = TransactionErrorStatus.ReleasedState;
                        _transactionErrorServices.UpdateTransactionError(transactionErrorByTransactionIdAndTransferMethod);
                    }
                }
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    response = e.ParseObject<TransactionResponse>();

                    Log.Write(request.Transaction.ExternalId + " TransaferZero Error");
                    foreach (var item in response.Object.Errors)
                    {
                        Log.Write(request.Transaction.ExternalId + " TransaferZero Error" + item.Key + "-" + item.Value);
                    }

                    // Process validation error
                    if (transactionError != null)
                    {
                        transactionError.TransactionErrorStatus = TransactionErrorStatus.ErrorState;
                        transactionError.Date = DateTime.Now;
                        _transactionErrorServices.AddTransactionError(transactionError);
                    }
                }

            }
            return response;


        }


        public AccountValidationResponse ValidateAccountNo(AccountValidationRequest accountValidationRequest)
        {
            Configuration configuration = new Configuration();

            //configuration.ApiKey = "IammYSjfAkddiNFPN4gA9Mu+cZYSYBQjJwGkzIcvKulwHOvdv/cS6MKnA19yiHGD8K/ASm0iIuA5zcyn43nSpQ==";
            //configuration.ApiSecret = "5fuBhR64H+hoPk4MS7glNh6zKSvPHUmEhm6dI7uo/ROUAr+H6F36fgtzSpsvGhsI8oJNVwjY4yFRSw2P+D1hZg==";
            //configuration.BasePath = "https://api.transferzero.com/v1";
            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");



            var url = configuration.BasePath;

            //using (var client = new WebClient())
            //{
            //    client.Headers.Add("Authorization-Nonce", Guid.NewGuid().ToString());
            //    client.Headers.Add("Authorization-Key", "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==");

            //    client.Headers.Add("Authorization-Signature", "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==");

            //    var payload = new NameValueCollection();

            //    // test key 
            //    payload.Add("bank_account", "");
            //    payload.Add("bank_code", "");
            //    payload.Add("country", "UG");
            //    payload.Add("currency", "UGX");
            //    payload.Add("phone_number" , "7087661234");
            //    payload.Add("method", "mobile");


            //    var responseString = "";
            //    try
            //    {
            //        var response = client.UploadValues(url+ "/account_validations", "POST", payload);
            //        responseString = Encoding.Default.GetString(response);

            //    }
            //    catch (Exception ex)
            //    {

            //        //write to  Log file 
            //    }

            //}





            AccountValidationApi accountValidationApi = new AccountValidationApi(configuration);
            AccountValidationResponse accountValidationResponse = new AccountValidationResponse();
            try
            {
                accountValidationResponse = accountValidationApi.PostAccountValidations(accountValidationRequest);
                //System.Console.WriteLine(result);
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    accountValidationResponse = e.ParseObject<AccountValidationResponse>();
                    //System.Console.WriteLine(result);
                }
                else
                {
                    //  throw e;
                }

            }

            return accountValidationResponse;
        }


        public TransferZero.Sdk.Model.PayoutMethodDetailsBalance GetBalance()
        {
            return new PayoutMethodDetailsBalance();
        }
        public Transaction CancelTransaction(Guid recipientid)
        {

            Configuration configuration = new Configuration();
            //configuration.ApiKey = "IammYSjfAkddiNFPN4gA9Mu+cZYSYBQjJwGkzIcvKulwHOvdv/cS6MKnA19yiHGD8K/ASm0iIuA5zcyn43nSpQ==";
            //configuration.ApiSecret = "5fuBhR64H+hoPk4MS7glNh6zKSvPHUmEhm6dI7uo/ROUAr+H6F36fgtzSpsvGhsI8oJNVwjY4yFRSw2P+D1hZg==";
            //configuration.BasePath = "https://api.transferzero.com/v1";
            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");

            RecipientsApi api = new RecipientsApi(configuration);

            //Recipient recipient = transaction.Recipients[0];

            try
            {
                api.DeleteRecipient(recipientid);
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // Process validation error
                }
            }

            return new Transaction();
        }

        public Transaction GetTransactionStatus(string externalId)
        {

            Configuration configuration = new Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);

            TransactionListResponse response = api.GetTransactions(externalId: externalId);

            if (response.Object.Count > 0)
            {
                Transaction transaction = response.Object[0];
                return transaction;
            }
            else
            {

                // handle not found scenario
            }

            return new Transaction();
        }


        public void CreateSender()
        {



            Configuration configuration = new Configuration();

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");

            var apiInstance = new SendersApi(configuration);
            Sender sender = new Sender(
             id: Guid.Parse("b6648ba3-1c7b-4f59-8580-684899c84a12"),
            firstName: "Moneyfex",
            lastName: "Moneyfex",

            phoneCountry: "GB",
            phoneNumber: "02081445215",

            country: "GB",
            city: "London",
            street: "North Westgate House Harlow",
            postalCode: "1YS",
            addressDescription: "",

            birthDate: DateTime.Parse("1974-12-24"),

            // you can usually use your company's contact email address here
            email: "support@moneyfex.com",

            // you'll need to set these fields but usually you can leave them the default
            ip: "127.0.0.1",
            documents: new List<Document>());

            SenderRequest senderRequest = new SenderRequest();
            senderRequest.Sender = sender;
            SenderResponse senderResponse = new SenderResponse();
            try
            {
                // Getting a list of possible requested currencies
                senderResponse = apiInstance.PostSenders(senderRequest);
                Debug.WriteLine(senderResponse);
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    senderResponse = e.ParseObject<SenderResponse>();
                    Debug.WriteLine("There was a validation error while processing!");
                    Debug.WriteLine(senderResponse);
                }
            }



        }
        public CurrencyExchangeListResponse GetCurrencyOut(string senderId)
        {

            Configuration configuration = new Configuration();

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");

            var apiInstance = new CurrencyInfoApi(configuration);

            CurrencyExchangeListResponse result = new CurrencyExchangeListResponse();
            try
            {
                // Getting a list of possible requested currencies
                result = apiInstance.InfoCurrenciesIn();
                Debug.WriteLine(result);
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    result = e.ParseObject<CurrencyExchangeListResponse>();
                    Debug.WriteLine("There was a validation error while processing!");
                    Debug.WriteLine(result);
                }
                else
                {
                    Debug.Print("Exception when calling CurrencyInfoApi.InfoCurrencies: " + e.Message);
                }
            }
            return result;
        }


    }

    public class EmergentApiMobileMoneyCustomerCheck
    {
        public EmergentApiMobileMoneyCustomerCheck()
        {

            //this.app_id = "2452014221";
            //this.app_key = "41030994";
            this.app_id = Common.Common.GetAppSettingValue("EmergentApiId");
            this.app_key = Common.Common.GetAppSettingValue("EmergentApikey");
        }
        public string app_id { get; set; }
        public string app_key { get; set; }

        public string mobile { get; set; }


    }

    public class EmergentApiMobileMoneyCustomerCheckResponse
    {

        public string status_code { get; set; }
        public string status_message { get; set; }
        public string firstname { get; set; }
        public string surname { get; set; }
        public string valid { get; set; }

    }
    public class EmergentApiRequestParamModel
    {
        public EmergentApiRequestParamModel()
        {

            //this.app_id = "2452014221";
            //this.app_key = "41030994";
            this.app_id = Common.Common.GetAppSettingValue("EmergentApiId");
            this.app_key = Common.Common.GetAppSettingValue("EmergentApikey");
        }

        public string app_id { get; set; }
        public string app_key { get; set; }
        public string transaction_date { get; set; }
        public string expiry_date { get; set; }
        public string transaction_type { get; set; }
        //Cash_At_Counter (CAC) 
        //Transfer_to_Mobile_Money(MMT)/
        //Transfe_To Bank_Account(BAT)
        public string payment_mode { get; set; }
        public string payee_name { get; set; }

        public string payee_email { get; set; }
        public string trans_currency { get; set; }
        public decimal trans_amount { get; set; }

        public string merch_trans_ref_no { get; set; }
        public string payee_mobile { get; set; }

        public string recipient_mobile { get; set; }
        public string recipient_name { get; set; }
        public string bank_name { get; set; }
        public string bank_branch_sort_code { get; set; }
        public string bank_account_no { get; set; }
        public string bank_account_title { get; set; }


        public string recipient_mobile_operator { get; set; }
        public string recipient_ID_type { get; set; }
        public string recipient_ID_number { get; set; }



        //"app_id": "2452014221",

        //"app_key": "41030994",

        //"transaction_date": "/Date(2020-03-09t11:06:08)/",

        //"expiry_date": "/Date(2020-03-09t11:06:08)/",

        //"transaction_type": "local",

        //"payment_mode": "MMT",

        //"payee_name": "MoneyFex Payee",

        //"payee_email": "email@moneyfex.com",

        //"trans_currency": "GHS",

        //"trans_amount": "1.11",

        //"merch_trans_ref_no": 
        //"310320201008",

        //"payee_mobile": "+233551212121",

        //"recipient_mobile": "+233551212121",

        //"recipient_name": "MoneyFex"



    }

    public class EmergentApiTransactionResponseModel
    {

        public int status_code { get; set; }
        public string status_message { get; set; }
        public string signature { get; set; }
        public string trans_ref_no { get; set; }
    }

    public class EmergentApiGetStatusResponseModel
    {

        public int status_code { get; set; }
        public string status_message { get; set; }
        public string bank_account_no { get; set; }
        public string bank_account_title { get; set; }
        public string payment_date { get; set; }
        public string recipient_address { get; set; }
        public string recipient_id_number { get; set; }
        public string recipient_id_type { get; set; }
        public string recipient_last_name { get; set; }
        public string recipient_mobile { get; set; }
        public string recipient_mobile_operator { get; set; }
        public string recipient_name { get; set; }
        public string trans_ref_no { get; set; }
    }
    public class EmergentApiGetTransactionStatusParamModel
    {
        public EmergentApiGetTransactionStatusParamModel()
        {
            //app_id = "2452014221";
            //app_key = "41030994";
            this.app_id = Common.Common.GetAppSettingValue("EmergentApiId");
            this.app_key = Common.Common.GetAppSettingValue("EmergentApikey");
        }
        public string app_id { get; set; }
        public string app_key { get; set; }
        public string merch_trans_ref_no { get; set; }
    }


    #region Zenith Api 

    public class ZenithApi
    {
        private string baseUrl;

        public ZenithApi()
        {
            this.baseUrl = Common.Common.GetAppSettingValue("ZenithApiUrl").ToString();

        }
        public async Task<T> Post<T>(string requrl, string model)
        {
            var url = baseUrl + requrl;

            var http = (HttpWebRequest)WebRequest.Create(url);

            String username = Common.Common.GetAppSettingValue("ZenithApiUsername");
            String password = Common.Common.GetAppSettingValue("ZenithApiPassword");
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            http.Headers.Add("Authorization", "Basic " + encoded);
            http.Headers.Add("Cache-Control", "no-cache");
            http.Method = "POST";
            http.ServerCertificateValidationCallback +=
         (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.ServerCertificateValidationCallback +=
(sender, cert, chain, sslPolicyErrors) => true;

            byte[] utf8bytes = Encoding.UTF8.GetBytes(model.ToString());

            byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);
            http.ContentType = "application/json";

            var content = new StringContent(model.ToString(), Encoding.UTF8, "application/json");

            http.ContentLength = model.Length;
            var sw = new StreamWriter(http.GetRequestStream());
            sw.Write(model);
            sw.Close();
            string body = "";
            try
            {
                var res = (HttpWebResponse)http.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                //Read the response into an xml document 

                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1"));
                body = reader.ReadToEnd();

            }
            catch (Exception ex)
            {

                throw;
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;


            //return await DeserializeObject<T>(responseString);




        }

        public async Task<T> Get<T>(string requrl)
        {
            var url = baseUrl + requrl;


            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            String username = Common.Common.GetAppSettingValue("ZenithApiUsername");
            String password = Common.Common.GetAppSettingValue("ZenithApiPassword");
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.ContentType = "application/json; charset=iso-8859-1";
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.ServicePoint.Expect100Continue = false;
            request.ServerCertificateValidationCallback +=
(sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.ServerCertificateValidationCallback +=
(sender, cert, chain, sslPolicyErrors) => true;

            string body = "";
            try
            {
                try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream
                        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                        body = reader.ReadToEnd();
                    }
                }
                catch (WebException wex)
                {
                    StreamReader reader = new StreamReader(wex.Response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                    body = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {

            }
            var result = await CommonExtension.DeserializeObject<T>(body);
            return result;



            //return await DeserializeObject<T>(responseString);




        }
        public ZenithTransferAccountBalanceReponse GetAccountBalance()
        {

            var reponse = Get<ZenithTransferAccountBalanceReponse>("/ZTransfer/GetAccountBalance/1111111111");

            return reponse.Result;

        }

        public List<ZenithMobileAndBankNetworkVm> GetMobileNetworks()
        {


            var reponse = Get<ZenithMobileListVm>("ZTransfer/GetMobileNetWorks");
            return reponse.Result.BanksList;

        }

        public ZenithTransferVerifyAccountResponseModel VerifyAccount(ZenithTransferVerifyAccountModel model)
        {
            var response = Post<ZenithTransferVerifyAccountResponseModel>("ZTransfer/VerifyAccount",
                CommonExtension.SerializeObject<ZenithTransferVerifyAccountModel>(model));

            return response.Result;

        }
        public ServiceResult<bool> IsValidAccount(ZenithTransferVerifyAccountModel model)
        {

            var zenithApiResponse = VerifyAccount(model);
            var result = new ServiceResult<bool>();
            switch (zenithApiResponse.ResponseCode)
            {
                case "000":
                    result.Data = true;
                    break;
                case "005":
                    result.Data = false;
                    result.Message = zenithApiResponse.ResponseMessage;
                    break;
                default:
                    break;
            }

            return result;
        }

        public ZenithTransferGetTransactionStatusResponseModel GetTransactionStatus(ZenithTransferTransactionStatusModel model)
        {

            var response = Post<ZenithTransferGetTransactionStatusResponseModel>("ZTransfer/GetTransactionStatus",
                CommonExtension.SerializeObject<ZenithTransferTransactionStatusModel>(model));
            return response.Result;

        }

        public ZenithTransferSendMoneyResponseModel SendMoney(ZenithTransferSendMoneyModel model)
        {

            var response = Post<ZenithTransferSendMoneyResponseModel>("ZTransfer/SendMoney", CommonExtension.SerializeObject<ZenithTransferSendMoneyModel>(model));
            return response.Result;

        }

        public List<ZenithTransferGetTransactionStatusResponseModel> GetTransactionHistory(ZenithTransferTransactionHistoryModel model)
        {

            var response = Post<List<ZenithTransferGetTransactionStatusResponseModel>>("ZTransfer/GetTransactionHistory", CommonExtension.SerializeObject<ZenithTransferTransactionHistoryModel>(model));

            return response.Result;

        }
        public ZenithApiBankDepositResponseModel CreateBankDepositTransaction(BankAccountDeposit bankAccountDeposit)
        {

            var createTrans = SendMoney(new ZenithTransferSendMoneyModel()
            {
                Amount = bankAccountDeposit.ReceivingAmount,
                DestAccount = bankAccountDeposit.ReceiverAccountNo,
                DestBankSortCode = bankAccountDeposit.BankCode,
                DestAccountName = bankAccountDeposit.ReceiverName,
                DestBankName = Common.Common.getBankName(bankAccountDeposit.BankId),
                Reference = "MF-" + bankAccountDeposit.ReceiptNo,
                //Reference = "TT12345678965412365470",
                TransferChannel = "BANK",
                SendingPartyName = Common.Common.GetSenderInfo(bankAccountDeposit.SenderId).FirstName,
                FromAccount = "6010123241"
            });
            var statusResponse = GetTransactionStatus(new ZenithTransferTransactionStatusModel()
            {
                Reference = "MF-" + bankAccountDeposit.ReceiptNo
            });

            var result = new ZenithApiBankDepositResponseModel();
            result.TransRefNo = statusResponse.Transaction.Reference;
            result.Status = BankDepositStatus.Incomplete;
            result.TransactionDate = statusResponse.Transaction.TimeStamp;
            switch (statusResponse.Transaction.StatusCode)
            {
                /// Status
                case "000": /// 000 SUCCESS
                    result.Status = BankDepositStatus.Confirm;
                    break;
                /// 005 ACCOUNT VERIFICATION FAILURE
                case "005":
                    result.Status = BankDepositStatus.Failed;
                    break;
                /// 007 REQUIRED FIELDS VALIDATION ERROR(S)
                case "007":
                    result.Status = BankDepositStatus.Failed;
                    break;
                /// 009 GIP TRANSFER FAILED
                case "009":
                    result.Status = BankDepositStatus.Failed;
                    break;
                /// 006 INVALID TRANSFER CHANNEL
                case "006":
                    result.Status = BankDepositStatus.Failed;
                    break;
                /// 017 TRANSACTION(S) NOT FOUND
                case "017":
                    result.Status = BankDepositStatus.Failed;
                    break;
                /// 010 ACCOUNT DAILY LIMIT REACHED
                case "010":
                    result.Status = BankDepositStatus.Failed;
                    break;
                /// 011 ACCOUNT TRANS LIMIT REACHED
                case "011":
                    result.Status = BankDepositStatus.Failed;
                    break;
                /// 012 ACCOUNT DAILY LIMIT REACHED
                case "012":
                    result.Status = BankDepositStatus.Failed;
                    break;
                /// 013 ACCOUNT TRANSACTION LIMIT REACHED
                case "013":
                    result.Status = BankDepositStatus.Failed;
                    break;
                /// 018 GLOBAL DAILY LIMIT REACHED
                case "018":
                    result.Status = BankDepositStatus.Failed;
                    break;
                /// 019 INSUFFICIENT FUNDS
                case "019":
                    result.Status = BankDepositStatus.Failed;
                    break;
                default:
                    break;
            }
            return result;
        }
        public ZenithApiMobileMoneyTransferResponseModel CreateMobileMoneyTransaction(MobileMoneyTransfer mobileMoneyTransfer)
        {

            var createTrans = SendMoney(new ZenithTransferSendMoneyModel()
            {
                Amount = mobileMoneyTransfer.ReceivingAmount,
                Reference = "MF" + mobileMoneyTransfer.ReceiptNo,
                TransferChannel = "MOBILEMONEY",
                DestAccount = mobileMoneyTransfer.PaidToMobileNo,
                DestAccountName = mobileMoneyTransfer.ReceiverName,
                DestBankName = Common.Common.GetMobileWalletInfo(mobileMoneyTransfer.WalletOperatorId).Code,
                DestBankSortCode = Common.Common.GetMobileWalletInfo(mobileMoneyTransfer.WalletOperatorId).MobileNetworkCode,
                SendingPartyName = Common.Common.GetSenderInfo(mobileMoneyTransfer.SenderId).FirstName
            });
            var statusResponse = GetTransactionStatus(new ZenithTransferTransactionStatusModel()
            {
                Reference = "MF" + mobileMoneyTransfer.ReceiptNo
            });

            var result = new ZenithApiMobileMoneyTransferResponseModel();
            result.TransRefNo = statusResponse.Transaction.Reference;
            result.Status = MobileMoneyTransferStatus.InProgress;
            result.TransactionDate = statusResponse.Transaction.TimeStamp;
            switch (statusResponse.Transaction.StatusCode)
            {
                /// Status
                case "000": /// 000 SUCCESS
                    result.Status = MobileMoneyTransferStatus.Paid;
                    break;
                /// 005 ACCOUNT VERIFICATION FAILURE
                case "005":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                /// 007 REQUIRED FIELDS VALIDATION ERROR(S)
                case "007":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                /// 009 GIP TRANSFER FAILED
                case "009":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                /// 006 INVALID TRANSFER CHANNEL
                case "006":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                /// 017 TRANSACTION(S) NOT FOUND
                case "017":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                /// 010 ACCOUNT DAILY LIMIT REACHED
                case "010":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                /// 011 ACCOUNT TRANS LIMIT REACHED
                case "011":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                /// 012 ACCOUNT DAILY LIMIT REACHED
                case "012":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                /// 013 ACCOUNT TRANSACTION LIMIT REACHED
                case "013":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                /// 018 GLOBAL DAILY LIMIT REACHED
                case "018":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                /// 019 INSUFFICIENT FUNDS
                case "019":
                    result.Status = MobileMoneyTransferStatus.Failed;
                    break;
                default:
                    break;
            }
            return result;
        }



    }

    public class ZenithApiBankDepositResponseModel
    {

        public string TransRefNo { get; set; }
        public BankDepositStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }

    }
    public class ZenithApiMobileMoneyTransferResponseModel
    {


        public string TransRefNo { get; set; }
        public MobileMoneyTransferStatus Status { get; set; }
        public DateTime TransactionDate { get; internal set; }
    }
    public class ZenithTransferVerifyAccountModel
    {

        public string TargetAccountNo { get; set; }
        public string DestSortCode { get; set; }

    }
    public class ZenithTransferReponse
    {


        public string ResponseMessage { get; set; }
        public string ResponseCode { get; set; }
    }

    public class ZenithMobileListVm : ZenithTransferReponse
    {
        public List<ZenithMobileAndBankNetworkVm> BanksList { get; set; }

    }
    public class ZenithMobileAndBankNetworkVm
    {

        public string BankName { get; set; }
        public string BankSortCode { get; set; }

    }

    public class ZenithTransferAccountBalanceReponse : ZenithTransferReponse
    {
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }


    }

    public class ZenithTransferVerifyAccountResponseModel : ZenithTransferReponse
    {

        public string AccountName { get; set; }
        public string AccountNo { get; set; }
    }

    public class ZenithTransferTransactionStatusModel
    {
        public string Reference { get; set; }
        public string GIPSessionId { get; set; }


    }

    public class ZenithTransferTransactionResponseModel
    {
        /// <summary>
        /// BANK/MOBILEMONEY
        /// </summary>
        public string TransferChannel { get; set; }

        public string Reference { get; set; }
        public string FromAccount { get; set; }
        public DateTime TimeStamp { get; set; }
        public string DestAccount { get; set; }
        public string Amount { get; set; }
        public string DestBankSortCode { get; set; }

        public string GIPResponseCode { get; set; }
        public string GIPSessionId { get; set; }
        public string StatusCode { get; set; }

    }
    /// <summary>
    /// Status
    /// 000 SUCCESS
    /// 005 ACCOUNT VERIFICATION FAILURE
    /// 007 REQUIRED FIELDS VALIDATION ERROR(S)
    /// 009 GIP TRANSFER FAILED
    /// 006 INVALID TRANSFER CHANNEL
    /// 017 TRANSACTION(S) NOT FOUND
    /// 010 ACCOUNT DAILY LIMIT REACHED
    /// 011 ACCOUNT TRANS LIMIT REACHED
    /// 012 ACCOUNT DAILY LIMIT REACHED
    /// 013 ACCOUNT TRANSACTION LIMIT REACHED
    /// 018 GLOBAL DAILY LIMIT REACHED
    /// 019 INSUFFICIENT FUNDS
    /// </summary>
    public class ZenithTransferGetTransactionStatusResponseModel : ZenithTransferReponse
    {

        public ZenithTransferTransactionResponseModel Transaction { get; set; }


    }

    public class ZenithTransferSendMoneyModel
    {
        public ZenithTransferSendMoneyModel()
        {
            this.FromAccount = Common.Common.GetAppSettingValue("ZenithApiFromAccount");
        }

        public string Reference { get; set; }
        public decimal Amount { get; set; }

        /// <summary>
        /// BANK/MOBILEMONEY
        /// </summary>
        public string TransferChannel { get; set; }
        public string DestAccount { get; set; }
        public string DestBankName { get; set; }
        public string DestBankSortCode { get; set; }
        public string PurposeOfTransfer { get; set; }
        public string FromAccount { get; set; }
        public string DestAccountName { get; set; }

        public string SendingPartyName { get; set; }
        public string FromAccountName { get; set; }
    }

    public class ZenithTransferSendMoneyResponseModel : ZenithTransferReponse
    {
        public string Reference { get; set; }
        public string TimeStamp { get; set; }
        public string DestAccount { get; set; }
        public string DestBankSortCode { get; set; }
        public string GIPResponseCode { get; set; }
        public string GIPSessionId { get; set; }

    }

    public class ZenithTransferTransactionHistoryModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string FromAcccount { get; set; }
        public string TransferChannel { get; set; }
        public string DestAccount { get; set; }

    }


    #endregion


    
    public static class CommonExtension
    {

        public static string SerializeObject<T>(T obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            return content;
        }
        public static async Task<T> DeserializeObject<T>(string obj)
        {
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj);
            return items;

        }
    }
}