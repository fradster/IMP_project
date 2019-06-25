using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTA_WebApi.Controllers
{
	public class HomeController : Controller
	{
		private DBBTAEntities db = new DBBTAEntities();

		public ActionResult Index()
		{
			ViewBag.Title = "Home Page";

			return View();
		}


		//******************************************
		//*drugi stepen aktivacije naloga
		public ActionResult Activation()
		{
			string poruka1 = String.Empty;
			string Guidobj1 = RouteData.Values["id"].ToString();

			Guid activationCode1 = new Guid();

			if (Guid.TryParse(Guidobj1, out activationCode1) && Guidobj1 != null)
			{

				Employee_Activation Activation1 = db.Employee_Activations.Where(p => p.ActivationCode.Equals(activationCode1)).FirstOrDefault();

				if (Activation1 != null)
				{
					db.Employee_Activations.Remove(Activation1);
					db.SaveChanges();

					CTEmployee user1 = db.CTEmployees.Find(Activation1.Id);

					ViewBag.message = "Activation successful! You can Login now.";
					return View();
				}
			}
			ViewBag.message = "Activation unsuccessful!";
			return View();
		}


		//********************************
		//GET
		public ActionResult enkriptuj () {
			return View();
		}

		//********************************
		//enciptuj POST
		[HttpPost]
		public ActionResult enkriptuj (string box1, string box2) {
			string pass1;
			string pass = box1;

			if (pass != null) {

				pass.Trim();

				pass1 = encrypt.encryptPass(pass);
				ViewBag.poruka = "password enkriptovan";
			}
			else 
				pass1 = String.Empty;

			return View("enkriptuj", "", pass1);
		}
	}
}
