using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace.Models
{
    public class User 
    {
        [Key]
        public int UserID {get; set;}

        [Required]
        [MinLength(3, ErrorMessage="Name must be at least 3 characters long.")]
        public string Alias {get; set;}

        [Required]
        [EmailAddress]
        public string Email {get; set;}

        public int Points {get; set;} = 0;

        public List<Connection> WordList {get; set;}

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Must be at least 8 characters long.")]
        public string Password {get; set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get; set;}

        [NotMapped]
        public List<string> UsedWords {get; set;}
    }
}