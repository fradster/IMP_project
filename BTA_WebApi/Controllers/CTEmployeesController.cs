using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using DBAccess;
//using Newtonsoft.Json.Linq;

namespace BTA_WebApi.Controllers
{
	public class CTEmployeesController : ApiController
	{
		private DBBTAEntities db = new DBBTAEntities();


		//*******************************************
		// GET: api/CTEmployees/GetCTEmployee/
		//get liste zaposlenih
		//******************************************
		[Authorize]
		public IHttpActionResult GetCTEmployee ()
		{
			CTEmployee empl1 = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji
			if (empl1 == null)
				return ResponseMessage(Request.CreateErrorResponse ( HttpStatusCode.BadRequest, "User invalid"));

			//ako nije admin
			if (!isAdmin)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			var EmpList = new List<Tuple<int, string, string >>()
					.Select(t => new { Id = t.Item1, Name = t.Item2, LastName= t.Item3 }).ToList();

			foreach (CTEmployee ct1 in db.CTEmployees) {
				EmpList.Add ( new { Id = ct1.ID, Name = ct1.FName, LastName = ct1.LName } ) ;
			}

			return Ok(EmpList);
		}

		//**************************
		// GET: api/CTEmployees/GetCTEmployee/5
		//get podatke zaposlenog
		//**************************
		[Authorize]
		public IHttpActionResult GetCTEmployee(int id)
		{
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji
			if (empLogged == null)
				return ResponseMessage (Request. CreateErrorResponse (HttpStatusCode. BadRequest, "User invalid"));

			//nađi usera prema traženom ID-ju
			CTEmployee empRequested = db.CTEmployees.Find(id);

			//ako ga nema
			if (empRequested == null)
			{
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));
			}
			//ako je običan user, može da traži samo svoje podatke
			if (!isAdmin && empLogged.ID != empRequested.ID) {
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));
			}
			//filtrira podatke za korisnika
			string pass = encrypt.decryptPass(empRequested.Pass);
			var emp = new { empRequested.ID, empRequested.FName, empRequested.LName, pass, empRequested.Email, empRequested.EmployeeType };
			return Ok(emp);
		}


		//*************************************************
		// PUT: api/CTEmployees/PutCTEmployee
		//edit podataka zaposlenog
		//*************************************************
		[Authorize]
		[HttpPut]
		//[ResponseType(typeof(void))]
		public IHttpActionResult PutCTEmployee(CTEmployee PutEmp1)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//Ima li usera prema traženom ID-ju
			CTEmployee ExistingEmployee = db.CTEmployees.Where(x => x.ID.Equals(PutEmp1.ID)).FirstOrDefault();

			//ako ga nema
			if (ExistingEmployee == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//ako je običan user, može da traži samo svoje podatke
			if (!isAdmin && empLogged.ID != PutEmp1.ID)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			//proveri da li je email jedinstven samo ako je admin ulogovan, jer ako nije, običan user svejedno ne može da menja email
			PutEmp1.Email = PutEmp1.Email.Trim();
			if (isAdmin) {
				bool empExists = db.CTEmployees.Any(x => (x.Email == PutEmp1.Email && x.ID != PutEmp1.ID));
				//ako već postoji
				if (empExists)
					return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Email address already exists!"));
			}

			//proveri da li password već postoji kod drugog usera
			PutEmp1.Pass = encrypt.encryptPass( PutEmp1.Pass.Trim());
			bool PassExists = db.CTEmployees.Any(x => (x.Pass == PutEmp1.Pass && x.ID != PutEmp1.ID));
			if (PassExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Password Alredy exists"));

			ExistingEmployee.FName = PutEmp1.FName;
			ExistingEmployee.LName = PutEmp1.LName;
			ExistingEmployee.Pass = PutEmp1.Pass;
			if (isAdmin)//samo admin sme da izmeni Email polje
				ExistingEmployee.Email = PutEmp1.Email;

			db.SaveChanges();

			return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "Podaci promenjeni"));
		}


		//************************************************************
		// POST: api/CTEmployees/PostCTEmployee
		// SignUp novog usera od strane admina, nema aktivacije naloga
		//************************************************************
		[HttpPost]
		[Authorize]
		public IHttpActionResult PostCTEmployee ([FromBody] CTEmployee PostEmp1) {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			//ako ne postoji user koji je logovan
			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			//ako nije admin
			if (!isAdmin)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			//mail dodatog usera
			PostEmp1.Email = PostEmp1.Email.Trim();
			bool empExists = db.CTEmployees.Any(e => e.Email == PostEmp1.Email);
			//ako već postoji
			if (empExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Email address already exists!"));

			//proveri da li password već postoji kod drugog usera
			PostEmp1.Pass = encrypt.encryptPass(PostEmp1.Pass.Trim());
			bool PassExists = db.CTEmployees.Any(x => (x.Pass == PostEmp1.Pass && x.Email != PostEmp1.Email));
			if (PassExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Password Alredy exists"));

			db.CTEmployees.Add(PostEmp1);
			db.SaveChanges();
			return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, "User added"));
		}

		//************************************************
		// DELETE: api/CTEmployees/DeleteCTEmployee/5
		// briše usera, samo admin
		//************************************************
		[HttpDelete]
		[Authorize]
		//[ResponseType(typeof(CTEmployee))]
		public IHttpActionResult DeleteCTEmployee(int id)
		{
			CTEmployee empLogged = GetLoggedEmp(out bool isAdmin);

			if (empLogged == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User invalid"));

			if (!isAdmin)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User not permited"));

			CTEmployee empToDelete = db.CTEmployees.Find(id);

			//ako je traženi employee admin, ne dozvoli brisanje
			if (empToDelete.EmployeeType == 0)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Operation not permited"));

			if (empToDelete == null)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee not found!"));

			List <Feedback> usersFeed1 = db.Feedbacks.Where(x => x.IDUser == id).ToList();
			db.Feedbacks.RemoveRange(usersFeed1);

			db.CTEmployees.Remove(empToDelete);
			db.SaveChanges();

			return ResponseMessage(Request.CreateResponse(HttpStatusCode.Moved, "User Deleted"));
		}


		//************************************************
		//vraća autorizovanog usera i njegov EmployeeType
		//************************************************
		private CTEmployee GetLoggedEmp (out bool isAdmin){
			isAdmin = false;

			string Mail = ControllerContext.RequestContext.Principal.Identity.Name;
			CTEmployee empl1 = db.CTEmployees.Where(x => x.Email.Equals(Mail)).FirstOrDefault();

			if (empl1 != null){
				isAdmin = (empl1.EmployeeType == 0);
			}
			return empl1;
		}

		//*******************
		//baca objekat u file
		//*******************
		private static void SendEmpToFile (CTEmployee emp1) {
			string path = $"{HttpRuntime.AppDomainAppPath}\\Izlaz_objekat.txt";

			StreamWriter izlaz = new StreamWriter(path);

			izlaz.WriteLine("Id= " + emp1.ID + '\n');
			izlaz.WriteLine("Fname= " + emp1.FName + '\n');
			izlaz.WriteLine("Lname= " + emp1.LName+ '\n');
			izlaz.Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}