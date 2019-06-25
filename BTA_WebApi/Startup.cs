using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BTA_WebApi.App_Start.Startup))]

namespace BTA_WebApi.App_Start
{
	public partial class Startup
	{
		public void Configuration (IAppBuilder app) {
			ConfigureAuth(app);
		}
	}
   
}