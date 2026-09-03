using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kaizen.Infrastructure.Identidad;

public sealed class ContextoIdentidad(DbContextOptions<ContextoIdentidad> options)
    : IdentityDbContext<UsuarioAplicacion>(options)
{
    public DbSet<SesionUsuario> SesionesUsuario => Set<SesionUsuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UsuarioAplicacion>(entity =>
        {
            entity.ToTable("Usuario");
            entity.Property(x => x.Id).HasColumnName("Id");
            entity.Property(x => x.UserName).HasColumnName("NombreUsuario");
            entity.Property(x => x.NormalizedUserName).HasColumnName("NombreUsuarioNormalizado");
            entity.Property(x => x.Email).HasColumnName("Correo");
            entity.Property(x => x.NormalizedEmail).HasColumnName("CorreoNormalizado");
            entity.Property(x => x.EmailConfirmed).HasColumnName("CorreoConfirmado");
            entity.Property(x => x.PasswordHash).HasColumnName("HashClave");
            entity.Property(x => x.SecurityStamp).HasColumnName("SelloSeguridad");
            entity.Property(x => x.ConcurrencyStamp).HasColumnName("SelloConcurrencia");
            entity.Property(x => x.PhoneNumber).HasColumnName("Telefono");
            entity.Property(x => x.PhoneNumberConfirmed).HasColumnName("TelefonoConfirmado");
            entity.Property(x => x.TwoFactorEnabled).HasColumnName("DobleFactorHabilitado");
            entity.Property(x => x.LockoutEnd).HasColumnName("FinBloqueo");
            entity.Property(x => x.LockoutEnabled).HasColumnName("BloqueoHabilitado");
            entity.Property(x => x.AccessFailedCount).HasColumnName("IntentosFallidos");
            entity.Property(x => x.DebeCambiarClave).HasColumnName("DebeCambiarClave");
            entity.HasData(new UsuarioAplicacion
            {
                Id = CuentaInicial.UsuarioId,
                UserName = CuentaInicial.Email,
                NormalizedUserName = CuentaInicial.Email.ToUpperInvariant(),
                Email = CuentaInicial.Email,
                NormalizedEmail = CuentaInicial.Email.ToUpperInvariant(),
                EmailConfirmed = true,
                DebeCambiarClave = true,
                SecurityStamp = "CUENTA-INICIAL-KAIZEN",
                ConcurrencyStamp = "CUENTA-INICIAL-KAIZEN"
            });
        });

        modelBuilder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable("Rol");
            entity.Property(x => x.Id).HasColumnName("Id");
            entity.Property(x => x.Name).HasColumnName("Nombre");
            entity.Property(x => x.NormalizedName).HasColumnName("NombreNormalizado");
            entity.Property(x => x.ConcurrencyStamp).HasColumnName("SelloConcurrencia");
        });

        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UsuarioRol");
            entity.Property(x => x.UserId).HasColumnName("UsuarioId");
            entity.Property(x => x.RoleId).HasColumnName("RolId");
        });
        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("DeclaracionUsuario");
            entity.Property(x => x.UserId).HasColumnName("UsuarioId");
            entity.Property(x => x.ClaimType).HasColumnName("Tipo");
            entity.Property(x => x.ClaimValue).HasColumnName("Valor");
        });
        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("InicioSesionExterno");
            entity.Property(x => x.LoginProvider).HasColumnName("Proveedor");
            entity.Property(x => x.ProviderKey).HasColumnName("ClaveProveedor");
            entity.Property(x => x.ProviderDisplayName).HasColumnName("NombreProveedor");
            entity.Property(x => x.UserId).HasColumnName("UsuarioId");
        });
        modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("DeclaracionRol");
            entity.Property(x => x.RoleId).HasColumnName("RolId");
            entity.Property(x => x.ClaimType).HasColumnName("Tipo");
            entity.Property(x => x.ClaimValue).HasColumnName("Valor");
        });
        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("TokenUsuario");
            entity.Property(x => x.UserId).HasColumnName("UsuarioId");
            entity.Property(x => x.LoginProvider).HasColumnName("Proveedor");
            entity.Property(x => x.Name).HasColumnName("Nombre");
            entity.Property(x => x.Value).HasColumnName("Valor");
        });

        modelBuilder.Entity<SesionUsuario>(entity =>
        {
            entity.ToTable("SesionUsuario");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UsuarioId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.DireccionIp).HasMaxLength(45);
            entity.Property(x => x.Dispositivo).HasMaxLength(250);
            entity.HasIndex(x => new { x.UsuarioId, x.FechaRevocacion, x.FechaVencimiento });
            entity.HasOne(x => x.Usuario)
                .WithMany(x => x.Sesiones)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
