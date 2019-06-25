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

namespace BTA_WebApi.Controllers{
	public class AccTraDesCategoriesController : ApiController {

		private DBBTAEntities db = new DBBTAEntities();

		//*************************************************
		// GET: api/AccTraDesCategories/GetAccTraDesCategories/
		// list of all AccTraDesCategories 
		//*************************************************
		[Authorize]
		[HttpGet]
		public IHttpActionResult GetAccTraDesCategories () {
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);
			
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			var CatList = new List<Tuple<int, string>>().Select(t=> new { Id= t.Item1, AccTraDesType = t.Item2}).ToList();

			foreach (AccTraDesCategory categ1 in db.AccTraDesCategories) {
				CatList.Add(new { Id = categ1.ID, AccTraDesType = categ1.AccTraDesType });
			}

			if (CatList == null) return StatusCode(HttpStatusCode.NoContent);

			return Ok(CatList);
		}

		/*
		// GET: api/AccTraDesCategories/5
		[ResponseType(typeof(AccTraDesCategory))]
		public IHttpActionResult GetAccTraDesCategory(int id) {
			AccTraDesCategory accTraDesCategory = db.AccTraDesCategories.Find(id);
			if (accTraDesCategory == null)
				return NotFound();

			return Ok(accTraDesCategory);
		}*/

		//*******************************************************
		// PUT: api/AccTraDesCategories/PutAccTraDesCategory/5
		// edit country's properties, admin only
		[ResponseType(typeof(void))]
		[HttpPut]
		[Authorize]
		public IHttpActionResult PutAccTraDesCategory(int id, AccTraDesCategory accTraDesCategory) {
			if (!ModelState.IsValid)
				 return BadRequest(ModelState);

			if (id != accTraDesCategory.ID)
				return BadRequest();

			db.Entry(accTraDesCategory).State = EntityState.Modified;

			try {
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException) {
				if (!AccTraDesCategoryExists(id))
					return NotFound();
				else
					throw;
			}

			return StatusCode(HttpStatusCode.NoContent);
		}

		// POST: api/AccTraDesCategories
		[ResponseType(typeof(AccTraDesCategory))]
		public IHttpActionResult PostAccTraDesCategory(AccTraDesCategory accTraDesCategory) {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			db.AccTraDesCategories.Add(accTraDesCategory);
			db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = accTraDesCategory.ID }, accTraDesCategory);
		}

		// DELETE: api/AccTraDesCategories/5
		[ResponseType(typeof(AccTraDesCategory))]
		public IHttpActionResult DeleteAccTraDesCategory(int id) {
			AccTraDesCategory accTraDesCategory = db.AccTraDesCategories.Find(id);
			if (accTraDesCategory == null)
				 return NotFound();

			db.AccTraDesCategories.Remove(accTraDesCategory);
			db.SaveChanges();

			return Ok(accTraDesCategory);
		}

		private CTEmployee GetLoggedEmp (out bool isAdmin) {
			isAdmin = false;

			string Mail = ControllerContext.RequestContext.Principal.Identity.Name;
			CTEmployee empl1 = db.CTEmployees.Where(x => x.Email.Equals(Mail)).FirstOrDefault();

			if (empl1 != null) {
				isAdmin = (empl1.EmployeeType == 0);
			}
			return empl1;
		}

		protected override void Dispose(bool disposing){
			if (disposing)
				 db.Dispose();
			base.Dispose(disposing);
		}

		private bool AccTraDesCategoryExists(int id){
			return db.AccTraDesCategories.Count(e => e.ID == id) > 0;
		}
	}
}
 