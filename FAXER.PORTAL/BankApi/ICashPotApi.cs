using FAXER.PORTAL.BankApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAXER.PORTAL.BankApi
{
    public interface ICashPotApi
    {

        SendTransGenericResponseVm PostTransaction(SendTransGenericRequestVm requestVm);
        TransactionStatusResposeCashPotVm GetTransactionStatus(TransactionStatusRequest request);
        TransactionStatusResposeCashPotVm UpdateTransaction(UpdateStatusRequest request);
        RateGenericResponse GetRates(RateGenericRequest request);
        CancelTransStatus CancelTransaction(CancelTransactionRequest request);
        BankAccountValidataionRequest ValidateBankAccount(BankAccountValidataionRequest request);
    }


}
