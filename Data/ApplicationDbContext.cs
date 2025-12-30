using ImageClassificationProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageClassificationProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor, veritabanı seçeneklerini alır
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Veritabanı tablosunu temsil eden DbSet
        public DbSet<ClassificationHistory> ClassificationHistories { get; set; }
    }
}