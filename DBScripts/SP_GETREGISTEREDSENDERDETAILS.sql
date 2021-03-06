USE [MoneyFexDB]
GO
/****** Object:  StoredProcedure [dbo].[SP_GETREGISTEREDSENDERDETAILS]    Script Date: 3/17/2021 1:28:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_GETREGISTEREDSENDERDETAILS]
	-- Add the parameters for the stored procedure here
	 @Country nvarchar(10) ='GB',
	 @City nvarchar(30) ='',
	 @SenderName nvarchar(30) ='',
	 @AccountNo nvarchar(30) = '',
	 @Address nvarchar(30) = '',
     @Telephone nvarchar(30) = '',
	 @Status nvarchar(30) ='',
	 @Email nvarchar(30) = '',
	 @DateRange nvarchar(30) = '',
	 @FromDate nvarchar(30) ='',
	 @ToDate nvarchar(30) = '',
	 @PageNumber int =1,
	 @PageSize int =30
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @count int = 0 ;
declare @senderQuery varchar(max);
	select @senderQuery = 'select * into ##registeredSenders from(
	select  
	doc.Id Id,
	ISNULL( doc.FirstName , '''')  FirstName,
	ISNULL( doc.MiddleName , '''') MiddleName,
	ISNULL( doc.LastName , '''')  LastName,
	ISNULL( doc.FirstName+ '' '' , '''')+
	ISNULL( doc.MiddleName+'' '' , '''') 
	+ ISNULL( doc.LastName , '''')  FullName,
	ISNULL( doc.City , '''') City,
	ISNULL(country.CountryName , '''')  Country,
	doc.Country CountryCode,
	ISNULL(doc.Address1 , '''')  Address1,
	ISNULL(doc.Address2 , '''')  Address2,
	doc.PostalCode PostalCode,
	doc.DateOfBirth DateOfBirth,
	doc.GGender GGender,
	doc.IdCardExpiringDate IDCardExpDate,
	doc.IDCardNumber IDCardNumber,
	doc.IDCardType IDCardType, 
	doc.IssuingCountry IssuingCountry,
	country.CountryPhoneCode CountryPhoneCode ,
	doc.PhoneNumber Phone,
	doc.State State , 
	ISNULL(doc.Email , '''') UsernameEmail, 
	isnuLL( senderLogin.LoginFailedCount , 0) LoginFailedCount,
	case when senderLogin.IsActive = 0 then
	''Inactive''  
	when senderLogin.IsActive = 1 then
	''Active''
	else
	''Inactive''
	end AccountStatusName,
	case when senderLogin.IsActive = 0 then
	cast( 0 AS BIT )  
	when senderLogin.IsActive = 1 then
	cast( 1 AS BIT )
	else
 	cast( 0 AS BIT )
	end AccountStatus ,
    cast(0 as  bit) IsFromBusiness,
	cast(0 as  bit) Confirm,
	cast(0 as bit ) TransactionOver1000,
	doc.AccountNo  MFAccountNo,
	ISNULL(doc.CreatedDate, '''') CreatedDate
	from FaxerInformation doc
	left join FaxerLogin senderLogin on doc.Id = senderLogin.FaxerId
	join Country country on doc.Country= country.CountryCode
	)t  where 1=1 '
	Begin
	if @City != ''  and @City is not null 
		Begin
		select @senderQuery = @senderQuery + ' and City = ''' + @City + '''' 
		End
	if @Country != ''  and @Country is not null 
		Begin
		select @senderQuery = @senderQuery + ' and CountryCode = ''' + @Country + '''' 
		End
	if @SenderName != ''  and @SenderName is not null 
		Begin
		select @senderQuery = @senderQuery + ' and FullName like ''%' + @SenderName + '%''' 
		End
	if @AccountNo != ''  and @AccountNo is not null 
		Begin
		select @senderQuery = @senderQuery + ' and MFAccountNo = ''' + @AccountNo + '''' 
		End
	if @Telephone != ''  and @Telephone is not null 
		Begin
		select @senderQuery = @senderQuery + ' and Phone = ''' + @Telephone + '''' 
		End
	if @email != ''  and @email is not null 
		Begin
		select @senderQuery = @senderQuery + ' and UsernameEmail = ''' + @email + '''' 
		End
	if @Status != ''  and @Status is not null 
		Begin
		select @senderQuery = @senderQuery + ' and AccountStatusName = ''' + @Status + '''' 
		print @senderQuery
		End
	End
exec(@senderQuery)
	 set @count =( select COUNT(*) from ##registeredSenders)
	
	select  * , @count TotalCount from ##registeredSenders 
	order by CreatedDate desc  
	OFFSET  (@PageNumber-1)*@PageSize  ROWS FETCH NEXT @PageSize ROWS ONLY;
	--drop table ##registeredSenders
	--drop table #registeredSenders
	--drop table #registeredSenders
END
