using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Homecare_Eksame_project.Filters
{
    public class CustomeAuthorize : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool baseAuth = httpContext.Request.IsAuthenticated;

            if (baseAuth && !string.IsNullOrWhiteSpace(Roles))
                return Roles.ToLower().Contains(((string)(httpContext.Session["RoleID"] ?? "")).ToLower());


            return baseAuth;
        }
    }
}