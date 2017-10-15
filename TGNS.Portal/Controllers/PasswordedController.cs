using System.Configuration;
using System.Security;
using System.Web.Mvc;

namespace TGNS.Portal.Controllers
{
    public class PasswordedEndpointsController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var password = Request.QueryString["password"] as string;
            if (!string.Equals(password, ConfigurationManager.AppSettings["EndpointsPassword"]))
            {
                throw new SecurityException("Password incorrect.");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}