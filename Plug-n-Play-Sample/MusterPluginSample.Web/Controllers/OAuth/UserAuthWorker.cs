using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using Sequencing.MusterPluginSample.Web.Models;


namespace Sequencing.MusterPluginSample.Web.Controllers.OAuth
{
    /// <summary>
    /// User authentication worker - stores/retrieves it from data source
    /// </summary>
    public class UserAuthWorker
    {
       

        private readonly string userNameOvr;

        public UserAuthWorker()
        {
        }

        public UserAuthWorker(string userNameOvr)
        {
            this.userNameOvr = userNameOvr;
        }

    }
}