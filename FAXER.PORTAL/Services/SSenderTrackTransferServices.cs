using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderTrackTransferServices
    {
        DB.FAXEREntities dbContext = null;
        public SSenderTrackTransferServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public TrackFaxDetails GetFaxDetails(TrackFax model)
        {
            var faxDetails = (from c in dbContext.MerchantNonCardTransaction.Where(x => x.MFCN == model.MoneyFaxControlNumber).ToList()
                              select new Models.TrackFaxDetails()
                              {
                                  MFCNNumber = c.MFCN,
                                  SenderSurName = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == c.MFBCCardID).Select(x => x.LastName).FirstOrDefault(),
                                  //StatusOfFax = Enum.GetName(typeof(FaxingStatus), c.FaxingStatus)
                                  StatusOfFax = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                              }).FirstOrDefault();
            return faxDetails;
        }
        public TrackFaxDetails GetFaxCardDetails(TrackFax model)
        {
            var faxDetails = (from c in dbContext.CardUserNonCardTransaction.Where(x => x.MFCN == model.MoneyFaxControlNumber).ToList()
                          select new Models.TrackFaxDetails()
                          {
                              MFCNNumber = c.MFCN,
                              SenderSurName = c.CardUserReceiverDetails.MFTCCardInformation.LastName,
                              //StatusOfFax = Enum.GetName(typeof(FaxingStatus), c.FaxingStatus)
                              StatusOfFax = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                          }).FirstOrDefault();

            return faxDetails;
        }
        public TrackFaxDetails GetFaxNonCardDetails(TrackFax model)
        {
             
            var faxDetails = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == model.MoneyFaxControlNumber).ToList()
                          select new Models.TrackFaxDetails()
                          {
                              MFCNNumber = c.MFCN,
                              SenderSurName = c.NonCardReciever.FaxerInformation.LastName,
                              //StatusOfFax = Enum.GetName(typeof(FaxingStatus), c.FaxingStatus)
                              StatusOfFax = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                          }).FirstOrDefault();
            return faxDetails;
        }

}
}