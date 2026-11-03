using DocTest.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DocTest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

    }
}
