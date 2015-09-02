using System.Net;
using Microsoft.SharePoint.Client;


namespace Utility.SPClaimsAuth
{
    public static class ClaimClientContext
    {
        /// <summary>
        /// Displays a pop up to login the user. An authentication Cookie is returned if the user is sucessfully authenticated.
        /// </summary>
        /// <param name="targetSiteUrl"></param>
        /// <param name="popUpWidth"></param>
        /// <param name="popUpHeight"></param>
        /// <returns></returns>
        public static CookieCollection GetAuthenticatedCookies(string targetSiteUrl, int popUpWidth, int popUpHeight)
        {
            CookieCollection authCookie = CookieReader.ExtractAuthCookiesFromUrl(targetSiteUrl);

            if (authCookie != null && authCookie["FedAuth"] != null)
            {
                return authCookie;
            }

            using (ClaimsWebAuth webAuth = new ClaimsWebAuth(targetSiteUrl, popUpWidth,popUpHeight))
            {
                authCookie = webAuth.Show();
            }
            return authCookie;
        }

        /// <summary>
        /// Override for for displaying pop. Default width and height values are used for the pop up window.
        /// </summary>
        /// <param name="targetSiteUrl"></param>
        /// <returns></returns>
        public static ClientContext GetAuthenticatedContext(string targetSiteUrl)
        {
            return (GetAuthenticatedContext(targetSiteUrl, 0, 0));
        }

        /// <summary>
        /// This method will return a ClientContext object with the authentication cookie set.
        /// The ClientContext should be disposed of as any other IDisposable
        /// </summary>
        /// <param name="targetSiteUrl"></param>
        /// <returns></returns>
        public static ClientContext GetAuthenticatedContext(string targetSiteUrl, int popUpWidth, int popUpHeight)
        {
            CookieCollection cookies = null;

            cookies = ClaimClientContext.GetAuthenticatedCookies(targetSiteUrl, popUpWidth, popUpHeight);

            if (cookies == null) return null;

            ClientContext context = new ClientContext(targetSiteUrl);

            try
            {
                context.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs e)
                {
                    e.WebRequestExecutor.WebRequest.CookieContainer = new CookieContainer();

                    foreach (Cookie cookie in cookies)
                    {
                        e.WebRequestExecutor.WebRequest.CookieContainer.Add(cookie);
                    }
                };
            }
            catch
            {
                if (context != null) context.Dispose();
                throw;
            }

            return context;
        }
    }
}
