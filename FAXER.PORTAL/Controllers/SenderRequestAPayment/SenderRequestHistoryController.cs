using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.SenderRequestAPayment
{
    public class SenderRequestHistoryController : Controller
    {
        SSenderSendARequest _sendARequestServices = null;
        
        public SenderRequestHistoryController()
        {
            _sendARequestServices = new SSenderSendARequest();
        }

        // GET: SenderRequestHistory
        public ActionResult Index(int filterKey = 0)
        {
            SetViewBagForFilter();
            var vm = _sendARequestServices.GetRequestHistory();
            if (filterKey == 1)
            {
                vm.RequestHistoryList = vm.RequestHistoryList.Where(x => x.Status == DB.RequestPaymentStatus.Paid).ToList();
            }
            else if (filterKey == 2)
            {
                vm.RequestHistoryList = vm.RequestHistoryList.Where(x => x.Status == DB.RequestPaymentStatus.UnPaid).ToList();
            }
            else if (filterKey == 3)
            {
                vm.RequestHistoryList = vm.RequestHistoryList.Where(x => x.Status == DB.RequestPaymentStatus.Cancelled).ToList();
            }
            vm.FilterKey = filterKey;
            return View(vm);
        }


        public ActionResult DeleteRequest(int id)
        {
            var data= _sendARequestServices.IsRequestAlreadyPaid(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            _sendARequestServices.Delete(data);

            return RedirectToAction("Index");
        }

        public ActionResult CancelRequest(int id)
        {
            var data = _sendARequestServices.IsRequestAlreadyPaid(id);
            if (data != null)
            {
                _sendARequestServices.CancelRequest(data);
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult EditRequest(int id)
        {
            if (id != 0)
            {
                var data = _sendARequestServices.IsRequestAlreadyPaid(id);
                if (data != null)
                {
                    PaymentType type = data.RequestType;
                    if (type == PaymentType.Local)
                    {
                        return RedirectToAction("EditLocalRequest", "SenderSendARequest", new { @id = id });
                    }
                    else if (type == PaymentType.International)
                    {
                        return RedirectToAction("EditInternationalRequest", "SenderSendARequest", new { @id = id });
                    }
                    return RedirectToAction("Index");
                }
               
            }
            return RedirectToAction("Index");
        }

        public void SetViewBagForFilter()
        {
            var filterList = _sendARequestServices.GetPaidStatusList();
            ViewBag.Filter = new SelectList(filterList, "Key", "Value");
        }
    }
}