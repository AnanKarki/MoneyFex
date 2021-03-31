using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Filters;

namespace FAXER.PORTAL
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RiddhaMVCAuthorizeAttribute());

            System.Web.Http.GlobalConfiguration.Configuration.Filters.Add(new RiddhaAPIAuthorizeAttribute());

            System.Web.Http.GlobalConfiguration.Configuration.Filters.Add(new DeflateCompressionAttribute());

        }
    }
    public class RiddhaMVCAuthorizeAttribute : AuthorizeAttribute
    {
        string areaName = "";
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            areaName = (httpContext.Request.RequestContext.RouteData.DataTokens["area"] ?? "").ToString();
            string controllerName = httpContext.Request.RequestContext.RouteData.GetRequiredString("controller");
            string actionName = httpContext.Request.RequestContext.RouteData.GetRequiredString("action");


            //if (controllerName.ToLower() == "home" && (actionName.ToLower() == "index" || actionName.ToLower() == "logout" || actionName.ToLower() == "changepassword" || actionName.ToLower() == "lockscreen" || actionName.ToLower() == "owner" || actionName.ToLower() == "captcha") || controllerName.ToLower() == "demologin")
            //{
            //    return true;
            //}


            if (areaName.ToLower() == "mobile")
            {

                return true;
            }
            if ((actionName.ToLower() == "logout" || actionName.ToLower() == "changepassword" || actionName.ToLower() == "lockscreen" || actionName.ToLower() == "owner" || actionName.ToLower() == "captcha") || controllerName.ToLower() == "demologin" || areaName.ToLower() == "emailtemplate")
            {
                return true;
            }
            if (controllerName.ToLower() == "whatsappwebhook" || actionName.ToLower() == "receivemessage")
            {

                return true;
            }
            bool authorize = false;

            if (controllerName.ToLower() == "home")
            {

                authorize = true;
            }
            if (controllerName.ToLower() == "moneyfexdemo")
            {

                authorize = true;
            }
            if (controllerName.ToLower() == "registrationcodeverification")
            {

                authorize = true;

            }
            if (areaName.ToLower() == "cardusers" || controllerName.ToLower() == "carduserhome" || actionName.ToLower() == "home")
            {

                authorize = true;
            }
            if (areaName.ToLower() == "businesses" || controllerName.ToLower() == "businesshome" || actionName.ToLower() == "home")
            {

                authorize = true;
            }
            if (areaName.ToLower() == "sender" || controllerName.ToLower() == "senderbankaccountdeposit" || actionName.ToLower() == "threedpageresponse")
            {
                authorize = true;
            }
            if (controllerName.ToLower() == "sendermobilemoneytransfer" && actionName.ToLower() == "threedqueryresponsecallback")
            {

                authorize = true;

            };
            if (controllerName.ToLower() == "sendercashpickup" && actionName.ToLower() == "threedqueryresponsecallback")
            {

                authorize = true;

            };
            if (areaName.ToLower() == "agent" || controllerName.ToLower() == "fundaccount" && actionName.ToLower() == "threedqueryresponsecallback")
            {
                authorize = true;

            }; if (areaName.ToLower() == "admin" || controllerName.ToLower() == "bankaccountpaymentconfirmation" && actionName.ToLower() == "threedqueryresponsecallback")
            {

                authorize = true;

            };
            if (areaName.ToLower() == "admin" || controllerName.ToLower() == "staffkiipaywallet" && actionName.ToLower() == "threedqueryresponsecallback")
            {

                authorize = true;

            };
            if (areaName.ToLower() == "admin" || controllerName.ToLower() == "staffcashpickuptransfer" && actionName.ToLower() == "threedqueryresponsecallback")
            {

                authorize = true;

            };
            if (areaName.ToLower() == "admin" || controllerName.ToLower() == "staffothermobilewalletstransfer" && actionName.ToLower() == "threedqueryresponsecallback")
            {

                authorize = true;

            };
            if (areaName.ToLower() == "admin" || controllerName.ToLower() == "staffbankaccountdeposit" && actionName.ToLower() == "threedqueryresponsecallback")
            {

                authorize = true;

            };

            if (areaName.ToLower() == "mobile" || controllerName.ToLower() == "mobilepaymentbankdeposit" && actionName.ToLower() == "threedqueryresponsecallback")
            {

                authorize = true;

            };
            if (controllerName.ToLower() == "transferzerowebhook" && (actionName.ToLower() == "trasactioncreated" || actionName.ToLower() == "callback"))
            {

                authorize = true;
            }

            if (areaName.ToLower() == "kiipaypersonal")
            {
                if (Common.CardUserSession.LoggedCardUserViewModel == null)
                {
                    if (controllerName.ToLower() == "login" || controllerName.ToLower() == "signup" || (controllerName.ToLower() == "dashboard" && actionName.ToLower() == "home"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }


            if (controllerName.ToLower() == "transferzerowebhook" && actionName.ToLower() == "GetRates")
            {
                if (Common.AdminSession.AgentStaffInfoAndLoginViewModel == null)
                {
                    return false;
                }
            }
            //if (areaName.ToLower() == "kiipaybusiness")
            //{
            //    if (Common.BusinessSession.LoggedKiiPayBusinessUserInfo == null)
            //    {
            //        if (controllerName.ToLower() == "login")
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //}
            if (Common.FaxerSession.DemoLoginModel != null)
            {
                authorize = true;
            }
            if (areaName.ToLower() == "agent")
            {
                if ((controllerName.ToLower() == "agentlogin" || controllerName.ToLower() == "agentregistration"))
                {

                    authorize = true;
                }
                else if (Common.AgentSession.LoggedUser != null)
                {
                    authorize = true;
                }
                else
                {
                    authorize = false;
                }

            }

            if (authorize == true)
            {
                if (areaName.ToLower() == "")
                {

                    if (Common.FaxerSession.LoggedUser != null)
                    {

                        var isActive = Common.Common.UserIsActive();

                        if (!isActive)
                        {
                            authorize = false;
                            Common.FaxerSession.LoggedUser = null;

                        }
                    }
                }
            }



            return authorize;
        }



        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            if (areaName.ToLower() == "kiipaypersonal")
            {

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "LogIn", area = "kiipaypersonal" }));
            }
            else if (areaName.ToLower() == "agent")
            {

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "AgentLogin", area = "Agent" }));
            }


            else
            {
                //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "demologin", area = "" }));
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "", controller = "", area = "" }));
            }

        }


    }

    public class PreventSpamAttribute : System.Web.Mvc.ActionFilterAttribute
    {

        // This stores the time between Requests (in seconds)
        public int DelayRequest = 10;
        // The Error Message that will be displayed in case of 
        // excessive Requests
        public string ErrorMessage = "Excessive Request Attempts Detected.";
        // This will store the URL to Redirect errors to
        public string RedirectURL;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var request = filterContext.HttpContext.Request;
            // Store our HttpContext.Cache (for easier reference and code brevity)
            var cache = filterContext.HttpContext.Cache;

            // Grab the IP Address from the originating Request (example)
            var originationInfo = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;

            // Append the User Agent
            originationInfo += request.UserAgent;

            // Now we just need the target URL Information
            var targetInfo = request.RawUrl + request.QueryString;

            // Generate a hash for your strings (appends each of the bytes of
            // the value into a single hashed string
            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(originationInfo + targetInfo)).Select(s => s.ToString("x2")));

            // Checks if the hashed value is contained in the Cache (indicating a repeat request)
            if (cache[hashValue] != null)
            {
                // Adds the Error Message to the Model and Redirect
                filterContext.Controller.ViewData.ModelState.AddModelError("ExcessiveRequests", ErrorMessage);
            }
            else
            {
                // Adds an empty object to the cache using the hashValue
                // to a key (This sets the expiration that will determine
                // if the Request is valid or not)
                cache.Add(hashValue, DateTime.Now.AddSeconds(DelayRequest), null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                //cache.Add(hashValue, null, null, DateTime.Now.AddSeconds(DelayRequest), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            base.OnActionExecuting(filterContext);
        }
    }

}
