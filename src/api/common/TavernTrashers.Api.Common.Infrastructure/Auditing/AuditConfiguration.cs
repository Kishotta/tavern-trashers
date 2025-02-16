using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TavernTrashers.Api.Common.Infrastructure.Auditing;

public class AuditConfiguration : IEntityTypeConfiguration<Audit>
{
    public void Configure(EntityTypeBuilder<Audit> builder)
    {
        builder.ToTable("audit_logs");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(audit => audit.PrimaryKey)
            .HasMaxLength(500)
	        .HasColumnType("jsonb");
        
        builder.Property(audit => audit.OldValues)
            .HasMaxLength(3000)
            .HasColumnType("jsonb");
        
        builder.Property(audit => audit.NewValues)
            .HasMaxLength(3000)
            .HasColumnType("jsonb");
    }
}