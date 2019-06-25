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
	public class DestinationsController : ApiController
	{
		private DBBTAEntities db = new DBBTAEntities();

		//**************************************
		// GET: api/Destinations/GetDestinations/1
		// list of Destinations for selected country
		//***************************************
		[Authorize]
		//[ResponseType(typeof(Country))]
		public IHttpActionResult GetDestinations([FromUri] int id)
		{
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			var cities = (	from c in db.Destinations
								where c.IDCountry == id
								select new { c.ID, c.CityName}).ToList();

			if (cities == null)
				return NotFound();

			return Ok(cities);
		}


		//********************************************
		//  PUT: /api/destinations/PutDestination
		//	edit Destination
		//********************************************
		[HttpPut]
		[Authorize]
		[ResponseType(typeof(void))]
		public IHttpActionResult PutDestination([FromBody] Destination Dest1)
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

			//Postoji li Destination prema  ID-ju
			bool PutDestExists = db.Destinations.Any(x => x.ID.Equals(Dest1.ID));

			//ako nema
			if (!PutDestExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Destination invalid"));

			db.Entry(Dest1).State = EntityState.Modified;
			db.SaveChanges();
			return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "Podaci promenjeni"));
		}

		//************************************************************
		// POST: api/Destinations/PostDestination
		// Dodati novu zemlju, samo admin
		//************************************************************
		//[ResponseType(typeof(Destination))]
		[HttpPost]
		[Authorize]
		public IHttpActionResult PostDestination(Destination Dest1)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//da li već postoji dest
			bool DestExists = db.Destinations.Any(e => e.CityName.Equals(Dest1.CityName, StringComparison.OrdinalIgnoreCase));

			if (DestExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Destination already exists!"));

			db.Destinations.Add(Dest1);
			db.SaveChanges();

			return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, "Destination added"));
		}

		//************************************************************
		// DELETE: api/Destinations/DeleteDestination/5
		// briše zemlju, samo admin
		//************************************************************
		[HttpDelete]
		[Authorize]
		public IHttpActionResult DeleteDestination(int id)
		{
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));
			Country country = db.Countries.Find(id);

			if (!isAdmin)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			Destination Dest2Delete = db.Destinations.Find(id);

			if (Dest2Delete == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Item not found!"));

			db.Destinations.Remove(Dest2Delete);
			db.SaveChanges();

			return ResponseMessage(Request.CreateResponse(HttpStatusCode.Moved, "Item Deleted"));
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

		protected override void Dispose (bool disposing) {
			if (disposing) {
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}