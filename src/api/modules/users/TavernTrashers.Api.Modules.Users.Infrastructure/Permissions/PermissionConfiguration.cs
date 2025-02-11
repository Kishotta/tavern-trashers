using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Users.Domain.Permissions;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Permissions;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        
        builder.HasKey(permission => permission.Code);
        builder.Property(permission => permission.Code).HasMaxLength(100);
        
        builder.HasData(
            Permission.GetUser,
            Permission.ModifyUser,
            Permission.GetCampaign,
            Permission.ModifyCampaign);

        builder.HasMany<Role>()
            .WithMany()
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("role_permissions");

                joinBuilder.HasData(
                    CreateRolePermission(Role.Administrator, Permission.GetUser),
                    CreateRolePermission(Role.Administrator, Permission.ModifyUser),
                    CreateRolePermission(Role.Administrator, Permission.GetCampaign),
                    CreateRolePermission(Role.Administrator, Permission.ModifyCampaign),
                    
                    CreateRolePermission(Role.DungeonMaster, Permission.GetUser),
                    CreateRolePermission(Role.DungeonMaster, Permission.ModifyUser),
                    CreateRolePermission(Role.DungeonMaster, Permission.GetCampaign),
                    CreateRolePermission(Role.DungeonMaster, Permission.ModifyCampaign),
                    
                    CreateRolePermission(Role.Player, Permission.GetUser),
                    CreateRolePermission(Role.Player, Permission.ModifyUser),
                    CreateRolePermission(Role.Player, Permission.GetCampaign)
                );
            });
        
    }

    private static object CreateRolePermission(Role role, Permission permission) =>
        new
        {
            RoleName       = role.Name,
            PermissionCode = permission.Code
        };
}