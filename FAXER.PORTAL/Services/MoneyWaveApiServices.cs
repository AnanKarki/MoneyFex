using FAXER.PORTAL.Controllers;
using FAXER.PORTAL.DB;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static FAXER.PORTAL.BankApi.Models.MoneyWaveViewModel;

namespace FAXER.PORTAL.Services
{
    public class MoneyWaveApiServices
    {
        public int CreatedById = 0;
        public string RecipientNo = "";
        public Module Module = Module.Faxer;

        FAXEREntities dbContext = null;
        public MoneyWaveApiServices()
        {
            dbContext = new FAXEREntities();
        }
        public MoneyWaveApiServices(int createdById , string recipientNo, Module module)
        {
            dbContext = new FAXEREntities();
            this.CreatedById = createdById;
            this.RecipientNo = recipientNo;
            this.Module = module;
        }
        public void AddMoneyWaveTransactionLog(ResponseVm<TransactionStatusResponseVm> responseVm)
        {
            MoneyWaveApiTransactionResponseLog model = new MoneyWaveApiTransactionResponseLog();
            model = BindDataInMoneyWaveTransctionLog(responseVm);
            AddTranactionLog(model);
        }

        private void AddTranactionLog(MoneyWaveApiTransactionResponseLog model)
        {
            dbContext.MoneyWaveApiTransactionResponseLog.Add(model);
            dbContext.SaveChanges();
        }

        private MoneyWaveApiTransactionResponseLog BindDataInMoneyWaveTransctionLog(ResponseVm<TransactionStatusResponseVm> responseVm)
        {
            MoneyWaveApiTransactionResponseLog model = new MoneyWaveApiTransactionResponseLog();
            if (responseVm.status.ToLower() == "error")
            {
                model = new MoneyWaveApiTransactionResponseLog()
                {
                    Status = responseVm.status,
                    Message = responseVm.message,
                    CreatedById = CreatedById,
                    Module = Module,
                    Ref = RecipientNo
                };
            }
            else
            {
                model = new MoneyWaveApiTransactionResponseLog()
                {
                    Status = responseVm.status,
                    Amount = responseVm.data.amount,
                    balance = responseVm.data.meta_Result.balance,
                    BeneficiaryAccountNumber = responseVm.data.beneficiary.accountNumber,
                    MetaNarration = responseVm.data.meta_Result.narration,
                    BeneficiaryBankCode = responseVm.data.beneficiary.bankCode,
                    AccountName = responseVm.data.beneficiary.accountName,
                    CreatedAt = responseVm.data.createdAt,
                    DisburseOrderId = responseVm.data.disburseOrderId,
                    FlutterReference = responseVm.data.flutterReference,
                    FlutterResponseCode = responseVm.data.flutterResponseCode,
                    FlutterResponseMessage = responseVm.data.flutterResponseMessage,
                    IPR = responseVm.data.ipr,
                    IPRC = responseVm.data.iprc,
                    LinkingReference = responseVm.data.linkingReference,
                    MetaSender = responseVm.data.meta_Result.sender,
                    MoreHostName = responseVm.data.meta_Result.req.more.hostname,
                    MoreId = responseVm.data.meta_Result.req.more.id,
                    MoreName = responseVm.data.meta_Result.req.more.name,
                    MorePID = responseVm.data.meta_Result.req.more.pid,
                    MoreType = responseVm.data.meta_Result.req.more.type,
                    R1 = responseVm.data.r1,
                    R2 = responseVm.data.r2,
                    Ref = responseVm.data.@ref,
                    Refund = responseVm.data.refund,
                    ReqIp = responseVm.data.meta_Result.req.ip,
                    Reversed = responseVm.data.reversed,
                    System_Type = responseVm.data.system_type,
                    TransactionStatus = responseVm.data.status,
                    UpdatedAt = responseVm.data.updatedAt,
                    WalletCharged = responseVm.data.walletCharged,
                    CreatedById =  CreatedById,
                    Module = Module
                };
            }
            return model;
        }
    }
}