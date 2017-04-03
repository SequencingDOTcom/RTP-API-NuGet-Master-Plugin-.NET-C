using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sequencing.MusterPluginSample.Web.Controllers.OAuth;
using Sequencing.MusterPluginSample.Web.Models;

namespace Sequencing.MusterPluginSample.Web.Controllers
{
    /// <summary>
    /// Action filter attribute for filling SharedContext data
    /// </summary>
    public class CommonDataModelAttribute : ActionFilterAttribute
    {

        public CommonDataModelAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as ControllerBase;
            if (controller != null)
            {
                (controller).Context = LoadContext(filterContext.RequestContext);
            }
            
            base.OnActionExecuting(filterContext);
        }

        protected SharedContext LoadContext(RequestContext ctx)
        {
            var _sharedContext = new SharedContext();
            var _user = ctx.HttpContext.User;
            if (_user.Identity.IsAuthenticated)
            {
                _sharedContext.IsAuthenticated = true;
                _sharedContext.UserName = _user.Identity.Name;
                _sharedContext.AuthToken = (string)HttpContext.Current.Session["AuthToken"] == null? null : (string)HttpContext.Current.Session["AuthToken"];
            }
            else
                _sharedContext.IsAuthenticated = false;
            return _sharedContext;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var model = filterContext.Controller.ViewData.Model as CommonData;
            var controller = filterContext.Controller as ControllerBase;
            
            if (model != null)
            {
                model.Context = controller != null && controller.Context != null
                    ? controller.Context
                    : LoadContext(filterContext.RequestContext);
            }

            base.OnResultExecuting(filterContext);
        }
    }
}