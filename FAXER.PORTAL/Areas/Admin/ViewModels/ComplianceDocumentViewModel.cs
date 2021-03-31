using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ComplianceDocumentViewModel
    {
        public List<ItemGroupKendoTreeViewModel> ItemGroupKendoTreeViewModel { get; set; }
        public List<FileViewModel> FileViewModel { get; set; }
    }
    public class ItemGroupKendoTreeViewModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public int? parentId { get; set; }
        public bool hasChildren { get; set; }
        public string ImageUrl { get; set; }
        public string spriteCssClass { get { return "folder"; } }
        public List<ItemGroupKendoTreeViewModel> items { get; set; }
    }
    public class FileViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SenderName { get; set; }

        public string Url { get; set; }
        public string FileExtension { get; set; }
        public int FolderId { get; set; }
        public bool IsFiles { get; set; }

    }

}