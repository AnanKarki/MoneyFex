using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static FAXER.PORTAL.BankApi.Models.FlutterWaveViewModel;

namespace FAXER.PORTAL.BankApi
{
    public class FlutterWaveApi : IFlutterWave
    {

        #region Rave Encryption
        public string GetEncryptionKey(string secretKey)
        {

            //MD5 is the hash algorithm expected by rave to generate encryption key
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //MD5CryptoServiceProvider works with bytes so a conversion of plain secretKey to it bytes equivalent is required.
            //UTF8Encoding.UTF8.GetBytes(secretKey) can also be used.
            byte[] secretKeyBytes = ASCIIEncoding.UTF8.GetBytes(secretKey);


            byte[] hashedSecret = md5.ComputeHash(secretKeyBytes, 0, secretKeyBytes.Length);
            byte[] hashedSecretLast12Bytes = new byte[12];
            Array.Copy(hashedSecret, hashedSecret.Length - 12, hashedSecretLast12Bytes, 0, 12);
            String hashedSecretLast12HexString = BitConverter.ToString(hashedSecretLast12Bytes);
            hashedSecretLast12HexString = hashedSecretLast12HexString.ToLower().Replace("-", "");
            String secretKeyFirst12 = secretKey.Replace("FLWSECK-", "").Substring(0, 12);
            byte[] hashedSecretLast12HexBytes = ASCIIEncoding.UTF8.GetBytes(hashedSecretLast12HexString);
            byte[] secretFirst12Bytes = ASCIIEncoding.UTF8.GetBytes(secretKeyFirst12);
            byte[] combineKey = new byte[24];
            Array.Copy(secretFirst12Bytes, 0, combineKey, 0, secretFirst12Bytes.Length);
            Array.Copy(hashedSecretLast12HexBytes, hashedSecretLast12HexBytes.Length - 12, combineKey, 12, 12);
            return ASCIIEncoding.UTF8.GetString(combineKey);
        }

        public string EncryptData(string encryptionKey, string data)
        {
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            des.Key = ASCIIEncoding.UTF8.GetBytes(encryptionKey);
            ICryptoTransform cryptoTransform = des.CreateEncryptor();
            byte[] dataBytes = ASCIIEncoding.UTF8.GetBytes(data);
            byte[] encryptedDataBytes = cryptoTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            des.Clear();
            return Convert.ToBase64String(encryptedDataBytes);
        }

