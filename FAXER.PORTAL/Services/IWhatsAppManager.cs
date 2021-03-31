using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAXER.PORTAL.Services
{
    public interface IWhatsAppManager
    {
        List<WhatsAppMessage> GetWhatsAppMessage();
        void AddMessage(WhatsAppMessage vm);
    }
}
