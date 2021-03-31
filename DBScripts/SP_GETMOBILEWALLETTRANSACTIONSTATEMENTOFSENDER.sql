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
Alter PROCEDURE [dbo].[SP_GETMOBILEWALLETTRANSACTIONSTATEMENTOFSENDER]
	-- Add the parameters for the stored procedure here
	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	  -- Insert statements for procedure here
	  select 
	 mobileMoneyTransfer.Id Id,
	 mobileMoneyTransfer.PaidToMobileNo AccountNumber,
     ISNULL( senderInfo.PhoneNumber , '') SenderTelephoneNo,
     ISNULL( mobileMoneyTransfer.ReceiverName  ,'No Name') ReceiverName,
     mobileMoneyTransfer.ReceiverCity ReceiverCity ,
     receivingCountry.CountryName   ReceiverCountry  ,
     mobileMoneyTransfer.Fee Fee ,
     mobileMoneyTransfer.SendingAmount     GrossAmount ,
	 5 Status,
      mobileMoneyTransfer.Status statusOfMobileWallet,
      mobileMoneyTransfer.Status StatusofMobileTransfer,
     2 StatusOfBankDepoist ,
     	case when mobileMoneyTransfer.Status = 0 then
	      'Failed'
	      when mobileMoneyTransfer.Status= 1 then
	      'In Progress'
          when mobileMoneyTransfer.Status= 2 then
	      'Paid'
	      when mobileMoneyTransfer.Status= 3 then
	      'Cancelled'
	      when mobileMoneyTransfer.Status= 4 then
	      'Payment Pending'
	       when mobileMoneyTransfer.Status= 5 then
	      'In Progress (ID Check)'
	      when mobileMoneyTransfer.Status= 6 then
	      'In progress'
	      when mobileMoneyTransfer.Status= 7 then
	      'In progress'
	      when mobileMoneyTransfer.Status= 8 then
	      'In progress '
	      when mobileMoneyTransfer.Status= 9 then
	      'Refund'
          when mobileMoneyTransfer.Status=10 then
	      'Refund' 
	      else
	      'In Progress'
	 end StatusName,
    
	 'Mobile Wallet' TransactionType ,
      receivingCountry.CurrencySymbol ReceivingCurrencySymbol ,
      receivingCountry.Currency ReceivingCurrrency ,
      sendingCountry.Currency  SendingCurrency ,
      sendingCountry.CurrencySymbol SendingCurrencySymbol ,
      mobileMoneyTransfer.ReceivingAmount ReceivingAmount ,
      mobileMoneyTransfer.TotalAmount TotalAmount ,
      mobileMoneyTransfer.SenderPaymentMode SenderPaymentMode ,
      IsNUll(creditDebitCardInfo.CardNumber ,'') CardNumber ,
	  mobileMoneyTransfer.ExchangeRate ExchangeRate ,
	  convert(varchar,  mobileMoneyTransfer.TransactionDate, 106) Date, 
	  1 TransactionServiceType,
	  mobileMoneyTransfer.TransactionDate TransactionDate,
	 ISNULL(mobileMoneyTransfer.PaymentReference ,'')  Reference,
	 ISNULL(mobileMoneyTransfer.PaymentReference ,'')  PaymentReference,
	  '' BankCode,
      '' WalletName,
	  '' BankName,
      ISNULL(mobileMoneyTransfer.ReceiptNo ,'')  TransactionIdentifier ,
      senderInfo.AccountNo FaxerAccountNo ,
      sendingCountry.CountryName FaxerCountry ,
      ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, '') SenderName ,
	   cast( 0 AS BIT ) IsManualBankDeposit ,
      mobileMoneyTransfer.SenderId senderId ,
       cast( 0 AS BIT ) IsRetryAbleCountry ,
      senderInfo.IsBusiness  IsBusiness ,
	  
	  case when mobileMoneyTransfer.Status = 7 then
	  cast( 1 AS BIT )
	  else
	  cast( 0 AS BIT )
	  end IsAbnormalTransaction ,
	 
	   cast( 0 AS BIT ) IsEuropeTransfer,
	 
	  case when mobileMoneyTransfer.IsComplianceApproved=1 then
	   cast( 0 AS BIT )
	  else
	  mobileMoneyTransfer.IsComplianceNeededForTrans
	   end IsAwaitForApproval,
      
	  mobileMoneyTransfer.PaidFromModule PaidFromModule ,         
      mobileMoneyTransfer.PayingStaffId AgentStaffId, 
      case when mobileMoneyTransfer.Apiservice = 0 then
	      'VGG'
	      when mobileMoneyTransfer.Apiservice= 1 then
	      'TransferZero'
          when mobileMoneyTransfer.Apiservice= 2 then
	      'EmergentApi'
	      when mobileMoneyTransfer.Apiservice= 3 then
	      'MTN'	 
		  when mobileMoneyTransfer.Apiservice= 4 then
	      'Zenith'	
		  else
		  'Wari'
		  end  ApiService,
       ISNULL(mobileMoneyTransfer.TransferReference ,'')  TransferReference ,
       cast( 0 AS BIT ) IsDuplicatedTransaction,
      mobileMoneyTransfer.ReceiptNo DuplicatedTransactionReceiptNo,
       ISNULL(ReInTrans.ReceiptNo,'') ReInitializedReceiptNo ,
	   case when ReInTrans.ReceiptNo IS NULL then 
	    cast( 0 AS BIT )
	   else 
	    cast( 1 AS BIT )
	   end IsReInitializedTransaction,
	   ISNULL(ReInTrans.CreatedByName ,'') ReInitializeStaffName ,
       ISNULL( convert(varchar,  ReInTrans.Date, 1) ,'') ReInitializedDateTime ,
	  mobileMoneyTransfer.RecipientId RecipientId ,
      mobileMoneyTransfer.SendingCountry SenderCountryCode ,
      ISNULL(senderInfo.Email,'') SenderEmail ,
	  mobileMoneyTransfer.ReceivingCountry ReceivingCountryCode,
	  case when mobileMoneyTransfer.PaidFromModule =0 then
	      'Sender'
		  when mobileMoneyTransfer.PaidFromModule =1 then 
		  ''
		  when mobileMoneyTransfer.PaidFromModule =2 then 
		  ''
		   when mobileMoneyTransfer.PaidFromModule =3 then 
		  'Agent'
		   when mobileMoneyTransfer.PaidFromModule =4 then 
		  'Admin Staff'
		   when mobileMoneyTransfer.PaidFromModule =5 then 
		   ''
		  else
		  ''
	  end TransactionPerformedBy

	 from MobileMoneyTransfer mobileMoneyTransfer
	 join FaxerInformation senderInfo on mobileMoneyTransfer.SenderId = senderInfo.Id
	 join Country sendingCountry on mobileMoneyTransfer.SendingCountry = sendingCountry.CountryCode
	 join Country receivingCountry on mobileMoneyTransfer.ReceivingCountry = receivingCountry.CountryCode
	 left join CardTopUpCreditDebitInformation creditDebitCardInfo on creditDebitCardInfo.TransferType =3
     And mobileMoneyTransfer.Id = creditDebitCardInfo.CardTransactionId 
     left join MobileWalletOperator wallet on mobileMoneyTransfer.WalletOperatorId = wallet.Id 
     left join ReinitializeTransaction ReInTrans on mobileMoneyTransfer.ReceiptNo = ReInTrans.NewReceiptNo
        
	
	
	 
END
GO
exec SP_GETMOBILEWALLETTRANSACTIONSTATEMENTOFSENDER
       