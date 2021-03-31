using SelectPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace FAXER.PORTAL.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {

            //http://cyberlinknepal.com/design/pdl/latest-design/report/career-profiler/

            var strbody = "http://cyberlinknepal.com/design/pdl/latest-design/report/career-profiler/";
            
            return View();
            //byte[] inputstrngBytes = Encoding.UTF8.GetBytes(strbody);
            //MemoryStream Memstream = new MemoryStream(inputstrngBytes);
            //var pdfContent = Memstream;
            //pdfContent.Position = 0;
            //TextWriter tx = new StreamWriter(Memstream);
            ////return new FileStreamResult(pdfContent, "application/pdf");
            //var pdfdoc = Common.Common.GetPdf(strbody);
            //Response.Clear();
            //MemoryStream ms = new MemoryStream(inputstrngBytes);
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=labtest.pdf");
            //Response.Buffer = true;
            //ms.WriteTo(Response.OutputStream);
            //Response.End();
            //PdfDocument pdfDocument = new PdfDocument();

            //byte[] bytes = streamToByteArray(Memstream);
            //string mimeType = "Application/pdf";
            //Response.Buffer = true;
            //Response.Clear();
            //Response.ContentType = mimeType;
            //Response.OutputStream.Write(inputstrngBytes, 0, inputstrngBytes.Length);

            //Response.Flush();
            //Response.End();
            //return File(pdfContent, "application/pdf");
            //XDocument document = new XDocument(Memstream, new HtmlLoadOptions());
            //document.Save(outpath + ".pdf");
            //return View();
        }

        public static byte[] streamToByteArray(Stream stream)
        {
            byte[] byteArray = new byte[16 * 1024];
            using (MemoryStream mSteram = new MemoryStream())
            {
                int bit;
                while ((bit = stream.Read(byteArray, 0, byteArray.Length)) > 0)
                {
                    mSteram.Write(byteArray, 0, bit);
                }
                return mSteram.ToArray();
            }
        }

        public void Test() {

            //string testStr = "Test for Inline style and CSS support" +
            //       " This text has yellow background color and is center aligned." +
            //       "This text has no background color and is center aligned.";
            //byte[] inputBytes = Encoding.UTF8.GetBytes(testStr);

            //MemoryStream stream = new MemoryStream(inputBytes);
            //using (FileStream file = new FileStream(" file.pdf", FileMode.Create, System.IO.FileAccess.Write))
            //{
            //    byte[] bytes = new byte[stream.Length];
            //    stream.Read(bytes, 0, (int)stream.Length);
            //    file.Write(bytes, 0, bytes.Length);
            //    stream.Close();
            //}
            
            // load HTML file
            // convert stream to string
            //StreamReader reader = new StreamReader(stream);
            //string text = reader.ReadToEnd();
            //string mimeType = "Application/pdf";
            //Response.Buffer = true;
            //Response.Clear();
            //Response.ContentType = mimeType;
            //Response.OutputStream.Write(reader, 0, 20);
            //Console.WriteLine(text);
            //Console.ReadLine();
            //stream, new HtmlLoadOptions()
            //PdfDocument doc = PdfDocument.;


            // save output PDF into file/stream

        }
        

  }

}

