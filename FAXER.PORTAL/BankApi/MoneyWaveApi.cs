using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static FAXER.PORTAL.BankApi.Models.MoneyWaveViewModel;

namespace FAXER.PORTAL.BankApi
{
    public class MoneyWaveApi : IMoneyWave
    {
        private string Token = "";
        public AuthResponseViewModel GetAuthorizationToken()
        {
            var url = "https://staging.moneywaveapp.com/v1/merchant/verify";
            var apiKey = Common.Common.GetAppSettingValue("MoneyWaveApiKey"); 
            var secretkey = Common.Common.GetAppSettingValue("MoneyWaveSecretKey"); 
            //AuthRequestViewModel vm = new AuthRequestViewModel()
            //{
            //    apiKey = apiKey,
            //    secret = secretkey,
            //};
            AuthRequestViewModel vm = new AuthRequestViewModel()
            {
                apiKey = "GB4P2u9AgcIhINYiSkfY8TMMlogLA0ltKKpdG4ky6NxYuntfIu",
                secret = "gnjr2eIe0FhqNO7oIyaSTKMqgsd8qlNNcbh9uFgpcCCbSG3CAQ",
            };

            var request = CommonExtension.SerializeObject(vm);
            var response = Post<AuthResponseViewModel>(url, request).Result;
            if (response != null)
            {
                Token = response.token;
            }
            return response;
        }
        public ResponseVm<AccountOwner> AccountVerification(ResolveAccountRequestVM vm)
        {
            var url = "https://staging.moneywaveapp.com/v1/resolve/account";
            var request = CommonExtension.SerializeObject(vm);
            var response = Post<ResponseVm<AccountOwner>>(url, request).Result;
            return response;
        }

        public ResponseVm<TransactionResponseVm> CreateTransaction(TransactionRequestViewModel vm)
        {
            var url = "https://staging.moneywaveapp.com/v1/disburse";
            var request = CommonExtension.SerializeObject(vm);
            var response = Post<ResponseVm<TransactionResponseVm>>(url, request).Result;
            return response;
        }
        public ResponseVm<TransactionStatusResponseVm> GetTransactionStatus(TransactionStatusResquestVm vm)
        {
            var url = "https://staging.moneywaveapp.com/v1/disburse/status";
            var request = CommonExtension.SerializeObject(vm);
            var response = Post<ResponseVm<TransactionStatusResponseVm>>(url, request).Result;
            var transaction_Result = response.data;
            var meta_Result = CommonExtension.DeserializeObject<MetaDetails>(response.data.meta);
            response.data.meta_Result = meta_Result.Result;
            return response;
        }

        public async Task<t> Post<t>(string url, string model)
        {
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Headers.Add("Cache-Control", "no-cache");

            if (!string.IsNullOrEmpty(Token))
            {
                http.Headers.Add("Authorization", Token);
            }
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
                var message = ex.Message;
                Log.Write(ex.Message, DB.ErrorType.MoneyWave, "MoneyWaveApi");
            }
            var result = await CommonExtension.DeserializeObject<t>(body);
            return result;
        }


    }
}