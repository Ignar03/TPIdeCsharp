CREATE VIEW vw_TorneosActivos AS
SELECT Id, Nombre, IdRegion, FechaInicio, FechaFin, Activo, MinimoMedallas
FROM Torneos
WHERE Activo = 1;
