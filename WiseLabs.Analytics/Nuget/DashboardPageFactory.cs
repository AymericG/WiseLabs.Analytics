using System.Diagnostics;
using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using Encoding = System.Text.Encoding;

namespace WiseLabs.Analytics
{
    /// <summary>
    /// HTTP handler factory that dispenses handlers for rendering views and 
    /// resources needed to display the error log.
    /// </summary>

    public class DashboardPageFactory : IHttpHandlerFactory
    {
        IHttpHandler IHttpHandlerFactory.GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            return GetHandler(new HttpContextWrapper(context), requestType, url, pathTranslated);
        }

        /// <summary>
        /// Returns an object that implements the <see cref="IHttpHandler"/> 
        /// interface and which is responsible for serving the request.
        /// </summary>
        /// <returns>
        /// A new <see cref="IHttpHandler"/> object that processes the request.
        /// </returns>

        public virtual IHttpHandler GetHandler(HttpContextBase context, string requestType, string url, string pathTranslated)
        {
            //
            // The request resource is determined by the looking up the
            // value of the PATH_INFO server variable.
            //

            var request = context.Request;
            var resource = request.PathInfo.Length == 0
                         ? string.Empty
                         : request.PathInfo.Substring(1).ToLowerInvariant();

            var handler = CreateTemplateHandler<DashboardPage>();

            if (handler == null)
                throw new HttpException(404, "Resource not found.");

            return handler;
        }

        
        static IHttpHandler CreateTemplateHandler<T>() where T : WebTemplateBase, new()
        {
            return new DelegatingHttpHandler(context =>
            {
                var template = new T { Context = context };
                context.Response.Write(template.TransformText());
            });
        }

        /// <summary>
        /// Enables the factory to reuse an existing handler instance.
        /// </summary>

        public virtual void ReleaseHandler(IHttpHandler handler) { }

    }

    public interface IRequestAuthorizationHandler
    {
        bool Authorize(HttpContextBase context);
    }
}
