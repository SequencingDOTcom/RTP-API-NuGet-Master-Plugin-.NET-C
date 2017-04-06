using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Sequencing.MasterPluginSample.Web.Controllers.AppChain;
using Sequencing.MasterPluginSample.Web.Controllers.OAuth;
using Sequencing.MasterPluginSample.Web.Models;
using Newtonsoft.Json;
using Sequencing.MasterPluginSample.Web.Controllers;
using log4net;


namespace Sequencing.MasterPluginSample.Web.Controllers
{

    public class DefaultController : ControllerBase
    {
        private readonly AuthWorker authWorker = new AuthWorker(Options.OAuthUrl, Options.OAuthRedirectUrl, Options.OAuthSecret, Options.OAuthAppId, Options.Scope);
        public ILog log = LogManager.GetLogger(typeof(DefaultController));


        /// <summary>
        /// Landing page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Startup()
        {
            return View();
        }

        /// <summary>
        /// OAuth workflow supporting callback action
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult AuthCallback(string code)
        {
            if (code != null)
            {
                var _authInfo = authWorker.GetAuthInfo(code);
                if (_authInfo.Success)
                {
                    Context.AuthToken = _authInfo.Token.access_token;
                    Session["AuthToken"] = _authInfo.Token.access_token;
                    var context = new CommonData();
                    context.Context = Context;
                    return View("SelectFile", context);
                }
                return new ContentResult { Content = "Error while retrieving access token:" + _authInfo.ErrorMessage };
            }
            return new ContentResult { Content = "User cancelled the auth sequence" };
        }
           
        /// <summary>
        /// File selection
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult SelectFile()
        {
            var context = new CommonData();
            context.Context = Context;
            return View("SelectFile", context);
        }

        /// <summary>
        /// Starts app chains
        /// </summary>
        /// <param name="selectedId"></param>
        /// <returns></returns>
        [Authorize]
        public CheckAppData StartJob(string selectedId)
        {
            try
            {
                var _srv = new SqApiServiceFacade(Options.ApiUrl);

                var appChainsParms = new Dictionary<string, string>()
                {
                    {SqApiServiceFacade.MELANOMA_APP_CHAIN_ID, selectedId},
                    {SqApiServiceFacade.VITD_APP_CHAIN_ID, selectedId}
                };
                var appChainsResult = _srv.StartAppChains(appChainsParms);
                if (appChainsResult == null)
                    return null;
                return new CheckAppData()
                {
                    selectedId = Context.DataFileId,
                    melanomaRisk = appChainsResult[SqApiServiceFacade.MELANOMA_APP_CHAIN_ID],
                    vitD = appChainsResult[SqApiServiceFacade.VITD_APP_CHAIN_ID]
                };
            }
            catch (Exception ex)
            {
                return null;
            }
                
        }


        /// <summary>
        /// Initiates oauth sequence
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult StartAuth()
        {
            return Redirect(authWorker.GetAuthUrl());
        }

        /// <summary>
        /// Logs out user
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Startup");
        }

        public const int MINUTES_FOR_RESULTS_REFRESH = 60;

        /// <summary>
        /// Checks if dashboard page shall be renewed
        /// </summary>
        /// <param name="jobdt"></param>
        /// <returns></returns>
        public bool CheckForJobRefresh(DateTime jobdt)
        {
            if (DateTime.Now.Subtract(jobdt).TotalMinutes > MINUTES_FOR_RESULTS_REFRESH)
                return true;
            return false;
        }

        [Authorize]
        public ActionResult SaveFile(string selectedId, string selectedName)
        {
            var context = new CommonData();
            Context.DataFileId = selectedId;
            context.Context = Context;
            Session["DataFile"] = selectedId;
            return View("Index", context);
        }

        [Authorize]
        public ActionResult GetAppChainResult(string appCode, string dataFileId)
        {         

            
            if (dataFileId == null)
                dataFileId = (string)Session["DataFile"];
            var appResults = StartJob(dataFileId);

            if (appResults == null)
            {
                ViewBag.Error =
                    "Personalization is not possible due to insufficient genetic data in the selected file. Choose a different genetic data file";
                return View("Index", new CommonData() { Context = Context });
            }
               
            switch (appCode)
            {
                case "Chain9":
                    ViewBag.MelanomaRisk = appResults.melanomaRisk;
                    ViewBag.VitaminD = null;
                    break;
                case "Both":
                    ViewBag.MelanomaRisk = appResults.melanomaRisk;
                    ViewBag.VitaminD = appResults.vitD;
                    break;
                case "Chain88":
                    ViewBag.MelanomaRisk = null;
                    ViewBag.VitaminD = appResults.vitD;
                    break;
            }
                    
            return View("Index", new CommonData() {Context = Context });
        }
    }
}