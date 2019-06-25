using System;
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
	public class LifeInCitiesController : ApiController
	{
		private DBBTAEntities db = new DBBTAEntities();

		//// GET: api/LifeInCities
		//public IQueryable<LifeInCity> GetLifeInCities()
		//{
		//	return db.LifeInCities;
		//}

		//************************************************
		//  GET: api/LifeInCities/5
		// list of LifeinCity for selected Destination
		// for all categories
		//*************************************************
		//[ResponseType(typeof(Destination))]
		[Authorize]
		[HttpGet]
		public IHttpActionResult GetLIfeinCity([FromUri] int id)
		{
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);
			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//dal postoji destin. za zadati id
			bool idDestExists = db.Destinations.Any(x => x.ID.Equals(id));

			if (!idDestExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Destination invalid"));

			//ako je admin, pošalji mu i datum izmene destination i ko je izmenio
			if (isAdmin) {
				var LIfeinCity1 = (
					from c in db.LifeInCities
					where c.IDDestination == id
					select new { c.ID, c.Description, c.ChangeDate, c.IDAdmin, c.IDAccTraDesCategory }).ToList();

				if (LIfeinCity1 == null)
					return NotFound();
				return Ok(LIfeinCity1);
			}
			//za običnog usera šaljem bez toga
			else {
				var LIfeinCity1 = (
					from c in db.LifeInCities
					where c.IDDestination == id
					select new { c.ID, c.Description, c.IDAccTraDesCategory }).ToList();

				if (LIfeinCity1 == null)
					return NotFound();
				return Ok(LIfeinCity1);
			}
		}

		//************************************************
		//  GET: api/LifeInCities/5/1
		// list of LifeinCity for selected Destination
		// and selected category
		//*************************************************
		//[ResponseType(typeof(Destination))]
		[Authorize]
		[HttpGet]
		public IHttpActionResult GetLIfeinCity([FromUri] int id, int id2)
		{
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);
			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//dal postoji destin. za zadati
			bool idDestExists = db.Destinations.Any(x => x.ID.Equals(id));

			if (!idDestExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Destination invalid"));

			//dal postoji kategorija za zadati id
			bool idCetegorytExists = db.AccTraDesCategories.Any(x => x.ID.Equals(id));
			if (!idCetegorytExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Category invalid"));

			//ako je admin, pošalji mu i datum izmene destination i ko je izmenio
			if (isAdmin) {
				var LIfeinCity1 = (
					from c in db.LifeInCities
					where (c.IDDestination == id && c.IDAccTraDesCategory == id2)
					select new { c.ID, c.Description, c.ChangeDate, c.IDAdmin}).ToList();

				if (LIfeinCity1 == null)
					return NotFound();
				return Ok(LIfeinCity1);
			}
			//za običnog usera đaljem bez toga
			else {
				var LIfeinCity1 = (
					from c in db.LifeInCities
					where (c.IDDestination == id && c.IDAccTraDesCategory == id2)
					select new { c.ID, c.Description}).ToList();

				if (LIfeinCity1 == null)
					return NotFound();
				return Ok(LIfeinCity1);
			}
		}

		//********************************************
		// PUT: api/LifeInCities/5
		// edit Life in city item, samo admin
		//********************************************
		[HttpPut]
		[Authorize]
		[ResponseType(typeof(void))]
		public IHttpActionResult PutLifeInCity(LifeInCity life1)
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

			//postoji li LifeInCity prema iD-ju ?
			bool LifeincityExists = db.LifeInCities.Any(x => x.ID.Equals(life1.ID));
			//ako nema
			if (!LifeincityExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "LifeinCIty Item not found"));

			life1.ChangeDate = System.DateTime.UtcNow;
			life1.IDAdmin = empLogged.ID;

			//polja IDDestination i IDAccTraDesCategory se ne menjaju
			db.LifeInCities.Attach(life1);
			db.Entry(life1).Property("Description").IsModified = true;
			db.SaveChanges();

			return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "Item changed"));
		}

		//************************************************************
		// POST: api/LifeInCities
		// Dodati novu LifeInCity, tj dodaje deskripciju za
		// novu kategoriju za datu destinaciju. Ne dozvoljava ako
		// ta kategorija za tu dest. već postoji. Samo admin
		//************************************************************
		[HttpPost]
		[Authorize]
		public IHttpActionResult PostDestination (LifeInCity Life1) {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//ako mije admin
			if (!isAdmin)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			//da li postoji destincija u listi destinacija
			bool DestExists = db.Destinations.Any(e => e.ID.Equals(Life1.IDDestination));

			if (!DestExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Destination not valid!"));

			//da li postoji kategorija u listi kategorija
			bool CatExists = db.AccTraDesCategories.Any(e => e.ID.Equals(Life1.IDAccTraDesCategory));

			if (!CatExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Category not valid!"));

			//da li postoji već opis te kategorije za taj grad
			bool DestAndCategExists = db.LifeInCities.Any(x => (x.IDAccTraDesCategory.Equals(Life1.IDAccTraDesCategory) && x.IDDestination.Equals(Life1.IDDestination)));

			//ako već ima, nemoj dodati
			if (DestAndCategExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Category alredy exists for given destination"));

			//inače dodaj
			else {
				Life1.ChangeDate = System.DateTime.UtcNow;
				Life1.IDAdmin = empLogged.ID;

				db.LifeInCities.Add(Life1);
				db.SaveChanges();

				return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, "Category added"));
			}
		}


		//************************************************************
		// DELETE: api/LifeInCities/5/1
		// briše Lifeincity, tj., data kategorija za dati
		// grad, samo admin
		//************************************************************
		[HttpDelete]
		[Authorize]
		public IHttpActionResult DeleteLifeInCity([FromUri] int id, [FromUri] int id2 = -1)
		{
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));
			Country country = db.Countries.Find(id);

			if (!isAdmin)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			//da li postoji destincija u listi destinacija
			bool DestExists = db.Destinations.Any(e => e.ID.Equals(id));

			if (!DestExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Destination not valid!"));

			//da li postoji kategorija u listi kategorija
			bool CatExists = db.AccTraDesCategories.Any(e => e.ID.Equals(id2));

			if (!CatExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Category not valid!"));

			//da li postoji LifeInCity te kategorije za taj grad
			LifeInCity life1 = db.LifeInCities.Where(x => (x.IDAccTraDesCategory.Equals(id2) && x.IDDestination.Equals(id))).FirstOrDefault();

			//ako nema, nema ni brisanja
			if (life1== null )
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Category dont exist for given destination"));

			//inače briši
			else {
				db.LifeInCities.Remove(life1);
				db.SaveChanges();

				return ResponseMessage(Request.CreateResponse(HttpStatusCode.Moved, "Item Deleted"));
			}
		}

		//************************************************
		//vraća autorizovanog usera i njegov EmployeeType
		//************************************************
		private CTEmployee GetLoggedEmp(out bool isAdmin)
		{
			isAdmin = false;

			string Mail = ControllerContext.RequestContext.Principal.Identity.Name;
			CTEmployee empl1 = db.CTEmployees.Where(x => x.Email.Equals(Mail)).FirstOrDefault();

			if (empl1 != null)
			{
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

		private bool LifeInCityExists(int id)
		{
			return db.LifeInCities.Count(e => e.ID == id) > 0;
		}
	}
}