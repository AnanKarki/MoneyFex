using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway
{
    public class WebServices
    {
        public async Task<T> PostTransaction<T>(string request)
        {

            string url = WebServicesConfiguration.url;
            var http = (HttpWebRequest)WebRequest.Create(url);
            String username = "support@moneyfex.com";
            String password = "MFsuplg4&";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            http.Headers.Add("Authorization", "Basic " + encoded);
            
            http.Method = "POST";
            http.ContentType = "application/json; charset=iso-8859-1";
            byte[] utf8bytes = Encoding.UTF8.GetBytes(request.ToString());

            byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);
            http.ContentType = "application/json";

            var content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");



            http.ContentLength = request.Length;
            var sw = new StreamWriter(http.GetRequestStream());
            sw.Write(request);
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


            var result = await DeserializeObject<T>(body);

            return result;
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
    }
}
