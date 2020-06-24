using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace.Models
{
    public class PlusWord
    {
        [Key]
        public int PlusWordID {get; set;}

        [Required]
        public string Title {get; set;}

        public int WordID {get; set;}
        public Word Root {get;set;}

    }
}