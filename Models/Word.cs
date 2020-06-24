using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace WordRace.Models
{
    public class Word
    {
        [Key]
        public int WordID {get; set;}

        [Required]
        public string Title {get; set;}

        public List<PlusWord> PlusWords {get; set;}

        public List<MinusWord> MinusWords {get; set;}

        public List<Connection> Users {get; set;}
    }
}