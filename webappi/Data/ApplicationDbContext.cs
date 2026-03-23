using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace webappi.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<ErpModule> ErpModules { get; set; }
        public DbSet<ErpSubscription> ErpSubscriptions { get; set; }
        public DbSet<ErpPage> ErpPages { get; set; }
        public DbSet<ErpUserPageRight> ErpUserPageRights { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<ErpDesignation> ErpDesignations { get; set; }
        public DbSet<ErpDepartment> ErpDepartments { get; set; }
        public DbSet<ErpEmployee> ErpEmployees { get; set; }
        

        public DbSet<ErpProgram> ErpPrograms { get; set; }
        public DbSet<ErpBranch> ErpBranches { get; set; }
        public DbSet<ErpAcademicYear> ErpAcademicYears { get; set; }
        public DbSet<ErpSemester> ErpSemesters { get; set; }
        

        public DbSet<ErpRoleDefaultRight> ErpRoleDefaultRights { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<AppUser>(b => b.ToTable("A_Users"));
            builder.Entity<AppRole>(b => b.ToTable("A_Roles"));
            builder.Entity<IdentityUserClaim<int>>(b => b.ToTable("A_UserClaims"));
            builder.Entity<IdentityUserRole<int>>(b => b.ToTable("A_UserRoles"));
            builder.Entity<IdentityUserLogin<int>>(b => b.ToTable("A_UserLogins"));
            builder.Entity<IdentityRoleClaim<int>>(b => b.ToTable("A_RoleClaims"));
            builder.Entity<IdentityUserToken<int>>(b => b.ToTable("A_UserTokens"));
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var currentUsername = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            foreach (var entry in entries)
            {

                if (entry.Entity is BaseEntity baseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        baseEntity.CreatedOn = DateTime.UtcNow;
                        baseEntity.CreatedBy = currentUsername;
                    }
                    baseEntity.LastModifiedOn = DateTime.UtcNow;
                    baseEntity.LastModifiedBy = currentUsername;
                }

                if (entry.Entity is AppUser appUser)
                {
                    if (entry.State == EntityState.Added)
                    {
                        appUser.CreatedOn = DateTime.UtcNow;
                        appUser.CreatedBy = currentUsername;
                    }
                    appUser.LastModifiedOn = DateTime.UtcNow;
                    appUser.LastModifiedBy = currentUsername;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
