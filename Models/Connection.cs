using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace.Models
{
    public class Connection
    {
        [Key]
        public int ConnectionID {get; set;}

        public int UserID {get; set;}
        public User Person {get; set;}
        public int WordID {get; set;}
        public Word Word {get;set;}

    }
}