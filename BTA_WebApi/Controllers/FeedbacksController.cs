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
	public class FeedbacksController : ApiController {
		private DBBTAEntities db = new DBBTAEntities();

		//**********************************
		// GET: api/Feedbacks/GetFeedback/5
		// lista feedbacks-a za zadatu kategoriju
		[ResponseType(typeof(Feedback))]
		[Authorize]
		[HttpGet]
		public IHttpActionResult GetFeedback(int id) {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);
			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//Ima li usera prema traženom ID-ju
			bool UserExists = db.CTEmployees.Any(x => x.ID.Equals(id));
			//ako ga nema
			if (!UserExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Requested User invalid"));

			//ako je običan user, može da traži samo svoje podatke
			if (!isAdmin && empLogged.ID != id)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			var commentlist = new List<Tuple<int, int, DateTime, string, string, int?>>().Select(t => new {
				IdTypeCategory = t.Item1,
				IdCategory = t.Item2,
				Date = t.Item3,
				Header = t.Item4,
				Comment = t.Item5,
				rating = t.Item6
			}).ToList();

			//vadi sve komentare za zadatog usera
			foreach (Feedback feed1 in db.Feedbacks) {
				if (feed1.IDUser == id) {
					commentlist.Add(new {
						IdTypeCategory = getCatCode(feed1),
						IdCategory = getCatId(feed1),
						Date = feed1.FeedbackDate,
						Header = feed1.Heading,
						Comment = feed1.Comment,
						rating = feed1.Rating
					});
				}
			}
			if (commentlist.Count== 0) {
				return NotFound();
			}
			return Ok(commentlist);
		}

		//****************************************
		//gets category code for given feedback
		//returns first one thas not null
		//1 - LifeInCity
		//2 - Transportation
		//3 - Accomodation
		//****************************************
		private int getCatCode (Feedback feed1) {
			int Catcode1;

			if (feed1.IDLifeInCity != null)
				Catcode1 = 1;
			else if (feed1.IDTransportation != null)
				Catcode1 = 2;
			else
				Catcode1 = 3;
			return Catcode1;
		}

		//****************************************
		//gets Id of category for given feedback
		// returns first one thas not null
		//****************************************
		private int getCatId (Feedback feed1) {
			int CatId;

			if (feed1.IDLifeInCity != null)
				CatId = (int) feed1.IDLifeInCity;
			else if (feed1.IDTransportation != null)
				CatId = (int) feed1.IDTransportation;
			else
				CatId = (int) feed1.IDAccommodation;

			return CatId;
		}

		//********************************************
		// PUT: api/Feedbacks/5
		// api/Feedbacks/PutFeedback/5
		// prepravlja ostavljeni feedback
		// admin može sve, user samo svoj
		[ResponseType(typeof(void))]
		public IHttpActionResult PutFeedback([FromBody] Feedback fdb1){
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//postoji li Feedback prema iD-ju ?
			bool FeedbackExists = db.Feedbacks.Any(x => x.ID.Equals(fdb1.ID));
			//ako nema
			if (!FeedbackExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Feedback Item not found"));

			//if (id != feedback.ID)
			//{
			//	return BadRequest();
			//}

			//db.Entry(feedback).State = EntityState.Modified;

			//try
			//{
			//	db.SaveChanges();
			//}
			//catch (DbUpdateConcurrencyException)
			//{
			//	if (!FeedbackExists(id))
			//	{
			//		return NotFound();
			//	}
			//	else
			//	{
			//		throw;
			//	}
			//}

			return StatusCode(HttpStatusCode.NoContent);
		}

		// POST: api/Feedbacks
		[ResponseType(typeof(Feedback))]
		public IHttpActionResult PostFeedback(Feedback feedback)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.Feedbacks.Add(feedback);
			db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = feedback.ID }, feedback);
		}

		// DELETE: api/Feedbacks/5
		[ResponseType(typeof(Feedback))]
		public IHttpActionResult DeleteFeedback(int id)
		{
			Feedback feedback = db.Feedbacks.Find(id);
			if (feedback == null)
			{
				return NotFound();
			}

			db.Feedbacks.Remove(feedback);
			db.SaveChanges();

			return Ok(feedback);
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

		private bool FeedbackExists(int id)
		{
			return db.Feedbacks.Count(e => e.ID == id) > 0;
		}
	}
}