using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TorneosAPI.Models;

public partial class TorneosDbContext : DbContext
{
    public TorneosDbContext()
    {
    }

    public TorneosDbContext(DbContextOptions<TorneosDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Torneo> Torneos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TorneosDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Torneo>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Torneos__3214EC07E0E743DC");

            entity.Property(e => e.torneo_activo).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
