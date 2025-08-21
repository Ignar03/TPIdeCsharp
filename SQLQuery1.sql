
CREATE DATABASE TorneosDB;
GO

USE TorneosDB;
GO

IF OBJECT_ID('dbo.Torneos', 'U') IS NOT NULL
    DROP TABLE dbo.Torneos;
GO

CREATE TABLE dbo.Torneos (
    id              INT             IDENTITY(1,1) PRIMARY KEY,
    nombre_torneo          NVARCHAR(100)   NOT NULL,
    id_region        INT             NOT NULL,
    fecha_inicio     DATE    NOT NULL,
    fecha_fin        DATE    NOT NULL,
    torneo_activo          BIT             NOT NULL CONSTRAINT DF_Torneos_Activo DEFAULT (1),
    minimo_medallas  INT             NOT NULL
);
GO

INSERT INTO dbo.Torneos (nombre_torneo, id_region, fecha_inicio, fecha_fin, torneo_activo, minimo_medallas)
VALUES
('La Docta',        4, '2025-04-08', '2025-04-15', 1, 3),
('Copa Córdoba',    2, '2025-05-10', '2025-05-20', 1, 2);
GO
