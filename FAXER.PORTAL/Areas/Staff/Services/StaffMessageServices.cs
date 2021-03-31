using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Staff.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.Services
{
    public class StaffMessageServices
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        Areas.Admin.Services.CommonServices CommonService = new Areas.Admin.Services.CommonServices();
        public List<StaffEmailInboxViewModel> getInboxList(string status = "")
        {
            var data = dbContext.StaffEmail.Where(x => x.To_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.EmailStatus != EmailStatus.Draft && x.EmailStatus != EmailStatus.Archived && x.To_IsDeleted == false).ToList();
            if (status == "read")
            {
                data = dbContext.StaffEmail.Where(x => x.To_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.EmailStatus == EmailStatus.Delivered && x.To_IsDeleted == false).ToList();
            }
            else if (status == "unread")
            {
                data = dbContext.StaffEmail.Where(x => x.To_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.EmailStatus == EmailStatus.Sent && x.To_IsDeleted == false).ToList();
            }
            else if (status == "none")
            {
                data = dbContext.StaffEmail.Where(x => x.Id == 0).ToList();
            }
            var result = (from c in data.OrderByDescending(x => x.EmailSentDate)
                          select new StaffEmailInboxViewModel()
                          {
                              Id = c.Id,
                              SenderId = c.From_StaffId,
                              SenderName = CommonService.getStaffName(c.From_StaffId),
                              ReceiverId = c.To_StaffId,
                              Subject = c.SubJect,
                              ShortBody = c.BankPaymentReference == null ? "" : c.BankPaymentReference.Length < 15 ? c.BankPaymentReference.Substring(0, c.BankPaymentReference.Length) + "..." : c.BankPaymentReference.Substring(0, 15) + "...",
                              EmailStatus = c.EmailStatus,
                              HasAttachment = c.HasAttachment,
                              AttachmentURL = c.AttachmentURL == null ? "" : c.AttachmentURL,
                              EmailDate = c.EmailSentDate.ToString("HH:mm")
                          }).ToList();

            return result;
        }

        public List<StaffEmailInboxViewModel> getArchivedList()
        {
            var result = (from c in dbContext.StaffEmail.Where(x => x.To_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.EmailStatus == EmailStatus.Archived && x.To_IsDeleted == false).OrderByDescending(x => x.EmailSentDate).ToList()
                          select new StaffEmailInboxViewModel()
                          {
                              Id = c.Id,
                              SenderId = c.From_StaffId,
                              SenderName = CommonService.getStaffName(c.From_StaffId),
                              ReceiverId = c.To_StaffId,
                              Subject = c.SubJect,
                              ShortBody = c.BankPaymentReference == null ? "" : c.BankPaymentReference.Length < 15 ? c.BankPaymentReference.Substring(0, c.BankPaymentReference.Length) + "..." : c.BankPaymentReference.Substring(0, 15) + "...",
                              EmailStatus = c.EmailStatus,
                              HasAttachment = c.HasAttachment,
                              AttachmentURL = c.AttachmentURL == null ? "" : c.AttachmentURL,
                              EmailDate = c.EmailSentDate.ToString("HH:mm")
                          }).ToList();
            return result;
        }

        public List<StaffEmailInboxViewModel> getJunkList()
        {
            var result = (from c in dbContext.StaffEmail.Where(x => (x.From_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.From_IsDeleted == true && x.From_IsPermanentlyDeleted == false ) || (x.To_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.To_IsDeleted == true && x.To_IsPermanentlyDeleted == false)).OrderByDescending(x => x.EmailSentDate).ToList()
                          select new StaffEmailInboxViewModel()
                          {
                              Id = c.Id,
                              SenderId = c.From_StaffId,
                              SenderName = CommonService.getStaffName(c.From_StaffId),
                              ReceiverName = CommonService.getStaffName(c.To_StaffId),
                              Name = c.From_StaffId == Common.StaffSession.LoggedStaff.StaffId ? CommonService.getStaffName(c.To_StaffId) : CommonService.getStaffName(c.From_StaffId),
                              ReceiverId = c.To_StaffId,
                              Subject = c.SubJect,
                              ShortBody = c.BankPaymentReference == null ? "" : c.BankPaymentReference.Length < 15 ? c.BankPaymentReference.Substring(0, c.BankPaymentReference.Length) + "..." : c.BankPaymentReference.Substring(0, 15) + "...",
                              EmailStatus = c.EmailStatus,
                              HasAttachment = c.HasAttachment,
                              AttachmentURL = c.AttachmentURL == null ? "" : c.AttachmentURL,
                              EmailDate = c.EmailSentDate.ToString("HH:mm")
                          }).ToList();
            return result;
        }

        public List<StaffEmailInboxViewModel> getSentMessagesList()
        {
            var result = (from c in dbContext.StaffEmail.Where(x => x.From_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.EmailStatus != EmailStatus.Draft && x.From_IsDeleted == false).OrderByDescending(x => x.EmailSentDate).ToList()
                          select new StaffEmailInboxViewModel()
                          {
                              Id = c.Id,
                              SenderId = c.From_StaffId,
                              SenderName = CommonService.getStaffName(c.From_StaffId),
                              ReceiverName = CommonService.getStaffName(c.To_StaffId),
                              ReceiverId = c.To_StaffId,
                              Subject = c.SubJect,
                              ShortBody = c.BankPaymentReference == null ? "" : c.BankPaymentReference.Length < 15 ? c.BankPaymentReference.Substring(0, c.BankPaymentReference.Length) + "..." : c.BankPaymentReference.Substring(0, 15) + "...",
                              EmailStatus = c.EmailStatus,
                              HasAttachment = c.HasAttachment,
                              AttachmentURL = c.AttachmentURL,
                              EmailDate = c.EmailSentDate.ToString("HH:mm")

                          }).ToList();
            return result;
        }

        public List<StaffEmailInboxViewModel> getDraftMessagesList()
        {
            var result = (from c in dbContext.StaffEmail.Where(x => x.From_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.EmailStatus == EmailStatus.Draft && x.From_IsDeleted == false).OrderByDescending(x => x.EmailSentDate).ToList()
                          select new StaffEmailInboxViewModel()
                          {
                              Id = c.Id,
                              SenderId = c.From_StaffId,
                              SenderName = CommonService.getStaffName(c.From_StaffId),
                              ReceiverName = CommonService.getStaffName(c.To_StaffId),
                              ReceiverId = c.To_StaffId,
                              Subject = c.SubJect,
                              ShortBody = c.BankPaymentReference == null ? "" : c.BankPaymentReference.Length < 15 ? c.BankPaymentReference.Substring(0, c.BankPaymentReference.Length) + "..." : c.BankPaymentReference.Substring(0, 15) + "...",
                              EmailStatus = c.EmailStatus,
                              HasAttachment = c.HasAttachment,
                              AttachmentURL = c.AttachmentURL,
                              EmailDate = c.EmailSentDate.ToString("HH:mm")

                          }).ToList();
            return result;
        }

        public StaffEmailComposeViewModel viewMessage(int id)
        {
            var result = (from c in dbContext.StaffEmail.Where(x => x.Id == id).ToList()
                          select new StaffEmailComposeViewModel()
                          {
                              Id = c.Id,
                              FromEmailAddress = getStaffEmail(c.From_StaffId),
                              ToEmailAddress = getStaffEmail(c.To_StaffId),
                              ReceivingStaffId = c.To_StaffId,
                              Subject = c.SubJect,
                              AttachmentURL = c.AttachmentURL,
                              BankPaymentReference = c.BankPaymentReference
                          }).FirstOrDefault();
            return result;
        }

        public bool saveComposeMessage(StaffEmailComposeViewModel model)
        {
            if (model != null)
            {
                bool hasAttachment = false;
                if (!string.IsNullOrEmpty(model.AttachmentURL))
                {
                    hasAttachment = true;
                }
                DB.StaffEmail mail = new DB.StaffEmail()
                {
                    From_StaffId = Common.StaffSession.LoggedStaff.StaffId,
                    To_StaffId = model.ReceivingStaffId,
                    SubJect = model.Subject,
                    HasAttachment = hasAttachment,
                    AttachmentURL = model.AttachmentURL,
                    BankPaymentReference = model.BankPaymentReference,
                    EmailStatus = EmailStatus.Sent,
                    EmailSentDate = DateTime.Now
                };
                dbContext.StaffEmail.Add(mail);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
        public bool UpdateMessage(StaffEmailComposeViewModel model)
        {

            var data = dbContext.StaffEmail.Where(x => x.Id == model.Id).FirstOrDefault();
            bool hasAttachment = false;
            if (!string.IsNullOrEmpty(model.AttachmentURL))
            {

                hasAttachment = true;
            }
            data.HasAttachment = hasAttachment;
            data.SubJect = model.Subject;
            data.AttachmentURL = model.AttachmentURL;
            data.To_StaffId = model.ReceivingStaffId;
            data.BankPaymentReference = model.BankPaymentReference;
            if (model.Draft == true)
            {
                data.EmailStatus = EmailStatus.Draft;
            }
            else
            {
                data.EmailStatus = EmailStatus.Sent;
            }
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;

        }
        public bool saveDraftMessage(StaffEmailComposeViewModel model)
        {
            if (model != null)
            {
                bool hasAttachment = false;
                if (!string.IsNullOrEmpty(model.AttachmentURL))
                {
                    hasAttachment = true;
                }
                DB.StaffEmail mail = new DB.StaffEmail()
                {
                    From_StaffId = Common.StaffSession.LoggedStaff.StaffId,
                    To_StaffId = model.ReceivingStaffId,
                    SubJect = model.Subject,
                    HasAttachment = hasAttachment,
                    AttachmentURL = model.AttachmentURL,
                    BankPaymentReference = model.BankPaymentReference,
                    EmailStatus = EmailStatus.Draft,
                    EmailSentDate = DateTime.Now
                };
                dbContext.StaffEmail.Add(mail);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }


        public void MarkAllMessagesDelivered()
        {
            var data = dbContext.StaffEmail.Where(x => x.To_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.EmailStatus == EmailStatus.Sent).ToList();
            foreach (var item in data)
            {
                item.EmailStatus = EmailStatus.Delivered;
                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }

        }

        public void MarkMessageDelivered(int id)
        {
            var data = dbContext.StaffEmail.Where(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                if (data.From_StaffId != Common.StaffSession.LoggedStaff.StaffId)
                {

                    if (data.EmailStatus == EmailStatus.Sent)
                    {
                        data.EmailStatus = EmailStatus.Delivered;
                        dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public void MarkMessageArchived(int id)
        {
            var data = dbContext.StaffEmail.Where(x => x.Id == id && x.To_StaffId == Common.StaffSession.LoggedStaff.StaffId).FirstOrDefault();
            if (data != null)
            {
                if (data.EmailStatus != EmailStatus.Archived)
                {
                    data.EmailStatus = EmailStatus.Archived;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
        }

        public void MarkMessageAsDeleted(int id)
        {
            var data = dbContext.StaffEmail.Where(x => x.Id == id).FirstOrDefault();
            if (data.From_StaffId == Common.StaffSession.LoggedStaff.StaffId)
            {
                data.From_IsDeleted = true;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            else if (data.To_StaffId == Common.StaffSession.LoggedStaff.StaffId)
            {
                data.To_IsDeleted = true;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public int getStaffIdFromEmail(string email)
        {
            var data = dbContext.StaffInformation.Where(x => x.EmailAddress.ToLower() == email.ToLower()).FirstOrDefault();
            if (data != null)
            {
                return data.Id;
            }
            return 0;
        }

        public string getStaffEmail(int id)
        {
            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                return data.EmailAddress;
            }
            return "";
        }

        public List<StaffEmails> getAllStaffEmails(string Country ="" , string City ="")
        {

            var data = dbContext.StaffLogin.Where(x => x.IsDeleted == false);
            if (!string.IsNullOrEmpty(Country)) {

                data = dbContext.StaffLogin.Where(x => x.IsDeleted == false && x.Staff.Country == Country);
            }
            if (!string.IsNullOrEmpty(Country) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.StaffLogin.Where(x => x.IsDeleted == false && x.Staff.Country == Country && x.Staff.City.ToLower() == City.ToLower());
            }
            var result = (from c in data.ToList()
                          select new StaffEmails()
                          {
                              Id = c.StaffId,
                              EmailId = c.Staff.EmailAddress,
                              FullName = c.Staff.FirstName + " " + c.Staff.MiddleName + " " +c.Staff.LastName,
                              City = c.Staff.City,
                              Country = Common.Common.GetCountryName(c.Staff.Country),
                              Telephone = c.Staff.PhoneNumber
                          }).OrderBy(x => x.EmailId).ToList();

            return result;

            //var result = (from c in dbContext.FaxingCardTransaction.Where(x => x.Id != 0).ToList()
            //              select new StaffEmails()
            //              {
            //                  Id = c.Id,
            //                  EmailId = c.TransactionDate.ToString()
            //              }).ToList();

            //return result;
        }

        public string getInboxCount()
        {
            int count = dbContext.StaffEmail.Where(x => x.To_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.EmailStatus == EmailStatus.Sent).Count();
            return count.ToString();
        }

        public string getDraftCount()
        {
            int count = dbContext.StaffEmail.Where(x => x.From_StaffId == Common.StaffSession.LoggedStaff.StaffId && x.EmailStatus == EmailStatus.Draft && x.From_IsDeleted == false).Count();
            return count.ToString();
        }

        public void deleteMessagePermanently(int id)
        {
            if (id != 0)
            {
                var data = dbContext.StaffEmail.Find(id);
                if (data != null)
                {
                    if ( data.From_StaffId == Common.StaffSession.LoggedStaff.StaffId)
                    {
                        if (data.To_IsPermanentlyDeleted == true)
                        {
                            dbContext.StaffEmail.Remove(data);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            data.From_IsPermanentlyDeleted = true;
                            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    else if (data.To_StaffId == Common.StaffSession.LoggedStaff.StaffId)
                    {
                        if (data.From_IsPermanentlyDeleted == true)
                        {
                            dbContext.StaffEmail.Remove(data);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            data.To_IsPermanentlyDeleted = true;
                            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                }                
            }
        }
         
        public void restoreMessage (int id)
        {
            if (id != 0)
            {
                var data = dbContext.StaffEmail.Where(x=>x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    if (data.To_StaffId == Common.StaffSession.LoggedStaff.StaffId)
                    {
                        data.To_IsDeleted = false;
                        dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else if (data.From_StaffId == Common.StaffSession.LoggedStaff.StaffId)
                    {
                        data.From_IsDeleted = false;
                        dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }                    
                }
            }
        }
        

    }
}