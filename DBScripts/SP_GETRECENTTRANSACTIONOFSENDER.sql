USE [MoneyFexDB]
GO
/****** Object:  StoredProcedure [dbo].[SP_GETRECENTTRANSACTIONOFSENDER]    Script Date: 17-Mar-21 7:11:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_GETRECENTTRANSACTIONOFSENDER]
	-- Add the parameters for the stored procedure here
	    @TransactionServiceType int = 0,
        @senderId int = 0,
        @SendingCountry nvarchar(10)  = '',
        @ReceivingCountry nvarchar(10) = '',
        @SenderName nvarchar(30) = '',
        @SenderEmail nvarchar(30) = '',
        @DateRange nvarchar(30) = '',
		@FromDate nvarchar(30) = '',
	    @ToDate nvarchar(30) = '',
        @ReceiverName nvarchar(30)='',
        @searchString nvarchar(30) = '',
        @Status nvarchar(30)= '',
        @PhoneNumber nvarchar(30) = '',
        @SendingCurrency nvarchar(30) = '',
        @TransactionWithAndWithoutFee nvarchar(10) = '',
        @ResponsiblePerson nvarchar(30) = '',
        @SearchByStatus nvarchar(30) ='',
        @MFCode nvarchar(30)= '',
        @PageNum  int =1,
        @PageSize int = 50,
		@IsBusiness bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @count int = 0 ;
declare 
@cashquery varchar(max),  @mobilequery varchar(max) ,  @query varchar(max);
	
	set @query = 'select * into ##bankAccountDeposit from (select
	 bankAccountDeposit.Id Id,
	 bankAccountDeposit.ReceiverAccountNo AccountNumber,
     ISNULL(senderInfo.PhoneNumber  , '''')  SenderTelephoneNo,
      bankAccountDeposit.ReceiverName  ReceiverName,
     bankAccountDeposit.ReceiverCity ReceiverCity ,
     receivingCountry.CountryName   ReceiverCountry  ,
     bankAccountDeposit.Fee Fee ,
	 case when  bankAccountDeposit.Fee > 0 then 
	  cast( 1 AS BIT ) 
	 else
	  cast( 0 AS BIT ) 
	 end  HasFee,
     bankAccountDeposit.SendingAmount     GrossAmount ,
	 5 Status,
     0 statusOfMobileWallet,
     0 StatusofMobileTransfer,
     bankAccountDeposit.Status StatusOfBankDepoist,
	  case when bankAccountDeposit.Status = 0 then
	      ''In Progress''
	      when bankAccountDeposit.Status= 1 then
	      ''In Progress''
          when bankAccountDeposit.Status= 2 then
	      ''Cancelled''
	      when bankAccountDeposit.Status= 3 then
	      ''Paid''
	      when bankAccountDeposit.Status= 4 then
	      ''In progress''
	       when bankAccountDeposit.Status= 5 then
	      ''Failed''
	      when bankAccountDeposit.Status= 6 then
	      ''Payment Pending''
	      when bankAccountDeposit.Status= 7 then
	      ''In progress (ID Check)''
	      when bankAccountDeposit.Status= 8 then
	      ''In progress (MoneyFex Bank Deposit)''
	      when bankAccountDeposit.Status= 9 then
	      ''In progress''
          when bankAccountDeposit.Status=10 then
	     '' Abnormal''
	      when bankAccountDeposit.Status= 11 then
	      ''Full Refund''
	      when bankAccountDeposit.Status= 12 then
	      ''Partial Refund'' 
	      when bankAccountDeposit.Status= 13 then
	      ''In Progress''
	      else
	      ''In Progress''
	 end StatusName,
	  ''Bank Deposit'' TransactionType ,
      '''' ReceivingCurrencySymbol ,
      IsNull(bankAccountDeposit.ReceivingCurrency , receivingCountry.Currency) ReceivingCurrrency ,
      IsNull(bankAccountDeposit.SendingCurrency, sendingCountry.Currency ) SendingCurrency ,
      '''' SendingCurrencySymbol ,
      bankAccountDeposit.ReceivingAmount ReceivingAmount ,
      bankAccountDeposit.TotalAmount TotalAmount ,
      bankAccountDeposit.SenderPaymentMode SenderPaymentMode ,
      IsNUll(creditDebitCardInfo.CardNumber ,'''') CardNumber ,
	  bankAccountDeposit.ExchangeRate ExchangeRate ,
	  convert(varchar,  bankAccountDeposit.TransactionDate, 106) Date, 
	  6 TransactionServiceType,
	  bankAccountDeposit.TransactionDate TransactionDate,
	  ISNULL(bankAccountDeposit.PaymentReference  ,'''')Reference,
	  ISNULL(bankAccountDeposit.PaymentReference  ,'''') PaymentReference,
	  bankAccountDeposit.BankCode BankCode,
     '''' WalletName,
	  case when  bankAccountDeposit.IsEuropeTransfer = 0 then
	      bankInfo.Name  
	      else 
	      bankAccountDeposit.BankName  
	  end BankName,
	  ISNULL(bankAccountDeposit.ReceiptNo  ,'''') TransactionIdentifier ,
      senderInfo.AccountNo FaxerAccountNo ,
      sendingCountry.CountryName FaxerCountry ,
      ISNULL(senderInfo.FirstName, '' '') + '' ''  + ISNULL(senderInfo.MiddleName + '' '' , '''') 
	  +ISNULL(senderInfo.LastName, '' '') SenderName ,
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
	      ''VGG''
	      when bankAccountDeposit.Apiservice= 1 then
	      ''TransferZero''
          when bankAccountDeposit.Apiservice= 2 then
	      ''EmergentApi''
	      when bankAccountDeposit.Apiservice= 3 then
	      ''MTN''	 
		  when bankAccountDeposit.Apiservice= 4 then
	      ''Zenith''	
		  else
		  ''Wari''
		  end  ApiService,
	  ISNULL(bankAccountDeposit.TransferReference ,'''') TransferReference ,

      bankAccountDeposit.IsTransactionDuplicated IsDuplicatedTransaction,
      bankAccountDeposit.ReceiptNo DuplicatedTransactionReceiptNo,
       ISNULL(ReInTrans.ReceiptNo,'''') ReInitializedReceiptNo ,
	  ( case when ReInTrans.ReceiptNo IS NULL then 
	  cast( 0 AS BIT )
	   else 
	   cast( 1 AS BIT )
	   end ) IsReInitializedTransaction,
	   ISNULL(ReInTrans.CreatedByName ,'''') ReInitializeStaffName ,
	   ISNULL( convert(varchar,  ReInTrans.Date, 1) ,'''') ReInitializedDateTime ,
      bankAccountDeposit.RecipientId RecipientId ,
      bankAccountDeposit.SendingCountry SenderCountryCode ,
      ISNULL(senderInfo.Email,'''') SenderEmail ,
	  
	   bankAccountDeposit.ReceivingCountry ReceivingCountryCode,
	  (case when bankAccountDeposit.PaidFromModule =0 then
	      ''Sender''
		  when bankAccountDeposit.PaidFromModule =1 then 
		  ''''
		  when bankAccountDeposit.PaidFromModule =2 then 
		  ''''
		   when bankAccountDeposit.PaidFromModule =3 then 
		  ''Agent''
		   when bankAccountDeposit.PaidFromModule =4 then 
		  ''Admin Staff''
		   when bankAccountDeposit.PaidFromModule =5 then 
		   ''''
		  else
		  ''''
	  end) TransactionPerformedBy  
	 from BankAccountDeposit bankAccountDeposit
	 join FaxerInformation  senderInfo  on bankAccountDeposit.SenderId = senderInfo.Id
	 join Country sendingCountry on bankAccountDeposit.SendingCountry = sendingCountry.CountryCode
	 join Country receivingCountry on bankAccountDeposit.ReceivingCountry = receivingCountry.CountryCode
	 left join CardTopUpCreditDebitInformation creditDebitCardInfo on creditDebitCardInfo.TransferType = 4
     And bankAccountDeposit.Id = creditDebitCardInfo.CardTransactionId 
     left join Bank bankInfo on bankAccountDeposit.BankId = bankInfo.Id 
     left join ReinitializeTransaction ReInTrans on bankAccountDeposit.ReceiptNo = ReInTrans.NewReceiptNo
	 where  senderInfo.IsBusiness = ' + CAST( @IsBusiness  as varchar) + '
	)t  where 1=1 ' 
	Begin	
		if @searchString !='' and @searchString is not null 
		Begin 
		select @query = @query +  ' and  TransactionIdentifier = ''' + @searchString+ '''' +
			' OR ' + 'PaymentReference = ''' + @searchString + ''''
		End 
		if @senderId > 0 
			Begin 
			select @query =  @query + ' and senderId = ''' + CAST( @senderId as varchar)  + '''' ;
			
			End
		if  @SendingCountry !=  '' and @SendingCountry is not null
				Begin
				
				select @query =  @query + ' and SenderCountryCode = ''' + @SendingCountry  + '''' ;
				--print(@query)
				End
		if @ReceivingCountry != '' and @ReceivingCountry is not  null 
				--print('ReceivingCountryCode')
				Begin 
				select @query = @query + ' and ReceivingCountryCode = ''' + @ReceivingCountry + '''' ;
				print @mobilequery
				End
		if   @SenderName != '' and @SenderName is not null
				Begin
				set @query = @query + ' and  SenderName like ''%' + @SenderName + '%'' ' ;
				print(@query)
				End
		if  @ReceiverName != '' and @ReceiverName is not null 
				Begin
				select @query = @query + ' and ReceiverName like ''%' + @ReceiverName + '%''' ;
				End

		if  @SendingCurrency != '' and @SendingCurrency is not null 
				Begin
				select @query = @query + ' and SendingCurrency = ''' + @SendingCurrency + '''' ;
				End
		if   @SenderEmail != '' and @SenderEmail is not null
				Begin
				select @query = @query + ' and SenderEmail = ''' + @SenderEmail + '''' ;
				End
		if  @MFCode != '' and @MFCode is not null
				Begin
				select  @query = @query + ' and FaxerAccountNo = ''' + @MFCode + '''' ;
				End
		if  @PhoneNumber != '' and @PhoneNumber is not null
				Begin
				select @query = @query + ' and SenderTelephoneNo = ''' + @PhoneNumber + '''' ;
				End
	End
	exec(@query)

	

--Create table ##mobileMoneyTransfer
select @mobilequery  = 'select * into ##mobileMoneyTransfer from ( select 
	 mobileMoneyTransfer.Id Id,
	 mobileMoneyTransfer.PaidToMobileNo AccountNumber,
     ISNULL( senderInfo.PhoneNumber , '''') SenderTelephoneNo,
     ISNULL( mobileMoneyTransfer.ReceiverName  ,''No Name'') ReceiverName,
     mobileMoneyTransfer.ReceiverCity ReceiverCity ,
     receivingCountry.CountryName   ReceiverCountry  ,
     mobileMoneyTransfer.Fee Fee ,
	case when  mobileMoneyTransfer.Fee > 0 then 
	  cast( 1 AS BIT ) 
	 else
	  cast( 0 AS BIT ) 
	 end  HasFee,
     mobileMoneyTransfer.SendingAmount     GrossAmount ,
	 5 Status,
      mobileMoneyTransfer.Status statusOfMobileWallet,
      mobileMoneyTransfer.Status StatusofMobileTransfer,
     2 StatusOfBankDepoist ,
     	case when mobileMoneyTransfer.Status = 0 then
	      ''Failed''
	      when mobileMoneyTransfer.Status= 1 then
	      ''In Progress''
          when mobileMoneyTransfer.Status= 2 then
	      ''Paid''
	      when mobileMoneyTransfer.Status= 3 then
	      ''Cancelled''
	      when mobileMoneyTransfer.Status= 4 then
	      ''Payment Pending''
	       when mobileMoneyTransfer.Status= 5 then
	      ''In Progress (ID Check)''
	      when mobileMoneyTransfer.Status= 6 then
	      ''In progress''
	      when mobileMoneyTransfer.Status= 7 then
	      ''In progress''
	      when mobileMoneyTransfer.Status= 8 then
	      ''In progress ''
	      when mobileMoneyTransfer.Status= 9 then
	      ''Refund''
          when mobileMoneyTransfer.Status=10 then
	      ''Refund'' 
	      else
	      ''In Progress''
	 end StatusName,
    
	 ''Mobile Wallet'' TransactionType ,
	 '''' ReceivingCurrencySymbol ,
      IsNull(mobileMoneyTransfer.ReceivingCurrency , receivingCountry.Currency) ReceivingCurrrency ,
      IsNull(mobileMoneyTransfer.SendingCurrency , sendingCountry.Currency ) SendingCurrency ,
      '''' SendingCurrencySymbol,
      mobileMoneyTransfer.ReceivingAmount ReceivingAmount ,
      mobileMoneyTransfer.TotalAmount TotalAmount ,
      mobileMoneyTransfer.SenderPaymentMode SenderPaymentMode ,
      IsNUll(creditDebitCardInfo.CardNumber ,'''') CardNumber ,
	  mobileMoneyTransfer.ExchangeRate ExchangeRate ,
	  convert(varchar,  mobileMoneyTransfer.TransactionDate, 106) Date, 
	  1 TransactionServiceType,
	  mobileMoneyTransfer.TransactionDate TransactionDate,
	 ISNULL(mobileMoneyTransfer.PaymentReference ,'''')  Reference,
	 ISNULL(mobileMoneyTransfer.PaymentReference ,'''')  PaymentReference,
	  '''' BankCode,
      '''' WalletName,
	  '''' BankName,
      ISNULL(mobileMoneyTransfer.ReceiptNo ,'''')  TransactionIdentifier ,
      senderInfo.AccountNo FaxerAccountNo ,
      sendingCountry.CountryName FaxerCountry ,
      ISNULL(senderInfo.FirstName, '''') + '' '' + ISNULL(senderInfo.MiddleName+'' '', '''') 
	  +ISNULL(senderInfo.LastName, '' '') SenderName ,
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
	      ''VGG''
	      when mobileMoneyTransfer.Apiservice= 1 then
	      ''TransferZero''
          when mobileMoneyTransfer.Apiservice= 2 then
	      ''EmergentApi''
	      when mobileMoneyTransfer.Apiservice= 3 then
	      ''MTN''	 
		  when mobileMoneyTransfer.Apiservice= 4 then
	      ''Zenith''	
		  else
		  ''Wari''
		  end  ApiService,
       ISNULL(mobileMoneyTransfer.TransferReference ,'''')  TransferReference ,
       cast( 0 AS BIT ) IsDuplicatedTransaction,
      mobileMoneyTransfer.ReceiptNo DuplicatedTransactionReceiptNo,
       ISNULL(ReInTrans.ReceiptNo,'''') ReInitializedReceiptNo ,
	   case when ReInTrans.ReceiptNo IS NULL then 
	    cast( 0 AS BIT )
	   else 
	    cast( 1 AS BIT )
	   end IsReInitializedTransaction,
	   ISNULL(ReInTrans.CreatedByName ,'''') ReInitializeStaffName ,
       ISNULL( convert(varchar,  ReInTrans.Date, 1) ,'''') ReInitializedDateTime ,
	  mobileMoneyTransfer.RecipientId RecipientId ,
      mobileMoneyTransfer.SendingCountry SenderCountryCode ,
      ISNULL(senderInfo.Email,'''') SenderEmail ,
	  mobileMoneyTransfer.ReceivingCountry ReceivingCountryCode,
	  case when mobileMoneyTransfer.PaidFromModule =0 then
	      ''Sender''
		  when mobileMoneyTransfer.PaidFromModule =1 then 
		  ''''
		  when mobileMoneyTransfer.PaidFromModule =2 then 
		  ''''
		   when mobileMoneyTransfer.PaidFromModule =3 then 
		  ''Agent''
		   when mobileMoneyTransfer.PaidFromModule =4 then 
		  ''Admin Staff''
		   when mobileMoneyTransfer.PaidFromModule =5 then 
		   ''''
		  else
		  ''''
	  end TransactionPerformedBy

	 from MobileMoneyTransfer mobileMoneyTransfer
	 join FaxerInformation senderInfo on mobileMoneyTransfer.SenderId = senderInfo.Id
	 join Country sendingCountry on mobileMoneyTransfer.SendingCountry = sendingCountry.CountryCode
	 join Country receivingCountry on mobileMoneyTransfer.ReceivingCountry = receivingCountry.CountryCode
	 left join CardTopUpCreditDebitInformation creditDebitCardInfo on creditDebitCardInfo.TransferType =3
     And mobileMoneyTransfer.Id = creditDebitCardInfo.CardTransactionId 
     left join MobileWalletOperator wallet on mobileMoneyTransfer.WalletOperatorId = wallet.Id 
     left join ReinitializeTransaction ReInTrans on mobileMoneyTransfer.ReceiptNo = ReInTrans.NewReceiptNo
		 where
	 senderInfo.IsBusiness ='  + Cast( @IsBusiness as varchar)  + '
	 )t where 1=1 '

	 	Begin	
		if @searchString !='' and @searchString is not null 
			Begin 
			select @mobilequery = @mobilequery +  ' and  TransactionIdentifier = ''' + @searchString+ '''' +
				' OR ' + 'PaymentReference = ''' + @searchString + ''''
			End 
		if @senderId > 0 
			Begin 
			select @mobilequery =  @mobilequery + ' and senderId = ' + CAST( @senderId as varchar) + ' ' ;
			
			End
		if  @SendingCountry !=  '' and @SendingCountry is not null
				Begin
				
				select @mobilequery =  @mobilequery + ' and SenderCountryCode = ''' + @SendingCountry  + '''' ;
				--print(@query)
				End
		if @ReceivingCountry != '' and @ReceivingCountry is not  null 
				--print('ReceivingCountryCode')
				Begin 
				select @mobilequery = @mobilequery + ' and ReceivingCountryCode = ''' + @ReceivingCountry + '''' ;
				print @mobilequery
				End
		if   @SenderName != '' and @SenderName is not null
				Begin
				set @mobilequery = @mobilequery + ' and  SenderName like ''%' + @SenderName + '%'' ' ;
				
				End
		if  @ReceiverName != '' and @ReceiverName is not null 
				Begin
				select @mobilequery = @mobilequery + ' and ReceiverName like ''%' + @ReceiverName + '%'' ' ;
				End

		if  @SendingCurrency != '' and @SendingCurrency is not null 
				Begin
				select @mobilequery = @mobilequery + ' and SendingCurrency = ''' + @SendingCurrency + '''' ;
				End
		if   @SenderEmail != '' and @SenderEmail is not null
				Begin
				select @mobilequery = @mobilequery + ' and SenderEmail = ''' + @SenderEmail + '''' ;
				End
		if  @MFCode != '' and @MFCode is not null
				Begin
				select  @mobilequery = @mobilequery + ' and FaxerAccountNo = ''' + @MFCode + '''' ;
				End
		if  @PhoneNumber != '' and @PhoneNumber is not null
				Begin
				select @mobilequery = @mobilequery + ' and SenderTelephoneNo = ''' + @PhoneNumber + '''' ;
				End
	End


	 exec(@mobilequery)

	
select @cashquery =  'select * into ##cashPickUp from( 
    select  cashPickup.Id Id,
	 receiverDetails.PhoneNumber AccountNumber,
     receiverDetails.PhoneNumber  SenderTelephoneNo,
     ISNULL( receiverDetails.FullName  ,''No Name'') ReceiverName,
     receiverDetails.City ReceiverCity ,
     receivingCountry.CountryName   ReceiverCountry  ,
     cashPickup.FaxingFee Fee ,
	 case when  cashPickup.FaxingFee > 0 then 
	  cast( 1 AS BIT ) 
	 else
	  cast( 0 AS BIT ) 
	 end  HasFee,
     cashPickup.FaxingAmount  GrossAmount ,
	 cashPickup.FaxingStatus Status,
     0 statusOfMobileWallet,
     0 StatusofMobileTransfer,
     2 StatusOfBankDepoist ,
       case when cashPickup.FaxingStatus = 0 then
	      ''Not Received''
	      when cashPickup.FaxingStatus = 1 then
	      ''Received''
          when cashPickup.FaxingStatus = 2 then
	      ''Cancelled''
	      when cashPickup.FaxingStatus = 3 then
	      ''Refunded''
	      when cashPickup.FaxingStatus = 4 then
	      ''In Progress''
	      when cashPickup.FaxingStatus = 5 then
	      ''Completed''
	      when cashPickup.FaxingStatus = 6 then
	      ''Payment Pending''
	      when cashPickup.FaxingStatus = 7 then
	      ''In Progress (ID Check)''
	      when cashPickup.FaxingStatus = 8 then
	      ''In progress ''
	      when cashPickup.FaxingStatus = 9 then
	      ''Refund''
          when cashPickup.FaxingStatus = 10 then
	      ''Refund''
		  when cashPickup.FaxingStatus = 11 then
	      ''In Progress''  
	      else
	      ''In Progress''
	      end StatusName,
          
	  ''Cash Pickup'' TransactionType ,
	  '''' ReceivingCurrencySymbol,
      IsNull(cashPickup.ReceivingCurrency , receivingCountry.Currency) ReceivingCurrrency ,
      IsNull(cashPickup.SendingCurrency , sendingCountry.Currency ) SendingCurrency ,
	  '''' SendingCurrencySymbol ,
      cashPickup.ReceivingAmount ReceivingAmount ,
      cashPickup.TotalAmount TotalAmount ,
      cashPickup.SenderPaymentMode SenderPaymentMode ,
      IsNUll(creditDebitCardInfo.CardNumber ,'''') CardNumber ,
	  cashPickup.ExchangeRate ExchangeRate ,
	  convert(varchar,  cashPickup.TransactionDate, 106) Date, 
	  5 TransactionServiceType,
	  cashPickup.TransactionDate TransactionDate,
	  ISNULL(cashPickup.PaymentReference ,'''')  Reference,
	  ISNULL(cashPickup.PaymentReference ,'''')  PaymentReference,
	  '''' BankCode,
      '''' WalletName,
	  '''' BankName,
	  ISNULL(cashPickup.ReceiptNumber ,'''')  TransactionIdentifier ,
      senderInfo.AccountNo FaxerAccountNo ,
      sendingCountry.CountryName FaxerCountry ,
      ISNULL(senderInfo.FirstName, '''') + '' '' + 
	  ISNULL(senderInfo.MiddleName+'' '', '''') +ISNULL(senderInfo.LastName, '''') SenderName ,
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
	      ''VGG''
	      when cashPickup.Apiservice= 1 then
	      ''TransferZero''
          when cashPickup.Apiservice= 2 then
	      ''EmergentApi''
	      when cashPickup.Apiservice= 3 then
	      ''MTN''	 
		  when cashPickup.Apiservice= 4 then
	      ''Zenith''	
		  else
		  ''Wari''
		  end  ApiService,
    ISNULL(cashPickup.TransferReference ,'''')  TransferReference ,
     cast( 0 AS BIT ) IsDuplicatedTransaction,
      cashPickup.ReceiptNumber DuplicatedTransactionReceiptNo,
       ISNULL(ReInTrans.ReceiptNo,'''') ReInitializedReceiptNo,
	   case when ReInTrans.ReceiptNo IS NULL then 
	   cast( 0 AS BIT )
	   else 
	   cast( 1 AS BIT )
	   end IsReInitializedTransaction,
	   ISNULL(ReInTrans.CreatedByName ,'''') ReInitializeStaffName ,
	   ISNULL( convert(varchar,  ReInTrans.Date, 1) ,'''') ReInitializedDateTime ,
      cashPickup.RecipientId RecipientId ,
      cashPickup.SendingCountry SenderCountryCode ,
      ISNULL(senderInfo.Email,'''') SenderEmail ,
	  cashPickup.ReceivingCountry ReceivingCountryCode,
	  case when cashPickup.OperatingUserType =0 then
	      ''Sender''
		  when cashPickup.OperatingUserType =1 then 
		  ''''
		  when cashPickup.OperatingUserType =2 then 
		  ''''
		   when cashPickup.OperatingUserType =3 then 
		  ''Agent''
		   when cashPickup.OperatingUserType =4 then 
		  ''Admin Staff''
		   when cashPickup.OperatingUserType =5 then 
		   '' ''
		  else
		  '' ''
	  end TransactionPerformedBy
	 from FaxingNonCardTransaction cashPickup
	 left join ReceiversDetails receiverDetails on cashPickup.NonCardRecieverId = receiverDetails.Id
	 join FaxerInformation senderInfo on cashPickup.SenderId = senderInfo.Id
	 join Country sendingCountry on cashPickup.SendingCountry = sendingCountry.CountryCode
	 join Country receivingCountry on cashPickup.ReceivingCountry = receivingCountry.CountryCode
	 left join CardTopUpCreditDebitInformation creditDebitCardInfo on creditDebitCardInfo.TransferType = 2
     And cashPickup.Id = creditDebitCardInfo.CardTransactionId 
     left join ReinitializeTransaction ReInTrans on cashPickup.ReceiptNumber = ReInTrans.NewReceiptNo
		 where
	 senderInfo.IsBusiness = '   + CAST( @IsBusiness  as varchar) + '
	 )t where 1=1 '
	Begin	
		if @searchString !='' and @searchString is not null 
			Begin 
			select @cashquery = @cashquery +  ' and  TransactionIdentifier = ''' + @searchString+ '''' +
				' OR ' + 'PaymentReference = ''' + @searchString + ''''
			End 
		if @senderId > 0 
			Begin 
			select @cashquery =  @cashquery + ' and senderId = ' + CAST( @senderId as varchar) + ' ' ;
			
			End
		if  @SendingCountry !=  '' and @SendingCountry is not null
				Begin
				
				select @cashquery =  @cashquery + ' and SenderCountryCode = ''' + @SendingCountry  + '''' ;
				--print(@query)
				End
		if @ReceivingCountry != '' and @ReceivingCountry is not  null 
				--print('ReceivingCountryCode')
				Begin 
				select @cashquery = @cashquery + ' and ReceivingCountryCode = ''' + @ReceivingCountry + '''' ;
				End
		if   @SenderName != '' and @SenderName is not null
				Begin
				set @cashquery = @cashquery + ' and  SenderName like ''%' + @SenderName + '%''' ;
				
				End
		if  @ReceiverName != '' and @ReceiverName is not null 
				Begin
				select @cashquery = @cashquery + ' and ReceiverName like ''%' + @ReceiverName + '%'' ' ;
				End

		if  @SendingCurrency != '' and @SendingCurrency is not null 
				Begin
				select @cashquery = @cashquery + ' and SendingCurrency = ''' + @SendingCurrency + '''' ;
				End
		if   @SenderEmail != '' and @SenderEmail is not null
				Begin
				select @cashquery = @cashquery + ' and SenderEmail = ''' + @SenderEmail + '''' ;
				End
		if  @MFCode != '' and @MFCode is not null
				Begin
				select  @cashquery = @cashquery + ' and FaxerAccountNo = ''' + @MFCode + '''' ;
				End
		if  @PhoneNumber != '' and @PhoneNumber is not null
				Begin
				select @cashquery = @cashquery + ' and SenderTelephoneNo = ''' + @PhoneNumber + '''' ;
				End
	End

 exec(@cashquery)



  select * into ##mergeTranscationStatement from(
	   select * from ##bankAccountDeposit
	   union all 
	   select * from ##mobileMoneyTransfer
	   union all 
	   select * from ##cashPickUp
	   )t

		--select * from ##mergeTranscationStatement
		declare @result_query varchar(max);

		select @result_query = ' select *  into ##FilterTranscationStatement  from (
    select *  from ##mergeTranscationStatement s 
	WHERE 1= 1 '
	Begin 
	if @TransactionServiceType !=0 and @TransactionServiceType is not null 
		Begin 
		select @result_query = @result_query  + ' and TransactionServiceType = ' + cast( @TransactionServiceType as varchar)
		End 
	if @Status !='' and @Status is not null 
		Begin 
		select @result_query = @result_query + ' and StatusName like ''%' + @Status+ '%'''
		End 
	if @SearchByStatus !='' and @SearchByStatus is not null 
		Begin 
		select @result_query = @result_query + ' and StatusName like ''%' + @SearchByStatus + '%'''
		
		End 
	if @ResponsiblePerson !='' and @ResponsiblePerson is not null 
		Begin 
		select @result_query = @result_query +  ' and TransactionPerformedBy = ''' + @ResponsiblePerson+ ''''
		End 
	if @searchString !='' and @searchString is not null 
		Begin 
		select @result_query = @result_query +  ' and  TransactionIdentifier = ''' + @searchString+ '''' +
			' OR ' + 'PaymentReference = ''' + @searchString + ''''
		End 
	if @TransactionWithAndWithoutFee !='' and @TransactionWithAndWithoutFee is not null 
		Begin 
		select @result_query = @result_query +  ' and  HasFee = '+ @TransactionWithAndWithoutFee
		End 
	End
	
	Begin
	select @result_query = @result_query + ' ) t'
		print @result_query
	End
	exec(@result_query)
	   set @count =( select COUNT(*) from ##FilterTranscationStatement)
	   select * , @count TotalCount  from ##FilterTranscationStatement
	   order by TransactionDate desc
	   OFFSET  (@PageNum-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
	   
	 --   drop table ##bankAccountDeposit
	 --   drop table ##mobileMoneyTransfer
		--drop table ##cashPickUp
		--drop table ##mergeTranscationStatement
		--drop table ##FilterTranscationStatement
	
  
	END

