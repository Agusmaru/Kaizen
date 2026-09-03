# Kaizen

Aplicación ASP.NET Core MVC para aplicar mejora continua a metas personales.

## Estructura

- `Kaizen.Web/Controllers`: solicitudes HTTP y coordinación de operaciones.
- `Kaizen.Web/Data`: `ApplicationDbContext` y datos de demostración.
- `Kaizen.Web/Models/Entities`: entidades de Entity Framework Core.
- `Kaizen.Web/ViewModels`: modelos específicos para formularios y pantallas.
- `Kaizen.Web/Services`: recurrencias, progreso, estados y sugerencias Kaizen.
- `Kaizen.Web/Views`: vistas Razor con Bootstrap 5.
- `Kaizen.Web/Migrations`: migraciones existentes de Entity Framework Core.
- `Kaizen.Tests`: pruebas unitarias.

La refactorización mantuvo los nombres de tablas, columnas, relaciones e identificadores de migración. No requiere recrear la base de datos.

## Conexión

La conexión predeterminada usa SQL Server LocalDB y continúa definida en `Kaizen.Web/appsettings.json`. Para otra instancia, reemplace solamente `ConnectionStrings:KaizenDb`.

## Ejecución

```powershell
dotnet restore Kaizen.slnx
dotnet ef database update --project Kaizen.Web --startup-project Kaizen.Web
dotnet run --project Kaizen.Web
```

## Verificación

```powershell
dotnet build Kaizen.slnx
dotnet test Kaizen.slnx
```
