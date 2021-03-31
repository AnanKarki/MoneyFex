using System;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.MoblieTransferApi.Models;
using PaymentGateway;
using FAXER.PORTAL.Services;
using FAXER.PORTAL.MoblieTransferApi;
using TransferZero.Sdk.Api;
using TransferZero.Sdk.Model;
using System.Collections.Generic;

namespace MoneyFax.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var outPut = FAXER.PORTAL.Common.Common.GenerateRandomDigit(10);
        }
        [TestMethod]
        public void TestValidatePassword()
        {
            var output = FAXER.PORTAL.Common.Common.ValidatePassword("abcd");
        }

        [TestMethod]
        public void TestHashPassword()
        {
            var output = FAXER.PORTAL.Common.Common.ToHash("Money18%&");
        }


        [TestMethod]

        public void TestSmsApi()
        {

            var api = new FAXER.PORTAL.Common.SmsApi();
            var message = api.GetCashToCashTransferMessage("Hello ", "123456", "$500", "$0.05", "$250");


            var lenth = message.Length;
            //string msg = string.Format("{0}{1}{2}{3}{4}{5}", "Hello Prince,\n", "Test message from MoneyFex,\n", "Please confirm,\n", "Thanks\n", "IT support Team\n", "MoneyFex\n");

            //api.SendSMS("+447440395950", "Hello Test Sms");

            List<string> phonenos = new List<string>();
            //NotifyUserSMSVm user1 = new NotifyUserSMSVm() { 
            //binding_type = "sms",
            //Address = 
            //};
            phonenos.Add("{\"binding_type\":\"sms\",\"address\":\"+9779818548590\"}");
            phonenos.Add("{\"binding_type\":\"sms\",\"address\":\"+9779866496303\"}");
            phonenos.Add("{\"binding_type\":\"sms\",\"address\":\"+9779865714436\"}");

            api.SendBulkSMS(phonenos, "Hello Test Sms");

            //api.SendSMS("+447946457670", msg);
            //api.SendSMS("+237650615123", msg);
            //api.SendSMS("+233243446345,", msg);


        }

        [TestMethod]

        public void TestAccountLookUpApi()
        {

            var api = new FAXER.PORTAL.Common.SmsApi();

            api.IsValidMobileNo("22999090901");


        }

        [TestMethod]

        public void TestRefundTransaction()
        {

            //var result = StripServices.RefundTransaction("BD372595" , 0);

        }


        [TestMethod]
        public void TestRandomNumber()
        {

            string[] r = new string[50];
            for (int i = 0; i < 50; i++)
            {
                r[i] = Common.GenerateRandomDigit(6);
            }
            string a = "Hello World";

        }

        [TestMethod]
        public void TestAgentCommission()
        {

            decimal x = 200; // Sending Amount  
            decimal y = 20;  // Agent Fee percent
            decimal z = 3; // Sending Fee percent

            decimal TotalFee = (z / 100) * x;

            //Agent Rate 
            var a = (y / 100) * TotalFee;

            var b = (a / 100) * x;


        }

        [TestMethod]
        public void TimeSpan()
        {

            var time = "20:00";
            var time2 = "00:29";
            var time3 = "03:01";
            var testdata = new TimeSpan();
            testdata = time.ToTimeSpan();
        }
        [TestMethod]
        public void TestPaymentPlan()
        {

            decimal Amount = 500000M;
            decimal OutStandingAmount = 500000M;
            int InstallentNo = 10;
            decimal Interest = 10;
            decimal[] installmentAmount = new decimal[10];
            for (int i = 0; i < InstallentNo; i++)
            {
                var val = (OutStandingAmount * (Interest / 100)) / (InstallentNo - i);
                OutStandingAmount = OutStandingAmount - val;
                installmentAmount[i] = val;

            }

            decimal sum = 0;
            for (int i = 0; i < installmentAmount.Length; i++)
            {
                sum = sum + installmentAmount[i];
            }

        }

        [TestMethod]
        public void Decrypt()
        {

            //string password = "Sk2HpjsgPnbYQzyKi3Vzrw==";
            string password = "x+5pclg6K4W533NLz22aYA==";

            var output = Common.Decrypt(password);



            var pass = Common.Encrypt("rIddh@s0ft");

        }
        [TestMethod]

        public void TestDb() { 
        
            //Serv
            //this.RunTest(session => {

            //    UserBankAccount userBank = new UserBankAccount();
            //    session.
            //})
        }

        private void RunTest(Func<object, object> p)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void BankApiTest()
        {

            SenderInformationVm Sendervm = new SenderInformationVm()
            {

                senderFirstname = "Dhruv",
                senderLastname = "Patel",
                senderEmail = "ayfadipe@gmail.com",
                senderAddress = "17 dudyemi street",
                senderCity = "Ikeja",
                senderCountry = "GB",
                senderPhoneNumber = "08062362436",
                state = "Lagos",
                id_type = "PAS",
                id_number = "P12345678",
                IdentificationIssuedDate = "2009-12-06",
                IdentificationExpiryDate = "2019-10-05"

            };

            BeneficiaryInformationVm beneficiaryInformationVm = new BeneficiaryInformationVm()
            {
                city = "Test",
                address = "17 dudyemi street",
                country = "AT",
                lastname = "Doe",
                firstname = "John",
                msisdn = "08062362436",
                state = "Test",
                email = "ayfadipe@gmail.com",
                beneficiaryMobileOperator = "MTN",
                beneficiaryBankRoutingNumber = "300304",
                BeneficiaryBankAccountNumber = "6031010249361"



            };

            CreditParty_IdentifierVm creditParty_IdentifierVm = new CreditParty_IdentifierVm()
            {

                bankName = "ErsteGroup Bank",
                msisdn = "",
                bank_account_number = "AT611904300234573201",
                iban = "",
                swift_bic_code = "",
                Account_Name = "",
                payer_id = "",
                bankAddress = "",
                bankCity = "",
                bankCountry = "",
                bankBranch = ""

            };
            BankDepositRequestViewModel model = new BankDepositRequestViewModel()
            {
                source_amount = 4,
                payment_amount = 4,
                source_currency = "GBP",
                payment_currency = "EUR",
                external_id = "ACCe0COdfhh4frIIJhhD4kiookjffe5970ff663dd",
                serviceType = "Bank",
                beneficiary = beneficiaryInformationVm,
                CreditParty_Identifier = creditParty_IdentifierVm,
                sender = Sendervm
            };

            //BankDepositLocalRequest bankDepositLocalRequest = new BankDepositLocalRequest()
            //{
            //    partnerTransactionReference = "SWV123254478412",
            //    baseCurrencyCode = "GBP",
            //    targetCurrencyCode = "NGN",
            //    baseCurrencyAmount = 200,
            //    targetCurrencyAmount = 200,
            //    partnerCode = "3000",
            //    purpose = "za cos tam",
            //    accountNumber = "0110412853",
            //    bankCode = "058",
            //    baseCountryCode = "3",
            //    targetCountryCode = "3",
            //    payerName = "VGG",
            //    payermobile = "08057362245"
            //};
            BankDepositLocalRequest bankDepositLocalRequest = new BankDepositLocalRequest()
            {
                partnerTransactionReference = "BD3516771202",
                baseCurrencyCode = "GBP",
                targetCurrencyCode = "NGN",
                baseCurrencyAmount = 1M,
                targetCurrencyAmount = 1M,
                partnerCode = null,
                purpose = "",
                accountNumber = "0110412853",
                bankCode = "058",
                baseCountryCode = "GB",
                targetCountryCode = "NG",
                payerName = "MoneyFex",
                payermobile = "9073665146"
            };

            //BankDepositLocalRequest bankDepositLocalRequest = new BankDepositLocalRequest()
            //{
            //    partnerTransactionReference = "BD1234564",
            //    baseCurrencyCode = "GBP",
            //    targetCurrencyCode = "NGN",
            //    baseCurrencyAmount = 2.0M,
            //    targetCurrencyAmount = 953.2M‬,
            //    partnerCode = "3000",
            //    purpose = "za cos tam",
            //    accountNumber = "0110412853",
            //    bankCode = "058",
            //    baseCountryCode = "3",
            //    targetCountryCode = "3",
            //    payerName = "MoneyFex",
            //    payermobile = "08057362245"
            //};
            BankDepositApi api = new BankDepositApi();
            var accessToken = api.Login<AccessTokenVM>();
            //var validateAccountNo = api.ValidateAccountNo<AccountNoLookUpResponse>(
            //    bankDepositLocalRequest.bankCode, bankDepositLocalRequest.accountNumber, accessToken.Result);
            //var transaction = api.Post<BankDepositResponseResult>(api.SerializeObject(bankDepositLocalRequest), accessToken.Result);

            //var refrence = "";

            var a = api.TransactionConfirmationResult<BankDepositResponseVm>("SWVFGAIAY5SVUQAMQNN9IHFDG", "BD682190", accessToken.Result);



        }


        [TestMethod]
        public void SecurePaymentMethod()
        {

            string[] vs = new string[1];
            vs[0] = "ACCOUNTCHECK";


            //StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            //{
            //    CardName = "Anan",
            //    ExpirationMonth = "12",
            //    ExpiringYear = "20",
            //    Number = "4111111111111111",
            //    SecurityCode = "123"
            //};

            // StripServices.IsValidCardNo(stripeResultIsValidCardVm);

            SecureTradingApiRequestVm secureTradingApiRequestVm = new SecureTradingApiRequestVm()
            {
                requesttypedescriptions = vs,
                baseamount = "100",
                currencyiso3a = "GBP",
                expirydate = "12/2020",
                orderreference = "123assddd456",
                pan = "4111110000000211",
                securitycode = "123",
                billingpostcode = "4000",
                billingpremise = "123"
            };

            SecureTradingApiRequestParam secureTradingApiRequestParam = new SecureTradingApiRequestParam()
            {
                request = secureTradingApiRequestVm
            };
            WebServices webServices = new WebServices();



            var result = webServices.PostTransaction<SecureTradingApiResponseVm>(webServices.SerializeObject<SecureTradingApiRequestParam>(secureTradingApiRequestParam));

            var fafsad = "1231";
        }




        [TestMethod]
        public void ThreedQueryMethod()
        {


            //StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            //{
            //    CardName = "Anan",
            //    ExpirationMonth = "12",
            //    ExpiringYear = "20",
            //    Number = "4111111111111111",
            //    SecurityCode = "123"
            //};

            // StripServices.IsValidCardNo(stripeResultIsValidCardVm);

            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {

                CardName = "Anan karki",
                ExpirationMonth = "12",
                ExpiringYear = "20",
                Number = "4111110000000211",
                SecurityCode = "123"
            };



            //var result = StripServices.CreateThreedQuery(stripeResultIsValidCardVm);


            var fafsad = "1231";
        }


        [TestMethod]
        public void MobileTransferTest()
        {


            FAXER.PORTAL.MoblieTransferApi.MobileTransferApi mobileTransferApiServices = new FAXER.PORTAL.MoblieTransferApi.MobileTransferApi();

            //var data = mobileTransferApiServices.Login<FAXER.PORTAL.MoblieTransferApi.Models.MobileTransferAccessTokeneResponse>();
            //            {
            //                "amount": "string",
            //  "currency": "string",
            //  "externalId": "string",
            //  "payee": {
            //                    "partyIdType": "MSISDN",
            //    "partyId": "string"
            //  },
            //  "payerMessage": "string",
            //  "payeeNote": "string"
            //}

            PayeeInfo payeeInfo = new PayeeInfo()
            {
                partyIdType = "MSISDN",
                //partyId = "46733123454",
                partyId = "237654725558",

            };

            //675106128
            //650324548
            //674031293
            //673075483

            MTNCameroonRequestParamVm model = new MTNCameroonRequestParamVm()
            {
                amount = "10",
                currency = "XAF",
                externalId = "201900250102",
                payerMessage = "Transfer",
                payeeNote = "Transfer",
                payee = payeeInfo
            };

            MobileTransferApiConfigurationVm configurationVm = new MobileTransferApiConfigurationVm()
            {
                apirefId = Guid.NewGuid().ToString(),
                apiKey = "",
                apiUrl = "",
                subscriptionKey = "ce6a18001f164a33825f3100491b749b"
            };

            CreateApiUserVm vm = new CreateApiUserVm();
            //mobileTransferApiServices.CreateApiUser(mobileTransferApiServices.SerializeObject<CreateApiUserVm>(vm), configurationVm.apiUserId);

            //var apiUserKey = mobileTransferApiServices.CreateApiKey<MobileTransferApiConfigurationVm>(configurationVm.apiUserId);
            //configurationVm.apiKey = apiUserKey.Result.apiKey;

            var accesstoken = mobileTransferApiServices.Login<MobileTransferAccessTokeneResponse>(configurationVm);

            MobileTransferAccessTokeneResponse tokenModel = new MobileTransferAccessTokeneResponse();
            tokenModel = accesstoken.Result;
            tokenModel.apirefId = configurationVm.apirefId;
            tokenModel.apiKey = configurationVm.apiKey;
            tokenModel.apiUrl = "";
            tokenModel.subscriptionKey = configurationVm.subscriptionKey;

            var postTransaction = mobileTransferApiServices.Post<MTNCameroonResponseParamVm>(mobileTransferApiServices.SerializeObject<MTNCameroonRequestParamVm>(model), tokenModel);
            var transactionStatus = mobileTransferApiServices.GetTransactionStatus<MTNCameroonResponseParamVm>(configurationVm.apirefId, accesstoken.Result.access_token);



        }


        [TestMethod]
        public void TestApi()
        {

            FAXER.PORTAL.MoblieTransferApi.MobileTransferApi mobileTransferApiServices = new FAXER.PORTAL.MoblieTransferApi.MobileTransferApi();
            var result = mobileTransferApiServices.PostTest<USerInfoVM>();

        }
        [TestMethod]
        public void AccountLookUpTest()
        {

            FAXER.PORTAL.MoblieTransferApi.MobileTransferApi mobileTransferApiServices = new FAXER.PORTAL.MoblieTransferApi.MobileTransferApi();

            PayeeInfo payeeInfo = new PayeeInfo()
            {
                partyIdType = "MSISDN",
                //partyId = "46733123454",
                partyId = "237654725558",

            };
            MTNCameroonRequestParamVm model = new MTNCameroonRequestParamVm()
            {
                amount = "500.33",
                currency = "XAF",
                externalId = "201903010802",
                payerMessage = "Hello World",
                payeeNote = "Test Payment",
                payee = payeeInfo
            };

            MobileTransferApiConfigurationVm configurationVm = new MobileTransferApiConfigurationVm()
            {
                apirefId = Guid.NewGuid().ToString(),
                apiKey = "",
                apiUrl = "",
                subscriptionKey = "daee07935c5d4750a2bae57e2e2550de"
            };

            CreateApiUserVm vm = new CreateApiUserVm();
            var accesstoken = mobileTransferApiServices.Login<MobileTransferAccessTokeneResponse>(configurationVm);

            MobileTransferAccessTokeneResponse tokenModel = new MobileTransferAccessTokeneResponse();
            tokenModel = accesstoken.Result;
            tokenModel.apirefId = configurationVm.apirefId;
            tokenModel.apiKey = configurationVm.apiKey;
            tokenModel.apiUrl = "";
            tokenModel.subscriptionKey = configurationVm.subscriptionKey;


            var result = mobileTransferApiServices.ValidateMobileNo<MobileNoLookUpResponse>(payeeInfo.partyIdType, payeeInfo.partyId, tokenModel.access_token);
        }

        [TestMethod]
        public void CallBackTest()
        {

            BankDepositApi bankDepositApi = new BankDepositApi();


            bankDepositApi.BankDepositGetStatusCallBack("SWVDFDVFCHGX0OUVCKFIQA47Q", "BD072742");

        }


        [TestMethod]
        public void CallBackTestT()
        {

            Transact365Api bankDepositApi = new Transact365Api();


            bankDepositApi.GetTransaction("https://gateway.transact365.com/transactions/72852558-18146a5cc4");

        }

        

        [TestMethod]
        public void GetMobileTransferTransactionStatus()
        {

            FAXER.PORTAL.MoblieTransferApi.MobileTransferApi mobileTransferApiServices = new FAXER.PORTAL.MoblieTransferApi.MobileTransferApi();

            MobileTransferApiConfigurationVm configurationVm = new MobileTransferApiConfigurationVm()
            {
                apirefId = "49c18257-7176-4a63-a2a3-de110c204d11",
                apiKey = "",
                apiUrl = "",
                subscriptionKey = "ce6a18001f164a33825f3100491b749b"
            };
            var accesstoken = mobileTransferApiServices.Login<MobileTransferAccessTokeneResponse>(configurationVm);

            var transactionStatus = mobileTransferApiServices.GetTransactionStatus<MTNCameroonResponseParamVm>(configurationVm.apirefId, accesstoken.Result.access_token);

        }


        [TestMethod]
        public void ATLBakApiTest()
        {

            BankDepositApi bankDepositApi = new BankDepositApi();


            CustomerVm customerVm = new CustomerVm()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "1980-09-01",
                Nationality = "GB",
                BirthNationality = "GB",
                Address = "128 Peckham Hill Street",
                City = "London",
                Region = "England",
                PostCode = "SE15 5JT",
                MobileNumber = "1234567890",
                EmailAddress = "support@atlmoneytransfer.com"
            };
            RecipientVm recipientVm = new RecipientVm()
            {
                Type = "IND",
                Relation = "BIZ",
                FirstName = "Richard",
                LastName = "Amoah",
                Address = "FreeTown",
                Region = "FreeTown",
                MobileNumber = "12345698"
            };
            TransactionVm transactionVm = new TransactionVm()
            {
                FromCountry = "GB",
                FromCurrency = "GBP",
                SendAmount = 500,
                ToCountry = "SL",
                ToCurrency = "SLL",
                PayoutAmount = 5000,
                PayoutMethod = "CP",
                PayoutPartner = "BCXSL",
                ThirdPartyReference = "OW125452",
                Purpose = "Test Payment",
                Customer = customerVm,
                Recipient = recipientVm
            };

            var login = bankDepositApi.LoginATLBank<AccessTokenVM>();

            var result = bankDepositApi.PostATLTransaction<TransactionVm>(CommonExtension.SerializeObject<TransactionVm>(transactionVm), login.Result);

        }


        [TestMethod]
        public void ThreedCreateTransactionTesT()
        {

        }

        [TestMethod]
        public void AdLoginTest()
        {

            MobileTransferApi api = new MobileTransferApi();
            NICAsiaLogin loginModel = new NICAsiaLogin()
            {
                Group = "rs",
                Name = "admin",
                Password = "Adm!n123"
            };

            var result = api.AdLogin<LoginResult>("http://192.168.1.154:10452/UIEH/KYC/GetApiKey", api.SerializeObject<NICAsiaLogin>(loginModel));
            var a = result.Result;



        }

        [TestMethod]
        public void TestTransferZeroApi()
        {
            TransferZeroApi transferZeroApi = new TransferZeroApi();

            Sender sender = new Sender(
                     firstName: "Jane",
                     lastName: "Doe",
                     phoneCountry: "GB",
                     phoneNumber: "07440395950",
                     country: "GB",
                     city: "Accra",
                     street: "1 La Rd",
                     postalCode: "GA100",
                     addressDescription: "",
                     birthDate: DateTime.Parse("1974-12-24"),
                     // you can usually use your company's contact email address here
                     email: "info@transferzero.com",
                     //gender: "M",
                     externalId: Guid.NewGuid().ToString(),
                     // you'll need to set these fields but usually you can leave them the default
                     ip: "127.0.0.1",
                     documents: new List<Document>()
            );
            PayoutMethodDetails details = new PayoutMethodDetails(
                 firstName: "Maiga",
                 lastName: "Abdoualmoutali",
                 //mobileProvider: PayoutMethodMobileProviderEnum.Orange,
                 phoneNumber: "212537718685"
                 //bankAccount: "123456789",
                 //bankCode: "030100",
                 //bankAccountType: PayoutMethodBankAccountTypeEnum._20
                 //               "reason" => "Remittance payment",
                 //// Optional; Default value is 'Remittance payment'
                 //"identity_card_type" => "NI",
                 //// Optional; Values: "PP": Passport, "NI": National ID
                 //"identity_card_id" => 'AB12345678'
                 );

            PayoutMethod payout = new PayoutMethod(
              type: "XOF::Cash",
              details: details);

            Recipient recipient = new Recipient(
              requestedAmount: 1445.02M,
              requestedCurrency: "XOF",
              payoutMethod: payout);


            Transaction transaction = new Transaction(
              sender: sender,
              recipients: new List<Recipient>() { recipient },
              inputCurrency: "GBP",
              externalId: Guid.NewGuid().ToString());
            TransactionRequest aa = new TransactionRequest(
                transaction: transaction);

            //BankDepositApi bankDepositApi = new BankDepositApi();
            //var md= bankDepositApi.SerializeObject<ValidateBankAndMobileAccountModel>(validateBankAndMobile);
            //transferZeroApi.ValidateAccountNo("");
            transferZeroApi.CreateTransaction(aa);

        }
        [TestMethod]
        public void TestTransfer()
        {

            WebhooksApi webhooks;

        }

        [TestMethod]

        public void EmergentApiTest()
        {

            EmergentApi emergentApi = new EmergentApi();
            var id = Guid.NewGuid().ToString();
            EmergentApiRequestParamModel paramModel = new EmergentApiRequestParamModel()
            {
                app_id = "2452014221",
                app_key = "41030994",
                transaction_date = "/Date(" + DateTime.Now.ToString("yyyy-MM-dd") + ")/",
                expiry_date = "/Date(" + DateTime.Now.ToString("yyyy-MM-dd") + ")/",
                transaction_type = "local",
                payment_mode = "BAT",
                payee_name = "MoneyFex Payee",
                payee_email = "email@moneyfex.com",
                trans_currency = "GHS",
                trans_amount = 1.11M,
                merch_trans_ref_no = id,
                payee_mobile = "+233551212121",
                recipient_mobile = "+233551212121",
                recipient_name = "MoneyFex",
                bank_branch_sort_code = "280100",
                bank_account_no = "123456789",
                bank_account_title = "MoneyFex",
                bank_name = "Access Bank"
            };
            var result = emergentApi.CreateTransaction<EmergentApiTransactionResponseModel>(CommonExtension.SerializeObject(paramModel));

            EmergentApiGetTransactionStatusParamModel statusModel = new EmergentApiGetTransactionStatusParamModel()
            {

                app_id = "2452014221",
                app_key = "41030994",
                merch_trans_ref_no = id
            };
            var data = emergentApi.GetTransactionStatus<EmergentApiGetStatusResponseModel>(CommonExtension.SerializeObject(paramModel));



        }

        [TestMethod]
        public void TestExcel()
        {

            List<EmergentApiRequestParamModel> arr = new List<EmergentApiRequestParamModel>();
            for (int i = 0; i < 10; i++)
            {

                EmergentApiRequestParamModel paramModel = new EmergentApiRequestParamModel()
                {
                    app_id = "2452014221",
                    app_key = "41030994",
                    transaction_date = "/Date(" + DateTime.Now.ToString("yyyy-MM-dd") + ")/",
                    expiry_date = "/Date(" + DateTime.Now.ToString("yyyy-MM-dd") + ")/",
                    transaction_type = "local",
                    payment_mode = "BAT",
                    payee_name = "MoneyFex Payee",
                    payee_email = "email@moneyfex.com",
                    trans_currency = "GHS",
                    trans_amount = 1.11M,
                    merch_trans_ref_no = "1",
                    payee_mobile = "+233551212121",
                    recipient_mobile = "+233551212121",
                    recipient_name = "MoneyFex",
                    bank_branch_sort_code = "280100",
                    bank_account_no = "123456789",
                    bank_account_title = "MoneyFex",
                    bank_name = "Access Bank"
                };
                arr.Add(paramModel);
            }
            FAXER.PORTAL.Common.Utilities utilities = new FAXER.PORTAL.Common.Utilities();
            //utilities.CreateExcelWorkSheet(arr, 18);


        }


        [TestMethod]
        public void TestSouthAfricaBankDeposit()
        {

            TransferZeroApi transferZeroApi = new TransferZeroApi();


            //BankDepositApi bankDepositApi = new BankDepositApi();
            //var md= bankDepositApi.SerializeObject<ValidateBankAndMobileAccountModel>(validateBankAndMobile);
            //transferZeroApi.ValidateAccountNo("");

            Sender sender = new Sender(
                     firstName: "Jane",
                     lastName: "Doe",

                     phoneCountry: "GB",
                     phoneNumber: "07440395950",

                     country: "GB",
                     city: "Accra",
                     street: "1 La Rd",
                     postalCode: "GA100",
                     addressDescription: "",

                     birthDate: DateTime.Parse("1974-12-24"),

                     // you can usually use your company's contact email address here
                     email: "info@transferzero.com",

                     externalId: Guid.NewGuid().ToString(),

                     // you'll need to set these fields but usually you can leave them the default
                     ip: "127.0.0.1",
                     documents: new List<Document>()
            );
            PayoutMethodDetails details = new PayoutMethodDetails(
                 firstName: "John",
                 lastName: "Deo",
                 phoneNumber: "302123456",
                 bankName: "BRM",
                 iban: "SN08SN0000000000000000000000",
                 bankCountry: "SN"
                 );
            details.Reason = "183";
            PayoutMethod payout = new PayoutMethod(
              type: "XOF::Bank",
              details: details);

            Recipient recipient = new Recipient(
              requestedAmount: 10,
              requestedCurrency: "XOF",
              payoutMethod: payout);


            Transaction transaction = new Transaction(
              sender: sender,
              recipients: new List<Recipient>() { recipient },
              inputCurrency: "XOF",
              externalId: Guid.NewGuid().ToString());
            TransactionRequest aa = new TransactionRequest(
                transaction: transaction);


            transferZeroApi.CreateTransaction(new TransferZero.Sdk.Model.TransactionRequest());
        }
        [TestMethod]
        public void CashPot()
        {

            CashPotApi cashPotApi = new CashPotApi();
            RateGenericRequest rateGeneric = new RateGenericRequest()
            {
                USER = "moneyfex",
                PASSWORD = "_8=WHx^Ksa64hZZB",
                PARTNER_ID = "MONEYFEX",
                RECEIVING_COUNTRY = "NG",
                RECEIVING_CURENCY = "USD",
                SENDING_COUNTRY = "GB",
                SENDING_CURRENCY = "USD",
                AMOUNT = "5",
                TRANS_TYPE_ID = "2"
            };

            var rateResponse = cashPotApi.GetRates(rateGeneric);

            //post Transaction
            SendTransGenericRequestVm sendTransGenericRequest = new SendTransGenericRequestVm()
            {
                USER = "moneyfex",
                PASSWORD = "_8=WHx^Ksa64hZZB",
                PARTNER_ID = "MONEYFEX",
                REFERENCE_CODE = "BD871259",
                DATE = "2021-01-13 07:34:01",
                AGENT = null,
                TRANSSTATUS = "1",
                SENDING_CURRENCY = "USD",
                RECEIVER_CURENCY = "USD",
                RATE = "1.00000000000000",
                //RATE = rateResponse.RATE,
                FEE = null,
                RECEIVING_AMOUNT = "5.00",
                SEND_AMOUNT = "5.00",
                SENDER_USER_NAME = "",
                SENDER_FIRST_NAME = "Louis",
                SENDER_LAST_NAME = "Anekeguh",
                SENDER_OCCUPATION = null,
                SENDER_ADDRESS = "Apartment 10 113 Newton Street",
                SENDER_COUNTRY = "GB",
                SENDER_MOBILE = "48600123123",
                LOCATION_ID = null,
                PAYER_ID = "10252",
                RECEIVER_TYPE = "01",
                COMPANYNAME = null,
                RECEIVER_FIRST_NAME = "Louis",
                RECEIVER_LAST_NAME = "Smith",
                RECEIVER_PHONE_NUMBER = "234555645879",
                RECEIVER_MOBILE_NUMBER = "234555645879",
                LOCAL_AMOUNT = null,
                RECEIVER_COUNTRY = "NG",
                TRANSACTION_TYPE = "2",
                RECEIVER_CITY = "citytt",
                RECEIVER_ZIP = "",
                RECEIVER_STATE = "",
                RECEIVER_ADDRESS = "address",
                RECEIVER_BANK_NAME = "Zenith Bank",
                RECEIVER_BANK_SORT = "",
                RECEIVER_BANK_CODE = "057",
                RECEIVER_BRANCH_CODE = "057",
                RECEIVER_BRANCH_ADDRESS = "",
                RECEIVER_BANK_ACCOUNT_TITLE = "GLOBAL EMARKET NIGERIA LIMITED",
                RECEIVER_BANK_ACCOUNT_NO = "5071728453",
                RECEIVER_BANK_ACCOUNT_TYPE = "",
                RECEIVER_BANK_ROUTING = "",
                RECEIVER_BANK_SWIFT = "",
                RECEIVER_BANK_IBAN = "",
                TRANSACTION_PURPOSE = "",
                TRANSACTION_DETAILS = "",
                SECRET_QUESTION = "What is your pickup reference number ? ",
                SECRET_ANSWER = "BD871259",
                SENDER_ID_NUMBER = "",
                SENDER_ID_ISSUE_DATE = "",
                SENDER_ID_EXPIRY_DATE = "",
                SENDER_DOB = "",
                SENDER_POST_CODE = "",
                SITE_LOCATION = ""
            };
            var response = cashPotApi.PostTransaction(sendTransGenericRequest);
            // Get Transaction Status

            //cancelTransaction
            //CancelTransactionRequest cancelTransactionRequest = new CancelTransactionRequest()
            //{
            //    USER = "moneyfex",
            //    PASSWORD = "money123",
            //    PARTNER_ID = "MONEYFEX",
            //    REFERENCE_CODE = "BD871258"
            //};
            //var cancelResponse  =cashPotApi.CancelTransaction(cancelTransactionRequest);
            ////update Transaction status  
            //UpdateStatusRequest updateStatusRequest = new UpdateStatusRequest()
            //{
            //    USER = "moneyfex",
            //    PASSWORD = "money123",
            //    PARTNER_ID = "MONEYFEX",
            //    REFERENCE_CODE = "BD871258",
            //    TRANS_STATUS_ID = "1"
            //};
            //var updateResponse = cashPotApi.UpdateTransaction(updateStatusRequest);

        }

        [TestMethod]
        public void GetCashpotTransactionStatus()
        {

            CashPotApi cashPotApi = new CashPotApi();
            TransactionStatusRequest statusRequest = new TransactionStatusRequest()
            {
                USER = "moneyfex",
                PASSWORD = "_8=WHx^Ksa64hZZB",
                PARTNER_ID = "MONEYFEX",
                REFERENCE_CODE = "BD871259"
            };
            var statusResponse = cashPotApi.GetTransactionStatus(statusRequest);
        }

        [TestMethod]
        public void CreateDbContext()
        {
            SCountry country = new SCountry();
            var data = country.GetCountry();
        }

    }

    public class ValidateBankAndMobileAccountModel
    {

        public string bank_account { get; set; }
        public string bank_code { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public string method { get; set; }








    }
    public class NICAsiaLogin
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class LoginResult
    {

        public string Data { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}


