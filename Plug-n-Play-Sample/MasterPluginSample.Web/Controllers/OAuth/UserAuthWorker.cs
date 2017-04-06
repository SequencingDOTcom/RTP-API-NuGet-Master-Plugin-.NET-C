using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using Sequencing.MasterPluginSample.Web.Models;


namespace Sequencing.MasterPluginSample.Web.Controllers.OAuth
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