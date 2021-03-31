using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static FAXER.PORTAL.BankApi.Models.FlutterWaveViewModel;

namespace FAXER.PORTAL.BankApi
{
    public interface IFlutterWave
    {
        string GetEncryptedData(string model);
        FWReponse<FlutterCommonResponseDataVm> CreateTransaction(FlutterCommonCustomerDetailsVm model);
        FWReponse<FlutterCommonResponseDataVm> ValidatedTransaction(string transactionreference, string otp);
        FWReponse<FlutterCommonVerifyResponseDataVm> VerifyTransation(string txref);

        //create transfer(payout)
        FWReponse<FlutterWaveResonse> CreateTransaction(string model);
        FWReponse<FlutterWaveResonse> GetTransactionById(int id);
        FWAllTransactionReponse<FlutterWaveAllTransactionResonse> GetAllTransactions();
        FWReponse<FlutterWaveResonse> CreateBulkTransfer(string model);
        FWReponse<FlutterWaveRateResponseVm> GetRate(decimal amount, string sendingCurrency, string receivingCurrency);
    }
}