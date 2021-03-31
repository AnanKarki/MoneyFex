using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FAXER.PORTAL.BankApi.Models.MoneyWaveViewModel;

namespace FAXER.PORTAL.BankApi
{
    public interface IMoneyWave
    {
        AuthResponseViewModel GetAuthorizationToken();
        ResponseVm<AccountOwner> AccountVerification(ResolveAccountRequestVM vm);
        ResponseVm<TransactionResponseVm> CreateTransaction(TransactionRequestViewModel vm);
        ResponseVm<TransactionStatusResponseVm> GetTransactionStatus(TransactionStatusResquestVm vm);

    }
}
