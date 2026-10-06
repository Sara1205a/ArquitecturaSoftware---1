using DentaCare.Refactored;
using DentaCare.Refactored.Dominio.Modelo;

Console.WriteLine("=================================================");
Console.WriteLine(" DENTACARE SYSTEM - MÓDULO REFACTORIZADO DE CITAS");
Console.WriteLine("=================================================\n");

try
{
    // Sin variable de entorno corre en modo DEMO (memoria + consola).
    bool modoProduccion = string.Equals(
        Environment.GetEnvironmentVariable("DENTACARE_MODO"), "PRODUCCION", StringComparison.OrdinalIgnoreCase);

    ServiciosClinica servicios = ComposicionRaiz.Construir(modoProduccion);

    var paciente1 = new Paciente(
        "PAC-101", "Ana María Gómez", "ana.gomez@email.com", "3001234567",
        TipoConvenio.Eps, esPrimeraVez: true);

    var odontologo1 = new Odontologo(
        "ODO-202", "Dr. Roberto Martínez", EspecialidadOdontologica.Cirugia, estaDisponible: true);

    Console.WriteLine("---> [FLUJO 1]: AGENDAMIENTO DE CITA ODONTOLÓGICA");
    DateTime fechaCita = DateTime.Now.AddHours(12);
    Cita citaAgendada = servicios.Agendar.Ejecutar(paciente1, odontologo1, fechaCita, requiereRadiografia: true);

    Console.WriteLine($"\n Cita generada exitosamente con ID: {citaAgendada.Id}");
    Console.WriteLine($" Copago Final Calculado: ${citaAgendada.CopagoCalculado:N2}");
    Console.WriteLine($" Estado de la Cita: {citaAgendada.Estado.ToString().ToUpperInvariant()}\n");

    Console.WriteLine("-------------------------------------------------");
    Console.WriteLine("---> [FLUJO 2]: CANCELACIÓN DE CITA (MENOS DE 24 HORAS)");

    decimal penalizacion = servicios.Cancelar.Ejecutar(citaAgendada, DateTime.Now);

    Console.WriteLine($"\n Cita ID #{citaAgendada.Id} ha sido actualizada.");
    Console.WriteLine($" Nuevo Estado: {citaAgendada.Estado.ToString().ToUpperInvariant()}");
    Console.WriteLine($" Penalización Aplicada: ${penalizacion:N2}\n");

    Console.WriteLine("-------------------------------------------------");
    Console.WriteLine("---> [FLUJO 3]: CONSULTA DE CITAS DEL PACIENTE");
    foreach (var r in servicios.Consultar.PorPaciente(paciente1.Id))
    {
        Console.WriteLine($" Cita #{r.Id} | {r.FechaHora:yyyy-MM-dd HH:mm} | Copago ${r.Copago:N2} | {r.Estado.ToString().ToUpperInvariant()} | Penalización ${r.Penalizacion:N2}");
    }

    Console.WriteLine("\n-------------------------------------------------");
    Console.WriteLine("---> [FLUJO 4]: REPORTE DE TOTALES ACUMULADOS");
    Console.WriteLine($" Total Recaudado en Copagos: ${servicios.Estadisticas.ObtenerTotalRecaudado():N2}");
    Console.WriteLine($" Total Citas Canceladas: {servicios.Estadisticas.ObtenerTotalCanceladas()}");
}
catch (Exception ex)
{
    Console.WriteLine($"\n ERROR CRÍTICO EN EL SISTEMA: {ex.Message}");
}

Console.WriteLine("\n=================================================");
Console.WriteLine("Presione cualquier tecla para salir...");
Console.ReadKey();