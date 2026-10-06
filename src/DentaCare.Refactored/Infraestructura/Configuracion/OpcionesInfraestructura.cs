namespace DentaCare.Refactored.Infraestructura.Configuracion;

public sealed record OpcionesSqlServer(string CadenaConexion);
public sealed record OpcionesSmtp(string Servidor, int Puerto, string Remitente);
public sealed record OpcionesTwilio(string ApiKey);