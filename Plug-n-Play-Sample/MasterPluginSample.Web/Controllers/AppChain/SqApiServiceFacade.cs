using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web;
using log4net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using Sequencing.AppChainsSample;
using Sequencing.MasterPluginSample.Web.Controllers.OAuth;

namespace Sequencing.MasterPluginSample.Web.Controllers.AppChain
{
    /// <summary>
    /// SqApiServiceFacade class is responsible for calling SQAPI operations (including app-chain creation, querying statuses and getting results
    /// </summary>
    public class SqApiServiceFacade
    {
        private string url;
        private UserAuthWorker userAuthWorker;

        public SqApiServiceFacade(string url)
        {
            this.url = url;
            userAuthWorker = new UserAuthWorker();
        }

        public SqApiServiceFacade(string apiUrl, string userName)
        {
            url = apiUrl;
            userAuthWorker = new UserAuthWorker(userName);
        }

        /// <summary>
        /// Returns results of executed app chain
        /// </summary>
        /// <param name="idJob">app chain job id</param>
        /// <returns></returns>
        public Dictionary<string, string> StartAppChains(Dictionary<string, string> jobParms)
        {
            Dictionary<string, string> jobResults = new Dictionary<string, string>();
            var authToken = (string) HttpContext.Current.Session["AuthToken"];
            if (authToken == null)
                return null;

            var chains = new AppChains(authToken, url + "/v2", Options.FrontendUrl);
            var jobReports= chains.GetReportBatch(jobParms);

            foreach (var report in jobReports)
            {
                foreach (var res in report.Value.getResults())
                {
                    ResultType type = res.getValue().getType();
                    if (type == ResultType.TEXT)
                    {
                        var valueResult = (TextResultValue)res.getValue();
                        if (report.Key.Equals(SqApiServiceFacade.MELANOMA_APP_CHAIN_ID) && res.getName().Equals("RiskDescription"))
                            jobResults[SqApiServiceFacade.MELANOMA_APP_CHAIN_ID] = valueResult.Data;
                        else if (report.Key.Equals(SqApiServiceFacade.VITD_APP_CHAIN_ID) && res.getName().Equals("result"))
                            jobResults[SqApiServiceFacade.VITD_APP_CHAIN_ID] = valueResult.Data.Equals("No") ? "False" : "True";
                    }
                }
            }
            return jobResults;
        }
        public const string MELANOMA_APP_CHAIN_ID = "Chain9";
        public const string VITD_APP_CHAIN_ID = "Chain88";


    }
}