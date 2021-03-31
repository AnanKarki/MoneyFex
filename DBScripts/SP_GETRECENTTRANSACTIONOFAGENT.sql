-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Alter PROCEDURE [dbo].[SP_GETRECENTTRANSACTIONOFAGENT]	
-- Add the parameters for the stored procedure here
        @SendingCountry nvarchar(10),
        @ReceivingCountry nvarchar(10),
		@FromDate nvarchar(30),
	    @ToDate nvarchar(30),
		@Status nvarchar(30),
		@PayingStaffId int, 
	    @SenderName nvarchar(30),
	    @ReceiverName nvarchar(30),
	    @ReceiptNo nvarchar(30),
		@PageNum  int,
        @PageSize int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @count int = 0 ;
  
    -- Insert statements for procedure here
	
 --  
  select * into #bank from( select 
   bankAccountDeposit.Id Id,
   4 TransactionType ,
   6 TransactionServiceType,
   'Bank Account Deposit' TransactionTypeName ,
   sendingCountry.CurrencySymbol CurrencySymbol,
   bankAccountDeposit.SendingAmount Amount ,
   bankAccountDeposit.Fee Fee ,
   bankAccountDeposit.ReceiptNo TransactionIdentifier ,
   bankAccountDeposit.TransactionDate DateAndTime ,
   bankAccountDeposit.PayingStaffName StaffName ,
   bankAccountDeposit.AgentCommission AgentCommission,
   1 Type ,
   bankAccountDeposit.ReceivingCountry  ReceivingCountry,
   receivingCountry.CountryName  ReceivingCountryName,
   bankAccountDeposit.SenderId SenderId ,
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
	 end StatusName ,
   bankAccountDeposit.ReceiverName ReceiverName,
   ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, '') SenderName ,
   ISNULL( convert(varchar,  bankAccountDeposit.TransactionDate, 1) ,'')  FormatedDate,
 
  bankAccountDeposit.PayingStaffId PayingStaffId,
  bankAccountDeposit.SendingCountry SendingCountry,
 sendingCountry.CountryName SendingCountryName,
  bankAccountDeposit.TotalAmount TotalAmount,
  bankAccountDeposit.RecipientId RecipientId,
  case when bankAccountDeposit.IsComplianceApproved=1 then
  cast( 0 AS BIT )
      else
	  bankAccountDeposit.IsComplianceNeededForTrans
     end IsAwaitForApproval,
