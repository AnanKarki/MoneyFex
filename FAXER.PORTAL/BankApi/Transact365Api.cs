using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static FAXER.PORTAL.BankApi.Models.Transact365ViewModel;

namespace FAXER.PORTAL.BankApi
{
    public class Transact365Api : ITransact365
    {

        String username = "";
        String password = "";

        public Transact365Api()
        {
            username = Common.Common.GetAppSettingValue("Transact365ShopId").ToString();
            password = Common.Common.GetAppSettingValue("Transact365Password").ToString();
        }

    
  



        #region Post Api and Get Api
        /// <summary>
        /// used by all for request and response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<T> CommonPost<T>(string model, string url)
        {
            var http = (HttpWebRequest)WebRequest.Create(url);

            String username = "14812";
            String password = "4a24170b6a3bd49114ab07a19e8124d361db7af2dc42c5b1344406a4f34fdf14";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));

            http.Headers.Add("Authorization", "Basic " + encoded);
            http.ContentType = "application/json";
            http.Method = "POST";
            byte[] utf8bytes = Encoding.UTF8.GetBytes(model.ToString());

            //var content = new StringContent(model.ToString(), Encoding.UTF8, "application/json");

            Log.Write(model);
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
                Log.Write(body);

            }
            catch (Exception ex)
            {


            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;



        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"> web url </param>
        /// <returns></returns>
        public async Task<T> CommonGet<T>(string url)
        {
            // Create the web request
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            String username = "14812";
            String password = "4a24170b6a3bd49114ab07a19e8124d361db7af2dc42c5b1344406a4f34fdf14";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Method = "GET";
            //request.ContentType = "application/json";
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:24.0) Gecko/20100101 Firefox/24";

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => { return true; };
            request.ServicePoint.Expect100Continue = false;
            request.KeepAlive = false;
            //request.ProtocolVersion = HttpVersion.Version10;
            //request.ServicePoint.ConnectionLimit = 1;
            ////request.ServerCertificateValidationCallback = SecurityProtocolType.Tls12;
            //System.Net.ServicePointManager.SecurityProtocol =
            //                                System.Net.SecurityProtocolType.Ssl3
            //                                | System.Net.SecurityProtocolType.Tls12
            //                                | SecurityProtocolType.Tls11
            //                                | SecurityProtocolType.Tls;

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
                    Log.Write(body);

                }
            }
            catch (Exception ex)
            {

            }
            var result = await CommonExtension.DeserializeObject<T>(body);
            return result;



         

        }

        #endregion

        #region For Authorization

        /// <summary>
        /// first authorizationtransaction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public T365ResponseVm<T365ResponseDataVm> AuthorizationPayment(T365RequestVm<T365RequestDataVm> model)
        {

            var param = CommonExtension.SerializeObject(model);

            //// Test Url 
            var url = "https://gateway.transact365.com/transactions/authorizations";

            //// var url = Common.Common.GetAppSettingValue("FlutterBankChargeTransactionTestURl").ToString();
            ///

            var authorizationData = CommonPost<T365ResponseVm<T365ResponseDataVm>>(param, url);

            return authorizationData.Result;
        }

        #endregion


        #region For Payment
        /// <summary>
        /// after authorization,do payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public T365ResponseVm<T365ResponseDataVm> CreatePayment(T365RequestVm<T365RequestDataVm> model)
        {

            var paramData = CommonExtension.SerializeObject(model);

            //// Test Url 
            //var url = "https://gateway.transact365.com/transactions/payments";

            var url = Common.Common.GetAppSettingValue("Transact365URL").ToString() + "payments";
            ///

            var Data = CommonPost<T365ResponseVm<T365ResponseDataVm>>(paramData, url);

            return Data.Result;
        }

        #endregion


        #region For Void or cancellation
        /// <summary>
        /// used for cancel
        /// </summary>
        /// <param name="model">T365CapturesAndRefundsRequestVm used</param>
        /// <returns></returns>
        public T365ResponseVm<T365ReponseTransationVm> CancelTransaction(T365RequestVm<T365CapturesAndRefundsRequestVm> model)
        {

            var param = CommonExtension.SerializeObject(model);

            //// Test Url 
            //var url = "https://api.transact365.com/beyag/transactions/void";

             var url = Common.Common.GetAppSettingValue("Transact365URL").ToString() + "void";
            
            var Data = CommonPost<T365ResponseVm<T365ReponseTransationVm>>(param, url);

            return Data.Result;
        }

        #endregion


        #region For refund

        /// <summary>
        /// used for cancel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public T365ResponseVm<T365ReponseTransationVm> RefundTransaction(T365RequestVm<T365CapturesAndRefundsRequestVm> model)
        {

            var param = CommonExtension.SerializeObject(model);

            //// Test Url 
            //var url = "https://gateway.transact365.com/transactions/refunds";
            var url = Common.Common.GetAppSettingValue("Transact365URL").ToString() + "refunds";

            //// var url = Common.Common.GetAppSettingValue("FlutterBankChargeTransactionTestURl").ToString();
            ///

            var Data = CommonPost<T365ResponseVm<T365ReponseTransationVm>>(param, url);

            return Data.Result;
        }

        #endregion


        #region For GetTransaciton
        /// <summary>
        /// used for getting all transation with payment status
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public T365ResponseVm<T365ResponseDataVm> GetTransaction(string uid = "", string trackingId = "")
        {
            //// Test Url
            var url =  "https://gateway.transact365.com/transactions/" + uid;
            //var url = Common.Common.GetAppSettingValue("Transact365URL").ToString() + uid;


            //Log.Write(url);
            if (!string.IsNullOrEmpty(trackingId) && uid == "")
            {

                //url = Common.Common.GetAppSettingValue("Transact365URL").ToString() + "tracking_id/" + trackingId;
                
                url = "https://gateway.transact365.com/v2/transactions/tracking_id/" + trackingId;

            }

            //// var url = Common.Common.GetAppSettingValue("FlutterBankChargeTransactionTestURl").ToString();
            try
            {


                var Data = CommonGet<T365ResponseVm<T365ResponseDataVm>>(url);

                return Data.Result;
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message , ErrorType.T365 , "Get Transaction status");

                return new T365ResponseVm<T365ResponseDataVm>();
            }
        }

        public T365TransactionsResponseVm<T365ResponseDataVm> GetTransactions(string uid = "", string trackingId = "")
        {
            //// Test Url
            var url = "https://gateway.transact365.com/transactions/" + uid;
            //var url = Common.Common.GetAppSettingValue("Transact365URL").ToString() + uid;


            //Log.Write(url);
            if (!string.IsNullOrEmpty(trackingId) && uid == "")
            {

                //url = Common.Common.GetAppSettingValue("Transact365URL").ToString() + "tracking_id/" + trackingId;

                url = "https://gateway.transact365.com/v2/transactions/tracking_id/" + trackingId;

            }

            //// var url = Common.Common.GetAppSettingValue("FlutterBankChargeTransactionTestURl").ToString();
            try
            {


                var Data = CommonGet<T365TransactionsResponseVm<T365ResponseDataVm>>(url);

                return Data.Result;
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.T365, "Get Transaction status");

                return new T365TransactionsResponseVm<T365ResponseDataVm>();
            }
        }

        #endregion


    }


}