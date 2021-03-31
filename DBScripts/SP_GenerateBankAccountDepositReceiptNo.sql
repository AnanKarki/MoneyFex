USE [MoneyFexDB]
GO
/****** Object:  StoredProcedure [dbo].[SP_GenerateBankAccountDepositReceiptNo]    Script Date: 12/2/2020 2:27:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Alter PROCEDURE [dbo].[SP_GenerateBankAccountDepositReceiptNo] 
	-- Add the parameters for the stored procedure here
	@IsManualDeposit bit 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	

       DECLARE @rc INT = 0,
	           @alias varchar(3) = (case when @IsManualDeposit = 1 then 'MBD' else  'BD' end) , 
         @CustomerID Varchar(10) = ABS(CHECKSUM(NEWID())) % 1000000 + 1;
                     -- or ABS(CONVERT(INT,CRYPT_GEN_RANDOM(3))) % 1000000 + 1;
                     -- or CONVERT(INT, RAND() * 1000000) + 1;
       
       WHILE @rc = 0
       BEGIN
         IF NOT EXISTS (SELECT 1 FROM dbo.BankAccountDeposit WHERE ReceiptNo =  'BD' + @CustomerID)
         BEGIN
           --INSERT dbo.Customers(CustomerID) SELECT @CustomerID;
           SET @rc = 1; 
         END
         ELSE
         BEGIN
           SELECT @CustomerID = ABS(CHECKSUM(NEWID())) % 1000000 + 1,
             @rc = 0;
         END
         select 'BD' + @CustomerID as ReceiptNo

END

END
