using FAXER.PORTAL.Common;
using FAXER.PORTAL.MoblieTransferApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace FAXER.PORTAL.MoblieTransferApi
{
    public class MobileTransferApi
    {



        public void CreateApiUser(string model, string refId)
        {



            //var url = "https://api.jnfx-emarkets.com/api/Account";

            //var url = "https://sandbox.momodeveloper.mtn.com/v1_0/apiuser";
            var url = "https://ericssonbasicapi1.azure-api.net/v1_0/apiuser";



            var http = (HttpWebRequest)WebRequest.Create(url);



            http.Headers.Add("X-Reference-Id", refId);
            http.Headers.Add("Ocp-Apim-Subscription-Key", "9277bd87e874418e928cfdba3032b423");
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

                //throw;
            }

        }


        public Task<T> CreateApiKey<T>(string refId)
        {

            //var url = "https://api.jnfx-emarkets.com/api/Account";

            //var url = "https://sandbox.momodeveloper.mtn.com/v1_0/apiuser/" + refId + "/apikey";
            var url = "https://ericssonbasicapi1.azure-api.net/v1_0/apiuser/" + refId + "/apikey";
            using (var client = new WebClient())
            {

                //client.Headers.Add("Ocp-Apim-Subscription-Key", "9277bd87e874418e928cfdba3032b423");
                client.Headers.Add("Ocp-Apim-Subscription-Key", Common.Common.GetAppSettingValue("MTNApiSubscriptionKey"));

                var payload = new NameValueCollection();

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


                var result = DeserializeObject<T>(responseString);
                return result;

            }
        }

        public Task<T> Login<T>(MobileTransferApiConfigurationVm configuration)
        {

            //var url = "https://ericssonbasicapi1.azure-api.net/disbursement/token/";
            var url = "https://proxy.momoapi.mtn.com/disbursement/token/";
            using (var client = new WebClient())
            {

                //String username = Common.Common.GetAppSettingValue("MTNApiUserId");
                //String password = Common.Common.GetAppSettingValue("MTNApiUserPassword");
                string username = "88577f9b-e19c-495e-bee1-da9997fa665d";
                string password = "46e6f00b1d2b47dca49630fa78207351";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                client.Headers.Add("Authorization", "Basic " + encoded);

                //client.Headers.Add("Ocp-Apim-Subscription-Key", Common.Common.GetAppSettingValue("MTNApiSubscriptionKey"));
                client.Headers.Add("Ocp-Apim-Subscription-Key", "ce6a18001f164a33825f3100491b749b");

                var payload = new NameValueCollection();

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

                var result = DeserializeObject<T>(responseString);
                return result;

            }
        }

        public async Task<T> Post<T>(string model, MobileTransferAccessTokeneResponse accessToken)
        {

            //var client = new HttpClient();
            //var queryString = HttpUtility.ParseQueryString(string.Empty);

            //// Request headers
            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken.access_token + "");
            //client.DefaultRequestHeaders.Add("X-Callback-Url", "https://www.moneyfex.com");
            //client.DefaultRequestHeaders.Add("X-Reference-Id", "f4396681-1e60-4222-8a5e-895f54918ecf");
            //client.DefaultRequestHeaders.Add("X-Target-Environment", "sandbox");
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "daee07935c5d4750a2bae57e2e2550de");

            //var uri = "https://proxy.momoapi.mtn.com/disbursement/v1_0/transfer?" + queryString;

            //HttpResponseMessage response;

            //// Request body
            //byte[] byteData = Encoding.UTF8.GetBytes(model.ToString());

            //using (var content = new ByteArrayContent(byteData))
            //{
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //    response = await client.PostAsync(uri, content);
            //}


            var url = "https://proxy.momoapi.mtn.com/disbursement/v1_0/transfer";
            var http = (HttpWebRequest)WebRequest.Create(url);
            Log.Write("MTN REFERENCE ID : " + accessToken.apirefId);
            http.Headers.Add("Authorization", "Bearer " + accessToken.access_token);
            http.Headers.Add("X-Reference-Id", accessToken.apirefId);
            http.Headers.Add("X-Callback-Url", "https://www.moneyfex.com/");
            http.Headers.Add("X-Target-Environment", "mtncameroon");
            http.Headers.Add("Ocp-Apim-Subscription-Key", "ce6a18001f164a33825f3100491b749b");
            http.Method = "POST";
            byte[] utf8bytes = Encoding.UTF8.GetBytes(model.ToString());
            http.ContentType = "application/json";
           // var content = new StringContent(model.ToString(), Encoding.UTF8, "application/json");
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
                Log.Write(body + " MTN Cameroon Error");

            }
            catch (Exception ex)
            {
                Log.Write("MTN Cameroon Error" + ex.Message);

            }
            var result = await DeserializeObject<T>(body);

            return result;

        }

        public async Task<T> GetTransactionStatus<T>(string refId, string accessToken)
        {

            var url = "https://ericssonbasicapi1.azure-api.net/disbursement/v1_0/transfer/" + refId;


            string body = "";
            // Create the web request

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Headers.Add("Authorization", "Bearer " + accessToken);
            //request.Headers.Add("X-Target-Environment", "sandbox");
            request.Headers.Add("X-Target-Environment", "mtncameroon");
            // live key 
          //  request.Headers.Add("Ocp-Apim-Subscription-Key", "ce6a18001f164a33825f3100491b749b");
            // Test Key
            request.Headers.Add("Ocp-Apim-Subscription-Key", Common.Common.GetAppSettingValue("MTNApiSubscriptionKey"));

            request.Method = "GET";
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
            var result = await DeserializeObject<T>(body);
            return result;


        }

        public async Task<T> ValidateMobileNo<T>(string accountHolderIdType, string MobileNo, string accessToken)
        {

            var url = "https://ericssonbasicapi1.azure-api.net/disbursement/v1_0/accountholder/";
            string body = "";
            // Create the web request

            HttpWebRequest request = WebRequest.Create(url + accountHolderIdType + "/" + MobileNo + "/active") as HttpWebRequest;

            request.Headers.Add("Authorization", "Bearer " + accessToken);
            //request.Headers.Add("X-Target-Environment", "sandbox");
            request.Headers.Add("X-Target-Environment", "mtncameroon");
            // live key 
            //client.Headers.Add("Ocp-Apim-Subscription-Key", "ce6a18001f164a33825f3100491b749b");
            // Test Key
            request.Headers.Add("Ocp-Apim-Subscription-Key", Common.Common.GetAppSettingValue("MTNApiSubscriptionKey"));

            request.Method = "GET";



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
            var result = await DeserializeObject<T>(body);
            return result;

        }


        public async Task<T> PostTest<T>()
        {


            try
            {

                var handler = new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    MaxAutomaticRedirections = 100,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                var client = new HttpClient(handler);

                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri("http://202.51.74.164:8085/");

                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                USerInfoVM vm = new USerInfoVM()
                {
                    Id = 0,
                    UserPin = "11",
                    Name = "",
                    Password = "",
                    Card = "36956",
                    BranchCode = "RS",
                    Privilege = 0,
                    Category = 0,
                    Group = 0,
                    DeviceSn ="",
                    PhotoURL ="",
                    TimeZone = ""
                };

                var value = SerializeObject<USerInfoVM>(vm);
                var content = new StringContent(value.ToString(), Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = client.PostAsync("api/ADMSex/updateuser", content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var str = await response.Content.ReadAsStringAsync();

                        T data = await DeserializeObject<T>(str);
                        return data;
                    }
                    else
                    {
                        return await DeserializeObject<T>("");
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }
            catch (Exception ex)
            {

                throw;
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

        public async Task<T> AdLogin<T>(string url, string Content)
        {

            try
            {
                var client = new HttpClient();

                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri("http://192.168.1.154:10452");

                client.DefaultRequestHeaders.Accept.Clear();

                var content = new StringContent(Content.ToString(), Encoding.UTF8, "application/json");
                //var response = client.GetAsync(url).Result;
                var response = client.PostAsync(url , content).Result;
                if (response.IsSuccessStatusCode) 
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    return await DeserializeObject<T>(str);

                }
                else
                {
                    return await DeserializeObject<T>("");
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
    public class USerInfoVM
    {
        public int Id { get; set; }
        public string UserPin { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Card { get; set; }
        public int Group { get; set; }
        public string TimeZone { get; set; }
        public string DeviceSn { get; set; }
        public string BranchCode { get; set; }
        public int Privilege { get; set; }
        public int Category { get; set; }
        public string PhotoURL { get; set; }
    }
}