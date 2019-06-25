using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using DBAccess;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace BTA_WebApi.Providers
{
	public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
	{
		private readonly string _publicClientId;


		//*******************************************************
		public ApplicationOAuthProvider (string publicClientId) {
			if (publicClientId == null) {
				throw new ArgumentNullException("publicClientId");
			}

			_publicClientId = publicClientId;
		}


		//*******************************************************
		public override async Task GrantResourceOwnerCredentials
		(OAuthGrantResourceOwnerCredentialsContext context) {

			string mail = null;
			using (DBBTAEntities obj = new DBBTAEntities())
			{
				string contMail = context.UserName.Trim();
				string contPass = context.Password.Trim();

				CTEmployee emp1 = obj.CTEmployees.Where (
					x => x.Email == contMail).FirstOrDefault();

				if (emp1 != null) {
					bool PassEq = encrypt.decryptPass(emp1.Pass) == contPass;
					mail = emp1.Email;

					if (!PassEq) {
						context.SetError("invalid_grant",
						"The password is incorrect.");
						return;
					}
				}
				else {
					context.SetError("invalid_grant",
					"User name not found.");
					return;
				}
			}

			ClaimsIdentity oAuthIdentity =
			new ClaimsIdentity(context.Options.AuthenticationType);
			ClaimsIdentity cookiesIdentity =
			new ClaimsIdentity(context.Options.AuthenticationType);

			//ovo dodajem
			oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, mail));

			AuthenticationProperties NameProperty = CreateProperties(context.UserName);

			AuthenticationTicket ticket =
			new AuthenticationTicket(oAuthIdentity, NameProperty);
			context.Validated(ticket);
			context.Request.Context.Authentication.SignIn(cookiesIdentity);
		}


		//***********************************************************
		public override Task TokenEndpoint (OAuthTokenEndpointContext context) {
			foreach (KeyValuePair<string,
			string> property in context.Properties.Dictionary) {
				context.AdditionalResponseParameters.Add(property.Key, property.Value);
			}

			//dodajem
			//context.AdditionalResponseParameters.Add("ID", context.Identity.GetUserId<int>());

			return Task.FromResult<object>(null);
		}

		//***********************************************************
		public override Task ValidateClientAuthentication
		(OAuthValidateClientAuthenticationContext context) {
			// Resource owner password credentials does not provide a client ID.
			if (context.ClientId == null) {
				context.Validated();
			}

			return Task.FromResult<object>(null);
		}

		//***********************************************************
		public override Task ValidateClientRedirectUri
		(OAuthValidateClientRedirectUriContext context) {
			if (context.ClientId == _publicClientId) {
				Uri expectedRootUri = new Uri(context.Request.Uri, "/");

				if (expectedRootUri.AbsoluteUri == context.RedirectUri) {
					context.Validated();
				}
			}

			return Task.FromResult<object>(null);
		}

		//***********************************************************
		public static AuthenticationProperties CreateProperties (string UserName) {
			IDictionary<string, string>
			data = new Dictionary<string, string>
			{
				{ "UserName", UserName }
			};
			return new AuthenticationProperties(data);
		}
	}
}