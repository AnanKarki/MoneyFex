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
ALTER PROCEDURE [dbo].[SP_GETKIIPAYWALLETTRANSACTIONSTATEMENTOFSENDER]
	-- Add the parameters for the stored procedure here
	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	  -- Insert statements for procedure here
 select 
	 kiipay.Id Id,
	 presonalDetails.MobileNo AccountNumber,
     presonalDetails.MobileNo  SenderTelephoneNo,
     ISNULL(presonalDetails.FirstName, '') + ' ' + ISNULL(presonalDetails.MiddleName+' ', '') +ISNULL(presonalDetails.LastName, '')  ReceiverName,
     presonalDetails.CardUserCity ReceiverCity ,
     receivingCountry.CountryName   ReceiverCountry  ,
     kiipay.FaxingFee Fee ,
     kiipay.FaxingAmount  GrossAmount ,
	 5 Status,
     0 statusOfMobileWallet,
     0 StatusofMobileTransfer,
     2 StatusOfBankDepoist ,
    'Completed' StatusName,
    'KiiPay Wallet' TransactionType ,
      receivingCountry.CurrencySymbol ReceivingCurrencySymbol ,
      receivingCountry.Currency ReceivingCurrrency ,
      sendingCountry.Currency  SendingCurrency ,
      sendingCountry.CurrencySymbol SendingCurrencySymbol ,
      kiipay.RecievingAmount ReceivingAmount ,
      kiipay.TotalAmount TotalAmount ,
      kiipay.SenderPaymentMode SenderPaymentMode ,
      IsNUll(creditDebitCardInfo.CardNumber ,'') CardNumber ,
	  kiipay.ExchangeRate ExchangeRate ,
	  convert(varchar,  kiipay.TransactionDate, 106) Date, 
	  2 TransactionServiceType,
	  kiipay.TransactionDate TransactionDate,


    ISNULL(kiipay.TopUpReference  ,'') Reference,
	  ISNULL(kiipay.TopUpReference  ,'')  PaymentReference,
	  '' BankCode,
     '' WalletName,
	  ''BankName,
     
	  ISNULL(kiipay.ReceiptNumber  ,'')   TransactionIdentifier ,
      senderInfo.AccountNo FaxerAccountNo ,
      sendingCountry.CountryName FaxerCountry ,
      ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, '') SenderName ,
	  cast( 0 AS BIT ) IsManualBankDeposit ,
      kiipay.FaxerId senderId ,
      cast( 0 AS BIT ) IsRetryAbleCountry ,
      senderInfo.IsBusiness  IsBusiness ,
	  cast( 0 AS BIT ) IsAbnormalTransaction ,
	 
	  cast( 0 AS BIT ) IsEuropeTransfer,
	 
	  cast( 0 AS BIT ) IsAwaitForApproval,
      
	  1 PaidFromModule ,         
      kiipay.PayingStaffId AgentStaffId, 
      '' ApiService,
       ISNULL(kiipay.TopUpReference  ,'')  TransferReference ,
      cast( 0 AS BIT ) IsDuplicatedTransaction,
      kiipay.ReceiptNumber DuplicatedTransactionReceiptNo,
       '' ReInitializedReceiptNo ,
	  cast( 0 AS BIT ) IsReInitializedTransaction,
	   '' ReInitializeStaffName ,
	   '' ReInitializedDateTime ,
	                    
      kiipay.RecipientId RecipientId ,
      kiipay.SendingCountry SenderCountryCode ,
      ISNULL(senderInfo.Email,'') SenderEmail ,
	  kiipay.ReceivingCountry ReceivingCountryCode,
	  case when kiipay.PayedBy =0 then
	      'Sender'
		  when kiipay.PayedBy =1 then 
		    'Admin Staff'
		  when kiipay.PayedBy =2 then 
		  ''
		  else
		  'Agent'
	  end TransactionPerformedBy

	 from TopUpSomeoneElseCardTransaction kiipay
	 join FaxerInformation senderInfo on kiipay.FaxerId = senderInfo.Id
	 left join KiiPayPersonalWalletInformation presonalDetails on kiipay.KiiPayPersonalWalletId = presonalDetails .Id
	 join Country sendingCountry on kiipay.SendingCountry = sendingCountry.CountryCode
	 join Country receivingCountry on kiipay.ReceivingCountry = receivingCountry.CountryCode
	 left join CardTopUpCreditDebitInformation creditDebitCardInfo on creditDebitCardInfo.TransferType = 0
     And kiipay.Id = creditDebitCardInfo.CardTransactionId 
	
	 	 
END
GO
exec SP_GETKIIPAYWALLETTRANSACTIONSTATEMENTOFSENDER
       