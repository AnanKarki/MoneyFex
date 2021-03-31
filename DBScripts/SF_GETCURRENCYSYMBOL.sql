Alter FUNCTION dbo.SF_GetCurrencySymbol(
    @CountryCode varchar(5)
)
RETURNS Varchar(5)
AS 
BEGIN
	Declare @CurrencySymbol  varchar(5);
	set @CurrencySymbol = ( select top 1 CurrencySymbol from Country where Currency = @CountryCode)
    
	--print(SF_GetCurrencySymbol)
	return @CurrencySymbol
END;



select  dbo.SF_GetCurrencySymbol(Null)