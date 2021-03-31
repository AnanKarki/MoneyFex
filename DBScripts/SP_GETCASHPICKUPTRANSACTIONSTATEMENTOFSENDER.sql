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
ALTER PROCEDURE [dbo].[SP_GETCASHPICKUPTRANSACTIONSTATEMENTOFSENDER]
	-- Add the parameters for the stored procedure here
	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	  -- Insert statements for procedure here
	   select 
	 cashPickup.Id Id,
	 receiverDetails.PhoneNumber AccountNumber,
     receiverDetails.PhoneNumber  SenderTelephoneNo,
     ISNULL( receiverDetails.FullName  ,'No Name') ReceiverName,
     receiverDetails.City ReceiverCity ,
     receivingCountry.CountryName   ReceiverCountry  ,
     cashPickup.FaxingFee Fee ,
     cashPickup.FaxingAmount  GrossAmount ,
	 cashPickup.FaxingStatus Status,
     0 statusOfMobileWallet,
     0 StatusofMobileTransfer,
     2 StatusOfBankDepoist ,
       case when cashPickup.FaxingStatus = 0 then
	      'Not Received'
	      when cashPickup.FaxingStatus = 1 then
	      'Received'
          when cashPickup.FaxingStatus = 2 then
	      'Cancelled'
	      when cashPickup.FaxingStatus = 3 then
	      'Refunded'
	      when cashPickup.FaxingStatus = 4 then
	      'In Progress'
	      when cashPickup.FaxingStatus = 5 then
	      'Completed'
	      when cashPickup.FaxingStatus = 6 then
	      'Payment Pending'
	      when cashPickup.FaxingStatus = 7 then
	      'In Progress (ID Check)'
	      when cashPickup.FaxingStatus = 8 then
	      'In progress '
	      when cashPickup.FaxingStatus = 9 then
	      'Refund'
          when cashPickup.FaxingStatus = 10 then
	      'Refund'
		  when cashPickup.FaxingStatus = 11 then
	      'In Progress'  
	      else
	      'In Progress'
	      end StatusName,
    
	 'Cash Pickup' TransactionType ,
      receivingCountry.CurrencySymbol ReceivingCurrencySymbol ,
      receivingCountry.Currency ReceivingCurrrency ,
      sendingCountry.Currency  SendingCurrency ,
      sendingCountry.CurrencySymbol SendingCurrencySymbol ,
      cashPickup.ReceivingAmount ReceivingAmount ,
      cashPickup.TotalAmount TotalAmount ,
      cashPickup.SenderPaymentMode SenderPaymentMode ,
      IsNUll(creditDebitCardInfo.CardNumber ,'') CardNumber ,
	  cashPickup.ExchangeRate ExchangeRate ,
	  convert(varchar,  cashPickup.TransactionDate, 106) Date, 
	  5 TransactionServiceType,
	  cashPickup.TransactionDate TransactionDate,
	  ISNULL(cashPickup.PaymentReference ,'')  Reference,
	  ISNULL(cashPickup.PaymentReference ,'')  PaymentReference,
	  '' BankCode,
     '' WalletName,
	  ''BankName,
     
	 ISNULL(cashPickup.ReceiptNumber ,'')  TransactionIdentifier ,
      senderInfo.AccountNo FaxerAccountNo ,
      sendingCountry.CountryName FaxerCountry ,
      ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, '') SenderName ,
	   cast( 0 AS BIT ) IsManualBankDeposit ,
      cashPickup.SenderId senderId ,
       cast( 0 AS BIT ) IsRetryAbleCountry ,
      senderInfo.IsBusiness  IsBusiness ,
	   cast( 0 AS BIT ) IsAbnormalTransaction ,
	 
	   cast( 0 AS BIT ) IsEuropeTransfer,
	 
	  case when cashPickup.IsComplianceApproved=1 then
	  cast( 0 AS BIT )
	  else
	  cashPickup.IsComplianceNeededForTrans
	   end IsAwaitForApproval,
      
	  cashPickup.OperatingUserType PaidFromModule ,         
      cashPickup.PayingStaffId AgentStaffId, 
      case when cashPickup.Apiservice = 0 then
	      'VGG'
	      when cashPickup.Apiservice= 1 then
	      'TransferZero'
          when cashPickup.Apiservice= 2 then
	      'EmergentApi'
	      when cashPickup.Apiservice= 3 then
	      'MTN'	 
		  when cashPickup.Apiservice= 4 then
	      'Zenith'	
		  else
		  'Wari'
		  end  ApiService,
    ISNULL(cashPickup.TransferReference ,'')  TransferReference ,
     cast( 0 AS BIT ) IsDuplicatedTransaction,
      cashPickup.ReceiptNumber DuplicatedTransactionReceiptNo,
       ISNULL(ReInTrans.ReceiptNo,'') ReInitializedReceiptNo ,
	   case when ReInTrans.ReceiptNo IS NULL then 
	   cast( 0 AS BIT )
	   else 
	   cast( 1 AS BIT )
	   end IsReInitializedTransaction,
	   ISNULL(ReInTrans.CreatedByName ,'') ReInitializeStaffName ,
	   ISNULL( convert(varchar,  ReInTrans.Date, 1) ,'') ReInitializedDateTime ,
	                    
      cashPickup.RecipientId RecipientId ,
      cashPickup.SendingCountry SenderCountryCode ,
      ISNULL(senderInfo.Email,'') SenderEmail ,
	  cashPickup.ReceivingCountry ReceivingCountryCode,
	  case when cashPickup.OperatingUserType =0 then
	      'Sender'
		  when cashPickup.OperatingUserType =1 then 
		  ''
		  when cashPickup.OperatingUserType =2 then 
		  ''
		   when cashPickup.OperatingUserType =3 then 
		  'Agent'
		   when cashPickup.OperatingUserType =4 then 
		  'Admin Staff'
		   when cashPickup.OperatingUserType =5 then 
		   ''
		  else
		  ''
	  end TransactionPerformedBy

	 from FaxingNonCardTransaction cashPickup
	 left join ReceiversDetails receiverDetails on cashPickup.NonCardRecieverId = receiverDetails.Id
	 join FaxerInformation senderInfo on cashPickup.SenderId = senderInfo.Id
	 join Country sendingCountry on cashPickup.SendingCountry = sendingCountry.CountryCode
	 join Country receivingCountry on cashPickup.ReceivingCountry = receivingCountry.CountryCode
	 left join CardTopUpCreditDebitInformation creditDebitCardInfo on creditDebitCardInfo.TransferType = 2
     And cashPickup.Id = creditDebitCardInfo.CardTransactionId 
     left join ReinitializeTransaction ReInTrans on cashPickup.ReceiptNumber = ReInTrans.NewReceiptNo
	
	 	 
END
GO
exec SP_GETCASHPICKUPTRANSACTIONSTATEMENTOFSENDER
       