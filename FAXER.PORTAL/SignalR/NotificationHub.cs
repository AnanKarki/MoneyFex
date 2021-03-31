using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.SignalR
{


    public class DashBoardHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
        public void AddCompanyGroup(string CompanyId)
        {
            Groups.Add(Context.ConnectionId, "C" + CompanyId);
        }
    }
    public class KiiPayBusinessHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
        public void addItem(int businessId, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.Group("KB" + businessId).AddItem(message);
        }
        public void addKiiPayBusiness(int businessId)
        {
            Groups.Add(Context.ConnectionId, "KB" + businessId);
        }
        public void removeKiiPayBusiness(int businessId)
        {
            Groups.Remove(Context.ConnectionId, "KB" + businessId);
        }
    }

    public class KiiPayPersonalHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
        public void addItem(int personalId, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.Group("KP" + personalId).AddItem(message);
        }
        public void addKiiPayPersonal(int personalId)
        {
            Groups.Add(Context.ConnectionId, "KP" + personalId);
        }
        public void removeKiiPayPersonal(int personalId)
        {
            Groups.Remove(Context.ConnectionId, "KP" + personalId);
        }
    }

    public class SenderHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
        public void addItem(int senderId, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.Group("SE" + senderId).AddItem(message);
        }
        public void addSender(int senderId)
        {
            Groups.Add(Context.ConnectionId, "SE" + senderId);
        }
        public void removeSender(int senderId)
        {
            Groups.Remove(Context.ConnectionId, "SE" + senderId);
        }
    }
    public class AgentHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);

        }
        public void addItem(int agentId , string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.Group("AG" + agentId).AddItem(message);

        }
        public void addAgent(int agentId)
        {
            Groups.Add(Context.ConnectionId, "AG" + agentId);

        }
        public void removeAgent(int agentId)
        {
            Groups.Remove(Context.ConnectionId, "AG" + agentId);
        }
    }
    public class AdminHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);

        }
        public void addItem(int staffId, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.Group("AD").AddItem(message);

        }
        public void addAdmin(int staffId)
        {
            Groups.Add(Context.ConnectionId, "AD");

        }
        public void removeAdmin(int staffId)
        {
            Groups.Remove(Context.ConnectionId, "AD");
        }
    }

    public class HubController
    {
        public static void SendToDashBoard(string BusinessId, string title, string message, string Amount, string Time, string hourAgo, object Data)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<DashBoardHub>();
            hubContext.Clients.Group("C" + BusinessId).broadcastMessage(title, message, Amount, Time, hourAgo);
        }
        public static void SendToKiiPayBusiness(string BusinessId, string notificationid, string title, string message, string Amount, string Time, string hourAgo, object Data)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<KiiPayBusinessHub>();
            hubContext.Clients.Group("KB" + BusinessId).broadcastMessage(notificationid, title, message, Amount, Time, hourAgo);
        }
        public static void SendToKiiPayPersonal(string PersonalId, string title, string message, string Amount, string Time, string hourAgo, object Data)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<KiiPayPersonalHub>();
            hubContext.Clients.Group("KP" + PersonalId).broadcastMessage(title, message, Amount, Time, hourAgo);
        }
        public static void SendToSender(string SenderId, string title, string message, string Amount, string Time, string hourAgo, object Data)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SenderHub>();
            hubContext.Clients.Group("SE" + SenderId).broadcastMessage(title, message, Amount, Time, hourAgo);
        }
        //public static void SendToKiiPayBusiness(int BusinessId , string message)
        //{

        //    var hubContext = GlobalHost.ConnectionManager.GetHubContext<KiiPayBusinessHub>();
        //    hubContext.Clients.Group("K" + BusinessId).addItem(message);
        //}
        public static void sendToAgent(string agentId, string title, string message, string Amount, string Time, string hourAgo, object Data)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<AgentHub>();
            hubContext.Clients.Group("AG" + agentId).broadcastMessage(title, message, Amount, Time, hourAgo);
        }   

        public static void sendToAdmin(string staffId, string title, string message, string Amount, string Time, string hourAgo, object Data)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<AdminHub>();
            hubContext.Clients.Group("AD").broadcastMessage(title, message, Amount, Time, hourAgo);
        }

    }
}