agentInfo.AgentMFSCode	 AgentAccountNo,
	sendingCountry.Currency SendingCurrency

 from BankAccountDeposit bankAccountDeposit
 join FaxerInformation senderInfo on bankAccountDeposit.SenderId = senderInfo.Id
 join AgentStaffInformation agentInfo on bankAccountDeposit.PayingStaffId = agentInfo.Id
 join Country sendingCountry on bankAccountDeposit.SendingCountry = sendingCountry.CountryCode
 join Country receivingCountry on bankAccountDeposit.ReceivingCountry = receivingCountry.CountryCode
 Where
 bankAccountDeposit.PayingStaffId != 0	 
 
 AND PayingStaffId =(case when @PayingStaffId  is null Or @PayingStaffId =0 then 
		            PayingStaffId
					else
					@PayingStaffId
					end)
 AND ReceiptNo=(case when @ReceiptNo  is null OR @ReceiptNo ='' then
	                   ReceiptNo 
					   else 
					   @ReceiptNo 
					   end)
 AND Convert( varchar(10) ,TransactionDate , 111 ) >= (case when  @FromDate is null OR @FromDate ='' then
	                    Convert( varchar(10) ,TransactionDate , 111 )
					   else 
					   Convert( varchar(10) ,@FromDate , 111 )
                        
					   end)
 AND Convert( varchar(10) ,TransactionDate , 111 ) <= (case when  @ToDate is null OR @ToDate =''then
	                   Convert( varchar(10) ,TransactionDate , 111 )
					   else 
                        
						Convert( varchar(10) ,@ToDate , 111 )
					   end)
 AND SendingCountry=(case when @SendingCountry is null OR @ReceivingCountry ='' then
	                   SendingCountry 
					   else 
					   @SendingCountry
					   end) 
 AND ReceivingCountry=(case when @ReceivingCountry is null OR @ReceivingCountry ='' then
	                   ReceivingCountry 
					   else 
					   @ReceivingCountry
					   end) 
 AND 
 (ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, ''))
					   LIKE (case when @SenderName is null OR @SenderName ='' then
	                   (ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, ''))
					   else 
					   '%' + @SenderName + '%' 
					   end)
 AND
 ReceiverName LIKE (case when @ReceiverName is null OR @ReceiverName ='' then
	                   ReceiverName 
					   else 
					   '%' + @ReceiverName + '%' 
					   end)					   
 )t   
  
   select * into #OtherWallet from( select 
   otherWallet.Id Id,
    3 TransactionType ,
  1 TransactionServiceType,
   
   'Mobile Money Transfer' TransactionTypeName ,
    sendingCountry.CurrencySymbol CurrencySymbol,
   otherWallet.SendingAmount Amount ,
   otherWallet.Fee Fee ,
   otherWallet.ReceiptNo TransactionIdentifier ,
   otherWallet.TransactionDate DateAndTime ,
   otherWallet.PayingStaffName StaffName ,
   otherWallet.AgentCommission AgentCommission,
   1 Type ,
   otherWallet.ReceivingCountry  ReceivingCountry ,
   receivingCountry.CountryName  ReceivingCountryName,
   otherWallet.SenderId SenderId ,
  case when otherWallet.Status = 0 then
	      'Failed'
	      when otherWallet.Status= 1 then
	      'In Progress'
          when otherWallet.Status= 2 then
	      'Paid'
	      when otherWallet.Status= 3 then
	      'Cancelled'
	      when otherWallet.Status= 4 then
	      'Payment Pending'
	       when otherWallet.Status= 5 then
	      'In Progress (ID Check)'
	      when otherWallet.Status= 6 then
	      'In progress'
	      when otherWallet.Status= 7 then
	      'In progress'
	      when otherWallet.Status= 8 then
	      'In progress '
	      when otherWallet.Status= 9 then
	      'Refund'
          when otherWallet.Status=10 then
	      'Refund' 
	      else
	      'In Progress'
	 end StatusName,
   otherWallet.ReceiverName ReceiverName,
   ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, '') SenderName ,
   ISNULL( convert(varchar,  otherWallet.TransactionDate, 1) ,'')  FormatedDate,
    otherWallet.PayingStaffId PayingStaffId,
  otherWallet.SendingCountry SendingCountry,
  sendingCountry.CountryName SendingCountryName,
  otherWallet.TotalAmount TotalAmount,
  otherWallet.RecipientId RecipientId,
  case when otherWallet.IsComplianceApproved=1 then
  cast( 0 AS BIT )
      else
	  otherWallet.IsComplianceNeededForTrans
     end IsAwaitForApproval,
	 agentInfo.AgentMFSCode	 AgentAccountNo,
	sendingCountry.Currency SendingCurrency


 from MobileMoneyTransfer otherWallet
 join FaxerInformation senderInfo on otherWallet.SenderId = senderInfo.Id
 join AgentStaffInformation agentInfo on otherWallet.PayingStaffId = agentInfo.Id
 join Country sendingCountry on otherWallet.SendingCountry = sendingCountry.CountryCode
 join Country receivingCountry on otherWallet.ReceivingCountry = receivingCountry.CountryCode
 Where otherWallet.PayingStaffId != 0
  AND PayingStaffId =(case when @PayingStaffId  is null Or @PayingStaffId =0 then 
		            PayingStaffId
					else
					@PayingStaffId
					end)

 AND ReceiptNo=(case when @ReceiptNo  is null OR @ReceiptNo ='' then
	                   ReceiptNo 
					   else 
					   @ReceiptNo 
					   end)
 AND Convert( varchar(10) ,TransactionDate , 111 ) >= (case when  @FromDate is null OR @FromDate ='' then
	                    Convert( varchar(10) ,TransactionDate , 111 )
					   else 
					   Convert( varchar(10) ,@FromDate , 111 )
                        
					   end)
 AND Convert( varchar(10) ,TransactionDate , 111 ) <= (case when  @ToDate is null OR @ToDate =''then
	                   Convert( varchar(10) ,TransactionDate , 111 )
					   else 
                        
						Convert( varchar(10) ,@ToDate , 111 )
					   end) 
 AND ReceivingCountry=(case when @ReceivingCountry is null OR @ReceivingCountry ='' then
	                   ReceivingCountry 
					   else 
					   @ReceivingCountry
					   end)
