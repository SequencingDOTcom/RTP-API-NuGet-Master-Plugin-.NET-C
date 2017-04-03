using System.Web.Mvc;
using Sequencing.MusterPluginSample.Web.Models;

namespace Sequencing.MusterPluginSample.Web.Controllers
{
    /// <summary>
    /// Base controller class for all controllers
    /// </summary>
    public class ControllerBase : Controller
    {
        public SharedContext Context { get; set; }

        public const string REDIRECT_URI_PAR = "r";
    }
}