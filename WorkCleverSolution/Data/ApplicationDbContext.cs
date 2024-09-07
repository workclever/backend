using WorkCleverSolution.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Column> Columns { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<TaskAttachment> TaskAttachments { get; set; }
    public DbSet<TaskRelationTypeDef> TaskRelationTypeDefs { get; set; }
    public DbSet<TaskParentRelation> TaskParentRelations { get; set; }
    public DbSet<TaskRelation> TaskRelations { get; set; }
    public DbSet<TaskChangeLog> TaskChangeLogs { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }
    public DbSet<TaskAssignee> TaskAssignees { get; set; }
    public DbSet<UserEntityAccess> UserEntityAccesses { get; set; }
    public DbSet<UserPreference> UserPreferences { get; set; }
    public DbSet<SiteSettings> SiteSettings { get; set; }
    public DbSet<CustomField> CustomFields { get; set; }
    public DbSet<CustomFieldSelectOption> CustomFieldSelectOptions { get; set; }
    public DbSet<TaskCustomFieldValue> TaskCustomFieldValues { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    private static void FixIdentity(ModelBuilder builder)
    {
        // identity stuff
        // builder.Entity<UserRole>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));
        builder.Entity<IdentityUserClaim<int>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
        builder.Entity<IdentityRoleClaim<int>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
        builder.Entity<IdentityUserLogin<int>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
        builder.Entity<IdentityUserLogin<int>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));
        builder.Entity<IdentityUserToken<int>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
        builder.Entity<IdentityUserToken<int>>(entity => entity.Property(m => m.Name).HasMaxLength(85));
        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<int>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        FixIdentity(builder);
        builder.Entity<CustomField>().HasMany(x => x.SelectOptions).WithOne(x => x.CustomField)
            .HasForeignKey(x => x.CustomFieldId);
        builder.Entity<TaskCustomFieldValue>().HasOne(x => x.CustomField);
    }

    public async Task<int> SaveChangesAsync()
    {
        AddTimestamps();
        return await base.SaveChangesAsync();
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker
            .Entries()
            .Where(
                x =>
                    x.Entity is TimeAwareEntity &&
                    (x.State == EntityState.Added || x.State == EntityState.Modified)
            );

        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                ((TimeAwareEntity) entity.Entity).DateCreated = DateTime.UtcNow;
            }

            ((TimeAwareEntity) entity.Entity).DateModified = DateTime.UtcNow;
        }
    }
}