using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Infrastructure.Persistence.Configurations;

public class ArticleTagConfiguration : IEntityTypeConfiguration<ArticleTag>
{
    public void Configure(EntityTypeBuilder<ArticleTag> builder)
    {
        builder.HasKey(at => at.Id);
        
        builder.Property(at => at.Id)
            .HasConversion(
                id => id.Value,
                value => ArticleTagId.Create(value));
        
        builder.Property(at => at.Name)
            .HasConversion(
                name => name.Value,
                value => TagName.Create(value))
            .IsRequired();
    }
} 