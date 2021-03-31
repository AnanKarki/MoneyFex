using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SBankDepositStatus
    {
        DB.FAXEREntities db = null;
        public SBankDepositStatus()
        {
            db = new DB.FAXEREntities();
        }

        public ServiceResult<bool> Update(BankDepositResponseVm model)
        {
            int transactionId = db.BankAccountDeposit.Where(x => x.ReceiptNo == model.result.partnerTransactionReference).Select(x => x.Id).FirstOrDefault();
            if (transactionId > 0)
            {

                var transactionResponseStatus = db.BankDepositResponseStatus.Where(x => x.TransactionId == transactionId).FirstOrDefault();
                transactionResponseStatus.message = model.message;
                transactionResponseStatus.response = model.response;
                transactionResponseStatus.status = model.status;
                transactionResponseStatus.extraResult = model.extraResult;

                db.Entry<BankDepositResponseStatus>(transactionResponseStatus).State = System.Data.Entity.EntityState.Modified;


                var transactionResponseStatusResult = db.BankDepositTransactionResponseResult.Where(x => x.BankDepositResponseStatusId == transactionResponseStatus.Id).FirstOrDefault();
                if (transactionResponseStatusResult != null)
                {
                    transactionResponseStatusResult.transactionStatus = model.result.transactionStatus;
                    transactionResponseStatusResult.transactionStatusDescription = model.result.transactionStatusDescription;
                    db.Entry<BankDepositTransactionResponseResult>(transactionResponseStatusResult).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                }
                else
                {
                    BankDepositTransactionResponseResult bankDepositTransactionResponseResult = new BankDepositTransactionResponseResult()
                    {
                        amountInBaseCurrency = model.result.amountInBaseCurrency,
                        BankDepositResponseStatusId = transactionResponseStatus.Id,
                        baseCurrencyCode = model.result.baseCurrencyCode,
                        beneficiaryAccountName = model.result.beneficiaryAccountName,
                        beneficiaryAccountNumber = model.result.beneficiaryAccountNumber,
                        beneficiaryBankCode = model.result.beneficiaryBankCode,
                        errorCode = model.result.errorCode,
                        errorDescription = model.result.errorDescription,
                        partnerTransactionReference = model.result.partnerTransactionReference,
                        payername = model.result.payername,
                        paymentAmount = model.result.paymentAmount,
                        processorGateway = model.result.processorGateway,
                        retriedCount = model.result.retriedCount,
                        senderName = model.result.senderName,
                        sourceAmount = model.result.sourceAmount,
                        targetAmount = model.result.targetAmount,
                        targetCurrencyCode = model.result.targetCurrencyCode,
                        transactioncharge = model.result.transactioncharge,
                        transactiondate = model.result.transactiondate,
                        transactionReference = model.result.transactionReference,
                        transactionStatus = model.result.transactionStatus,
                        transactionStatusDescription = model.result.transactionStatusDescription,
                    };
                    db.BankDepositTransactionResponseResult.Add(bankDepositTransactionResponseResult);
                    db.SaveChanges();
                }

                SSenderBankAccountDeposit _bankdeposit = new SSenderBankAccountDeposit();
                var data = _bankdeposit.List().Data.Where(x => x.Id == transactionId).FirstOrDefault();
                if (model.result.transactionStatus == 0 || model.result.transactionStatus == 1)
                {
                    data.Status = BankDepositStatus.Incomplete;
                }
                else if (model.result.transactionStatus == 3)
                {
                    data.Status = BankDepositStatus.Failed;
                }
                else if (model.result.transactionStatus == 2)
                {
                    data.Status = BankDepositStatus.Confirm;
                }
                _bankdeposit.Update(data);


            }
            return new ServiceResult<bool>()
            {

                Data = true
            };
        }


        public BankAccountDeposit getTransactionDetail(string refno)
        {

            var result = db.BankAccountDeposit.Where(x => x.ReceiptNo == refno).FirstOrDefault();
            return result;
        }





    }
}