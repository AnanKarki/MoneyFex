using FAXER.PORTAL.Common;
using FAXER.PORTAL.Migrations;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Http;
using System.Web.Routing;
using System.Threading;

namespace FAXER.PORTAL
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DbMigration();
            //AutoPaymentFunction();
            //UpdateTransactionStatusThread();
            //var dataTokens = HttpContext.Current.Request.RequestContext.RouteData.DataTokens;
            //if (dataTokens.ContainsKey("Agent"))
            //{
            //    Session.Timeout = 1;
            //}
            //else
            //{
            //    Session.Timeout = 20;
            //}
            //SAutoTopUpForAll sAutoTopUpForAll = new SAutoTopUpForAll();
            //sAutoTopUpForAll.KiiPayBusinessAutoPaymentByBusiness();
            //Session.Timeout = 140;
        }
        private static void DbMigration()
        {
            var configuration = new Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();

        }


        private void AutoPaymentFunction() 
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = new TimeSpan(23,30,0 ).TotalMilliseconds;
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
        }

        private void UpdateTransactionStatusThread()
        {

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = new TimeSpan(0, 15, 0).TotalMilliseconds;
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateTransactionStatus);
            timer.Enabled = true;

        }
        private void UpdateTransactionStatus(object sender, System.Timers.ElapsedEventArgs e) {

            SSenderBankAccountDeposit SSenderBankAccountDeposit = new SSenderBankAccountDeposit();
            SSenderBankAccountDeposit.UpdateTransactionStatus();

        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {


            //SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();

            //SSenderMobileMoneyTransfer senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();

            SMerchantAutoPayment sMerchantAutoPayment = new SMerchantAutoPayment();
            sMerchantAutoPayment.MerchantAutoPayment();

            SSomeOneElseMFTCCardAutoPayment sSomeOneElseMFTCCardAutoPayment = new SSomeOneElseMFTCCardAutoPayment();
            sSomeOneElseMFTCCardAutoPayment.MFTCCardAutoPayment();
        }
        void Session_End(Object sender, EventArgs E)
        {
            // Clean up session resources
            //this is session id
            //put clearing here
            Session.Clear();
            //Session.Abandon();
            //Session.
        }
    }
}
