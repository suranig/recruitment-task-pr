using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Infrastructure.Persistence.Configurations;

public class ArticleAuthorConfiguration : IEntityTypeConfiguration<ArticleAuthor>
{
    public void Configure(EntityTypeBuilder<ArticleAuthor> builder)
    {
        builder.HasKey(aa => aa.Id);
        
        builder.Property(aa => aa.Id)
            .HasConversion(
                id => id.Value,
                value => ArticleAuthorId.Create(value));
        
        builder.Property(aa => aa.AuthorId)
            .HasConversion(
                id => id.Value,
                value => AuthorId.Create(value))
            .IsRequired();
    }
} 