        public string DecryptData(string encryptedData, string encryptionKey)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = ASCIIEncoding.UTF8.GetBytes(encryptionKey);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cryptoTransform = des.CreateDecryptor();
            byte[] EncryptDataBytes = Convert.FromBase64String(encryptedData);
            byte[] plainDataBytes = cryptoTransform.TransformFinalBlock(EncryptDataBytes, 0, EncryptDataBytes.Length);
            des.Clear();
            return ASCIIEncoding.UTF8.GetString(plainDataBytes);

        }
        public string GetEncryptedData(string model)
        {

            // var secertKey = "FLWSECK-e6db11d1f8a6208de8cb2f94e293450e-X";
            var secertKey = "FLWSECK_TEST-fbb46390c0a6b427e685bb5c16803cbd-X";
            // var secertKey = Common.Common.GetAppSettingValue("FlutterBankAccounTestSecrectKey").ToString();
            var encrytionKey = "FLWSECK_TESTe8ffb1c31994";
            //var encrytionKey = Common.Common.GetAppSettingValue("FlutterEncryptionKey").ToString();

            #region Rave Encryption
            string encrytionCombineKey = GetEncryptionKey(secertKey);

            string encrytedData = EncryptData(encrytionKey, model);

            return encrytedData;

            #endregion
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"> send from create transaction data  object</param>
        /// <returns></returns>


        public async Task<T> ChargeOrInitial<T>(object data)
        {

            //// Test Url 
            var url = "https://ravesandboxapi.flutterwave.com/flwv3-pug/getpaidx/api/charge";

            ////live url 

            //// var url = "https://api.ravepay.co/flwv3-pug/getpaidx/api/charge";

            //// var url = Common.Common.GetAppSettingValue("FlutterBankChargeTransactionTestURl").ToString();

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseMessage = client.PostAsJsonAsync(url, data).Result;
                var responseStr = responseMessage.Content.ReadAsStringAsync().Result;
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseStr);
                return response;

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<T> Validation<T>(string model)
        {


            //var url = Common.Common.GetAppSettingValue("FlutterBankValdidationTransactionTestURl").ToString();
            var url = "https://ravesandboxapi.flutterwave.com/flwv3-pug/getpaidx/api/validate";
            var http = (HttpWebRequest)WebRequest.Create(url);
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

        public async Task<T> Verify<T>(string model)
        {
            // var url = Common.Common.GetAppSettingValue("FlutterBankVerifyTransactionTestURl").ToString();

            var url = "https://ravesandboxapi.flutterwave.com/flwv3-pug/getpaidx/api/v2/verify";
            var http = (HttpWebRequest)WebRequest.Create(url);
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

        #region For Bank Transfer and mobile money
        /// <summary>
        /// For initial transaction
        /// </summary>
        /// <param name="model"> customer details view model. it is bind with FlutterCommonCustomerDetailsVm</param>
        /// <returns>FWReponse </returns>
        public FWReponse<FlutterCommonResponseDataVm> CreateTransaction(FlutterCommonCustomerDetailsVm model)
        {
            var customerdetails = CommonExtension.SerializeObject<FlutterCommonCustomerDetailsVm>(model);

            var client = GetEncryptedData(customerdetails);
            var publicKey = "FLWPUBK_TEST-4eee78c097f4c67361fc338c9a678fa3-X";
            //var publicKey = "FLWPUBK-7adb6177bd71dd43c2efa3f1229e3b7f-X";
            // var publicKey = Common.Common.GetAppSettingValue("FlutterBankAccounTestPublicKey").ToString();

            //FlutterRequestDataVm requestParam = new FlutterRequestDataVm()
            //{
            //    PBFPubKey = publicKey,
            //    client = client,
            //    alg = "3DES-24"
            //};

            var data = new { PBFPubKey = publicKey, client = client, alg = "3DES-24" };
            var chargeData = ChargeOrInitial<FWReponse<FlutterCommonResponseDataVm>>(data);

            return chargeData.Result;
        }
        /// <summary>
        /// For valdation transaction
        /// </summary>
        /// <param name="transactionreference"> This is the flwRef returned in the Initiate payment response for this transaction </param>
        /// <param name="otp">This is the one time pin sent to the customer from the bank.</param>
        /// <returns></returns>
        public FWReponse<FlutterCommonResponseDataVm> ValidatedTransaction(string transactionreference, string otp)
        {
            //var publicKey = Common.Common.GetAppSettingValue("FlutterBankAccounTestPublicKey").ToString();
            var publicKey = "FLWPUBK_TEST-4eee78c097f4c67361fc338c9a678fa3-X";
            FlutterRequestDataVm requestParam = new FlutterRequestDataVm()
            {
                PBFPubKey = publicKey,
                transactionreference = transactionreference,
                otp = otp,
                use_access = true

            };
            var validationData = Validation<FWReponse<FlutterCommonResponseDataVm>>(CommonExtension.SerializeObject(requestParam));

            return validationData.Result;
        }

        /// <summary>
        /// 
        /// for status
        ///   if (response.data.status == "successful" && response.data.amount == amount && response.data.chargecode == "00")
        ///    redirect to successful payement page for customer
        ///    amount reference sending amount by user
        ///    
        /// </summary>
        /// <param name="txref">merchant unique reference (it can be get in validation response data.txref</param>
        /// <returns>
        ///  
        /// </returns>
        public FWReponse<FlutterCommonVerifyResponseDataVm> VerifyTransation(string txref)
        {
            //var secretKey = Common.Common.GetAppSettingValue("FlutterBankAccounTestSecrectKey").ToString();
            var secretKey = "FLWSECK_TEST-fbb46390c0a6b427e685bb5c16803cbd-X";
            FlutterRequestDataVm requestParam = new FlutterRequestDataVm()
            {
                txref = txref,
                SECKEY = secretKey
            };
            var verifyData = Verify<FWReponse<FlutterCommonVerifyResponseDataVm>>(CommonExtension.SerializeObject(requestParam));

            return verifyData.Result;
        }
        #endregion


        public FWReponse<FlutterWaveResonse> CreateTransaction(string model)
        {
            string url = "https://api.flutterwave.com/v3/transfers";
            var result = Post<FWReponse<FlutterWaveResonse>>(model, url).Result;
            return result;
        }
        public FWReponse<FlutterWaveResonse> GetTransactionById(int id)
        {
            string url = "https://api.flutterwave.com/v3/transfers/" + id;
            var result = Get<FWReponse<FlutterWaveResonse>>(url).Result;
            return result;
        }
        public FWAllTransactionReponse<FlutterWaveAllTransactionResonse> GetAllTransactions()
        {
            string url = "https://api.flutterwave.com/v3/transfers";
            var result = Get<FWAllTransactionReponse<FlutterWaveAllTransactionResonse>>(url).Result;
            return result;
        }

        public FWReponse<FlutterWaveResonse> CreateBulkTransfer(string model)
        {
            string url = "https://api.flutterwave.com/v3/bulk-transfers";
            var result = Post<FWReponse<FlutterWaveResonse>>(model, url).Result;
            return result;
        }

        public FWReponse<FlutterWaveRateResponseVm> GetRate(decimal amount, string sendingCurrency, string receivingCurrency)
        {
            string url = "https://api.flutterwave.com/v3/transfers/rates?amount=" + amount +
                         "&destination_currency=" + receivingCurrency + "&source_currency=" + sendingCurrency;
            var result = Get<FWReponse<FlutterWaveRateResponseVm>>(url).Result;
            return result;
        }


        public async Task<t> Post<t>(string model, string url)
        {
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Headers.Add("Cache-Control", "no-cache");
            string token = "FLWSECK_TEST-SANDBOXDEMOKEY-X";
            http.Headers.Add("Authorization", "Bearer" + " " + token);
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
                Log.Write(ex.Message, DB.ErrorType.FlutterWave, "FlutterWaveApi");
            }
            var result = await CommonExtension.DeserializeObject<t>(body);
            return result;
        }
        public async Task<t> Get<t>(string url)
        {
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Headers.Add("Cache-Control", "no-cache");
            string token = "FLWSECK_TEST-SANDBOXDEMOKEY-X";
            http.Headers.Add("Authorization", "Bearer" + " " + token);
            http.Method = "GET";
            http.ContentType = "application/json";
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
                Log.Write(ex.Message, DB.ErrorType.FlutterWave, "FlutterWaveApi");
            }

            var result = await CommonExtension.DeserializeObject<t>(body);
            return result;
        }

    }
}