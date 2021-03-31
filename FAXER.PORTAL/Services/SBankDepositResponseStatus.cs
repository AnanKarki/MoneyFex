using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SBankDepositResponseStatus
    {
        DB.FAXEREntities dbContext = null;
        public SBankDepositResponseStatus()
        {
            dbContext = new DB.FAXEREntities();
        }

        public void AddLog(BankDepositResponseVm vm, int transactionId)
        {

            try
            {

                BankDepositResponseStatus bankDepositResponseStatus = new BankDepositResponseStatus()
                {
                    extraResult = vm.extraResult,
                    message = vm.message,
                    response = vm.response,
                    status = vm.status,
                    TransactionDateTime = DateTime.Now,
                    TransactionId = transactionId
                };

                var result_bankDepositResponseStatus = dbContext.BankDepositResponseStatus.Add(bankDepositResponseStatus);
                dbContext.SaveChanges();

                BankDepositTransactionResponseResult bankDepositTransactionResponseResult = new BankDepositTransactionResponseResult()
                {

                    amountInBaseCurrency = vm.result.amountInBaseCurrency,
                    BankDepositResponseStatusId = result_bankDepositResponseStatus.Id,
                    beneficiaryBankCode = vm.result.beneficiaryBankCode,
                    beneficiaryAccountNumber = vm.result.beneficiaryAccountNumber,
                    beneficiaryAccountName = vm.result.beneficiaryAccountName,
                    baseCurrencyCode = vm.result.baseCurrencyCode,
                    errorCode = vm.result.errorCode,
                    errorDescription = vm.result.errorDescription,
                    partnerTransactionReference = vm.result.partnerTransactionReference,
                    payername = vm.result.payername,
                    paymentAmount = vm.result.paymentAmount,
                    processorGateway = vm.result.processorGateway,
                    retriedCount = vm.result.retriedCount,
                    senderName = vm.result.senderName,
                    sourceAmount = vm.result.sourceAmount,
                    targetAmount = vm.result.targetAmount,
                    targetCurrencyCode = vm.result.targetCurrencyCode,
                    transactioncharge = vm.result.transactioncharge,
                    transactiondate = vm.result.transactiondate,
                    transactionReference = vm.result.transactionReference,
                    transactionStatus = vm.result.transactionStatus,
                    transactionStatusDescription = vm.result.transactionStatusDescription
                };
                var result_bankDepositTransactionResponseResult = dbContext.BankDepositTransactionResponseResult.Add(bankDepositTransactionResponseResult);
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                Log.Write("Bank Deposit Api " + DateTime.Now + " " + "Transaction Cannot added" + transactionId);
            }
        }

        public bool UpdateTransferZeroTransactionStatus(int transactionId, TransferZero.Sdk.Model.TransactionState transactionStatus)
        {

            //dbContext.Entry<BankDepositTransactionResponseResult>(bankDepositTransactionResponse).State = System.Data.Entity.EntityState.Modified;
            //var bankDepositStatus = dbContext.BankDepositResponseStatus.
            //    Where(x => x.Id == bankDepositTransactionResponse.BankDepositResponseStatusId).FirstOrDefault();
            var bankDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == transactionId).FirstOrDefault();
            SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit(dbContext);
            
            switch ((TransferZero.Sdk.Model.TransactionState)transactionStatus)
            {
                case TransferZero.Sdk.Model.TransactionState.Initial:
                    bankDeposit.Status = BankDepositStatus.Incomplete;

                    break;
                case TransferZero.Sdk.Model.TransactionState.Approved:
                    bankDeposit.Status = BankDepositStatus.Incomplete;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Pending:
                    bankDeposit.Status = BankDepositStatus.Incomplete;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Received:
                    bankDeposit.Status = BankDepositStatus.Incomplete;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Mispaid:
                    bankDeposit.Status = BankDepositStatus.Failed;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Manual:
                    bankDeposit.Status = BankDepositStatus.Confirm;
                    _senderBankAccountDepositServices.SendEmailAndSms(bankDeposit);
                    break;
                case TransferZero.Sdk.Model.TransactionState.Paid:
                    bankDeposit.Status = BankDepositStatus.Confirm;
                    _senderBankAccountDepositServices.SendEmailAndSms(bankDeposit);
                    break;
                case TransferZero.Sdk.Model.TransactionState.Canceled:
                    bankDeposit.Status = BankDepositStatus.Cancel;
                    _senderBankAccountDepositServices.SendEmailAndSms(bankDeposit);
                    break;
                case TransferZero.Sdk.Model.TransactionState.Refunded:
                    break;
                case TransferZero.Sdk.Model.TransactionState.Processing:
                    bankDeposit.Status = BankDepositStatus.Incomplete;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Exception:
                    bankDeposit.Status = BankDepositStatus.Failed;
                    break;
                default:
                    break;
            }
            dbContext.Entry<BankAccountDeposit>(bankDeposit).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();


        

            return true;
        }

        public BankAccountDeposit GetBankTransactionReponse(string transactionRef) {
            //var bankDepositTransactionResponseResult = dbContext.BankDepositTransactionResponseResult.Where(x => x.transactionReference == transactionRef).FirstOrDefault();
            //return bankDepositTransactionResponseResult;

            var bankDeposit = dbContext.BankAccountDeposit.Where(x => x.ReceiptNo.Trim() == transactionRef.Trim()).FirstOrDefault();
            return bankDeposit;
        }

        //public BankAccountDeposit GetBankTransactionResult(string transactionRef)
        //{

        //    var bankDeposit = dbContext.BankAccountDeposit.Where(x => x.ReceiptNo.Trim() == transactionRef.Trim()).FirstOrDefault();
        //    return bankDeposit;

        //}
    }
}