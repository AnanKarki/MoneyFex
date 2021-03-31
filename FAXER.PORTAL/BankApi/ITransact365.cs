using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static FAXER.PORTAL.BankApi.Models.Transact365ViewModel;

namespace FAXER.PORTAL.BankApi
{
    public interface ITransact365
    {

        T365ResponseVm<T365ResponseDataVm> AuthorizationPayment(T365RequestVm<T365RequestDataVm> model);
        T365ResponseVm<T365ResponseDataVm> CreatePayment(T365RequestVm<T365RequestDataVm> model);
        T365ResponseVm<T365ReponseTransationVm> CancelTransaction(T365RequestVm<T365CapturesAndRefundsRequestVm> model);

        T365ResponseVm<T365ReponseTransationVm> RefundTransaction(T365RequestVm<T365CapturesAndRefundsRequestVm> model);

        T365ResponseVm<T365ResponseDataVm> GetTransaction(string uid = "", string trackingId = "");
    }
}