AND 
 (ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, ''))
					   LIKE (case when @SenderName is null OR @SenderName ='' then
	                   (ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, ''))
					   else 
					   '%' + @SenderName + '%' 
					   end)
 AND
 ReceiverName LIKE (case when @ReceiverName is null OR @ReceiverName ='' then
	                   ReceiverName 
					   else 
					   '%' + @ReceiverName + '%' 
					   end)		
)t   

   select * into #cashPickUp from( select 
   cashPickup.Id Id,
 1 TransactionType ,
  5 TransactionServiceType,
   'Cash Pick up' TransactionTypeName ,
   sendingCountry.CurrencySymbol CurrencySymbol,
   cashPickup.FaxingAmount Amount ,
   cashPickup.FaxingFee Fee ,
   cashPickup.ReceiptNumber TransactionIdentifier ,
   cashPickup.TransactionDate DateAndTime ,
    ISNULL(agentStaffInfo.FirstName , '') + ' ' + ISNULL(agentStaffInfo.MiddleName+' ', '') +ISNULL(agentStaffInfo.LastName, '')StaffName ,
   cashPickup.AgentCommission AgentCommission,
   1 Type ,
   cashPickup.ReceivingCountry  ReceivingCountry ,
   receivingCountry.CountryName  ReceivingCountryName,
   cashPickup.SenderId SenderId ,
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
          
   receiverDetails.ReceiverName ReceiverName,
   ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, '') SenderName ,
   ISNULL( convert(varchar,  cashPickup.TransactionDate, 1) ,'')  FormatedDate,
    cashPickup.PayingStaffId PayingStaffId,
  cashPickup.SendingCountry SendingCountry,
  sendingCountry.CountryName SendingCountryName,
  cashPickup.TotalAmount TotalAmount,
  cashPickup.RecipientId RecipientId,
  case when cashPickup.IsComplianceApproved=1 then
  cast( 0 AS BIT )
      else
	  cashPickup.IsComplianceNeededForTrans
     end IsAwaitForApproval,
	 agentStaffInfo.AgentMFSCode	 AgentAccountNo,
	sendingCountry.Currency SendingCurrency

 from FaxingNonCardTransaction cashPickup
 join FaxerInformation senderInfo on cashPickup.SenderId = senderInfo.Id
 join AgentStaffInformation agentStaffInfo on cashPickup.PayingStaffId = agentStaffInfo.Id
 join Recipients receiverDetails on cashPickup.RecipientId = receiverDetails.Id
 join Country sendingCountry on cashPickup.SendingCountry = sendingCountry.CountryCode
 join Country receivingCountry on cashPickup.ReceivingCountry = receivingCountry.CountryCode
 Where 
 cashPickup.PayingStaffId != 0
  AND PayingStaffId =(case when @PayingStaffId  is null Or @PayingStaffId =0 then 
		            PayingStaffId
					else
					@PayingStaffId
					end)
 AND ReceiptNumber=(case when @ReceiptNo  is null OR @ReceiptNo ='' then
	                   ReceiptNumber 
					   else 
					   @ReceiptNo 
					   end)
 AND Convert( varchar(10) ,TransactionDate , 111 ) >= (case when  @FromDate is null OR @FromDate ='' then
	                    Convert( varchar(10) ,TransactionDate , 111 )
					   else 
					   Convert( varchar(10) ,@FromDate , 111 )
                        
					   end)
 AND Convert( varchar(10) ,TransactionDate , 111 ) <= (case when  @ToDate is null OR @ToDate =''then
	                   Convert( varchar(10) ,TransactionDate , 111 )
					   else 
                        
						Convert( varchar(10) ,@ToDate , 111 )
					   end) 
AND ReceivingCountry=(case when @ReceivingCountry is null OR @ReceivingCountry ='' then
	                   ReceivingCountry 
					   else 
					   @ReceivingCountry
					   end)
 AND 
 (ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, ''))
					   LIKE (case when @SenderName is null OR @SenderName ='' then
	                   (ISNULL(senderInfo.FirstName, '') + ' ' + ISNULL(senderInfo.MiddleName+' ', '') +ISNULL(senderInfo.LastName, ''))
					   else 
					   '%' + @SenderName + '%' 
					   end)
 AND
 ReceiverName LIKE (case when @ReceiverName is null OR @ReceiverName ='' then
	                   ReceiverName 
					   else 
					   '%' + @ReceiverName + '%' 
					   end)		
)t   
  

  select * into #mergeTranscationStatement from(
	   select * from #bank
	   union all 
	   select * from #OtherWallet
	   union all 
	   select * from #cashPickUp
	   )t
   select * into #filteredTransactionStatement from (
    select * from #mergeTranscationStatement s where 

	  StatusName LIKE (case when @Status is null OR @Status ='' then
	                   StatusName 
					   else 
					   '%' + @Status + '%' 
					   end)
	)t
	   set @count =( select COUNT(*) from #filteredTransactionStatement)
	   select * , @count TotalCount  from #filteredTransactionStatement

	   order by DateAndTime desc
	   OFFSET  (@PageNum-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

END
GO
EXEC [dbo].[SP_GETRECENTTRANSACTIONOFAGENT]	
        @SendingCountry ='',
        @ReceivingCountry ='',
		@FromDate ='',
	    @ToDate ='',
		@Status ='',
		@PayingStaffId =0 , 
	    @SenderName ='',
	    @ReceiverName ='',
	    @ReceiptNo ='',
	    @PageNum =1 ,
		@PageSize = 10
