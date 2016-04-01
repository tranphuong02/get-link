using Framework.Datatable.RequestBinder;
using Framework.DI.Unity;
using Framework.Utility;
using Microsoft.Practices.Unity;
using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Transverse.Utils.ModelBinders;
using Web.Models;
using UnityDependencyResolver = Framework.DI.Unity.UnityDependencyResolver;

namespace Web
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // DI
            DependencyInjectionConfiguration();

            // migration database
            DatabaseMirgation.ApplyDatabaseMigrations();

            // Data tables
            DataTableConfiguration();

            // MVC ValidateAntiForgeryToken config
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }

        /// <summary>
        ///     Receive all errors of the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            
            Response.Clear();

            // Log exception
            Framework.Logger.Log4Net.Provider.Instance.LogError(exception);

            Server.ClearError();

            var httpEx = exception as HttpException;
            string currentDomain = DomainHelper.FullyQualifiedApplicationPath;
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            if (httpEx != null && httpEx.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
            {
                //handle the error
                var path = urlHelper.Action("TooBigFile", "Error", new { Area = string.Empty }).WithoutRootPath().TrimStart('/');
                Response.Redirect(Path.Combine(currentDomain, path), true);
            }
            if (httpEx != null && httpEx.GetHttpCode() == (int)HttpStatusCode.Forbidden)
            {
                var path = urlHelper.Action("UrlInvalid", "Error", new { Area = string.Empty }).WithoutRootPath().TrimStart('/');
                Response.Redirect(Path.Combine(currentDomain, path), true);
            }
            else if (httpEx != null && httpEx.GetHttpCode() == (int)HttpStatusCode.NotFound)
            {
                var path = urlHelper.Action("PageNotFound", "Error", new { Area = string.Empty }).WithoutRootPath().TrimStart('/');
                Response.Redirect(Path.Combine(currentDomain, path), true);
            }
            else
            {
                var path = urlHelper.Action("Index", "Error", new { Area = string.Empty }).WithoutRootPath().TrimStart('/');
                Response.Redirect(Path.Combine(currentDomain, path), true);
            }

            Response.End();
        }

        /// <summary>
        ///     DI configuration with Unity Framework
        /// </summary>
        private static void DependencyInjectionConfiguration()
        {
            IUnityContainer unityContainer = IoCFactory.Instance.CurrentContainer.Container;

            UnityServices.AutoRegister(unityContainer);

            // Web controllers register
            DependencyResolver.SetResolver(new UnityDependencyResolver(unityContainer));

            // Web controllers API register

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(unityContainer);
        }

        /// <summary>
        ///     Data table service configuration
        /// </summary>
        private static void DataTableConfiguration()
        {
            ModelBinders.Binders.Add(typeof(IDataTablesRequest), new DataTablesBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
        }
    }
}