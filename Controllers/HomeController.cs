using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Wedding_Planner.Models;
// using Google.Maps;
// using Google.Maps.StaticMaps;
// using Google.Maps.Geocoding;

namespace Wedding_Planner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }



        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View("RootPartial");
        }



        [HttpGet]
        [Route("Dashboard")]
        public IActionResult ResultMatch()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {

                return View("Login");

            }
            List<User> AllUsers = dbContext.UserTable.ToList();
            // User Created = dbContext.Users.FirstOrDefault(); ya ispolzovala kogda sozdala bes session , i vmesto Signedin byl Created

            int UserID = (int)HttpContext.Session.GetInt32("UserID");//bolwaya bykva ID eto session
            User SignedIn = dbContext.UserTable.FirstOrDefault(u => u.UserId == UserID); // session dannye idut sravnivat s basa dannuh UserId ,gde uje sohranilis dannuye user v UserId

            //this is for displaying all weddings:
            List<WeddPlan> AllWeddings = dbContext.WeddPlanTable
            .Include(x => x.GuestList)
            .ThenInclude(y => y.User)
           .ToList();
            ViewBag.AllCreatedWeddings = AllWeddings;
            ViewBag.UserInSession = UserID;
            return View("Dashboard", SignedIn);
        }



        [HttpPost]
        [Route("Register")]
        public IActionResult Register(User newUser)
        {
            // Check initial ModelState
            if (ModelState.IsValid)
            {

                // We can take the User object created from a form submission
                // And pass this object to the .Add() method
                // OR dbContext.Users.Add(newUser);

                // If a User exists with provided email
                if (dbContext.UserTable.Any(u => u.Email == newUser.Email))
                {

                    // Manually add a ModelState error to the Email field, with provided

                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");

                    // You may consider returning to the View at this point
                    //return View("RootPartial");
                    return View("Index");

                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserID", newUser.UserId);
                    return RedirectToAction("ResultMatch");
                }

            }
            // other code
            //return View("RootPartial");
            return View("Index");

        }

        // logic code for link button wich render register page from login
        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            List<User> AllUsers = dbContext.UserTable.ToList();
            Console.WriteLine("-------678564--------");
            return View("Index");
        }


        //////////////////////////////////////////////

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            List<User> AllUsers = dbContext.UserTable.ToList();
            Console.WriteLine("-------678564--------");
            return View("Login");
        }


        [HttpPost]
        [Route("LoginPost")]
        public IActionResult LoginPost(Login userSubmission)
        {
            Console.WriteLine("------1-------");
            if (ModelState.IsValid)
            {
                Console.WriteLine("--------2--------");
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.UserTable.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    Console.WriteLine("-----------3-----------");
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    //return View("RootPartial");
                    return View("Login");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<Login>();

                // varify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    Console.WriteLine("-------4-----------");
                    // handle failure (this should be similar to how "existing email" is handled)

                    return View("Login");
                }
                Console.WriteLine("------------5----------");
                HttpContext.Session.SetInt32("UserID", userInDb.UserId);
                return RedirectToAction("ResultMatch");
            }
            Console.WriteLine("-------------6------------");
            return View("Login");

        }

        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return View("Login");
        }

        //////////////////////////////////////////////////////////////////////// 


        [HttpGet("newWedding")]
        public IActionResult NewWedding()
        {
            return View("NewWedding");
        }

        //create process wedding
        [HttpPost]
        [Route("addWedding")]
        public IActionResult addWedding(WeddPlan newWedding)
        {
            if (ModelState.IsValid)
            { 
            newWedding.WeddingPlanner = (int)HttpContext.Session.GetInt32("UserID");
            dbContext.WeddPlanTable.Add(newWedding);
            dbContext.SaveChanges();
            return RedirectToAction("DisplayWedd", new { id = newWedding.WeddPlanId });
            }
            return View("NewWedding");
        }

                        // //Display One Wedding in Display page(only one by ID) without grabing assosiations
                        // [HttpGet("displayWedding/{id}")]
                        // public IActionResult DisplayWedd (int id) 
                        // {   
                        //     //In case of If u want display thru ViewBag in html:
                        //     //List<WeddPlan> OneWedding = dbContext.WeddPlanTable.Where (wedd => wedd.WeddPlanId == id).ToList ();
                        //     ViewBag.OneWedding = OneWedding;
                        //     ViewBag.WeddPlanId = id;
                        //     //But we dont have to pass a data inside of  View, to est v skobkah ne nado pisat, eto nepravilno=> ("DisplayWedd", OneWedding)
                        //     return View("DisplayWedd")
                        // }


        //Display One Wedding in Display page(only one by ID)with grabbing Assosiation
        [Route("displayWedding/{id}")]
        [HttpGet]
        public IActionResult DisplayWedd(int id)
        {
            // List<User> AllUsers = dbContext.users.ToList();
            // ViewBag.Users = AllUsers;
            WeddPlan TheOneWedding = dbContext.WeddPlanTable
            .Include(r => r.GuestList)
            .ThenInclude(u => u.User)
            .FirstOrDefault(w => w.WeddPlanId == id);


            // GoogleSigned.AssignAllServices(new GoogleSigned("MY_GOOGLE_MAP_API_KEY_"));

            // var request = new GeocodingRequest();
            // request.Address = TheOneWedding.Address;
            // var response = new GeocodingService().GetResponse(request);

            // //The GeocodingService class submits the request to the API web service, and returns the
            // //response strongly typed as a GeocodeResponse object which may contain zero, one or more results.

            // //Assuming we received at least one result, let's get some of its properties:
            // if (response.Status == ServiceResponseStatus.Ok && response.Results.Count() > 0)
            // {
            //     var result = response.Results.First();

            //     Console.WriteLine("Full Address: " + result.FormattedAddress);         // "1600 Pennsylvania Ave NW, Washington, DC 20500, USA"
            //     Console.WriteLine("Latitude: " + result.Geometry.Location.Latitude);   // 38.8976633
            //     Console.WriteLine("Longitude: " + result.Geometry.Location.Longitude); // -77.0365739
            //     Console.WriteLine();
            //     ViewBag.Lati = result.Geometry.Location.Latitude;
            //    ViewBag.Long = result.Geometry.Location.Longitude;
            // }
            // else
            // {
            //     Console.WriteLine("Unable to geocode.  Status={0} and ErrorMessage={1}", response.Status, response.ErrorMessage);
            // }

            return View("DisplayWedd", TheOneWedding);
        }


        [Route("/reserve/{weddingID}")]
        [HttpGet]
        public IActionResult Reserve(int weddingID, RSVP newRSVP)
        {
            newRSVP.WeddPlanId = weddingID;
            newRSVP.UserId = (int)HttpContext.Session.GetInt32("UserID");
            dbContext.RSVPTable.Add(newRSVP);
            dbContext.SaveChanges();
            return RedirectToAction("ResultMatch");
        }

        [Route("/un-reserve/{weddingID}")]
        [HttpGet]
        public IActionResult UnReserve(int weddingID)
        {
            IEnumerable<RSVP> ReserveGuests = dbContext.RSVPTable.Where(a => a.WeddPlanId == weddingID);//IEnumer beret vse vidy DataType iz <RVSP>GuestLIst and sravnyaet eto k filtered opredelennomu WeddPlanId, kotoroe nahoditsya v classe RSVP
            RSVP UnReserveGuest = ReserveGuests.SingleOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("UserID"));//Getting to RSVP list s pomowyu "ReserveGuests", a vnutri nego est mini spisok ludei kto idet na opredelennuyu svadbu, i UserId who is in session mojet delat singling out(vydelit) grabing his/her UserId iz spiska RSVP ReservedGuests(kotoroe lejit v table WeddPlanTable/stolbec <RVSP>GuestList)
            dbContext.RSVPTable.Remove(UnReserveGuest);// to chto single out i derjim v ruke udalyaem
            dbContext.SaveChanges();
            return RedirectToAction("ResultMatch");
        }




    }
}
