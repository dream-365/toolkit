using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace EasyAnalysis.Models
{
    public class ThreadTypeConfigration : EntityTypeConfiguration<ThreadModel>
    {
        public ThreadTypeConfigration()
        {
            // MAP TABLE
            ToTable("Threads");

            // SET PRIMARY KEY
            HasKey(m => m.Id);
            Property(m => m.Id)
                .HasMaxLength(128)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // COLLECTIONS
            HasMany(m => m.Tags)
                .WithMany()
                .Map((config) => {
                    config.ToTable("ThreadTags")
                          .MapLeftKey("ThreadId")
                          .MapRightKey("TagId");
                });
        }
    }

    public class TagTypeConfigration : EntityTypeConfiguration<Tag>
    {
        public TagTypeConfigration()
        {
            ToTable("Tags");

            HasKey(m => m.Id);
            Property(m => m.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}