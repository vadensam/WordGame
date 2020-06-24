using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace.Models
{
    public class Wrapper
    {
        public User Person {get; set;}
        public LoginUser LogPers {get; set;} 
    }

}