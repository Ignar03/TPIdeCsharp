USE TorneosDB;
GO

IF OBJECT_ID('dbo.Torneos', 'U') IS NOT NULL
    DROP TABLE dbo.Torneos;
GO

CREATE TABLE dbo.Torneos (
    Id              INT             IDENTITY(1,1) PRIMARY KEY,
    Nombre          NVARCHAR(100)   NOT NULL,
    IdRegion        INT             NOT NULL,
    FechaInicio     DATETIME2(0)    NOT NULL,
    FechaFin        DATETIME2(0)    NOT NULL,
    Activo          BIT             NOT NULL CONSTRAINT DF_Torneos_Activo DEFAULT (1),
    MinimoMedallas  INT             NOT NULL
);
GO

INSERT INTO dbo.Torneos (Nombre, IdRegion, FechaInicio, FechaFin, Activo, MinimoMedallas)
VALUES
('La Docta',        4, '2025-04-08', '2025-04-15', 1, 3),
('Copa Córdoba',    2, '2025-05-10', '2025-05-20', 1, 2);
GO
