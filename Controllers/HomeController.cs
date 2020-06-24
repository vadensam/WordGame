using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using WordRace.Models;
using Newtonsoft.Json;

namespace WordRace.Controllers
{
    public static class SessionExtensions
    {
            public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        private static List<string> words = new List<string>{
            "mountain", "computers", "distance", "discovery", "independent"
        };

        private static List<string> goodwords = new List<string>{
            "mount,1", "tan,1", "nation,1", "ton,1", "tin,1", "mint,1", "main,1", "at,1", "mountain,1",
            "computers,2", "compost,2", "composter,2", "post,2", "sour,2", "poster,2",
            "stance,3", "ice,3", "stand,3", "dance,3", "since,3", "sit,3", "aid,3", "distance,3",
            "cover,4", "over,4", "dive,4", "very,4", "ride,4", "soy,4", "ice,4", "cry,4", "discovery,4",
            "independent,5", "depend,5", "dint,5", "indeed,5", "deed,5", "died,5", "end,5", "pie,5"
        };

        private static List<string> badwords = new List<string>{
            "man,1", "mat,1", "unto,1", "ant,1", "not,1",
            "put,2", "our,2", "sour,2", "stop,2", "use,2",
            "tan,3", "cat,3", "eat,3", "east,3", "ace,3",
            "dove,4", "rode,4", "yes,4", "ire,4", "iced,4",
            "ten,5", "net,5", "pen,5", "pet,5", "did,5"
        };

       [HttpGet("")]
        public IActionResult Index()
        {   
            if(dbContext.Words.Count() < 2){

                foreach(string w in words){
                    dbContext.Words.Add(
                        new Word(){Title = w}
                    );
                }
                dbContext.SaveChanges();

                foreach(string gw in goodwords){
                    string[] info = gw.Split(",");
                    dbContext.PlusWords.Add(
                        new PlusWord (){
                            Title = info[0],
                            WordID = Int32.Parse(info[1])
                        }
                    );
                }
                dbContext.SaveChanges();

                foreach(string gw in badwords){
                    string[] info = gw.Split(",");
                    dbContext.MinusWords.Add(
                        new MinusWord (){
                            Title = info[0],
                            WordID = Int32.Parse(info[1])
                        }
                    );
                }
        
                dbContext.SaveChanges();
                return Redirect("/");
            
            }
            return View();
        }

