using System;
using System.Collections.Generic;
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Ts.EntityFramework.Core
{
    public class Blog:Entity, IEntityTypeBuilder<Blog>
    {
        public string Name { get; set; }
        public virtual List<Post> Posts { get; set; } 
        public void Configure(EntityTypeBuilder<Blog> entityBuilder, DbContext dbContext, Type dbContextLocator)
        {
            entityBuilder.HasKey(u => u.Id);
            
        }
    }

    /// <summary>
    /// wendang
    /// </summary>
    public class Post:Entity,IEntityTypeBuilder<Post>
    { 
        public string Title { get; set; }
        public string Content { get; set; }
        public virtual Blog Blog { get; set; }
        public void Configure(EntityTypeBuilder<Post> entityBuilder, DbContext dbContext, Type dbContextLocator)
        {
            entityBuilder.HasKey(u => u.Id);
        }
    }


    public class PostModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
    public class BlogModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PostModel> Posts { get; set; }
    }
}