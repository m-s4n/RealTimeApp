using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealTimeApp.API.Entities;

namespace RealTimeApp.API.Database.EntityConfig
{
    public class TeamConfig : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("team");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasColumnType("varchar(25)").HasColumnName("name");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.HasMany(x => x.Users).WithOne(x => x.Team).HasForeignKey(x => x.TeamId);
        }
    }
}
