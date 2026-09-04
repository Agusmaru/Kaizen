/*
    KAIZEN - Objetos SQL para consulta y soporte
    Compatible con SQL Server / SQL Server Express.

    Características:
    - No modifica las tablas administradas por Entity Framework.
    - Puede ejecutarse más de una vez.
    - No expone hashes de contraseñas ni sellos de seguridad.
    - Las intervenciones escriben una auditoría y exigen un motivo.

    Ejecutar conectado a la base de datos de Kaizen.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF SCHEMA_ID(N'soporte') IS NULL
    EXEC(N'CREATE SCHEMA soporte AUTHORIZATION dbo;');
GO

IF OBJECT_ID(N'soporte.AuditoriaIntervencion', N'U') IS NULL
BEGIN
    CREATE TABLE soporte.AuditoriaIntervencion
    (
        Id                  bigint IDENTITY(1,1) NOT NULL,
        FechaUtc            datetime2(7) NOT NULL
            CONSTRAINT DF_AuditoriaIntervencion_FechaUtc DEFAULT SYSUTCDATETIME(),
        Operador            nvarchar(200) NOT NULL,
        Procedimiento       sysname NOT NULL,
        UsuarioAfectadoId   nvarchar(450) NULL,
        Entidad             nvarchar(100) NULL,
        EntidadId           nvarchar(450) NULL,
        Motivo              nvarchar(1000) NOT NULL,
        ValoresAnteriores   nvarchar(max) NULL,
        ValoresNuevos       nvarchar(max) NULL,
        Resultado           nvarchar(100) NOT NULL,
        CONSTRAINT PK_AuditoriaIntervencion PRIMARY KEY (Id),
        CONSTRAINT CK_AuditoriaIntervencion_Operador CHECK (LEN(LTRIM(RTRIM(Operador))) > 0),
        CONSTRAINT CK_AuditoriaIntervencion_Motivo CHECK (LEN(LTRIM(RTRIM(Motivo))) > 0),
        CONSTRAINT CK_AuditoriaIntervencion_JsonAnterior CHECK (ValoresAnteriores IS NULL OR ISJSON(ValoresAnteriores) = 1),
        CONSTRAINT CK_AuditoriaIntervencion_JsonNuevo CHECK (ValoresNuevos IS NULL OR ISJSON(ValoresNuevos) = 1)
    );

    CREATE INDEX IX_AuditoriaIntervencion_Usuario_Fecha
        ON soporte.AuditoriaIntervencion(UsuarioAfectadoId, FechaUtc DESC);
END;
GO

CREATE OR ALTER VIEW soporte.Vista_UsuariosResumen
AS
    SELECT
        u.Id AS UsuarioId,
        u.Correo,
        u.NombreUsuario,
        u.CorreoConfirmado,
        u.DebeCambiarClave,
        u.BloqueoHabilitado,
        u.FinBloqueo,
        u.IntentosFallidos,
        CASE
            WHEN u.FinBloqueo IS NOT NULL AND u.FinBloqueo > SYSDATETIMEOFFSET() THEN N'Bloqueado'
            WHEN u.DebeCambiarClave = 1 THEN N'Debe cambiar la clave'
            ELSE N'Activo'
        END AS EstadoCuenta,
        (SELECT COUNT_BIG(*) FROM dbo.Meta m WHERE m.UsuarioId = u.Id) AS CantidadMetas,
        (SELECT COUNT_BIG(*) FROM dbo.Meta m WHERE m.UsuarioId = u.Id AND m.Estado = 1) AS MetasActivas,
        (SELECT COUNT_BIG(*)
         FROM dbo.SesionUsuario s
         WHERE s.UsuarioId = u.Id
           AND s.FechaRevocacion IS NULL
           AND s.FechaVencimiento > SYSUTCDATETIME()) AS SesionesActivas,
        (SELECT MAX(s.UltimaActividad) FROM dbo.SesionUsuario s WHERE s.UsuarioId = u.Id) AS UltimaActividadUtc
    FROM dbo.Usuario u;
GO

CREATE OR ALTER VIEW soporte.Vista_MetasPorUsuario
AS
    SELECT
        u.Id AS UsuarioId,
        u.Correo,
        m.Id AS MetaId,
        m.Titulo,
        m.Descripcion,
        a.Nombre AS AreaPersonal,
        m.FechaInicio,
        m.FechaObjetivo,
        m.FechaProximaRevision,
        m.FechaCreacion,
        m.FechaActivacion,
        m.FechaArchivo,
        m.Estado AS EstadoCodigo,
        CASE m.Estado
            WHEN 0 THEN N'Borrador'
            WHEN 1 THEN N'Activa'
            WHEN 2 THEN N'Pausada'
            WHEN 3 THEN N'Completada'
            WHEN 4 THEN N'Archivada'
            ELSE N'Desconocido'
        END AS Estado,
        (SELECT COUNT_BIG(*) FROM dbo.AccionPlanificada ap WHERE ap.MetaId = m.Id) AS CantidadAcciones,
        (SELECT COUNT_BIG(*) FROM dbo.AccionPlanificada ap WHERE ap.MetaId = m.Id AND ap.Estado = 0) AS AccionesActivas
    FROM dbo.Meta m
    INNER JOIN dbo.Usuario u ON u.Id = m.UsuarioId
    LEFT JOIN dbo.AreaPersonal a ON a.Id = m.AreaPersonalId;
GO

CREATE OR ALTER VIEW soporte.Vista_AccionesDiarias
AS
    SELECT
        m.UsuarioId,
        u.Correo,
        m.Id AS MetaId,
        m.Titulo AS Meta,
        ap.Id AS AccionPlanificadaId,
        ap.Nombre AS Accion,
        ap.Hora,
        ap.CantidadObjetivo,
        ap.UnidadMetrica,
        ag.Id AS AccionProgramadaId,
        ag.FechaProgramada,
        ag.Orden,
        ag.Estado AS EstadoCodigo,
        CASE ag.Estado
            WHEN 0 THEN N'Pendiente'
            WHEN 1 THEN N'Completada'
            WHEN 2 THEN N'No realizada'
            ELSE N'Desconocido'
        END AS Estado,
        r.ValorReal,
        r.Nota,
        r.FechaRegistro
    FROM dbo.AccionProgramada ag
    INNER JOIN dbo.AccionPlanificada ap ON ap.Id = ag.AccionPlanificadaId
    INNER JOIN dbo.Meta m ON m.Id = ap.MetaId
    INNER JOIN dbo.Usuario u ON u.Id = m.UsuarioId
    LEFT JOIN dbo.RegistroAccion r ON r.AccionProgramadaId = ag.Id;
GO

CREATE OR ALTER VIEW soporte.Vista_ProgresoSemanal
AS
    SELECT
        m.UsuarioId,
        u.Correo,
        m.Id AS MetaId,
        m.Titulo AS Meta,
        COUNT_BIG(*) AS AccionesProgramadas,
        SUM(CASE WHEN ag.Estado = 1 THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END) AS AccionesCompletadas,
        SUM(CASE WHEN ag.Estado = 2 THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END) AS AccionesNoRealizadas,
        SUM(CASE WHEN ag.Estado = 0 THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END) AS AccionesPendientes,
        CONVERT(decimal(6,2), 100.0 * SUM(CASE WHEN ag.Estado = 1 THEN 1 ELSE 0 END) / NULLIF(COUNT_BIG(*), 0)) AS PorcentajeCumplimiento
    FROM dbo.AccionProgramada ag
    INNER JOIN dbo.AccionPlanificada ap ON ap.Id = ag.AccionPlanificadaId
    INNER JOIN dbo.Meta m ON m.Id = ap.MetaId
    INNER JOIN dbo.Usuario u ON u.Id = m.UsuarioId
    WHERE ag.FechaProgramada >= DATEADD(day, -6, CONVERT(date, GETDATE()))
      AND ag.FechaProgramada <= CONVERT(date, GETDATE())
    GROUP BY m.UsuarioId, u.Correo, m.Id, m.Titulo;
GO

CREATE OR ALTER VIEW soporte.Vista_RevisionesPendientes
AS
    SELECT
        m.UsuarioId,
        u.Correo,
        m.Id AS MetaId,
        m.Titulo,
        m.FechaProximaRevision,
        DATEDIFF(day, CONVERT(date, GETDATE()), m.FechaProximaRevision) AS DiasRestantes,
        CASE
            WHEN m.FechaProximaRevision < CONVERT(date, GETDATE()) THEN N'Vencida'
            WHEN m.FechaProximaRevision = CONVERT(date, GETDATE()) THEN N'Vence hoy'
            ELSE N'Próxima'
        END AS Situacion
    FROM dbo.Meta m
    INNER JOIN dbo.Usuario u ON u.Id = m.UsuarioId
    WHERE m.Estado = 1
      AND m.FechaProximaRevision IS NOT NULL;
GO

CREATE OR ALTER VIEW soporte.Vista_SesionesActivas
AS
    SELECT
        s.Id AS SesionId,
        s.UsuarioId,
        u.Correo,
        s.FechaInicio,
        s.UltimaActividad,
        s.FechaVencimiento,
        s.DireccionIp,
        s.Dispositivo,
        DATEDIFF(minute, s.UltimaActividad, SYSUTCDATETIME()) AS MinutosSinActividad
    FROM dbo.SesionUsuario s
    INNER JOIN dbo.Usuario u ON u.Id = s.UsuarioId
    WHERE s.FechaRevocacion IS NULL
      AND s.FechaVencimiento > SYSUTCDATETIME();
GO

CREATE OR ALTER VIEW soporte.Vista_DiagnosticoIntegridad
AS
    SELECT
        m.UsuarioId,
        m.Id AS MetaId,
        CONVERT(nvarchar(450), ag.Id) AS EntidadId,
        N'AccionProgramada' AS Entidad,
        N'Acción resuelta sin registro asociado' AS Problema
    FROM dbo.AccionProgramada ag
    INNER JOIN dbo.AccionPlanificada ap ON ap.Id = ag.AccionPlanificadaId
    INNER JOIN dbo.Meta m ON m.Id = ap.MetaId
    LEFT JOIN dbo.RegistroAccion r ON r.AccionProgramadaId = ag.Id
    WHERE ag.Estado IN (1, 2) AND r.Id IS NULL

    UNION ALL

    SELECT
        m.UsuarioId,
        m.Id,
        CONVERT(nvarchar(450), ag.Id),
        N'AccionProgramada',
        N'Registro asociado a una acción pendiente'
    FROM dbo.AccionProgramada ag
    INNER JOIN dbo.AccionPlanificada ap ON ap.Id = ag.AccionPlanificadaId
    INNER JOIN dbo.Meta m ON m.Id = ap.MetaId
    INNER JOIN dbo.RegistroAccion r ON r.AccionProgramadaId = ag.Id
    WHERE ag.Estado = 0

    UNION ALL

    SELECT
        m.UsuarioId,
        m.Id,
        CONVERT(nvarchar(450), ag.Id),
        N'AccionProgramada',
        N'Orden diario negativo'
    FROM dbo.AccionProgramada ag
    INNER JOIN dbo.AccionPlanificada ap ON ap.Id = ag.AccionPlanificadaId
    INNER JOIN dbo.Meta m ON m.Id = ap.MetaId
    WHERE ag.Orden < 0;
GO

CREATE OR ALTER PROCEDURE soporte.Soporte_BuscarUsuarioPorCorreo
    @Correo nvarchar(256)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Correo = NULLIF(LTRIM(RTRIM(@Correo)), N'');
    IF @Correo IS NULL
        THROW 50001, N'Ingresá un correo o una parte del correo.', 1;

    SELECT *
    FROM soporte.Vista_UsuariosResumen
    WHERE Correo LIKE N'%' + @Correo + N'%'
    ORDER BY Correo;
END;
GO

CREATE OR ALTER PROCEDURE soporte.Soporte_ObtenerResumenUsuario
    @UsuarioId nvarchar(450)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Id = @UsuarioId)
        THROW 50002, N'El usuario indicado no existe.', 1;

    SELECT * FROM soporte.Vista_UsuariosResumen WHERE UsuarioId = @UsuarioId;
    SELECT * FROM soporte.Vista_MetasPorUsuario WHERE UsuarioId = @UsuarioId ORDER BY FechaCreacion DESC;
    SELECT * FROM soporte.Vista_AccionesDiarias WHERE UsuarioId = @UsuarioId ORDER BY FechaProgramada DESC, Orden, Hora;
    SELECT * FROM soporte.Vista_SesionesActivas WHERE UsuarioId = @UsuarioId ORDER BY UltimaActividad DESC;
    SELECT * FROM soporte.Vista_DiagnosticoIntegridad WHERE UsuarioId = @UsuarioId ORDER BY MetaId, Entidad, EntidadId;
END;
GO

CREATE OR ALTER PROCEDURE soporte.Soporte_ValidarIntegridadUsuario
    @UsuarioId nvarchar(450)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Id = @UsuarioId)
        THROW 50003, N'El usuario indicado no existe.', 1;

    SELECT *
    FROM soporte.Vista_DiagnosticoIntegridad
    WHERE UsuarioId = @UsuarioId
    ORDER BY MetaId, Entidad, EntidadId;
END;
GO

CREATE OR ALTER PROCEDURE soporte.Soporte_DesbloquearUsuario
    @UsuarioId nvarchar(450),
    @Operador nvarchar(200),
    @Motivo nvarchar(1000)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Operador = NULLIF(LTRIM(RTRIM(@Operador)), N'');
    SET @Motivo = NULLIF(LTRIM(RTRIM(@Motivo)), N'');
    IF @Operador IS NULL OR @Motivo IS NULL
        THROW 50004, N'El operador y el motivo son obligatorios.', 1;

    DECLARE @FinBloqueoAnterior datetimeoffset(7);
    DECLARE @IntentosAnteriores int;
    SELECT @FinBloqueoAnterior = FinBloqueo, @IntentosAnteriores = IntentosFallidos
    FROM dbo.Usuario WHERE Id = @UsuarioId;

    IF @@ROWCOUNT = 0
        THROW 50005, N'El usuario indicado no existe.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.Usuario
        SET FinBloqueo = NULL,
            IntentosFallidos = 0
        WHERE Id = @UsuarioId;

        INSERT soporte.AuditoriaIntervencion
            (Operador, Procedimiento, UsuarioAfectadoId, Entidad, EntidadId, Motivo, ValoresAnteriores, ValoresNuevos, Resultado)
        VALUES
            (@Operador, N'Soporte_DesbloquearUsuario', @UsuarioId, N'Usuario', @UsuarioId, @Motivo,
             (SELECT @FinBloqueoAnterior AS FinBloqueo, @IntentosAnteriores AS IntentosFallidos FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
             N'{"FinBloqueo":null,"IntentosFallidos":0}', N'Correcto');

        COMMIT TRANSACTION;
        SELECT CAST(1 AS bit) AS Exito, N'El usuario fue desbloqueado.' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE soporte.Soporte_CerrarSesionesUsuario
    @UsuarioId nvarchar(450),
    @Operador nvarchar(200),
    @Motivo nvarchar(1000)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Operador = NULLIF(LTRIM(RTRIM(@Operador)), N'');
    SET @Motivo = NULLIF(LTRIM(RTRIM(@Motivo)), N'');
    IF @Operador IS NULL OR @Motivo IS NULL
        THROW 50006, N'El operador y el motivo son obligatorios.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Id = @UsuarioId)
        THROW 50007, N'El usuario indicado no existe.', 1;

    DECLARE @CantidadSesiones int;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.SesionUsuario
        SET FechaRevocacion = SYSUTCDATETIME()
        WHERE UsuarioId = @UsuarioId
          AND FechaRevocacion IS NULL
          AND FechaVencimiento > SYSUTCDATETIME();

        SET @CantidadSesiones = @@ROWCOUNT;

        INSERT soporte.AuditoriaIntervencion
            (Operador, Procedimiento, UsuarioAfectadoId, Entidad, EntidadId, Motivo, ValoresAnteriores, ValoresNuevos, Resultado)
        VALUES
            (@Operador, N'Soporte_CerrarSesionesUsuario', @UsuarioId, N'SesionUsuario', NULL, @Motivo,
             (SELECT @CantidadSesiones AS SesionesActivas FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
             (SELECT 0 AS SesionesActivas FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), N'Correcto');

        COMMIT TRANSACTION;
        SELECT CAST(1 AS bit) AS Exito, @CantidadSesiones AS SesionesCerradas,
               N'Las sesiones activas fueron cerradas.' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE soporte.Soporte_RepararOrdenAcciones
    @UsuarioId nvarchar(450),
    @Fecha date,
    @Operador nvarchar(200),
    @Motivo nvarchar(1000)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Operador = NULLIF(LTRIM(RTRIM(@Operador)), N'');
    SET @Motivo = NULLIF(LTRIM(RTRIM(@Motivo)), N'');
    IF @Operador IS NULL OR @Motivo IS NULL
        THROW 50008, N'El operador y el motivo son obligatorios.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Id = @UsuarioId)
        THROW 50009, N'El usuario indicado no existe.', 1;

    DECLARE @CantidadAcciones int;

    BEGIN TRY
        BEGIN TRANSACTION;

        ;WITH AccionesOrdenadas AS
        (
            SELECT ag.Id,
                   ROW_NUMBER() OVER
                   (
                       ORDER BY CASE WHEN ag.Orden > 0 THEN ag.Orden ELSE 2147483647 END,
                                ap.Hora,
                                ag.Id
                   ) AS NuevoOrden
            FROM dbo.AccionProgramada ag
            INNER JOIN dbo.AccionPlanificada ap ON ap.Id = ag.AccionPlanificadaId
            INNER JOIN dbo.Meta m ON m.Id = ap.MetaId
            WHERE m.UsuarioId = @UsuarioId
              AND ag.FechaProgramada = @Fecha
              AND m.Estado = 1
              AND ap.Estado = 0
        )
        UPDATE ag
        SET Orden = ao.NuevoOrden
        FROM dbo.AccionProgramada ag
        INNER JOIN AccionesOrdenadas ao ON ao.Id = ag.Id;

        SET @CantidadAcciones = @@ROWCOUNT;

        INSERT soporte.AuditoriaIntervencion
            (Operador, Procedimiento, UsuarioAfectadoId, Entidad, EntidadId, Motivo, ValoresAnteriores, ValoresNuevos, Resultado)
        VALUES
            (@Operador, N'Soporte_RepararOrdenAcciones', @UsuarioId, N'AccionProgramada', CONVERT(nvarchar(30), @Fecha, 23), @Motivo,
             NULL, (SELECT @Fecha AS Fecha, @CantidadAcciones AS AccionesReordenadas FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), N'Correcto');

        COMMIT TRANSACTION;
        SELECT CAST(1 AS bit) AS Exito, @CantidadAcciones AS AccionesReordenadas,
               N'El orden diario fue normalizado.' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

PRINT N'Objetos de soporte de Kaizen creados o actualizados correctamente.';
PRINT N'No se asignaron permisos automáticamente. Otorgue SELECT sobre las vistas y EXECUTE sobre los procedimientos solamente al personal autorizado.';
GO

/*
    EJEMPLOS (no se ejecutan):

    EXEC soporte.Soporte_BuscarUsuarioPorCorreo
        @Correo = N'usuario@correo.com';

    EXEC soporte.Soporte_ObtenerResumenUsuario
        @UsuarioId = N'ID-DEL-USUARIO';

    EXEC soporte.Soporte_DesbloquearUsuario
        @UsuarioId = N'ID-DEL-USUARIO',
        @Operador = N'Nombre del operador',
        @Motivo = N'Solicitud validada mediante ticket #1234';

    EXEC soporte.Soporte_CerrarSesionesUsuario
        @UsuarioId = N'ID-DEL-USUARIO',
        @Operador = N'Nombre del operador',
        @Motivo = N'El usuario informó un dispositivo perdido';

    EXEC soporte.Soporte_RepararOrdenAcciones
        @UsuarioId = N'ID-DEL-USUARIO',
        @Fecha = '2026-09-03',
        @Operador = N'Nombre del operador',
        @Motivo = N'Orden diario inconsistente según ticket #1235';
*/
