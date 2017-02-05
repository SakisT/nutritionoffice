using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace nutritionoffice.CustomHelpers
{
    public static class CustomHelpers
    {
        public static IHtmlString ExportTo(this HtmlHelper helper, string Action, string Controller, object RouteValues)
        {

            string linktext = "<div class='form-group ExportTo'>" +
                  "<label for='exprttoselect' style='margin-top:7px'>" + Resource.ExportTo + "</label>" +
                  "<button data-link='#' class='btn btn-primary btn-xs ExportToButton'><span class='glyphicon glyphicon-export'></span> " + Resource.Export + "</button>" +
                  "<select class='form-control'>" +
                  "<option value='EXCEL'>Excel</option>" +
                  "<option selected='selected' value='WORD'>Word</option>" +
                  "<option value='PDF'>PDF</option>" +
                  "<option value='IMAGE'>Image</option>" +
                  "</select>" +
                  "</div>";
            System.Web.Routing.RouteValueDictionary dictionary = new System.Web.Routing.RouteValueDictionary(RouteValues);
            dictionary.Add("filetype", "filetypetext");
            UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            var value = urlHelper.Action(actionName: Action, controllerName: Controller, routeValues: dictionary);

            TagBuilder tag = new TagBuilder("div");
            tag.InnerHtml = linktext.Replace("#", VirtualPathUtility.ToAbsolute(value));
            return new MvcHtmlString(tag.InnerHtml.ToString());
        }

    }
}