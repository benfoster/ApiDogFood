using ApiDogFood.WebClient.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace ApiDogFood.WebClient.Controllers
{
    [AllowAnonymous]
    public class UsersController : Controller
    {
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(UsersLogInModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(Constants.ApiBaseUri);
                httpClient.SetBasicAuthentication(model.Username, model.Password);

                var response = await httpClient.GetAsync("token");

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(tokenResponse);
                    var token = json["access_token"].ToString();

                    Session[Constants.SessionTokenKey] = token;
                    
                    FormsAuthentication.SetAuthCookie(model.Username, createPersistentCookie: true);
                    return Redirect("~/");
                }
                else // could check specific error code here
                {
                    ModelState.AddModelError("", "The username and password provided do not match any accounts on record.");
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login");
        }
    }
}
