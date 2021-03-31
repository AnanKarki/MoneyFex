using Svg;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Announcements
{
    public class NewCorridorEmailController : Controller
    {
        // GET: EmailTemplate/NewCorridorEmail
        public ActionResult Index(string countryName = "", string countryCurrency = "", string countryCode = "")
        {
            ViewBag.CountryCode = countryCode;
            ViewBag.CountryName = countryName;
            ViewBag.CountryCurrencyName = countryCurrency;
            ConvertImageToJPG(countryCode);

            return View();
        }

        public string ConvertImageToJPG(string CountryCode)
        {


            string directory = Server.MapPath("/Content/flag-icons/fonts/" + CountryCode.ToLower() + ".svg");

            string jpgdirectory = Server.MapPath("/Content/Images/" + "flag.Jpeg");
            try
            {

                string path = "";
                var svgDocument = SvgDocument.Open(directory);
                string pngPath = "";

                try
                {
                    // Check if file exists with its full path    
                    if (System.IO.File.Exists(Path.Combine("/Content/Images/", "flag.Jpeg")))
                    {
                        // If file found, delete it    
                        System.IO.File.Delete(Path.Combine("/Content/Images/", "flag.Jpeg"));
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
            catch (Exception)
            {

                return "";
            }
        }
    }
}