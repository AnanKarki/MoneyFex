using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class CardProcessorManager : ICardProcessorManager
    {
        FAXEREntities dbContext = null;
        public CardProcessorManager()
        {
            dbContext = new FAXEREntities();
        }
        #region cardProcessor
        public IQueryable<CardProcessor> CardProcessors()
        {
            return dbContext.CardProcessor;
        }
        public CardProcessor CardProcessorById(int id)
        {
            CardProcessor cardProcessor = CardProcessors().SingleOrDefault(x => x.Id == id);
            return cardProcessor;
        }
        public int AddCardProcessor(CardProcessor cardProcessor)
        {
            dbContext.CardProcessor.Add(cardProcessor);
            dbContext.SaveChanges();
            return cardProcessor.Id;
        }
        public void DeleteCardProcessor(int id)
        {
            var cardProcessor = CardProcessorById(id);
            dbContext.CardProcessor.Remove(cardProcessor);
            dbContext.SaveChanges();
        }

        public void UpdateCardProcessor(CardProcessor cardProcessor)
        {
            dbContext.Entry(cardProcessor).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        public List<CardProcessorViewModel> GetCardProcessors(int transfertype = 0, int transferMethod = 0, string country = "")
        {
            var cardProcessor = CardProcessors();
            if (transfertype > 0)
            {
                cardProcessor = cardProcessor.Where(x => x.TransactionTransferType == (TransactionTransferType)transfertype);
            }
            if (transferMethod > 0)
            {
                cardProcessor = cardProcessor.Where(x => x.TransferMethod == (TransactionTransferMethod)transferMethod);
            }
            if (!string.IsNullOrEmpty(country))
            {
                cardProcessor = cardProcessor.Where(x => x.Country == country);
            }
            var result = (from c in cardProcessor.ToList()
                          join Country in dbContext.Country on c.Country equals Country.CountryCode
                          select new CardProcessorViewModel()
                          {
                              CardProcessorName = c.CardProcessorName,
                              ContactName = c.ContactName,
                              Country = c.Country,
                              CountryName = Country.CountryName,
                              CreatedBy = c.CreatedBy,
                              CreatedDate = c.CreatedDate,
                              Email = c.Email,
                              Id = c.Id,
                              TelephoneNo = c.TelephoneNo,
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),
                              TransferType = c.TransactionTransferType,
                              TransferTypeName = c.TransactionTransferType.ToString(),
                              CardProcessorApi = c.CardProcessorApi,
                              CardProcessorApiName = Common.Common.GetEnumDescription(c.CardProcessorApi),
                          }).ToList();
            return result;
        }

        public CardProcessorViewModel GetCardProcessorById(int id = 0)
        {
            var cardProcessor = CardProcessorById(id);
            CardProcessorViewModel vm = new CardProcessorViewModel()
            {
                Id = cardProcessor.Id,
                TelephoneNo = cardProcessor.TelephoneNo,
                CreatedDate = cardProcessor.CreatedDate,
                CreatedBy = cardProcessor.CreatedBy,
                Country = cardProcessor.Country,
                ContactName = cardProcessor.ContactName,
                CardProcessorName = cardProcessor.CardProcessorName,
                Email = cardProcessor.Email,
                TransferMethod = cardProcessor.TransferMethod,
                TransferType = cardProcessor.TransactionTransferType,
                CardProcessorApi = cardProcessor.CardProcessorApi,
            };
            return vm;
        }

        public CardProcessor BindViewModelIntoCardProcessorModel(CardProcessorViewModel vm)
        {
            CardProcessor cardProcessor = new CardProcessor()
            {
                Email = vm.Email,
                TransactionTransferType = vm.TransferType,
                CardProcessorName = vm.CardProcessorName,
                TransferMethod = vm.TransferMethod,
                ContactName = vm.ContactName,
                Country = vm.Country,
                CreatedBy = vm.CreatedBy,
                CreatedDate = vm.CreatedDate,
                TelephoneNo = vm.TelephoneNo,
                Id = vm.Id ?? 0,
                CardProcessorApi = vm.CardProcessorApi
            };
            return cardProcessor;
        }

        #endregion

        #region card Processor selection
        public IQueryable<CardProcessorSelection> CardProcessorSelections()
        {
            return dbContext.CardProcessorSelection;
        }

        public CardProcessorSelection CardProcessorSelectionById(int id)
        {
            return CardProcessorSelections().SingleOrDefault(x => x.Id == id);
        }

        public List<DropDownViewModel> GetCardProcessDropdown(string Country = "", string currency = "")
        {
            var cardProcessor = CardProcessors();
            
            //This is not needed because card processor will not be 
            // providing services to individual currency 
            // the country on card processor selection is not for the filter 
            // it is just for the information purpose

            //CommonServices _commonServices = new CommonServices();
            //if (!string.IsNullOrEmpty(currency))
            //{
            //    cardProcessor = (from c in cardProcessor
            //                     join ctry in dbContext.Country.Where(x => x.Currency == currency) on c.Country equals ctry.CountryCode
            //                     select c);
            //}
            //if (!string.IsNullOrEmpty(Country))
            //{
            //    if (Country != "All")
            //    {
            //        cardProcessor = cardProcessor.Where(x => x.Country == Country);
            //    }
            //}
            var result = (from c in cardProcessor
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.CardProcessorName
                          }).ToList();
            return result;
        }

        public List<CardProcessorSelectionViewModel> GetCardProcessorSelections(int transfertype = 0, int transferMethod = 0,
            string sendingCountry = "", string receivingCountry = "")
        {
            var cardProcessorSelections = CardProcessorSelections();
            if (transfertype > 0)
            {
                cardProcessorSelections = cardProcessorSelections.Where(x => x.TransferType == (TransactionTransferType)transfertype);
            }
            if (transferMethod > 0)
            {
                cardProcessorSelections = cardProcessorSelections.Where(x => x.TransferMethod == (TransactionTransferMethod)transferMethod);
            }
            if (!string.IsNullOrEmpty(sendingCountry))
            {
                cardProcessorSelections = cardProcessorSelections.Where(x => x.SendingCountry == sendingCountry);
            }
            if (!string.IsNullOrEmpty(receivingCountry))
            {
                cardProcessorSelections = cardProcessorSelections.Where(x => x.ReceivingCountry == receivingCountry);
            }
            var result = (from c in cardProcessorSelections.ToList()
                          join CardProcessor in dbContext.CardProcessor on c.CardProcessorId equals CardProcessor.Id
                          join SendingCountry in dbContext.Country on c.SendingCountry equals SendingCountry.CountryCode into SC
                          from SendingCountry in SC.DefaultIfEmpty()
                          join ReceivingCountry in dbContext.Country on c.ReceivingCountry equals ReceivingCountry.CountryCode into RC
                          from ReceivingCountry in RC.DefaultIfEmpty()
                          select new CardProcessorSelectionViewModel()
                          {
                              Id = c.Id,
                              CardProcessorId = c.CardProcessorId,
                              CardProcessorName = CardProcessor.CardProcessorName,
                              CreatedBy = c.CreatedBy,
                              CreatedDate = c.CreatedDate,
                              FromRange = c.FromRange,
                              Range = c.Range,
                              RangeName = Common.Common.GetEnumDescription(c.Range),
                              ReceivingCountry = c.ReceivingCountry,
                              ReceivingCountryName = ReceivingCountry == null ? "All" : ReceivingCountry.CountryName,
                              ReceivingCurrency = c.ReceivingCurrency,
                              SendingCountry = c.SendingCountry,
                              SendingCountryName = SendingCountry == null ? "All" : SendingCountry.CountryName,
                              SendingCurrency = c.SendingCurrency,
                              ToRange = c.ToRange,
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),
                              TransferType = c.TransferType,
                              TransferTypeName = Common.Common.GetEnumDescription(c.TransferType),

                          }).ToList();
            return result;
        }

        public CardProcessorSelectionViewModel GetCardProcessorSelectionById(int id = 0)
        {
            var cardPocessorSelection = CardProcessorSelectionById(id);
            var result = new CardProcessorSelectionViewModel()
            {
                Id = cardPocessorSelection.Id,
                CardProcessorId = cardPocessorSelection.CardProcessorId,
                CreatedBy = cardPocessorSelection.CreatedBy,
                CreatedDate = cardPocessorSelection.CreatedDate,
                FromRange = cardPocessorSelection.FromRange,
                Range = cardPocessorSelection.Range,
                RangeName = Common.Common.GetEnumDescription(cardPocessorSelection.Range),
                ReceivingCountry = cardPocessorSelection.ReceivingCountry,
                ReceivingCurrency = cardPocessorSelection.ReceivingCurrency,
                SendingCountry = cardPocessorSelection.SendingCountry,
                SendingCurrency = cardPocessorSelection.SendingCurrency,
                ToRange = cardPocessorSelection.ToRange,
                TransferMethod = cardPocessorSelection.TransferMethod,
                TransferMethodName = cardPocessorSelection.TransferMethod.ToString(),
                TransferType = cardPocessorSelection.TransferType,
                TransferTypeName = cardPocessorSelection.TransferType.ToString()
            };
            return result;
        }

        public int AddCardProcessorSelection(CardProcessorSelection model)
        {
            dbContext.CardProcessorSelection.Add(model);
            dbContext.SaveChanges();
            return model.Id;
        }

        public void UpdateCardProcessorSelection(CardProcessorSelection model)
        {
            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteCardProcessorSelection(int id)
        {
            var CardProcessorSelection = CardProcessorSelectionById(id);
            dbContext.CardProcessorSelection.Remove(CardProcessorSelection);
            dbContext.SaveChanges();

        }

        public CardProcessorSelection BindViewModelIntoCardProcessorSelectionModel(CardProcessorSelectionViewModel vm)
        {
            GetFromAndToRange(vm);
            CardProcessorSelection cardProcessorSelection = new CardProcessorSelection()
            {
                Id = vm.Id ?? 0,
                Range = GetRange(vm.RangeName),
                CardProcessorId = vm.CardProcessorId,
                CreatedBy = vm.CreatedBy,
                CreatedDate = vm.CreatedDate,
                FromRange = vm.FromRange,
                ReceivingCountry = vm.ReceivingCountry,
                ReceivingCurrency = vm.ReceivingCurrency,
                SendingCountry = vm.SendingCountry,
                SendingCurrency = vm.SendingCurrency,
                ToRange = vm.ToRange,
                TransferMethod = vm.TransferMethod,
                TransferType = vm.TransferType

            };
            return cardProcessorSelection;
        }
        private TranfserFeeRange GetRange(string Range)
        {
            TranfserFeeRange FeeRange = new TranfserFeeRange();
            if (Range == "0-0")
            {
                FeeRange = TranfserFeeRange.All;
            }
            else if (Range == "1-100")
            {

                FeeRange = TranfserFeeRange.OneToHundred;
            }
            else if (Range == "101-500")
            {
                FeeRange = TranfserFeeRange.HundredOnetoFiveHundred;
            }
            else if (Range == "501-1000")
            {
                FeeRange = TranfserFeeRange.FivehundredOneToThousand;
            }
            else if (Range == "1001-1500")
            {
                FeeRange = TranfserFeeRange.ThousandOneToFifteenHundred;
            }
            else if (Range == "1501-2000")
            {
                FeeRange = TranfserFeeRange.FifteenHundredOneToTwoThousand;
            }

            else if (Range == "2001-3000 ")
            {
                FeeRange = TranfserFeeRange.TwothousandOneToThreeThousand;
            }
            else if (Range == "3001-5000")
            {
                FeeRange = TranfserFeeRange.ThreeTHousandOneToFiveThousand;
            }
            else if (Range == "5001-10000")
            {
                FeeRange = TranfserFeeRange.FivethousandOneToTenThousand;
            }
            else if (Range == "11000+")
            {
                FeeRange = TranfserFeeRange.AboveTenThousand;
            }
            else
            {
                FeeRange = TranfserFeeRange.Select;
            }
            return FeeRange;
        }

        private CardProcessorSelectionViewModel GetFromAndToRange(CardProcessorSelectionViewModel vm)
        {
            string[] Range = vm.RangeName.Split('-');
            decimal FromRange = decimal.Parse(Range[0]);
            decimal ToRange = 0;
            if (Range.Length < 2)
            {
                ToRange = int.MaxValue;
            }
            else
            {
                ToRange = decimal.Parse(Range[1]);
            }
            vm.FromRange = FromRange;
            vm.ToRange = ToRange;
            return vm;
        }

        #endregion
    }
}