using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class TransactionErrorServices
    {
        FAXEREntities dbContext = null;
        public TransactionErrorServices()
        {
            dbContext = new FAXEREntities();
        }

        public IQueryable<TransactionError> TransactionErrors()
        {
            return dbContext.TransactionError;
        }
        public TransactionError GetTransactionErrorByTransactionIdAndTransferMethod(int transactionId, TransactionTransferMethod transferMethod)
        {
            return TransactionErrors().Where(x => x.TransactionId == transactionId && x.TransferMethod == transferMethod).FirstOrDefault();
        }
        public void AddTransactionError(TransactionError transactionError)
        {
            dbContext.TransactionError.Add(transactionError);
            dbContext.SaveChanges();
        }
        public void UpdateTransactionError(TransactionError transactionError)
        {
            dbContext.Entry(transactionError).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

    }
}