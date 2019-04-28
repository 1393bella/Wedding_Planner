using Microsoft.EntityFrameworkCore;

namespace Wedding_Planner.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<WeddPlan> WeddPlanTable {get;set;}
        public DbSet<RSVP> RSVPTable {get;set;}
        public DbSet<User> UserTable {get;set;}
    }
}
