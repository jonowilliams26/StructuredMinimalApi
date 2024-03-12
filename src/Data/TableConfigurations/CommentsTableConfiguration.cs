using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chirper.Data.TableConfigurations;

public class CommentsTableConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasOne<Comment>()
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ReplyToCommentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Post>()
            .WithMany(p => p.Comments)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
