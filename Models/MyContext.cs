using Microsoft.EntityFrameworkCore;

namespace WordRace.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options){}

        public DbSet<User> Users {get; set;}

        public DbSet<Word> Words {get; set;}
        public DbSet<PlusWord> PlusWords {get; set;}
        public DbSet<MinusWord> MinusWords {get; set;}

        public DbSet<Connection> Connections {get; set;}

    }
}