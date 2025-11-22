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

            // Configure unique indexes for authentication
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.Email)
                .IsUnique();

            // Configure relationships
            modelBuilder.Entity<TeacherLanguage>()
                .HasKey(tl => new { tl.TeacherId, tl.LanguageId });

            modelBuilder.Entity<TeacherLanguage>()
                .HasOne(tl => tl.Teacher)
                .WithMany(t => t.TeacherLanguages)
                .HasForeignKey(tl => tl.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeacherLanguage>()
                .HasOne(tl => tl.Language)
                .WithMany(l => l.TeacherLanguages)
                .HasForeignKey(tl => tl.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure other relationships
            modelBuilder.Entity<Level>()
                .HasOne(l => l.Language)
                .WithMany(la => la.Levels)
                .HasForeignKey(l => l.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Level)
                .WithMany(l => l.Groups)
                .HasForeignKey(g => g.LevelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Teacher)
                .WithMany(t => t.Groups)
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add Language relationship for Group
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Language)
                .WithMany()
                .HasForeignKey(g => g.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Student -> Group relationship
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Group)
                .WithMany(g => g.Enrollments)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Configure table names explicitly
            modelBuilder.Entity<Language>().ToTable("Languages");
            modelBuilder.Entity<Teacher>().ToTable("Teachers");
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Level>().ToTable("Levels");
            modelBuilder.Entity<Classroom>().ToTable("Classrooms");
            modelBuilder.Entity<Group>().ToTable("Groups");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollments");
            modelBuilder.Entity<Exam>().ToTable("Exams");
            modelBuilder.Entity<ExamResult>().ToTable("ExamResults");
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<PaymentDeferral>().ToTable("PaymentDeferrals");
            modelBuilder.Entity<Schedule>().ToTable("Schedules");
            modelBuilder.Entity<TeacherLanguage>().ToTable("TeacherLanguages");
        }
    }
}