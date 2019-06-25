using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BTA_WebApi.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace BTA_WebApi.App_Start
{
	public partial class Startup
	{
		public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

		public static string PublicClientId { get; private set; }


		public void ConfigureAuth (IAppBuilder app) {
			// Configure the application for OAuth based flow
			PublicClientId = "self";
			OAuthOptions = new OAuthAuthorizationServerOptions {
				TokenEndpointPath = new PathString("/Token"),
				Provider = new ApplicationOAuthProvider(PublicClientId),
				AuthorizeEndpointPath =
					new PathString("/api/Account/ExternalLogin"),
				AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(10),
				// In production mode set AllowInsecureHttp = false
				AllowInsecureHttp = true
			};

			// Enable the application to use bearer tokens to authenticate users
			app.UseOAuthBearerTokens(OAuthOptions);
		}
	}
}