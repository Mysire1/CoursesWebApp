using CoursesWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }
        
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentDeferral> PaymentDeferrals { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TeacherLanguage> TeacherLanguages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<TeacherLanguage>()
                .HasKey(tl => new { tl.TeacherId, tl.LanguageId });

            modelBuilder.Entity<TeacherLanguage>()
                .HasOne(tl => tl.Teacher)
                .WithMany(t => t.TeacherLanguages)
                .HasForeignKey(tl => tl.TeacherId);

            modelBuilder.Entity<TeacherLanguage>()
                .HasOne(tl => tl.Language)
                .WithMany(l => l.TeacherLanguages)
                .HasForeignKey(tl => tl.LanguageId);

            // Configure decimal precision
            modelBuilder.Entity<Level>()
                .Property(l => l.BaseCost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.Cost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PaymentDeferral>()
                .Property(pd => pd.DeferredAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Student>()
                .Property(s => s.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Languages
            modelBuilder.Entity<Language>().HasData(
                new Language { LanguageId = 1, Name = "Англійська", Code = "EN" },
                new Language { LanguageId = 2, Name = "Німецька", Code = "DE" },
                new Language { LanguageId = 3, Name = "Французька", Code = "FR" },
                new Language { LanguageId = 4, Name = "Іспанська", Code = "ES" },
                new Language { LanguageId = 5, Name = "Італійська", Code = "IT" }
            );

            // Teachers
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { TeacherId = 1, FirstName = "Іван", LastName = "Петренко", Email = "ivan.petrenko@courses.com", Phone = "+380501234567", HireDate = new DateTime(2020, 1, 15) },
                new Teacher { TeacherId = 2, FirstName = "Марія", LastName = "Коваленко", Email = "maria.kovalenko@courses.com", Phone = "+380502345678", HireDate = new DateTime(2019, 5, 20) },
                new Teacher { TeacherId = 3, FirstName = "Олександр", LastName = "Сидоренко", Email = "oleksandr.sydorenko@courses.com", Phone = "+380503456789", HireDate = new DateTime(2021, 3, 10) },
                new Teacher { TeacherId = 4, FirstName = "Катерина", LastName = "Мельник", Email = "kateryna.melnyk@courses.com", Phone = "+380504567890", HireDate = new DateTime(2020, 9, 1) },
                new Teacher { TeacherId = 5, FirstName = "Андрій", LastName = "Бондаренко", Email = "andriy.bondarenko@courses.com", Phone = "+380505678901", HireDate = new DateTime(2022, 1, 15) }
            );

            // Levels
            modelBuilder.Entity<Level>().HasData(
                new Level { LevelId = 1, Name = "Beginner (A1)", Description = "Початковий рівень", LanguageId = 1, BaseCost = 2500m, DurationMonths = 3 },
                new Level { LevelId = 2, Name = "Elementary (A2)", Description = "Елементарний рівень", LanguageId = 1, BaseCost = 2700m, DurationMonths = 3 },
                new Level { LevelId = 3, Name = "Intermediate (B1)", Description = "Середній рівень", LanguageId = 1, BaseCost = 3000m, DurationMonths = 4 },
                new Level { LevelId = 4, Name = "Upper-Intermediate (B2)", Description = "Вище-середній рівень", LanguageId = 1, BaseCost = 3200m, DurationMonths = 4 },
                new Level { LevelId = 5, Name = "Advanced (C1)", Description = "Просунутий рівень", LanguageId = 1, BaseCost = 3500m, DurationMonths = 5 },
                
                new Level { LevelId = 6, Name = "Anfänger (A1)", Description = "Початковий рівень", LanguageId = 2, BaseCost = 2600m, DurationMonths = 3 },
                new Level { LevelId = 7, Name = "Grundstufe (A2)", Description = "Елементарний рівень", LanguageId = 2, BaseCost = 2800m, DurationMonths = 3 },
                new Level { LevelId = 8, Name = "Mittelstufe (B1)", Description = "Середній рівень", LanguageId = 2, BaseCost = 3100m, DurationMonths = 4 },
                
                new Level { LevelId = 9, Name = "Débutant (A1)", Description = "Початковий рівень", LanguageId = 3, BaseCost = 2700m, DurationMonths = 3 },
                new Level { LevelId = 10, Name = "Élémentaire (A2)", Description = "Елементарний рівень", LanguageId = 3, BaseCost = 2900m, DurationMonths = 3 }
            );

            // Students
            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, FirstName = "Анна", LastName = "Іваненко", DateOfBirth = new DateTime(1995, 3, 15), Phone = "+380671234567", Email = "anna.ivanenko@email.com", RegistrationDate = new DateTime(2023, 1, 10), HasDiscount = false, DiscountPercentage = 0, CreatedAt = new DateTime(2023, 1, 10) },
                new Student { StudentId = 2, FirstName = "Петро", LastName = "Коваль", DateOfBirth = new DateTime(1992, 7, 22), Phone = "+380672345678", Email = "petro.koval@email.com", RegistrationDate = new DateTime(2023, 2, 5), HasDiscount = true, DiscountPercentage = 10, CreatedAt = new DateTime(2023, 2, 5) },
                new Student { StudentId = 3, FirstName = "Ольга", LastName = "Шевченко", DateOfBirth = new DateTime(1998, 11, 8), Phone = "+380673456789", Email = "olga.shevchenko@email.com", RegistrationDate = new DateTime(2023, 1, 20), HasDiscount = false, DiscountPercentage = 0, CreatedAt = new DateTime(2023, 1, 20) },
                new Student { StudentId = 4, FirstName = "Михайло", LastName = "Гриценко", DateOfBirth = new DateTime(1990, 4, 12), Phone = "+380674567890", Email = "mykhailo.hrytsenko@email.com", RegistrationDate = new DateTime(2023, 3, 1), HasDiscount = true, DiscountPercentage = 5, CreatedAt = new DateTime(2023, 3, 1) },
                new Student { StudentId = 5, FirstName = "Софія", LastName = "Морозова", DateOfBirth = new DateTime(1997, 9, 30), Phone = "+380675678901", Email = "sofia.morozova@email.com", RegistrationDate = new DateTime(2023, 2, 15), HasDiscount = false, DiscountPercentage = 0, CreatedAt = new DateTime(2023, 2, 15) }
            );

            // Classrooms
            modelBuilder.Entity<Classroom>().HasData(
                new Classroom { ClassroomId = 1, RoomNumber = "101", Capacity = 15, Equipment = "Проектор, дошка, аудіосистема" },
                new Classroom { ClassroomId = 2, RoomNumber = "102", Capacity = 20, Equipment = "Інтерактивна дошка, комп'ютери" },
                new Classroom { ClassroomId = 3, RoomNumber = "201", Capacity = 12, Equipment = "Проектор, дошка" },
                new Classroom { ClassroomId = 4, RoomNumber = "202", Capacity = 25, Equipment = "Проектор, дошка, аудіосистема, комп'ютери" }
            );
        }
    }
}