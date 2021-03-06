USE [MoneyFexDB]
GO
/****** Object:  StoredProcedure [dbo].[SP_GETSENDERDOCUMENTDETAILS]    Script Date: 3/17/2021 1:27:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_GETSENDERDOCUMENTDETAILS]
	-- Add the parameters for the stored procedure here
	 @Country nvarchar(30) = '',
	 @City nvarchar(30)  = '',
	 @SenderName nvarchar(30) = '',
	 @AccountNo varchar(30) = '',
	 @Telephone varchar(10) = '',
     @Status int  = 3,
	 @email nvarchar(30) = '',
	 @PageNumber int= 1,
	 @PageSize int = 30

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

declare @documentQuery varchar(Max);
	Declare @count int = 0 ;

	select @documentQuery = 'select * into ##documents  from(
	SELECT 
	c.CountryName  Country, 
	doc.Country CountryCode, 
	doc.CreatedDate CreatedDate,
	ISNULL(doc.AccountNo , '''') AccountNo,
	doc.Id Id,
	doc.SenderId SenderId,
	ISNULL( doc.City , '''') City,
	doc.CreatedBy CreatedBy,
	doc.DocumentExpires DocumentExpires,
	doc.DocumentName DocumentName,
	staff.FirstName StaffFirstName,
	staff.MiddleName StaffMiddleName,
	staff.LastName   StaffLastName,
	doc.ExpiryDate ExpiryDate, 
	doc.DocumentPhotoUrl DocumentPhotoUrl ,
	 doc.DocumentType DocumentType,
	ISNULL( sender.FirstName , '''') SenderFirstName ,
	ISNULL( sender.MiddleName , '''') SenderMiddleName ,  
	ISNULL( sender.LastName , '''') SenderLastName,
	ISNULL( sender.FirstName+ '' '' , '''')
	+ISNULL( sender.MiddleName+'' '' , '''') + 
	ISNULL( sender.LastName , '''')  SenderFullName,
	doc.Status Status ,
	case when doc.Status = 0 then
	''Approved''  
	when doc.Status= 1 then
	''Disapproved''
	else
	''In progress''
	end StatusName,
	case when doc.Status = 0 then
	3
	when doc.Status= 1 then
	2
	else
	1
	end OrderBy,
	
	ISNULL( sender.PhoneNumber , '''') TelephoneNo  , 
	ISNULL( sender.Email  ,'''') SenderEmailAddress
	FROM SenderBusinessDocumentation doc
	join FaxerInformation sender on doc.SenderId = sender.Id
	left join StaffInformation staff on staff.Id = doc.CreatedBy
	join Country c on c.CountryCode = doc.Country
	where 1 = 1
	 '
	Begin 
	if @City != '' and @City is not null 
		Begin
		select @documentQuery = @documentQuery + ' and doc.City = ''' + @City + ''''  
		End
	if @Country != '' and @Country is not null 
		Begin
		select @documentQuery = @documentQuery + ' and doc.Country = ''' + @Country + ''''  
		End
	if @SenderName != '' and @SenderName is not null 
		Begin
		select @documentQuery = @documentQuery + ' and (sender.FirstName + '''' + IsNUll(sender.MiddleName + '' '' , '''') + sender.LastName) like ''%' + @SenderName + '%'''  
		End
	if @AccountNo != '' and @AccountNo is not null 
		Begin
		select @documentQuery = @documentQuery + ' and doc.AccountNo = ''' + @AccountNo  + ''''  
		End
	if @Telephone != '' and @Telephone is not null 
		Begin
		select @documentQuery = @documentQuery + ' and sender.PhoneNumber = ''' + @Telephone  + ''''  
		End
	if @Status != 3 and @Status is not null 
		Begin
		select @documentQuery = @documentQuery + ' and doc.Status = ' + cast( @Status  as varchar) + ''  
		print(@documentQuery)
		End
	if @email != '' and @email is not null 
		Begin
		select @documentQuery = @documentQuery + ' and sender.Email = ''' + @email  + ''''  
		End
	End
	 Begin
	select @documentQuery = @documentQuery + ' ) t'
	End
exec(@documentQuery)

 set @count =( select COUNT(*) from ##documents)
		   
	select  * , @count TotalCount from ##documents
 		order by OrderBy , CreatedDate desc  
	OFFSET  (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

--drop table ##documents
END
