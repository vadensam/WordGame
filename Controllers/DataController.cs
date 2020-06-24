using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WordRace.Models;

namespace WordRace.Controllers
{
    public class DataController : Controller
    {
        private static MyContext context;
        
        public DataController(MyContext DBContext)
        {
            context = DBContext;
        }

        private static List<string> words = new List<string>{
            "mountain", "computer", "distance", "discovery", "independent"
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

        public IActionResult GetData()
        {
            foreach(string w in words){
                context.Words.Add(
                    new Word(){Title = w}
                );
            }
            context.SaveChanges();

            foreach(string gw in goodwords){
                string[] info = gw.Split(",");
                context.PlusWords.Add(
                    new PlusWord (){
                        Title = info[0],
                        WordID = Int32.Parse(info[1])
                    }
                );
            }
            context.SaveChanges();

            foreach(string gw in badwords){
                string[] info = gw.Split(",");
                context.MinusWords.Add(
                    new MinusWord (){
                        Title = info[0],
                        WordID = Int32.Parse(info[1])
                    }
                );
            }
            context.SaveChanges();
            return Redirect("/");
            
        }
    }
}