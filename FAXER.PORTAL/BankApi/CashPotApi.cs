using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FAXER.PORTAL.BankApi
{
    public class CashPotApi : ICashPotApi
    {

        public CancelTransStatus CancelTransaction(CancelTransactionRequest request)
        {
            string key = "refnumber";
            var content = new JSONXMLConvertor()
                            .JsonToXML(
                            Newtonsoft.Json.JsonConvert.SerializeObject(request)
                            , key);
            var xmlContent = ParseModelTOXML(content, "cancelTransaction");

            //string url = "http://cp.cybussolutions.com/cybusTransService/CybusServiceServer.php/cancelTransaction";
            string url = "https://afripay.co.uk/cybustrans/CybusServiceServer.php/cancelTransaction";
            var response = Post<CancelTransStatus>(url, xmlContent, "/Envelope/Body/cancelTransaction").Result;
            return response;
        }

        public RateGenericResponse GetRates(RateGenericRequest request)
        {
            string key = "refnumber";
            var content = new JSONXMLConvertor()
                            .JsonToXML(
                            Newtonsoft.Json.JsonConvert.SerializeObject(request)
                            , key);
            var xmlContent = ParseModelTOXML(content, "getRateGeneric");

            //string url = "http://cp.cybussolutions.com/cybusTransService/CybusServiceServer.php/getRateGeneric";
            string url = "https://afripay.co.uk/cybustrans/CybusServiceServer.php/getRateGeneric";
            var response = Post<RateGenericResponse>(url, xmlContent, "/Envelope/Body/getRateGeneric").Result;
            return response;
        }

        public TransactionStatusResposeCashPotVm GetTransactionStatus(TransactionStatusRequest request)
        {
            string key = "refnumber";
            var content = new JSONXMLConvertor()
                            .JsonToXML(
                            Newtonsoft.Json.JsonConvert.SerializeObject(request)
                            , key);
            var xmlContent = ParseModelTOXML(content, "getTransStatus");

            //string url = "http://cp.cybussolutions.com/cybusTransService/CybusServiceServer.php/getTransStatus";

            string url = "https://afripay.co.uk/cybustrans/CybusServiceServer.php/getTransStatus";
            var response = Post<TransactionStatusResposeCashPotVm>(url, xmlContent, "/Envelope/Body/getTransStatusResponse").Result;
            return response;

        }

        public void TestParse(string body)
        {


            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<SendTransGenericRequestVm>(body);
            var result = PostTransaction(model);

        }

        public SendTransGenericResponseVm PostTransaction(SendTransGenericRequestVm requestVm)
        {
            string key = "refnumber";
            var content = new JSONXMLConvertor()
                            .JsonToXML(
                            Newtonsoft.Json.JsonConvert.SerializeObject(requestVm)
                            , key);
            var xmlContent = ParseModelTOXML(content, "sendTransGeneric");

            //string url = "http://cp.cybussolutions.com/cybusTransService/CybusServiceServer.php/sendTransGeneric";

            string url = "https://afripay.co.uk/cybustrans/CybusServiceServer.php/sendTransGeneric";
            var response = Post<SendTransGenericResponseVm>(url, xmlContent, "/Envelope/Body/sendTransGenericResponse").Result;


            return response;

            //throw new NotImplementedException();
        }

        public TransactionStatusResposeCashPotVm UpdateTransaction(UpdateStatusRequest request)
        {
            string key = "refnumber";
            var content = new JSONXMLConvertor()
                            .JsonToXML(
                            Newtonsoft.Json.JsonConvert.SerializeObject(request)
                            , key);
            var xmlContent = ParseModelTOXML(content, "updateStatus");

           // string url = "http://cp.cybussolutions.com/cybusTransService/CybusServiceServer.php/updateStatus";

            string url = "https://afripay.co.uk/cybustrans/CybusServiceServer.php/updateStatus";
            var response = Post<TransactionStatusResposeCashPotVm>(url, xmlContent, "/Envelope/Body/updateStatus").Result;
            return response;
        }

        public BankAccountValidataionRequest ValidateBankAccount(BankAccountValidataionRequest request)
        {
            throw new NotImplementedException();
        }


        public async Task<T> Post<T>(string url, string model, string responsepath)
        {
            var data = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(model);
                request.ContentType = "text/xml;";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                string responseStr = "";
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    Stream responseStream = response.GetResponseStream();

                    responseStr = new StreamReader(responseStream).ReadToEnd();
                }
                data = ParseXMlToModel(responseStr, responsepath);
            }
            catch (Exception ex)
            {
            }

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
            return result;
        }

        public string ParseXMlToModel(string xmlmodel, string modelpath)
        {

            var content = new JSONXMLConvertor()
                          .XMLToJson(xmlmodel, modelpath);
            return content;
        }

        private string ParseModelTOXML(string body, string request)
        {
            string content = string.Format("{0}{1}{2}", XMLHeader(request), body, XMLFooter(request));
            return content;
        }
        private string XMLHeader(string request)
        {

            return string.Format("{0}", "<?xml version=" + "\"1.0\"" + " encoding=\"UTF-8\"?> " + "\n" +
                "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"" +
                " xmlns:ns1=\"urn:trans\"" +
                " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"" +
                " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
                " xmlns:SOAP-ENC=\"http://schemas.xmlsoap.org/soap/encoding/\"" +
                " SOAP-ENV:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
                " <SOAP-ENV:Body>" +
                "<ns1:" + request + ">");

        }

        private string XMLFooter(string request)
        {
            return "</ns1:" + request + ">" +
                "</SOAP-ENV:Body>" +
                "</SOAP-ENV:Envelope>";
        }

        public void AutoCheckStatus()
        {
            FAXEREntities db = new FAXEREntities();
            SSenderBankAccountDeposit sSenderBankAccountDeposit = new SSenderBankAccountDeposit();
            SSenderCashPickUp sSenderCashPickUp = new SSenderCashPickUp();
            var bankTransaction = db.BankAccountDeposit.Where(x => x.Apiservice == Apiservice.CashPot &&
                                                                  (x.Status == BankDepositStatus.PaymentPending ||
                                                                   x.Status == BankDepositStatus.Incomplete)).ToList();
            foreach (var item in bankTransaction)
            {
                var statusRsponse = sSenderBankAccountDeposit.GetStatusResponse(item.ReceiptNo);
                item.Status = sSenderBankAccountDeposit.GetCashPotTransactionStatus(statusRsponse.STATUS_CODE);
                sSenderBankAccountDeposit.Update(item);
            }

            var cashPickUpTransaction = db.FaxingNonCardTransaction.Where(x => x.Apiservice == Apiservice.CashPot &&
                                                                          (x.FaxingStatus == FaxingStatus.PaymentPending ||
                                                                          x.FaxingStatus == FaxingStatus.NotReceived)).ToList();
            foreach (var item in cashPickUpTransaction)
            {
                var statusRsponse = sSenderCashPickUp.GetStatusResponse(item.ReceiptNumber);
                item.FaxingStatus = sSenderCashPickUp.GetCashPotCAshPickUpTransactionStatus(statusRsponse.STATUS_CODE);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

        }


    }
}