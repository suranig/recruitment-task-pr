using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Infrastructure.Persistence.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .HasConversion(
                id => id.Value,
                value => ArticleId.Create(value));
        
        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(a => a.Content)
            .IsRequired();
        
        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(a => a.CreatedAt)
            .IsRequired();
        
        builder.HasMany(a => a.Authors)
            .WithOne(aa => aa.Article)
            .HasForeignKey("ArticleId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(a => a.Tags)
            .WithOne(at => at.Article)
            .HasForeignKey("ArticleId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}