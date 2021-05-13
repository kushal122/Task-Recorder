using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskRecorder_DataModels.Models;

namespace TaskRecorder.Data
{
    public class ConnectionDBContext: DbContext
    {
        public ConnectionDBContext()
        { }
        public ConnectionDBContext(DbContextOptions<ConnectionDBContext> options)
                 : base(options)
        { }
        public virtual DbSet<Users> Users{ get; set; }
        public virtual DbSet<UsersRoles> UsersRoles{ get; set; }
        public virtual DbSet<Roles> Roles{ get; set; }
        public virtual DbSet<TasksModel> Tasks { get; set; }
        public virtual DbSet<TaskRequest> TaskRequests { get; set; }
        public virtual DbSet<UserActivity> UserActivity { get; set; }
        public virtual DbSet<NonUserActivity> NonUserActivity { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsersRoles>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.UserId });
                entity.HasOne(d => d.Users)
                .WithMany(p => p.UsersRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role_User");

                entity.HasOne(d => d.Roles)
                .WithMany(p => p.UsersRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role_Role");
            });
        }
    }
}
