using ArdantOffical.Models;
using Microsoft.EntityFrameworkCore;
#nullable disable
namespace ArdantOffical.Data
{
    public partial class FGCDbContext : DbContext
    {
        public FGCDbContext()
        {
        }

        public FGCDbContext(DbContextOptions<FGCDbContext> options)
            : base(options)
        {
        }

        //public Db_Context(DbContextOptions<Db_Context> options)
        //   : base(options)
        //{
        //}
        
      
        public virtual DbSet<ApiCredential> ApiCredentials { get; set; }
        public virtual DbSet<TblUser> TblUsers { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleClaim> RoleClaims { get; set; }
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<APIErrorLog> Tbl_APIErrorLog { get; set; }
        public virtual DbSet<MenuItem> MenuItems { get; set; }
        public virtual DbSet<Attachments> Attachments { get; set; }
        public virtual DbSet<TblUsersAttachments> TblUserAttachments { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-950QGOTB; Database=ArdantOfficalDateBase; Trusted_Connection=True; MultipleActiveResultSets=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
       
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<TblUser>().ToTable("Tbl_Users");
            modelBuilder.Entity<UserRole>().ToTable("Tbl_UserRole");
            modelBuilder.Entity<Role>().ToTable("Tbl_Role");
            modelBuilder.Entity<RoleClaim>().ToTable("Tbl_RoleClaim");
            modelBuilder.Entity<APIErrorLog>().ToTable("Tbl_APIErrorLog");

            //Seeding the User to TblRole table
            modelBuilder.Entity<Role>().HasData(
             new Role() { Id = 1, Name = "Admin", MeaningfulName = "Admin" }, // Admin
             new Role() { Id = 2, Name = "Compliance", MeaningfulName = "Compliance" }, //Compliance
             new Role() { Id = 3, Name = "Auditor", MeaningfulName = "Auditor" }, //Auditor
             new Role() { Id = 4, Name = "DMLRO", MeaningfulName = "Deputy Money Laundering Reporting Officer" }, //Deputy Money Laundering Reporting Officer
             new Role() { Id = 5, Name = "MLRO", MeaningfulName = "Money Laundering Reporting Officer" },//Money Laundering Reporting Officer
             new Role() { Id = 6, Name = "Finance", MeaningfulName = "Finance" },//Finance
             new Role() { Id = 7, Name = "Internee", MeaningfulName = "Internee" },//Internee
             new Role() { Id = 8, Name = "Business Relationship", MeaningfulName = "Business Relationship" },//Business Relationship
             new Role() { Id = 9, Name = "Operations", MeaningfulName = "Operations" },//Operations
             new Role() { Id = 10, Name = "On-Boarding", MeaningfulName = "On-Boarding" },//On-Boarding
             new Role() { Id = 11, Name = "Compliance Commitee", MeaningfulName = "Compliance Commitee" }//Money Laundering Reporting Officer

             );


            modelBuilder.Entity<MenuItem>()
           .HasOne<MenuItem>(s => s.MenuItems)
           .WithMany(g => g.MenuItemChild)
           .HasForeignKey(s => s.MenuItemParentID).OnDelete(DeleteBehavior.Restrict);

            //Seeding the User to TblUsers table  
            modelBuilder.Entity<TblUser>().HasData(
                new TblUser
                {
                    UserId = 1, // primary key
                    UserKey = "0a2w38de90123",
                    Firstname = "John".ToUpper(),
                    Lastname = "Smith".ToUpper(),
                    Email = "jsmith@gmail.com",
                    Username = "jsmith@gmail.com",
                    // PasswordHash = hasher.HashPassword(null, "Sa123!@#"),
                    Password = "admin",
                    UserStatus = 1,
                    City = null,
                    ZipCode = null,
                    Designation = "Administrator",
                    PasswordReset = null,
                    DateModified = null,
                }
            );
            //Seeding the User to TblUserRole table
            modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                Id = 1,
                RoleId = 1,
                UserId = 1
            }
        );
            modelBuilder.Entity<UserRole>().HasIndex(x => new
            {
                x.UserId,
                x.RoleId
            }).IsUnique();

            ///////////////////////////////////////////////////////////////////////////////////
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ApiCredential>(entity =>
            {
                entity.HasKey(e => e.AuthId);

                entity.Property(e => e.AuthId).HasColumnName("AuthID");

                entity.Property(e => e.AuthKey)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Certificate)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Company)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ip)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("IP");

                entity.Property(e => e.IssuedDate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Purpose)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("Tbl_Users");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Avatar).HasMaxLength(250);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.Designation).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Firstname).HasMaxLength(50);

                entity.Property(e => e.Lastname).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.PasswordReset).HasMaxLength(50);

                entity.Property(e => e.UserKey).HasMaxLength(50);

                entity.Property(e => e.UserRole)
                    .HasMaxLength(50)
                    .HasComment("Admin,Compliance,Auditor");

                entity.Property(e => e.UserStatus).HasComment("0=Blocked, 1=Approved/Allowed");

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.Property(e => e.ZipCode).HasMaxLength(50);
            });

           

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    }
}
