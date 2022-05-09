#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;

    public class MaKoreContext : DbContext
    {
        public MaKoreContext (DbContextOptions<MaKoreContext> options)
            : base(options)
        {
        }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>()
            .HasOne(b => b.RemoteUser)
            .WithOne(i => i.Conversation)
            .HasForeignKey<RemoteUser>(b => b.ConversationId);

        modelBuilder.Entity<RemoteUser>()
           .HasOne(b => b.Conversation)
           .WithOne(i => i.RemoteUser)
           .HasForeignKey<Conversation>(b => b.RemoteUserId);
    }


    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Convesations { get; set; }
    public DbSet<RemoteUser> RemoteUsers { get; set; }

}
