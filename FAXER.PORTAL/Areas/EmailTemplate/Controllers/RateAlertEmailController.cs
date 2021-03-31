using Svg;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class RateAlertEmailController : Controller
    {
        // GET: EmailTemplate/RateAlertEmail
        public ActionResult Index(string SenderFristName="", string TodayDate="", decimal TodayRate=0, string YesterdayDate="", decimal YesterdayRate=0,
            string RateAlert=""
            , string SendingCurrency="", string ReceivingCurrency="", string ReceivingCountry="" , string SendingCountry = "")
        {
            ViewBag.FirstName = SenderFristName;
            ViewBag.YesterdayRate = YesterdayRate;
            ViewBag.YesterdayDate = YesterdayDate;
            ViewBag.TodayRate = TodayRate;
            ViewBag.TodayDate = TodayDate;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;

            if (ReceivingCurrency == "Euro") {
                ViewBag.SendingCurrencySymbol = "";
            }
            else {
                ViewBag.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry);
            }

            ViewBag.ReceivingCountryCode = ReceivingCountry;
            ViewBag.Difference = Math.Round(TodayRate - YesterdayRate , 2);

            if (YesterdayRate < TodayRate)
            {

                ViewBag.DifferenceUp = true;
            }
            else if (YesterdayRate > TodayRate)
            {
                ViewBag.DifferenceDown = true;

            }
            else
            {
                ViewBag.DifferenceZero = true;
            }
           ConvertImageToJPG(ReceivingCountry);
            return View();
        }

        public string ConvertImageToJPG(string CountryCode) {


            string directory = Server.MapPath("/Content/flag-icons/fonts/" + CountryCode.ToLower() + ".svg");

            string jpgdirectory = Server.MapPath("/Content/flag-icons/fonts/" + CountryCode + ".Jpeg");
            string path = "";
            var svgDocument = SvgDocument.Open(directory);
            string pngPath = "";

            try
            {
                // Check if file exists with its full path    
                if (System.IO.File.Exists(Path.Combine("/Content/flag-icons/fonts/", CountryCode + ".Jpeg")))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(Path.Combine("/Content/flag-icons/fonts/", CountryCode + ".Jpeg"));
                    //Console.WriteLine("File deleted.");
                }
            }
            catch (IOException ioExp)
            {

            }
            using (var smallBitmap = svgDocument.Draw())
            {
                var width = smallBitmap.Width;
                var height = smallBitmap.Height;
                //if (width != 2000)// I resize my bitmap
                //{
                //    width = 2000;
                //    height = 2000 / smallBitmap.Width * height;
                //}

                using (var bitmap = svgDocument.Draw(width, height))//I render again
                {
                    bitmap.Save(jpgdirectory, ImageFormat.Jpeg);
                }
            }
            return jpgdirectory;
        }
    }
}