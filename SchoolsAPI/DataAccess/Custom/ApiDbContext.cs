using Microsoft.EntityFrameworkCore;
using SchoolsAPI.Configuration;
using SchoolsAPI.Models;
using System.Reflection;
using System.Reflection.Emit;

namespace SchoolsAPI.DataAccess
{
    public class ApiDbContext : DbContext
    {
        private bool enableSeeding = false;
        private Action<ModelBuilder> CustomSeedData;
        public DatabaseServer DBSetting { get; set; }
        public ApiDbContext()
        {
        }
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        public ApiDbContext(DatabaseServer DBSetting)
        {
            this.DBSetting = DBSetting;
        }

        public ApiDbContext(DatabaseServer DBSetting, bool enableSeeding, Action<ModelBuilder> customSeed = null)
        {
            this.DBSetting = DBSetting;
            this.enableSeeding = enableSeeding;
            this.CustomSeedData = customSeed;
        }

        public static void Open(DatabaseServer DBSetting, Action<ApiDbContext> action, bool enableSeeding = false, Action<ModelBuilder> customSeed = null)
        {
            using (ApiDbContext db = new ApiDbContext(DBSetting, enableSeeding, customSeed))
            {
                action(db);
            }
        }

        protected void SeedData(ModelBuilder modelBuilder)
        {
            var asm = Assembly.GetAssembly(this.GetType());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                //var connectionString = configuration.GetConnectionString("MainConfig.MainDB.ConnectionString");
                var connectionString = configuration.GetSection("MainConfig:MainDB:ConnectionString").Value;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public virtual DbSet<Course> Courses { get; set; }

        public virtual DbSet<Standard> Standards { get; set; }

        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<StudentAddress> StudentAddresses { get; set; }

        public virtual DbSet<StudentCourse> StudentCourses { get; set; }

        public virtual DbSet<Teacher> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DefaultModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        protected void DefaultModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course");

                entity.Property(e => e.CourseName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Teacher).WithMany(p => p.Courses)
                    .HasForeignKey(d => d.TeacherId)
                    .HasConstraintName("FK_Course_Teacher");
            });

            modelBuilder.Entity<Standard>(entity =>
            {
                entity.ToTable("Standard");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.StandardName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.Property(e => e.StudentId).HasColumnName("StudentID");
                entity.Property(e => e.BirthDate).HasColumnType("date");
                entity.Property(e => e.BirthPlace)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.NomorInduk)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Standard).WithMany(p => p.Students)
                    .HasForeignKey(d => d.StandardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Student_Standard");
            });

            modelBuilder.Entity<StudentAddress>(entity =>
            {
                entity.HasKey(e => e.StudentAddressId).HasName("PK_StudentAddress_1");

                entity.ToTable("StudentAddress");

                entity.Property(e => e.StudentAddressId).ValueGeneratedNever();
                entity.Property(e => e.Address1)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Address2)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Student).WithMany(p => p.StudentAddresses)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentAddress_Student");
            });

            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.ToTable("StudentCourse");

                entity.HasOne(d => d.Course).WithMany(p => p.StudentCourses)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_StudentCourse_Course");

                entity.HasOne(d => d.Student).WithMany(p => p.StudentCourses)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentCourse_Student");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teacher");

                entity.Property(e => e.TeacherName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Standard).WithMany(p => p.Teachers)
                    .HasForeignKey(d => d.StandardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Teacher_Standard");
            });
        }
    }
}
