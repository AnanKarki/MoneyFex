USE [MoneyFexDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Alter PROCEDURE [dbo].[SP_GETBANKTRANSACTIONSTATEMENTOFSENDER]
	-- Add the parameters for the stored procedure here
	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	  -- Insert statements for procedure here
	  select 
	 bankAccountDeposit.Id Id,
	 bankAccountDeposit.ReceiverAccountNo AccountNumber,
     ISNULL( senderInfo.PhoneNumber , '') SenderTelephoneNo,
     ISNULL( bankAccountDeposit.ReceiverName  ,'No Name') ReceiverName,
     bankAccountDeposit.ReceiverCity ReceiverCity ,
     receivingCountry.CountryName   ReceiverCountry  ,
     bankAccountDeposit.Fee Fee ,
     bankAccountDeposit.SendingAmount     GrossAmount ,
	 5 Status,
     0 statusOfMobileWallet,
     0 StatusofMobileTransfer,
     bankAccountDeposit.Status StatusOfBankDepoist ,
     
	 case when bankAccountDeposit.Status = 0 then
	      'In Progress'
	      when bankAccountDeposit.Status= 1 then
	      'In Progress'
          when bankAccountDeposit.Status= 2 then
	      'Cancelled'
	      when bankAccountDeposit.Status= 3 then
	      'Paid'
	      when bankAccountDeposit.Status= 4 then
	      'In progress'
	       when bankAccountDeposit.Status= 5 then
	      'Failed'
	      when bankAccountDeposit.Status= 6 then
	      'Payment Pending'
	      when bankAccountDeposit.Status= 7 then
	      'In progress (ID Check)'
	      when bankAccountDeposit.Status= 8 then
	      'In progress (MoneyFex Bank Deposit) '
	      when bankAccountDeposit.Status= 9 then
	      'In progress'
          when bankAccountDeposit.Status=10 then
	      'Abnormal' 
	      when bankAccountDeposit.Status= 11 then
	      'Full Refund' 
	      when bankAccountDeposit.Status= 12 then
	      'Partial Refund' 
	      when bankAccountDeposit.Status= 13 then
	      'In Progress'
	      else
	      'In Progress'
	 end StatusName,
    
	 'Bank Deposit' TransactionType ,
      receivingCountry.CurrencySymbol ReceivingCurrencySymbol ,
      receivingCountry.Currency ReceivingCurrrency ,
      sendingCountry.Currency  SendingCurrency ,
      sendingCountry.CurrencySymbol SendingCurrencySymbol ,
      bankAccountDeposit.ReceivingAmount ReceivingAmount ,
      bankAccountDeposit.TotalAmount TotalAmount ,
      bankAccountDeposit.SenderPaymentMode SenderPaymentMode ,
      IsNUll(creditDebitCardInfo.CardNumber ,'') CardNumber ,
	  bankAccountDeposit.ExchangeRate ExchangeRate ,
	  convert(varchar,  bankAccountDeposit.TransactionDate, 106) Date, 
	  6 TransactionServiceType,
	  bankAccountDeposit.TransactionDate TransactionDate,
	  ISNULL(bankAccountDeposit.PaymentReference  ,'')Reference,
	  ISNULL(bankAccountDeposit.PaymentReference  ,'') PaymentReference,
	  bankAccountDeposit.BankCode BankCode,
     '' WalletName,
	  case when  bankAccountDeposit.IsEuropeTransfer = 0 then
	      ''
	      else 
	      bankInfo.Name  
	  end BankName,
     
	 ISNULL(bankAccountDeposit.ReceiptNo  ,'') TransactionIdentifier ,
      senderInfo.AccountNo FaxerAccountNo ,
      sendingCountry.CountryName FaxerCountry ,
      ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, '') SenderName ,
	  bankAccountDeposit.IsManualDeposit IsManualBankDeposit ,
      bankAccountDeposit.SenderId senderId ,
      cast( 0 AS BIT ) IsRetryAbleCountry ,
      senderInfo.IsBusiness  IsBusiness ,
	  
	  case when bankAccountDeposit.Status = 10 then
	  cast( 1 AS BIT )
	  else
	  cast( 0 AS BIT )
	  end IsAbnormalTransaction ,
	 
	  bankAccountDeposit.IsEuropeTransfer IsEuropeTransfer,
	 
	  case when bankAccountDeposit.IsComplianceApproved=1 then
	  cast( 0 AS BIT )
	  else
	  bankAccountDeposit.IsComplianceNeededForTrans
	   end IsAwaitForApproval,
      
	  bankAccountDeposit.PaidFromModule PaidFromModule ,         
      bankAccountDeposit.PayingStaffId AgentStaffId, 
      case when bankAccountDeposit.Apiservice = 0 then
	      'VGG'
	      when bankAccountDeposit.Apiservice= 1 then
	      'TransferZero'
          when bankAccountDeposit.Apiservice= 2 then
	      'EmergentApi'
	      when bankAccountDeposit.Apiservice= 3 then
	      'MTN'	 
		  when bankAccountDeposit.Apiservice= 4 then
	      'Zenith'	
		  else
		  'Wari'
		  end  ApiService,
	  ISNULL(bankAccountDeposit.TransferReference ,'') TransferReference ,

      bankAccountDeposit.IsTransactionDuplicated IsDuplicatedTransaction,
      bankAccountDeposit.ReceiptNo DuplicatedTransactionReceiptNo,
       ISNULL(ReInTrans.ReceiptNo,'') ReInitializedReceiptNo ,
	   case when ReInTrans.ReceiptNo IS NULL then 
	  cast( 0 AS BIT )
	   else 
	   cast( 1 AS BIT )
	   end IsReInitializedTransaction,
	   ISNULL(ReInTrans.CreatedByName ,'') ReInitializeStaffName ,
	   ISNULL( convert(varchar,  ReInTrans.Date, 1) ,'') ReInitializedDateTime ,
      bankAccountDeposit.RecipientId RecipientId ,
      bankAccountDeposit.SendingCountry SenderCountryCode ,
      ISNULL(senderInfo.Email,'') SenderEmail ,
	  bankAccountDeposit.ReceivingCountry ReceivingCountryCode,
	  case when bankAccountDeposit.PaidFromModule =0 then
	      'Sender'
		  when bankAccountDeposit.PaidFromModule =1 then 
		  ''
		  when bankAccountDeposit.PaidFromModule =2 then 
		  ''
		   when bankAccountDeposit.PaidFromModule =3 then 
		  'Agent'
		   when bankAccountDeposit.PaidFromModule =4 then 
		  'Admin Staff'
		   when bankAccountDeposit.PaidFromModule =5 then 
		   ''
		  else
		  ''
	  end TransactionPerformedBy

	 from BankAccountDeposit bankAccountDeposit
	 join FaxerInformation senderInfo on bankAccountDeposit.SenderId = senderInfo.Id
	 join Country sendingCountry on bankAccountDeposit.SendingCountry = sendingCountry.CountryCode
	 join Country receivingCountry on bankAccountDeposit.ReceivingCountry = receivingCountry.CountryCode
	 left join CardTopUpCreditDebitInformation creditDebitCardInfo on creditDebitCardInfo.TransferType = 4
     And bankAccountDeposit.Id = creditDebitCardInfo.CardTransactionId 
     left join Bank bankInfo on bankAccountDeposit.BankId = bankInfo.Id 
     left join ReinitializeTransaction ReInTrans on bankAccountDeposit.ReceiptNo = ReInTrans.NewReceiptNo
	 
	
	 
END
GO
exec SP_GETBANKTRANSACTIONSTATEMENTOFSENDER
       