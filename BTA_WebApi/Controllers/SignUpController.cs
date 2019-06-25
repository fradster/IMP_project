using DBAccess;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BTA_WebApi.Controllers
{
	public class SignUpController : ApiController
	{

		private DBBTAEntities db = new DBBTAEntities();

		//******************************************************
		// POST: api/SignUp/PostCTEmployee
		// SignUp uz aktivaciju naloga, nema logovanog korisnika
		//******************************************************
		[HttpPost]
		public IHttpActionResult PostCTEmployee ([FromBody] CTEmployee PostEmp1) {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			
			//identifikuje jedinstvenog usera preko mail-a
			PostEmp1.Email = PostEmp1.Email.Trim();
			bool empExists = db.CTEmployees.Any(e => e.Email == PostEmp1.Email);

			if (empExists)
				return ResponseMessage(Request. CreateErrorResponse (HttpStatusCode. Conflict, "Email address already exists!"));

			//proveri da li password već postoji kod drugog usera
			PostEmp1.Pass = encrypt.encryptPass(PostEmp1.Pass.Trim());
			bool PassExists = db.CTEmployees.Any(x => (x.Pass == PostEmp1.Pass && x.Email != PostEmp1.Email));
			if (PassExists)
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Password Alredy exists"));


			PostEmp1.EmployeeType = 1;
			db.CTEmployees.Add(PostEmp1);
			db.SaveChanges();

			SendActivationEmail(PostEmp1);
			return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, "To successfully finish the process of registration click the link in the activation email!"));
		}

		//***************************************************
		//*******Send Activation Mail
		//***************************************************
		private void SendActivationEmail (CTEmployee user1) {
			Guid activationCode1 = Guid.NewGuid();
			string host; string email; string password;
			db.Employee_Activations.Add(new Employee_Activation {
				Id = user1.ID,
				ActivationCode = activationCode1
			});

			var par1 = db.Admin_SMTP_parameteres.FirstOrDefault();
			host = par1.host;
			email = par1.UserName;
			password = encrypt.decryptPass(par1.password);
			db.SaveChanges();

			int port = 587;
			string mailFrom = "noreply@mail.com";
			string mailTo = user1.Email;
			string mailTitle = "Account activation";
			string mailMessage = "Hi, " + user1.FName + " " + user1.LName + ", ";
			mailMessage += "<br /><br />Click on the link below to activate accoount";
			var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
			mailMessage += "<br /><a href = '" + string.Format("{0}/Home/Activation/{1}", baseUrl, activationCode1) + "'>Activate.</a>";

			var message1 = new MimeMessage();
			message1.From.Add(new MailboxAddress(mailFrom));
			message1.To.Add(new MailboxAddress(mailTo));
			message1.Subject = mailTitle;
			message1.Body = new TextPart("html") { Text = mailMessage };
			using (var client = new SmtpClient()) {

				client.Connect(host, port, SecureSocketOptions.StartTls);
				client.Authenticate(email, password);

				client.Send(message1);
				client.Disconnect(true);
			}
		}
	}
}
