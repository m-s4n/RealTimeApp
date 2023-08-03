using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealTimeApp.API.Entities;

namespace RealTimeApp.API.Database.EntityConfig
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Name).HasColumnType("varchar(250)").HasColumnName("name");
            builder.Property(x => x.TeamId).HasColumnType("integer").HasColumnName("team_id");   
        }
    }
}
