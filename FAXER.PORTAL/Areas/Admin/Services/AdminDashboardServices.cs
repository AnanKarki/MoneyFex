using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AdminDashboardServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();


        public AdminDashboardViewModel GetCounts(string Country = "", string City = "", string Year = "", int Month = 0)
        {
            if (!string.IsNullOrEmpty(City))
            {
                City = City.Trim().ToLower();
            }

            int intYear = 0;
            if (!string.IsNullOrEmpty(Year))
            {
                intYear = int.Parse(Year);
            }

            string currency = "";




            var totalAgentsQuery = dbContext.AgentInformation.Where(x => x.IsDeleted == false);
            var totalBusinessMerchantsQuery = dbContext.KiiPayBusinessLogin.Where(x => x.IsDeleted == false);
            var totalSendersQuery = dbContext.FaxerInformation.Where(x => x.IsDeleted == false);
            var totalCardUsersQuery = dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false);
            var totalNonCardUsersQuery = dbContext.ReceiversDetails.Where(x => x.IsDeleted == false);

            var amountNotWithdrawnFromMFTCQuery1 = dbContext.FaxingCardTransaction.Where(x => x.FaxingStatus != FaxingStatus.Cancel && x.FaxingStatus != FaxingStatus.Refund);
            var amountNotWithdrawnFromMFTCQuery3 = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.KiiPayPersonalWalletId != 0);
            var amountNotWithdrawnFromMFTCQuery2 = dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformationId != 0);

            var amountNotWithdrawnFromMFBCQuery1 = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessWalletInformationId != 0);
            var amountNotWithdrawnFromMFBCQuery2 = dbContext.FaxerMerchantPaymentTransaction.Where(x => x.SenderKiiPayBusinessPaymentInformationId != 0);
            var amountNotWithdrawnFromMFBCQuery3 = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId != 0);
            var amountNotWithdrawnFromMFBCQuery4 = dbContext.MFBCCardWithdrawls.Where(x => x.KiiPayBusinessWalletInformationId != 0);

            var totalLocalTransactionQuery = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessWalletInformationId != 0);
            var totalLocalTransactionQuery1 = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId != 0);

            var totalMerchantPaymentsQuery1 = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessWalletInformationId != 0);
            var totalMerchantPaymentsQuery2 = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId != 0);
            var totalMerchantPaymentsQuery3 = dbContext.FaxerMerchantPaymentTransaction.Where(x => x.SenderKiiPayBusinessPaymentInformationId != 0);

            var totalStaffsQuery = dbContext.StaffLogin.Where(x => x.IsDeleted == false);

            var totalFeesPaidQuery1 = dbContext.FaxingCardTransaction.Where(x => x.FaxerId != 0);
            var totalFeesPaidQuery2 = dbContext.FaxingNonCardTransaction.Where(x => x.NonCardRecieverId != 0);
            var totalFeesPaidQuery3 = dbContext.FaxerMerchantPaymentTransaction.Where(x => x.SenderKiiPayBusinessPaymentInformationId != 0);
            var totalFeesPaidQuery4 = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.KiiPayPersonalWalletId != 0);
            //var totalWebVisitorsQuery = 
            var totalRefundRequestQuery = dbContext.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.Refund);

            var totalCompletedRefundQuery1 = dbContext.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.Refund);
            var totalCompletedRefundQuery2 = dbContext.RefundOnDeletedMFTCCard.Where(x => x.MFTCCardNumber != null);
            var totalCompletedRefundQuery3 = dbContext.RefundOnDeletedMFBCCard.Where(x => x.MFBCCardNumber != null);

            var totalUncompletedRefundsQuery1 = dbContext.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.Cancel);
            var totalUncompletedRefundsQuery2 = dbContext.DeletedMFTCCards.Where(x => x.MoblieNumber != null); //dbContext.MFTCCardInformation.Where(x=>x.IsDeleted == true);
            var totalUncompletedRefundsQuery3 = dbContext.DeletedMFBCCards.Where(x => x.MFBCCardNumber != null); //dbContext.MFBCCardInformation.Where(x => x.CardStatus == CardStatus.IsDeleted);

            var totalUpdateOnTransferInfoQuery1 = dbContext.FaxingUpdatedInformation.Where(x => x.NonCardTransactionId != 0);
            var totalUpdateOnTransferInfoQuery2 = dbContext.FaxingUpdatedInformationByAdmin.Where(x => x.NonCardTransactionId != 0);

            var totalCommissionPaidQuery = dbContext.AgentCommissionPayment.Where(x => x.IsDeleted == false);
            //var totalCommissionUnpaidQuery = dbContext
            //var totalFailedTransactionsQuery = dbContext
            var totalDeletedStaffsQuery = dbContext.StaffLogin.Where(x => x.IsDeleted == true);
            var totalDeletedSendersQuery = dbContext.FaxerInformation.Where(x => x.IsDeleted == true);
            var totalDeletedCardUsersQuery = dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == true);
            var totalDeletedBusinessMerchantsQuery = dbContext.KiiPayBusinessLogin.Where(x => x.IsDeleted == true);
            var totalDeletedAgentsQuery = dbContext.AgentInformation.Where(x => x.IsDeleted == true);
            var totalDeletedNonCardUsersQuery = dbContext.ReceiversDetails.Where(x => x.IsDeleted == true);


            if (string.IsNullOrEmpty(Country) == false)
            {
                currency = CommonService.getCurrencyCodeFromCountry(Country);
                totalAgentsQuery = totalAgentsQuery.Where(x => x.CountryCode == Country);
                totalDeletedAgentsQuery = totalDeletedAgentsQuery.Where(x => x.CountryCode == Country);
                totalSendersQuery = totalSendersQuery.Where(x => x.Country == Country);
                totalDeletedSendersQuery = totalDeletedSendersQuery.Where(x => x.Country == Country);
                totalCardUsersQuery = totalCardUsersQuery.Where(x => x.CardUserCountry == Country);
                totalDeletedCardUsersQuery = totalDeletedCardUsersQuery.Where(x => x.CardUserCountry == Country);
                totalNonCardUsersQuery = totalNonCardUsersQuery.Where(x => x.Country == Country);
                totalDeletedNonCardUsersQuery = totalDeletedNonCardUsersQuery.Where(x => x.Country == Country);
                totalBusinessMerchantsQuery = totalBusinessMerchantsQuery.Where(x => x.KiiPayBusinessInformation.BusinessOperationCountryCode == Country);
                totalDeletedBusinessMerchantsQuery = totalDeletedBusinessMerchantsQuery.Where(x => x.KiiPayBusinessInformation.BusinessOperationCountryCode == Country);
                totalStaffsQuery = totalStaffsQuery.Where(x => x.Staff.Country == Country);
                totalDeletedStaffsQuery = totalDeletedStaffsQuery.Where(x => x.Staff.Country == Country);
                totalLocalTransactionQuery = totalLocalTransactionQuery.Where(x => x.PayedToKiiPayBusinessWalletInformation.Country == Country);
                totalLocalTransactionQuery1 = totalLocalTransactionQuery1.Where(x => x.KiiPayBusinessWalletInformation.Country == Country);
                amountNotWithdrawnFromMFTCQuery1 = amountNotWithdrawnFromMFTCQuery1.Where(x => x.KiiPayPersonalWalletInformation.CardUserCountry == Country);
                amountNotWithdrawnFromMFTCQuery2 = amountNotWithdrawnFromMFTCQuery2.Where(x => x.KiiPayPersonalWalletInformation.CardUserCountry == Country);
                amountNotWithdrawnFromMFTCQuery3 = amountNotWithdrawnFromMFTCQuery3.Where(x => x.KiiPayPersonalWalletInformation.CardUserCountry == Country);
                amountNotWithdrawnFromMFBCQuery1 = amountNotWithdrawnFromMFBCQuery1.Where(x => x.PayedToKiiPayBusinessWalletInformation.Country == Country);
                amountNotWithdrawnFromMFBCQuery2 = amountNotWithdrawnFromMFBCQuery2.Where(x => x.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessOperationCountryCode == Country);
                amountNotWithdrawnFromMFBCQuery3 = amountNotWithdrawnFromMFBCQuery3.Where(x => x.KiiPayBusinessWalletInformation.Country == Country);
                amountNotWithdrawnFromMFBCQuery4 = amountNotWithdrawnFromMFBCQuery4.Where(x => x.KiiPayBusinessWalletInformation.Country == Country);
                totalRefundRequestQuery = totalRefundRequestQuery.Where(x => x.NonCardReciever.Country == Country);
                totalMerchantPaymentsQuery1 = totalMerchantPaymentsQuery1.Where(x => x.PayedToKiiPayBusinessWalletInformation.Country == Country);
                totalMerchantPaymentsQuery2 = totalMerchantPaymentsQuery2.Where(x => x.KiiPayBusinessWalletInformation.Country == Country);
                totalMerchantPaymentsQuery3 = totalMerchantPaymentsQuery3.Where(x => x.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessOperationCountryCode == Country);
                totalFeesPaidQuery2 = totalFeesPaidQuery2.Where(x => x.NonCardReciever.FaxerInformation.Country == Country);
                totalFeesPaidQuery3 = totalFeesPaidQuery3.Where(x => x.SenderKiiPayBusinessPaymentInformation.SenderInformation.Country == Country);
                var faxerCountryQuery = dbContext.FaxerInformation.Where(x => x.Country == Country).Select(x => x.Id);
                totalFeesPaidQuery4 = totalFeesPaidQuery4.Where(x => faxerCountryQuery.Contains(x.FaxerId));

                totalCompletedRefundQuery1 = totalCompletedRefundQuery1.Where(x => x.NonCardReciever.Country == Country);
                totalCompletedRefundQuery2 = totalCompletedRefundQuery2.Where(x => x.Faxer.Country == Country);
                totalCompletedRefundQuery3 = totalCompletedRefundQuery3.Where(x => x.Business.BusinessOperationCountryCode == Country);
                totalUncompletedRefundsQuery1 = totalUncompletedRefundsQuery1.Where(x => x.NonCardReciever.Country == Country);
                var countryQuery = dbContext.KiiPayPersonalWalletInformation.Where(x => x.CardUserCountry == Country).Select(x => x.MobileNo);
                totalUncompletedRefundsQuery2 = totalUncompletedRefundsQuery2.Where(x => countryQuery.Contains(x.MoblieNumber));
                var mfbcCountryQuery = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Country == Country).Select(x => x.MobileNo);
                totalUncompletedRefundsQuery3 = totalUncompletedRefundsQuery3.Where(x => mfbcCountryQuery.Contains(x.MFBCCardNumber));

                totalUpdateOnTransferInfoQuery1 = totalUpdateOnTransferInfoQuery1.Where(x => x.NonCardTransaction.NonCardReciever.Country == Country);
                totalUpdateOnTransferInfoQuery2 = totalUpdateOnTransferInfoQuery2.Where(x => x.NonCardTransaction.NonCardReciever.Country == Country);
                totalCommissionPaidQuery = totalCommissionPaidQuery.Where(x => x.Agent.CountryCode == Country);


            }
            if (string.IsNullOrEmpty(City) == false)
            {
                totalAgentsQuery = totalAgentsQuery.Where(x => x.City.ToLower() == City);
                totalDeletedAgentsQuery = totalDeletedAgentsQuery.Where(x => x.City.ToLower() == City);
                totalSendersQuery = totalSendersQuery.Where(x => x.City.ToLower() == City);
                totalDeletedSendersQuery = totalDeletedSendersQuery.Where(x => x.City.ToLower() == City);
                totalCardUsersQuery = totalCardUsersQuery.Where(x => x.CardUserCity.ToLower() == City);
                totalDeletedCardUsersQuery = totalDeletedCardUsersQuery.Where(x => City.ToLower() == City);
                totalNonCardUsersQuery = totalNonCardUsersQuery.Where(x => x.City.ToLower() == City);
                totalDeletedNonCardUsersQuery = totalDeletedNonCardUsersQuery.Where(x => x.City.ToLower() == City);
                totalBusinessMerchantsQuery = totalBusinessMerchantsQuery.Where(x => x.KiiPayBusinessInformation.BusinessOperationCity.ToLower() == City);
                totalDeletedBusinessMerchantsQuery = totalDeletedBusinessMerchantsQuery.Where(x => x.KiiPayBusinessInformation.BusinessOperationCity.ToLower() == City);
                totalStaffsQuery = totalStaffsQuery.Where(x => x.Staff.City.ToLower() == City);
                totalDeletedStaffsQuery = totalDeletedStaffsQuery.Where(x => x.Staff.City.ToLower() == City);
                totalLocalTransactionQuery = totalLocalTransactionQuery.Where(x => x.PayedFromKiiPayBusinessWalletInformation.City.ToLower() == City);
                totalLocalTransactionQuery1 = totalLocalTransactionQuery1.Where(x => x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City);
                amountNotWithdrawnFromMFTCQuery1 = amountNotWithdrawnFromMFTCQuery1.Where(x => x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City);
                amountNotWithdrawnFromMFTCQuery2 = amountNotWithdrawnFromMFTCQuery2.Where(x => x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City);
                amountNotWithdrawnFromMFTCQuery3 = amountNotWithdrawnFromMFTCQuery3.Where(x => x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City);
                amountNotWithdrawnFromMFBCQuery1 = amountNotWithdrawnFromMFBCQuery1.Where(x => x.PayedToKiiPayBusinessWalletInformation.City.ToLower() == City);
                amountNotWithdrawnFromMFBCQuery2 = amountNotWithdrawnFromMFBCQuery2.Where(x => x.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessOperationCity.ToLower() == City);
                amountNotWithdrawnFromMFBCQuery3 = amountNotWithdrawnFromMFBCQuery3.Where(x => x.KiiPayBusinessWalletInformation.City.ToLower() == City);
                amountNotWithdrawnFromMFBCQuery4 = amountNotWithdrawnFromMFBCQuery4.Where(x => x.KiiPayBusinessWalletInformation.City.ToLower() == City);
                totalRefundRequestQuery = totalRefundRequestQuery.Where(x => x.NonCardReciever.City.ToLower() == City);
                totalMerchantPaymentsQuery1 = totalMerchantPaymentsQuery1.Where(x => x.PayedToKiiPayBusinessWalletInformation.City.ToLower() == City);
                totalMerchantPaymentsQuery2 = totalMerchantPaymentsQuery2.Where(x => x.KiiPayBusinessWalletInformation.City.ToLower() == City);
                totalMerchantPaymentsQuery3 = totalMerchantPaymentsQuery3.Where(x => x.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessOperationCity.ToLower() == City);
                //totalFeesPaidQuery1 = totalFeesPaidQuery1.Where(x => x.KiiPayPersonalWalletInformation.FaxerInformation.City.ToLower() == City);
                totalFeesPaidQuery2 = totalFeesPaidQuery2.Where(x => x.NonCardReciever.FaxerInformation.City.ToLower() == City);
                totalFeesPaidQuery3 = totalFeesPaidQuery3.Where(x => x.SenderKiiPayBusinessPaymentInformation.SenderInformation.City.ToLower() == City);
                var faxerCityQuery = dbContext.FaxerInformation.Where(x => x.City.ToLower() == City).Select(x => x.Id);
                totalFeesPaidQuery4 = totalFeesPaidQuery4.Where(x => faxerCityQuery.Contains(x.FaxerId));
                totalCompletedRefundQuery1 = totalCompletedRefundQuery1.Where(x => x.NonCardReciever.City.ToLower() == City);
                totalCompletedRefundQuery2 = totalCompletedRefundQuery2.Where(x => x.Faxer.City.ToLower() == City);
                totalCompletedRefundQuery3 = totalCompletedRefundQuery3.Where(x => x.Business.BusinessOperationCity.ToLower() == City);
                totalUncompletedRefundsQuery1 = totalUncompletedRefundsQuery1.Where(x => x.NonCardReciever.City.ToLower() == City);
                var cityQuery = dbContext.KiiPayPersonalWalletInformation.Where(x => x.CardUserCity.ToLower() == City).Select(x => x.MobileNo);
                totalUncompletedRefundsQuery2 = totalUncompletedRefundsQuery2.Where(x => cityQuery.Contains(x.MoblieNumber));
                var mfbcCityQuery = dbContext.KiiPayBusinessWalletInformation.Where(x => x.City.ToLower() == City).Select(x => x.MobileNo);
                totalUncompletedRefundsQuery3 = totalUncompletedRefundsQuery3.Where(x => mfbcCityQuery.Contains(x.MFBCCardNumber));

                totalUpdateOnTransferInfoQuery1 = totalUpdateOnTransferInfoQuery1.Where(x => x.NonCardTransaction.NonCardReciever.City.ToLower() == City);
                totalUpdateOnTransferInfoQuery2 = totalUpdateOnTransferInfoQuery2.Where(x => x.NonCardTransaction.NonCardReciever.City.ToLower() == City);
                totalCommissionPaidQuery = totalCommissionPaidQuery.Where(x => x.Agent.City.ToLower() == City);

            }
            if (string.IsNullOrEmpty(Year) == false)
            {
                // totalAgentsQuery.Where(x=>x.)
                //totalBusinessMerchantsQuery
                //totalSendersQuery.Where(x=>x.r)
                //totalCardUsersQuery.Where(x=>x.dat)
                totalNonCardUsersQuery = totalNonCardUsersQuery.Where(x => x.CreatedDate.Year == intYear);
                amountNotWithdrawnFromMFTCQuery1 = amountNotWithdrawnFromMFTCQuery1.Where(x => x.TransactionDate.Year == intYear);
                amountNotWithdrawnFromMFTCQuery2 = amountNotWithdrawnFromMFTCQuery2.Where(x => x.TransactionDate.Year == intYear);
                amountNotWithdrawnFromMFTCQuery3 = amountNotWithdrawnFromMFTCQuery3.Where(x => x.TransactionDate.Year == intYear);
                amountNotWithdrawnFromMFBCQuery1 = amountNotWithdrawnFromMFBCQuery1.Where(x => x.TransactionDate.Year == intYear);
                amountNotWithdrawnFromMFBCQuery2 = amountNotWithdrawnFromMFBCQuery2.Where(x => x.PaymentDate.Year == intYear);
                amountNotWithdrawnFromMFBCQuery3 = amountNotWithdrawnFromMFBCQuery3.Where(x => x.TransactionDate.Year == intYear);
                amountNotWithdrawnFromMFBCQuery4 = amountNotWithdrawnFromMFBCQuery4.Where(x => x.TransactionDate.Year == intYear);
                totalLocalTransactionQuery = totalLocalTransactionQuery.Where(x => x.TransactionDate.Year == intYear);
                totalLocalTransactionQuery1 = totalLocalTransactionQuery1.Where(x => x.TransactionDate.Year == intYear);
                totalMerchantPaymentsQuery1 = totalMerchantPaymentsQuery1.Where(x => x.TransactionDate.Year == intYear);
                totalMerchantPaymentsQuery2 = totalMerchantPaymentsQuery2.Where(x => x.TransactionDate.Year == intYear);
                totalMerchantPaymentsQuery3 = totalMerchantPaymentsQuery3.Where(x => x.PaymentDate.Year == intYear);
                //totalStaffsQuery
                totalFeesPaidQuery1 = totalFeesPaidQuery1.Where(x => x.TransactionDate.Year == intYear);
                totalFeesPaidQuery2 = totalFeesPaidQuery2.Where(x => x.TransactionDate.Year == intYear);
                totalFeesPaidQuery3 = totalFeesPaidQuery3.Where(x => x.PaymentDate.Year == intYear);
                totalFeesPaidQuery4 = totalFeesPaidQuery4.Where(x => x.TransactionDate.Year == intYear);
                totalRefundRequestQuery = totalRefundRequestQuery.Where(x => x.TransactionDate.Year == intYear);
                totalCompletedRefundQuery1 = totalCompletedRefundQuery1.Where(x => x.StatusChangedDate.Value.Year == intYear);
                totalCompletedRefundQuery2 = totalCompletedRefundQuery2.Where(x => x.RefundRequestDate.Year == intYear);
                totalCompletedRefundQuery3 = totalCompletedRefundQuery3.Where(x => x.RefundRequestDate.Year == intYear);
                totalUncompletedRefundsQuery1 = totalUncompletedRefundsQuery1.Where(x => x.StatusChangedDate.Value.Year == intYear);
                totalUncompletedRefundsQuery2 = totalUncompletedRefundsQuery2.Where(x => x.Date.Year == intYear);
                totalUncompletedRefundsQuery3 = totalUncompletedRefundsQuery3.Where(x => x.Date.Year == intYear);
                totalUpdateOnTransferInfoQuery1 = totalUpdateOnTransferInfoQuery1.Where(x => x.Date.Year == intYear);
                totalUpdateOnTransferInfoQuery2 = totalUpdateOnTransferInfoQuery2.Where(x => x.Date.Year == intYear);
                totalCommissionPaidQuery = totalCommissionPaidQuery.Where(x => x.TransactionDateTime.Year == intYear);
                //totalDeletedStaffsQuery 
                //totalDeletedSendersQuery
                //totalDeletedCardUsersQuery.Where(x=>x.d)
                //totalDeletedBusinessMerchantsQuery  
                //totalDeletedAgentsQuery.where()
                //totalDeletedNonCardUsersQuery.Where(x => x.CreatedDate.Year == intYear);                

                if (Month != 0)
                {
                    // totalAgentsQuery.Where(x=>x.)
                    //totalBusinessMerchantsQuery
                    //totalSendersQuery
                    //totalCardUsersQuery
                    totalNonCardUsersQuery = totalNonCardUsersQuery.Where(x => x.CreatedDate.Month == Month);
                    amountNotWithdrawnFromMFTCQuery1 = amountNotWithdrawnFromMFTCQuery1.Where(x => x.TransactionDate.Month == Month);
                    amountNotWithdrawnFromMFTCQuery2 = amountNotWithdrawnFromMFTCQuery2.Where(x => x.TransactionDate.Month == Month);
                    amountNotWithdrawnFromMFTCQuery3 = amountNotWithdrawnFromMFTCQuery3.Where(x => x.TransactionDate.Month == Month);
                    amountNotWithdrawnFromMFBCQuery1 = amountNotWithdrawnFromMFBCQuery1.Where(x => x.TransactionDate.Month == Month);
                    amountNotWithdrawnFromMFBCQuery2 = amountNotWithdrawnFromMFBCQuery2.Where(x => x.PaymentDate.Month == Month);
                    amountNotWithdrawnFromMFBCQuery3 = amountNotWithdrawnFromMFBCQuery3.Where(x => x.TransactionDate.Month == Month);
                    amountNotWithdrawnFromMFBCQuery4 = amountNotWithdrawnFromMFBCQuery4.Where(x => x.TransactionDate.Month == Month);
                    totalLocalTransactionQuery = totalLocalTransactionQuery.Where(x => x.TransactionDate.Month == Month);
                    totalLocalTransactionQuery1 = totalLocalTransactionQuery1.Where(x => x.TransactionDate.Month == Month);
                    totalMerchantPaymentsQuery1 = totalMerchantPaymentsQuery1.Where(x => x.TransactionDate.Month == Month);
                    totalMerchantPaymentsQuery2 = totalMerchantPaymentsQuery2.Where(x => x.TransactionDate.Month == Month);
                    totalMerchantPaymentsQuery3 = totalMerchantPaymentsQuery3.Where(x => x.PaymentDate.Month == Month);
                    //totalStaffsQuery
                    totalFeesPaidQuery1 = totalFeesPaidQuery1.Where(x => x.TransactionDate.Month == Month);
                    totalFeesPaidQuery2 = totalFeesPaidQuery2.Where(x => x.TransactionDate.Month == Month);
                    totalFeesPaidQuery3 = totalFeesPaidQuery3.Where(x => x.PaymentDate.Month == Month);
                    totalFeesPaidQuery4 = totalFeesPaidQuery4.Where(x => x.TransactionDate.Month == Month);
                    totalRefundRequestQuery = totalRefundRequestQuery.Where(x => x.TransactionDate.Month == Month);
                    totalCompletedRefundQuery1 = totalCompletedRefundQuery1.Where(x => x.StatusChangedDate.Value.Month == Month);
                    totalCompletedRefundQuery2 = totalCompletedRefundQuery2.Where(x => x.RefundRequestDate.Month == Month);
                    totalCompletedRefundQuery3 = totalCompletedRefundQuery3.Where(x => x.RefundRequestDate.Month == Month);
                    totalUncompletedRefundsQuery1 = totalUncompletedRefundsQuery1.Where(x => x.StatusChangedDate.Value.Month == Month);
                    totalUncompletedRefundsQuery2 = totalUncompletedRefundsQuery2.Where(x => x.Date.Month == Month);
                    totalUncompletedRefundsQuery3 = totalUncompletedRefundsQuery3.Where(x => x.Date.Month == Month);
                    totalUpdateOnTransferInfoQuery1 = totalUpdateOnTransferInfoQuery1.Where(x => x.Date.Month == Month);
                    totalUpdateOnTransferInfoQuery2 = totalUpdateOnTransferInfoQuery2.Where(x => x.Date.Month == Month);
                    totalCommissionPaidQuery = totalCommissionPaidQuery.Where(x => x.TransactionDateTime.Month == Month);
                    //totalDeletedStaffsQuery
                    //totalDeletedSendersQuery
                    //totalDeletedCardUsersQuery
                    //totalDeletedBusinessMerchantsQuery 
                    //totalDeletedAgentsQuery.where()
                    //totalDeletedNonCardUsersQuery.Where(x => x.CreatedDate.Month == Month);                   


                }
            }


            decimal totalCommissionPaid = 0;
            if (!string.IsNullOrEmpty(Country))
            {
                if (totalCommissionPaidQuery.Any())
                {
                    totalCommissionPaid = totalCommissionPaidQuery.Sum(x => x.TotalCommission);
                }
            }

            decimal amountNotWithdrawnFromMFTCQuery1Val = 0;
            decimal amountNotWithdrawnFromMFTCQuery2Val = 0;
            decimal amountNotWithdrawnFromMFTCQuery3Val = 0;
            decimal AmountNotwithdrawnFromMFTCVal = 0;
            if (!string.IsNullOrEmpty(Country))
            {
                if (amountNotWithdrawnFromMFTCQuery1.Any())
                {
                    amountNotWithdrawnFromMFTCQuery1Val = amountNotWithdrawnFromMFTCQuery1.Sum(x => x.RecievingAmount);
                }
                if (amountNotWithdrawnFromMFTCQuery2.Any())
                {
                    amountNotWithdrawnFromMFTCQuery2Val = amountNotWithdrawnFromMFTCQuery2.Sum(x => x.TransactionAmount);
                }
                if (amountNotWithdrawnFromMFTCQuery3.Any())
                {
                    amountNotWithdrawnFromMFTCQuery3Val = amountNotWithdrawnFromMFTCQuery3.Sum(x => x.RecievingAmount);
                }
            }
            AmountNotwithdrawnFromMFTCVal = amountNotWithdrawnFromMFTCQuery1Val + amountNotWithdrawnFromMFTCQuery3Val - amountNotWithdrawnFromMFTCQuery2Val;


            decimal amountNotWithdrawnFromMFBCQuery1Val = 0;
            decimal amountNotWithdrawnFromMFBCQuery2Val = 0;
            decimal amountNotWithdrawnFromMFBCQuery3Val = 0;
            decimal amountNotWithdrawnFromMFBCQuery4Val = 0;
            decimal AmountNotwithdrawnfromMFBCVal = 0;
            if (!string.IsNullOrEmpty(Country))
            {
                if (amountNotWithdrawnFromMFBCQuery1.Any())
                {
                    amountNotWithdrawnFromMFBCQuery1Val = amountNotWithdrawnFromMFBCQuery1.Sum(x => x.AmountSent);
                }
                if (amountNotWithdrawnFromMFBCQuery2.Any())
                {
                    amountNotWithdrawnFromMFBCQuery2Val = amountNotWithdrawnFromMFBCQuery2.Sum(x => x.ReceivingAmount);
                }
                if (amountNotWithdrawnFromMFBCQuery3.Any())
                {
                    amountNotWithdrawnFromMFBCQuery3Val = amountNotWithdrawnFromMFBCQuery3.Sum(x => x.AmountSent);
                }
                if (amountNotWithdrawnFromMFBCQuery4.Any())
                {
                    amountNotWithdrawnFromMFBCQuery4Val = amountNotWithdrawnFromMFBCQuery4.Sum(x => x.TransactionAmount);
                }
            }
            AmountNotwithdrawnfromMFBCVal = amountNotWithdrawnFromMFBCQuery1Val + amountNotWithdrawnFromMFBCQuery2Val + amountNotWithdrawnFromMFBCQuery3Val - amountNotWithdrawnFromMFBCQuery4Val;
            var counts = new AdminDashboardViewModel()
            {
                Currency = currency,
                TotalAgents = totalAgentsQuery.Count(),
                TotalBusinessMerchants = totalBusinessMerchantsQuery.Count(),
                TotalSenders = totalSendersQuery.Count(),
                TotalCardUsers = totalCardUsersQuery.Count(),
                TotalNonCardUsers = totalNonCardUsersQuery.Count(),
                AmountNotwithdrawnFromMFTC = AmountNotwithdrawnFromMFTCVal,// (amountNotWithdrawnFromMFTCQuery1.Sum(x => x.RecievingAmount) - amountNotWithdrawnFromMFTCQuery2.Sum(x => x.TransactionAmount)),
                AmountNotwithdrawnfromMFBC = AmountNotwithdrawnfromMFBCVal, //(amountNotWithdrawnFromMFBCQuery1.Sum(x => x.AmountSent) + amountNotWithdrawnFromMFBCQuery2.Sum(x => x.ReceivingAmount) + amountNotWithdrawnFromMFBCQuery3.Sum(x => x.AmountSent) - amountNotWithdrawnFromMFBCQuery4.Sum(x => x.TransactionAmount)),

                TotalLocalTransaction = totalLocalTransactionQuery.Count() + totalLocalTransactionQuery1.Count(),
                TotalCompleteMerchantPayment = totalMerchantPaymentsQuery1.Count() + totalMerchantPaymentsQuery2.Count() + totalMerchantPaymentsQuery3.Count(),
                TotalStaffs = totalStaffsQuery.Count(),
                TotalFeesPaid = totalFeesPaidQuery1.Count() + totalFeesPaidQuery2.Count() + totalFeesPaidQuery3.Count() + totalFeesPaidQuery4.Count(),
                TotalWebVisitors = 0,
                TotalRefundRequest = totalRefundRequestQuery.Count(),
                TotalCompletedRefund = totalCompletedRefundQuery1.Count() + totalCompletedRefundQuery2.Count() + totalCompletedRefundQuery3.Count(),
                TotalUnCompleteRefund = totalUncompletedRefundsQuery1.Count() + totalUncompletedRefundsQuery2.Count() + totalUncompletedRefundsQuery3.Count(),
                TotalUpdateonTransferInfo = totalUpdateOnTransferInfoQuery1.Count() + totalUpdateOnTransferInfoQuery2.Count(),
                TotalCommissionPaid = totalCommissionPaid,
                TotalCommissionUnPaid = 0,
                TotalFailTransaction = 0,
                TotalDeletedStaffs = totalDeletedStaffsQuery.Count(),
                TotalDeletedSenders = totalDeletedSendersQuery.Count(),
                TotalDeletedCardUsers = totalDeletedCardUsersQuery.Count(),
                TotalDeletedBusinessMerchants = totalDeletedBusinessMerchantsQuery.Count(),
                TotalDeletedAgents = totalDeletedAgentsQuery.Count(),
                TotalDeletedNonCardUsers = totalDeletedNonCardUsersQuery.Count()

            };
            return counts;

        }
    }
}