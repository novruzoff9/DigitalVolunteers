using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    internal class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ActivityPoints> ActivityPoints { get; set; }
        public DbSet<EventGallery> EventGallery { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<DailyLogin> DailyLogins { get; set; }
        public DbSet<PasswordTokens> PasswordTokens { get; set; }
        //public DbSet<NewsReading> NewsReadings { get; set; }
    }
}