        [HttpPost("addUser")]
        public IActionResult AddUser(Wrapper Fromform)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == Fromform.Person.Email)){
                    ModelState.AddModelError("Email", "Email already in use.");
                    return Index();
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                Fromform.Person.Password = hasher.HashPassword(Fromform.Person, Fromform.Person.Password);
                dbContext.Add(Fromform.Person);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("loggedUser", Fromform.Person.UserID);
                return RedirectToAction("GamePage");
            }
            return Index();
            
        }

        [HttpPost("login")]
        public IActionResult Login(Wrapper Fromform)
        {
            if(ModelState.IsValid){
                var user = dbContext.Users.FirstOrDefault(u => u.Email == Fromform.LogPers.Email);
                if(user == null){
                    ModelState.AddModelError("LogPers.Email", "Invalid entry.");
                    return Index();
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(Fromform.LogPers, user.Password, Fromform.LogPers.Password);
                if(result == 0){
                    ModelState.AddModelError("LogPers.Password", "Invalid Entry");
                    return Index();
                }
                HttpContext.Session.SetInt32("loggedUser", user.UserID);
                return RedirectToAction("GamePage");
            }
            return Index();
        }

        [HttpGet("GamePage")]
        public IActionResult GamePage()
        {
            if(HttpContext.Session.GetInt32("loggedUser") == null){
                return Index();
            }
            int ID = (int)HttpContext.Session.GetInt32("loggedUser");
            ViewBag.loggedin = dbContext.Users.Include(u => u.WordList).ThenInclude(u => u.Word).ThenInclude(w => w.PlusWords).FirstOrDefault(u => u.UserID == ID);
            ViewBag.usedWords = HttpContext.Session.GetObjectFromJson<List<object>>("WordList");
            ViewBag.mainWord = HttpContext.Session.GetString("Word");
            ViewBag.Message = TempData["Message"];
            ViewBag.Next = TempData["Next"];
            return View("GamePage");
        }

        [HttpGet("level1")]
        public IActionResult StartGame()
        {
            if(HttpContext.Session.GetInt32("loggedUser") == null){
                return Index();
            }
            HttpContext.Session.SetInt32("level", 1);
            int level = (int)HttpContext.Session.GetInt32("level");
            int ID = (int)HttpContext.Session.GetInt32("loggedUser");
            dbContext.Connections.Add(
                new Connection(){
                    UserID = ID,
                    WordID = level
                }
            );
            dbContext.SaveChanges();
            return RedirectToAction("GamePage");
        }

        [HttpGet("level2")]
        public IActionResult Level2()
        {
            if(HttpContext.Session.GetInt32("loggedUser") == null){
                return Index();
            }
            if(HttpContext.Session.GetInt32("loggedUser") == null){
                return Index();
            }
            int ID = (int)HttpContext.Session.GetInt32("loggedUser");
            HttpContext.Session.SetInt32("level", 2);
            int level = (int)HttpContext.Session.GetInt32("level");
            dbContext.Connections.Add(
                new Connection(){
                    UserID = ID,
                    WordID = level
                }
            );
            dbContext.SaveChanges();
            ViewBag.loggedin = dbContext.Users.Include(u => u.WordList).ThenInclude(u => u.Word).ThenInclude(w => w.PlusWords).FirstOrDefault(u => u.UserID == ID);
            ViewBag.Message = TempData["Message"];
            return View("Level2");
        }

        [HttpPost("submit")]
        public IActionResult Submit(string word)
        {
            if(HttpContext.Session.GetInt32("loggedUser") == null){
                return Index();
            }
            if(HttpContext.Session.GetInt32("level") == null){
                return RedirectToAction("StartGame");
            }
            int ID = (int)HttpContext.Session.GetInt32("loggedUser");
            User loggedin = dbContext.Users.Include(u => u.WordList).ThenInclude(u => u.Word).ThenInclude(w => w.PlusWords).FirstOrDefault(u => u.UserID == ID);

            List<string> GoodWords = new List<string>();
            Word MainWord = dbContext.Words.Include(w => w.PlusWords).Include(w => w.MinusWords).FirstOrDefault(w => w.WordID == (int)HttpContext.Session.GetInt32("level"));
            foreach(PlusWord item in MainWord.PlusWords){
                GoodWords.Add(item.Title);
            }

            List<string> ZonkWords = new List<string>();
            foreach(MinusWord item in MainWord.MinusWords){
                ZonkWords.Add(item.Title);
            }
            
            if(HttpContext.Session.GetObjectFromJson<List<object>>("WordList") == null){
                List<string> words = new List<string>();
                words.Add(word);
                HttpContext.Session.SetString("Word", word);
                HttpContext.Session.SetObjectAsJson("WordList", words);
            }
            else if(HttpContext.Session.GetObjectFromJson<List<object>>("WordList").Contains(word)){
                TempData["Message"] = "Word already used.";
                return RedirectToAction("GamePage");
            }
            else{
                var words = HttpContext.Session.GetObjectFromJson<List<object>>("WordList");
                words.Add(word);
                HttpContext.Session.SetString("Word", word);
                HttpContext.Session.SetObjectAsJson("WordList", words); 
            }
            if(GoodWords.Contains(word)){
                loggedin.Points += word.Count();
                TempData["Message"] = $"Nice Word! Plus {word.Count()} points.";
                dbContext.SaveChanges();
                if(loggedin.Points > 25){
                    TempData["Next"] = "Congrats. Next Word?";
                }
                return RedirectToAction("GamePage");
            }
            else if(ZonkWords.Contains(word)){
                loggedin.Points -= 2;
                TempData["Message"] = "Zonk Word!! You lose 2 points.";
                dbContext.SaveChanges();
                return RedirectToAction("GamePage");
            }
            else{
                TempData["Message"] = "Try again.";
                return RedirectToAction("GamePage");
            }
        }
        [HttpPost("submit2")]
        public IActionResult Submit2(string word)
        {
            if(HttpContext.Session.GetInt32("loggedUser") == null){
                return Index();
            }
            if(HttpContext.Session.GetInt32("level") == null){
                return RedirectToAction("Level2");
            }
            int ID = (int)HttpContext.Session.GetInt32("loggedUser");
            User loggedin = dbContext.Users.Include(u => u.WordList).ThenInclude(u => u.Word).ThenInclude(w => w.PlusWords).FirstOrDefault(u => u.UserID == ID);

            List<string> GoodWords = new List<string>();
            Word MainWord = dbContext.Words.Include(w => w.PlusWords).Include(w => w.MinusWords).FirstOrDefault(w => w.WordID == (int)HttpContext.Session.GetInt32("level"));
            foreach(PlusWord item in MainWord.PlusWords){
                GoodWords.Add(item.Title);
            }

            List<string> ZonkWords = new List<string>();
            foreach(MinusWord item in MainWord.MinusWords){
                ZonkWords.Add(item.Title);
            }
            
            if(HttpContext.Session.GetObjectFromJson<List<object>>("WordList") == null){
                List<string> words = new List<string>();
                words.Add(word);
                HttpContext.Session.SetString("Word", word);
                HttpContext.Session.SetObjectAsJson("WordList", words);
            }
            else if(HttpContext.Session.GetObjectFromJson<List<object>>("WordList").Contains(word)){
                TempData["Message"] = "Word already used.";
                return RedirectToAction("Level2");
            }
            else{
                var words = HttpContext.Session.GetObjectFromJson<List<object>>("WordList");
                words.Add(word);
                HttpContext.Session.SetString("Word", word);
                HttpContext.Session.SetObjectAsJson("WordList", words); 
            }
            if(GoodWords.Contains(word)){
                loggedin.Points += word.Count();
                TempData["Message"] = $"Nice Word! Plus {word.Count()} points.";
                dbContext.SaveChanges();
                if(loggedin.Points > 50){
                    TempData["Next"] = "Congrats. Next Word?";
                }
                return RedirectToAction("Level2");
            }
            else if(ZonkWords.Contains(word)){
                loggedin.Points -= 2;
                TempData["Message"] = "Zonk Word!! You lose 2 points.";
                dbContext.SaveChanges();
                return RedirectToAction("Level2");
            }
            else{
                TempData["Message"] = "Try again.";
                return RedirectToAction("Level2");
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            if(HttpContext.Session.GetInt32("loggedUser") == null){
                return Index();
            }
            int ID = (int)HttpContext.Session.GetInt32("loggedUser");
            User loggedin = dbContext.Users.Include(u => u.WordList).ThenInclude(u => u.Word).ThenInclude(w => w.PlusWords).FirstOrDefault(u => u.UserID == ID);
            loggedin.WordList = new List<Connection>();
            loggedin.Points = 0;
            dbContext.SaveChanges();
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
