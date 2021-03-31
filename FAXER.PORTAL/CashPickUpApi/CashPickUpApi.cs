using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.CashPickUpApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FAXER.PORTAL.CashPickUpApi
{
    public class CashPickUpApi
    {
    }
    public class WariApi
    {
        public async Task<T> Login<T>(string model)
        {

            var url = "https://upgrade.warime.com/wari_services/rest/b2p/partner/session";

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
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;

        }
        public async Task<T> PushTransaction<T>(string model)
        {

            var url = "https://upgrade.warime.com/wari_services/rest/b2p/partner/sendtrans";

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
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;

        }
        public async Task<T> UpdateTransaction<T>(string model)
        {

            var url = "https://upgrade.warime.com/wari_services/rest/b2p/partner/updatetrans";

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
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;

        }
        public async Task<T> TransactionSatuts<T>(string model)
        {

            var url = "https://upgrade.warime.com/wari_services/rest/b2p/partner/statustrans";

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
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;

        }
        public async Task<T> CancelTransaction<T>(string model)
        {

            var url = "https://upgrade.warime.com/wari_services/rest/b2p/partner/canceltrans";

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
            }


            var result = await CommonExtension.DeserializeObject<T>(body);

            return result;

        }

    }
}