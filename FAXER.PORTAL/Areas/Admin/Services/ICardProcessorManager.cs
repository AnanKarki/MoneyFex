using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using OfficeOpenXml.FormulaParsing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public interface ICardProcessorManager
    {
        IQueryable<CardProcessor> CardProcessors();
        CardProcessor CardProcessorById(int id);
        List<CardProcessorViewModel> GetCardProcessors(int transfertype = 0, int transferMethod = 0, string country = "");
        CardProcessorViewModel GetCardProcessorById(int id = 0);
        int AddCardProcessor(CardProcessor cardProcessor);
        void UpdateCardProcessor(CardProcessor cardProcessor);
        void DeleteCardProcessor(int id);
        CardProcessor BindViewModelIntoCardProcessorModel(CardProcessorViewModel vm);


        IQueryable<CardProcessorSelection> CardProcessorSelections();
        CardProcessorSelection CardProcessorSelectionById(int id);
        List<DropDownViewModel> GetCardProcessDropdown(string Country = "", string currency = "");
        List<CardProcessorSelectionViewModel> GetCardProcessorSelections(int transfertype = 0, int transferMethod = 0, string sendingCountry = "", string receivingCountry = "");
        CardProcessorSelectionViewModel GetCardProcessorSelectionById(int id = 0);
        int AddCardProcessorSelection(CardProcessorSelection model);
        void UpdateCardProcessorSelection(CardProcessorSelection model);
        void DeleteCardProcessorSelection(int id);
        CardProcessorSelection BindViewModelIntoCardProcessorSelectionModel(CardProcessorSelectionViewModel vm);
    }
}
