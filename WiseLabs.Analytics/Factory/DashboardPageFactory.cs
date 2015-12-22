using System;
using System.Collections;
using System.Globalization;
using System.Web;

namespace analytics
{

    public interface IHttpHandlerFactory
    {
        //
        // Summary:
        //     Returns an instance of a class that implements the System.Web.IHttpHandler interface.
        //
        // Parameters:
        //   context:
        //     An instance of the System.Web.HttpContext class that provides references to intrinsic
        //     server objects (for example, Request, Response, Session, and Server) used to
        //     service HTTP requests.
        //
        //   requestType:
        //     The HTTP data transfer method (GET or POST) that the client uses.
        //
        //   url:
        //     The System.Web.HttpRequest.RawUrl of the requested resource.
        //
        //   pathTranslated:
        //     The System.Web.HttpRequest.PhysicalApplicationPath to the requested resource.
        //
        // Returns:
        //     A new System.Web.IHttpHandler object that processes the request.
        IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated);
        //
        // Summary:
        //     Enables a factory to reuse an existing handler instance.
        //
        // Parameters:
        //   handler:
        //     The System.Web.IHttpHandler object to reuse.
        void ReleaseHandler(IHttpHandler handler);
    }



    public class DashboardPageFactory : IHttpHandlerFactory
    {
        private static readonly object _authorizationHandlersKey = new object();

        /// <summary>
        /// Returns an object that implements the <see cref="IHttpHandler"/> 
        /// interface and which is responsible for serving the request.
        /// </summary>
        /// <returns>
        /// A new <see cref="IHttpHandler"/> object that processes the request.
        /// </returns>

        public virtual IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            //
            // The request resource is determined by the looking up the
            // value of the PATH_INFO server variable.
            //

            string resource = context.Request.PathInfo.Length == 0 ? string.Empty :
                context.Request.PathInfo.Substring(1).ToLower(CultureInfo.InvariantCulture);

            IHttpHandler handler = FindHandler(resource);

            if (handler == null)
                throw new HttpException(404, "Resource not found.");

            //
            // Check if authorized then grant or deny request.
            //

         
            return handler;
        }

        private static IHttpHandler FindHandler(string name)
        {
            return null ;
        }

        /// <summary>
        /// Enables the factory to reuse an existing handler instance.
        /// </summary>

        public virtual void ReleaseHandler(IHttpHandler handler)
        {
        }

        /// <summary>
        /// Determines if the request is authorized by objects implementing
        /// <see cref="IRequestAuthorizationHandler" />.
        /// </summary>
        /// <returns>
        /// Returns zero if unauthorized, a value greater than zero if 
        /// authorized otherwise a value less than zero if no handlers
        /// were available to answer.
        /// </returns>

  

   

        internal static Uri GetRequestUrl(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            Uri url = context.Items["ELMAH_REQUEST_URL"] as Uri;
            return url != null ? url : context.Request.Url;
        }
    }





}