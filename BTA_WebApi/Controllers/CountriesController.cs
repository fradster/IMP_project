using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DBAccess;

namespace BTA_WebApi.Controllers
{
	public class CountriesController : ApiController
	{
		private DBBTAEntities db = new DBBTAEntities();

		//**************************************
		// GET: api/countries/GetCountries/
		// list of countries
		//***************************************
		[Authorize]
		public IHttpActionResult GetCountries()
		{
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji user koji je logovan
			if (empLogged == null)
			return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			var CountryList = new List<Tuple<int, string>>()
					.Select(t => new { Id = t.Item1, Name = t.Item2}).ToList();

			foreach (Country ct1 in db.Countries)
				CountryList.Add(new { Id = ct1.ID, Name = ct1.Name});

				return Ok(CountryList);
		}

		//**************************************
		// GET: api/countries/GetCountries/5
		// get selected country
		//***************************************
		[Authorize]
		[ResponseType(typeof(Country))]
		public IHttpActionResult GetCountry([FromUri] int id)
		{
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage (Request. CreateErrorResponse (HttpStatusCode.BadRequest, "User invalid"));

			//ako mije admin
			if (!isAdmin)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			Country CountryRequested = db.Countries.Find(id);

			if (CountryRequested == null)
				return NotFound();

			return Ok(new { Id= CountryRequested.ID, Name= CountryRequested.Name });
		}

		//**************************************
		// PUT: api/Countries/PutCountry
		// edit country's properties, admin only
		//**************************************
		[ResponseType(typeof(void))]
		[HttpPut]
		[Authorize]
		public IHttpActionResult PutCountry([FromBody] Country PutCountry)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//ako mije admin
			if (!isAdmin)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			//>Postoji li Country prema  ID-ju
			bool PutCountExists = db.Countries.Any(x => x.ID.Equals(PutCountry.ID));

			//ako nema
			if (!PutCountExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Country is invalid"));

			db.Entry(PutCountry).State = EntityState.Modified;
			db.SaveChanges();
			return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "Podaci promenjeni"));
		}

		//************************************************************
		// POST: api/Countries/PostCountry
		// Dodati novu zemlju, samo admin
		//************************************************************
		[HttpPost]
		[Authorize]
		//[ResponseType(typeof(Country))]
		public IHttpActionResult PostCountry( [FromBody] Country PostCountry)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//ako nije admin
			if (!isAdmin)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			//da li već postoji Country
			bool CountryExists = db.Countries.Any(e => e.Name == PostCountry.Name);

			if (CountryExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Country already exists!"));

			db.Countries.Add(PostCountry);
			db.SaveChanges();

			return ResponseMessage (Request.CreateResponse (HttpStatusCode.Created, "Country added")); ;
		}


		//************************************************************
		// DELETE: api/Countries/DeleteCountry/4
		// briše zemlju, samo admin
		//************************************************************
		//[ResponseType(typeof(Country))]
		[HttpDelete]
		[Authorize]
		public IHttpActionResult DeleteCountry(int id)
		{
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));
			Country country = db.Countries.Find(id);

			if (!isAdmin)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			Country DeleteCountry = db.Countries.Find(id);

			if (DeleteCountry == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Item not found!"));

			db.Countries.Remove(DeleteCountry);
			db.SaveChanges();

			return ResponseMessage(Request.CreateResponse(HttpStatusCode.Moved, "Item Deleted"));
		}

		//************************************************
		//vraća autorizovanog usera i njegov EmployeeType
		//************************************************
		private CTEmployee GetLoggedEmp (out bool isAdmin) {
			isAdmin = false;

			string Mail = ControllerContext.RequestContext.Principal.Identity.Name;
			CTEmployee empl1 = db.CTEmployees.Where(x => x.Email.Equals(Mail)).FirstOrDefault();

			if (empl1 != null) {
				isAdmin = (empl1.EmployeeType == 0);
			}
			return empl1;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool CountryExists(int id)
		{
			return db.Countries.Count(e => e.ID == id) > 0;
		}
	}
}