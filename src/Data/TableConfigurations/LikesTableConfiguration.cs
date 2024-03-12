using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chirper.Data.TableConfigurations;

public class LikesTableConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasOne<Post>()
            .WithMany(p => p.Likes)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<User>()
            .WithMany(u => u.LikedPosts)
            .OnDelete(DeleteBehavior.NoAction);
    }
}