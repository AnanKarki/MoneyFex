using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard
{
    /// <summary>
    /// Sender or Sender Virtual Account Card
    /// </summary>
    public class FaxerMoneyFaxTopUpCardController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();

        // GET: FaxerMoneyFaxTopUpCard
        public ActionResult Index()
        {
            if ((FaxerSession.LoggedUser == null))
            {
                FaxerSession.FromUrl = "/FaxerMoneyFaxTopUpCard";
                return RedirectToAction("Login", "FaxerAccount");
            }


            List<KiiPayPersonalWalletInformation> list = (from c in  context.KiiPayPersonalWalletInformation.Where(x =>  x.IsDeleted == false).OrderBy(x => x.FirstName).ToList()
                                                         join d in context.SenderKiiPayPersonalAccount on c.Id equals d.KiiPayPersonalWalletId 
                                                         select c).ToList();
            foreach (var item in list)
            {
                var MFTCcard = item.MobileNo.Decrypt();
                string[] token = MFTCcard.Split('-');

                item.MobileNo = token[1] + "-" + token[2];
            }
            ViewBag.TopUpCard = new SelectList(list, "Id", "MFTCCardNumber");
            var MFTCCardCount = list.Where(x => x.CardStatus == CardStatus.Active || x.CardStatus == CardStatus.InActive).Count();
            if (MFTCCardCount == 0) {

                TempData["MFTCCount"] = 0;
                return RedirectToAction("Index", "DashBoard");
            }
            // Dynamic 
            Common.FaxerSession.BackButtonURL = Request.Url.ToString();

            // final
            Common.FaxerSession.BackButtonURLMyMoneyFex = Request.Url.ToString();

            return View();
        }
        public ActionResult FaxMoney()
        {
            return View();
        }
        #region Yes
        public ActionResult FaxMoneyLogin()
        {
            return View();
        }

        public ActionResult FaxCardToTopUp()
        {
            return View();
        }
        public ActionResult CalculateFaxingFees()
        {
            return View();
        }
        public ActionResult TopUpPaymentMethod()
        {
            return View();
        }
        #endregion

        #region No
        public ActionResult TopUpCardRegistrationLogin()
        {
            return View();
        }
        public ActionResult FaxingNoMFCT()
        {
            return View();
        }
        #region Continue with MoneyFax Top-up Card Registration
        public ActionResult TopUpCardRegistration()
        {
            return View();
        }
        public ActionResult CardUsersContactDetails()
        {
            return View();
        }
        public ActionResult CardUsersIdentification()
        {
            return View();
        }
        public ActionResult TopCardRegistrationPayment()
        {
            return View();
        }
        public ActionResult NonMoneyFaxTopUpCardUser()
        {
            return View();
        }
        public ActionResult NonMoneyFaxEstimateFees()
        {
            return View();
        }
        public ActionResult NonMoneyFaxRreceiversDetails()
        {
            return View();
        }
        public ActionResult NonMoneyFaxPaymentMethod()
        {
            return View();
        }

        #endregion
        #endregion

        public ActionResult DetailsOnMoneyFaxTopUpCard(int moneyFaxTopUpCardId = 0)
        {
            int FaxerInformationId = FaxerSession.LoggedUser.Id;
            List<KiiPayPersonalWalletInformation> list = (from c in context.KiiPayPersonalWalletInformation.Where(x =>  x.IsDeleted == false)
                                                         join d in context.SenderKiiPayPersonalAccount.Where(x => x.SenderId == FaxerInformationId) on c.Id equals d.KiiPayPersonalWalletId
                                                         select c).ToList();
            foreach (var item in list)
            {
                var MFTCcard = item.MobileNo.Decrypt();
                item.MobileNo = Common.Common.FormatMFTCCard(MFTCcard);
            }
            DetailsOnMoneyFaxTopUpCardViewModel Vm = new DetailsOnMoneyFaxTopUpCardViewModel();
            if (moneyFaxTopUpCardId == 0)
            {
                ViewBag.MFTCCardNumber = new SelectList(list, "Id", "MFTCCardNumber");
            }
            else
            {
                ViewBag.MFTCCardNumber = new SelectList(list, "Id", "MFTCCardNumber", moneyFaxTopUpCardId);
                Vm = (from c in context.KiiPayPersonalWalletInformation.Where(x => x.Id == moneyFaxTopUpCardId).ToList()
                      select new DetailsOnMoneyFaxTopUpCardViewModel()
                      {
                          Id = c.Id,
                          //CardNumber = c.MFTCCardNumber.Contains("MFS") ? c.MFTCCardNumber :  c.MFTCCardNumber.Decrypt(),
                          CardNumber = c.MobileNo,
                          NameOnMoneyFaxTopUpCard = c.FirstName + " " + c.MiddleName + "" + c.LastName,
                          CountryOnMoneyFaxTopUpCard = c.CardUserCountry,
                          CityOnMoneyFaxTopUpCard = c.CardUserCity,
                          TelephoneNumber = c.CardUserTel,
                          EmailAddress = c.CardUserEmail,
                          AmountOnMoneyFaxTopUpCard = Common.Common.GetCurrencySymbol(c.CardUserCountry) + " "+ c.CurrentBalance ,
                          StatusOfMoneyFaxTopUpCard = Enum.GetName(typeof(CardStatus), c.CardStatus),
                          CardPhoto = c.UserPhoto,
                      }).FirstOrDefault();
            }
            Common.FaxerSession.BackButtonURL = Request.Url.ToString();

            return View(Vm);

        }

        [HttpPost]
        public ActionResult UpdoadCardUserPhoto()
        {

            var fileName = "";

            string id = Request.Form.GetValues("moneyFaxTopUpCardId").FirstOrDefault();

            int MFTCCardId = int.Parse(id);
            if (Request.Files.Count > 0)
            {
                //check model validation
                if (ModelState.IsValid)
                {
                    var upload = Request.Files[0];
                    if (upload == null)
                    {
                        ModelState.AddModelError("File", "Please Upload Identity Document");
                        return View();
                    }

                    string directory = Server.MapPath("/Documents");
                    fileName = "";
                    if (upload != null && upload.ContentLength > 0)
                    {
                        fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1]; //Path.GetFileName(upload.FileName);
                        upload.SaveAs(Path.Combine(directory, fileName));
                    }

                    string CardUrl = "/Documents/" + fileName;

                    var data = context.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
                    data.UserPhoto = CardUrl;
                    context.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();


                    return RedirectToAction("DetailsOnMoneyFaxTopUpCard" , new { moneyFaxTopUpCardId  = MFTCCardId});


                }
            }
            return View();
        }



        // Access Code for the card user which will be used to withdraw money from te agent 
        public JsonResult GetWithdrawalCode(int CardId)
        {

            DB.FAXEREntities db = new FAXEREntities();


            var data = db.KiiPayPersonalWalletWithdrawalCode.Where(x => x.KiiPayPersonalWalletId == CardId && x.IsExpired == false).FirstOrDefault();

            if (data == null)
            {

                KiiPayPersonalWalletWithdrawalCode cardUserWithdrawalCode = new KiiPayPersonalWalletWithdrawalCode()
                {
                    KiiPayPersonalWalletId = CardId,
                    AccessCode = Common.Common.GetNewAccessCodeForCardUser(),
                    IsExpired = false,
                    CreatedDateTime = DateTime.Now,

                };
                data = db.KiiPayPersonalWalletWithdrawalCode.Add(cardUserWithdrawalCode);
                db.SaveChanges();

            }

            return Json(new
            {
                AccessCode = data.AccessCode
            }, JsonRequestBehavior.AllowGet);

        }
    }
}