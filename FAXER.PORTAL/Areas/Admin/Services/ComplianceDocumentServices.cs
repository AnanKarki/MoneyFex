using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ComplianceDocumentServices
    {
        FAXEREntities dbContext = null;
        CommonServices _commonSerivces = null;
        public ComplianceDocumentServices()
        {
            dbContext = new FAXEREntities();
            _commonSerivces = new CommonServices();
        }

        public List<ItemGroupKendoTreeViewModel> GetCountryFolder(int senderType = 0)
        {
            IEnumerable<ItemGroupKendoTreeViewModel> iTreeViewData = (from c in dbContext.Country.OrderBy(x => x.CountryName)
                                                                      select new ItemGroupKendoTreeViewModel()
                                                                      {
                                                                          id = c.Id,
                                                                          text = c.CountryName
                                                                      });

            List<ItemGroupKendoTreeViewModel> CountryAndSenderData = iTreeViewData.ToList();

            var senderDocumentationData = dbContext.SenderBusinessDocumentation.ToList();
            var senderinfo = dbContext.FaxerInformation.ToList();
            if (senderType == 0)
            {
                senderinfo = senderinfo.Where(x => x.IsBusiness == false).ToList();
            }
            else
            {
                senderinfo = senderinfo.Where(x => x.IsBusiness == true).ToList();
            }
            foreach (var item in iTreeViewData)
            {
                var datauser = _commonSerivces.GetAllSenderInfo();
                var result = (from c in senderDocumentationData
                              join sender in senderinfo on c.SenderId equals sender.Id
                              join d in _commonSerivces.GetCountries().Where(x => x.Id == item.id) on c.Country equals d.Code
                              select new ItemGroupKendoTreeViewModel()
                              {
                                  id = c.Id,
                                  text = sender.FirstName + (sender.MiddleName == null || sender.MiddleName == null ? "" : " ") + sender.MiddleName + " " + sender.LastName,
                                  parentId = item.id
                              }).ToList();

                CountryAndSenderData.AddRange(result);
            }
            if (iTreeViewData.Count() > 0)
            {
                iTreeViewData = BuildTree(CountryAndSenderData.AsEnumerable());
            }
            return iTreeViewData.ToList();
        }
        public List<FileViewModel> GetSendersDocumentByCountryId(int CountryId, int senderType = 0)
        {
            IQueryable<FaxerInformation> senderinfo = dbContext.FaxerInformation;
            if (senderType == 0)
            {
                senderinfo = senderinfo.Where(x => x.IsBusiness == false);
            }
            else
            {
                senderinfo = senderinfo.Where(x => x.IsBusiness == true);
            }
            var file = (from c in dbContext.SenderBusinessDocumentation.ToList()
                        join sender in senderinfo on c.SenderId equals sender.Id
                        join country in dbContext.Country.Where(x => x.Id == CountryId) on c.Country equals country.CountryCode
                        select new FileViewModel()
                        {
                            Id = c.Id,
                            SenderName = sender.FirstName + " " + (string.IsNullOrEmpty(sender.MiddleName) == true ? sender.MiddleName + " " : "") + sender.LastName,
                            Name = c.DocumentName,
                            Url = c.DocumentPhotoUrl,
                            FileExtension = c.DocumentPhotoUrl.Split('.')[1]
                        }).ToList();
            return file;
        }
        public List<FileViewModel> GetFilesByFolder(int CountryId, int senderType = 0)
        {
            var foldersAndFiles = new List<FileViewModel>();

            IQueryable<FaxerInformation> senderinfo = dbContext.FaxerInformation;
            if (senderType == 0)
            {
                senderinfo = senderinfo.Where(x => x.IsBusiness == false);
            }
            else
            {
                senderinfo = senderinfo.Where(x => x.IsBusiness == true);
            }
            var Folders = (from c in dbContext.SenderBusinessDocumentation.ToList()
                           join sender in senderinfo on c.SenderId equals sender.Id
                           join country in dbContext.Country.Where(x => x.Id == CountryId) on c.Country equals country.CountryCode
                           select new FileViewModel()
                           {
                               Id = c.Id,
                               Name = sender.FirstName + " " + (string.IsNullOrEmpty(sender.MiddleName) == true ? sender.MiddleName + " " : "") + sender.LastName,
                               FolderId = CountryId,
                               IsFiles = false,
                           }).ToList();
            foldersAndFiles.AddRange(Folders);
            foreach (var item in Folders)
            {
                var filesData = _commonSerivces.GetSenderBusinessDocumentations().Where(x => x.Id == item.Id).ToList();
                if (filesData.Count > 0)
                {
                    var filesDocument = (from c in filesData
                                         select new FileViewModel()
                                         {
                                             Id = c.Id,
                                             Name = c.DocumentName,
                                             Url = c.DocumentPhotoUrl,
                                             FileExtension = c.DocumentPhotoUrl.Split('.')[1],
                                             FolderId = item.Id,
                                             IsFiles = true,
                                         }).ToList();
                    foldersAndFiles.AddRange(filesDocument);
                }
            }
            return foldersAndFiles;
        }
        internal IList<ItemGroupKendoTreeViewModel> BuildTree(IEnumerable<ItemGroupKendoTreeViewModel> source)
        {
            var groups = source.GroupBy(i => i.parentId);

            var roots = groups.FirstOrDefault(g => g.Key.HasValue == false).ToList();

            if (roots.Count > 0)
            {
                var dict = groups.Where(g => g.Key.HasValue).ToDictionary(g => g.Key.Value, g => g.ToList());
                for (int i = 0; i < roots.Count; i++)
                    AddChildren(roots[i], dict);
            }
            return roots;
        }

        private void AddChildren(ItemGroupKendoTreeViewModel node, IDictionary<int, List<ItemGroupKendoTreeViewModel>> source)
        {
            if (source.ContainsKey(node.id))
            {
                node.items = source[node.id];
                for (int i = 0; i < node.items.Count; i++)
                    AddChildren(node.items[i], source);
            }
            else
            {
                node.items = new List<ItemGroupKendoTreeViewModel>();
            }
        }

    }
}