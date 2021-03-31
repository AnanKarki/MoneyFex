using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.MoblieTransferApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Services
{
    public class SMobileMoneyTransferResopnseStatus
    {

        DB.FAXEREntities dbContext = null;
        public SMobileMoneyTransferResopnseStatus()
        {
            dbContext = new DB.FAXEREntities();
        }

        public void AddLog(MTNCameroonResponseParamVm vm, int transactionId)
        {
            try
            {

                MobileMoneyTransferResposeStatus mobileMoneyTransferResposeStatus = new MobileMoneyTransferResposeStatus()
                {
                    Code = vm.code,
                    Message = vm.message,
                    Reason = vm.reason,
                    Status = vm.status,
                    TransactionDateTime = DateTime.Now,
                    TransactionId = transactionId,
                    refId = vm.refId
                };
                var result_MobileMoneyTransferResponseStatus = dbContext.MobileMoneyTransferResposeStatus.Add(mobileMoneyTransferResposeStatus);
                dbContext.SaveChanges();

                MobileMoneyTransferTransactionResponseResult MobileMoneyTransferTransactionResponseResult = new MobileMoneyTransferTransactionResponseResult()
                {
                    MobileMoneyTransferResposeStatusId = result_MobileMoneyTransferResponseStatus.Id,
                    PaymentAmount = vm.amount.ToString(),
                    externalId = vm.externalId,
                    partyId = vm.payee.partyId,
                    partyIdType = vm.payee.partyIdType,
                    payeeNote = vm.payeeNote,
                    payerMessage = vm.payerMessage,
                    Currency = vm.currency

                };
                var result_bankDepositTransactionResponseResult = dbContext.MobileMoneyTransferTransactionResponseResult.Add(MobileMoneyTransferTransactionResponseResult);
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                Log.Write("Mobile Transfer Api " + DateTime.Now + " " + ex.Message);
            }
        }

        public bool UpdateTransferZeroMobileTransactionStatus(MobileMoneyTransferResposeStatus mobileMoneyTransferRespose , int Status)
        {

            dbContext.Entry<MobileMoneyTransferResposeStatus>(mobileMoneyTransferRespose).State = System.Data.Entity.EntityState.Modified;
            var data = dbContext.MobileMoneyTransfer.Where(x => x.Id == mobileMoneyTransferRespose.TransactionId).FirstOrDefault();
            data.Status = MobileMoneyTransferStatus.InProgress;
            switch ((TransactionState)Status)
            {

                case TransferZero.Sdk.Model.TransactionState.Initial:
                    data.Status = MobileMoneyTransferStatus.InProgress;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Approved:
                    data.Status = MobileMoneyTransferStatus.InProgress;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Pending:
                    data.Status = MobileMoneyTransferStatus.InProgress;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Received:
                    data.Status = MobileMoneyTransferStatus.Paid;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Mispaid:
                    data.Status = MobileMoneyTransferStatus.Failed;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Manual:
                    data.Status = MobileMoneyTransferStatus.Paid;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Paid:
                    data.Status = MobileMoneyTransferStatus.Paid;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Canceled:
                    data.Status = MobileMoneyTransferStatus.Cancel;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Refunded:
                    break;
                case TransferZero.Sdk.Model.TransactionState.Processing:
                    data.Status = MobileMoneyTransferStatus.InProgress;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Exception:
                    data.Status = MobileMoneyTransferStatus.Failed;
                    break;
                default:
                    break;
            }
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }

        public MobileMoneyTransferResposeStatus GetMobileReponseLog(string transactionId)
        {


            var result = dbContext.MobileMoneyTransferTransactionResponseResult.Where(x => x.externalId == transactionId).FirstOrDefault();

            if (result == null) {

                return null;
            }
            return result.MobileMoneyTransferResposeStatus;
        }
    }
}