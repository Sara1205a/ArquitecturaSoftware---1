using DentaCare.Legacy;

Console.WriteLine("=================================================");
Console.WriteLine(" DENTACARE SYSTEM - MÓDULO LEGADO DE CITAS"); 
Console.WriteLine("=================================================\\n"); 
try 
{
    GestorCitasOdontologicas gestor = new GestorCitasOdontologicas(); 
    
    Paciente paciente1 = new Paciente 
    { 
        Id = "PAC-101", 
        NombreCompleto = "Ana María Gómez", 
        Correo = "ana.gomez@email.com", 
        Celular = "3001234567", TipoConvenio = 2, // 2 = EPS (Convenio del 70% de cobertura)
        EsPrimeraVez = true 
    }; 
    
    Odontologo odontologo1 = new Odontologo 
    { 
        Id = "ODO-202", 
        Nombre = "Dr. Roberto Martínez", 
        EspecialidadId = 3, // 3 = Cirugía
        EstaDisponible = true
    }; 
    
    Console.WriteLine("---> [FLUJO 1]: AGENDAMIENTO DE CITA ODONTOLÓGICA"); 
    DateTime fechaCita = DateTime.Now.AddHours(12); 
    
    Cita citaAgendada = gestor.AgendarCita( paciente1, odontologo1, fechaCita, 3, /* 3 = Cirugía */ true); 

    Console.WriteLine($"\n Cita generada exitosamente con ID: {citaAgendada.Id}"); 
    Console.WriteLine($" Copago Final Calculado: ${citaAgendada.CopagoCalculado:N2}"); 
    Console.WriteLine($" Estado de la Cita: {citaAgendada.Estado}\\n"); 

    Console.WriteLine("-------------------------------------------------"); 
    Console.WriteLine("---> [FLUJO 2]: CANCELACIÓN DE CITA (MENOS DE 24 HORAS)");

    DateTime fechaCancelacion = DateTime.Now;
    decimal penalizacion = gestor.CancelarCita(citaAgendada, fechaCancelacion);
    
    Console.WriteLine($"\n Cita ID #{citaAgendada.Id} ha sido actualizada."); 
    Console.WriteLine($" Nuevo Estado: {citaAgendada.Estado}");     
    Console.WriteLine($" Penalización Aplicada: ${penalizacion:N2}\\n"); 

    Console.WriteLine("-------------------------------------------------"); 
    Console.WriteLine("---> [FLUJO 3]: REPORTE DE TOTALES ACUMULADOS EN MEMORIA");
    Console.WriteLine($" Total Recaudado en Copagos: ${gestor.ObtenerTotalRecaudado():N2}"); 
    Console.WriteLine($" Total Citas Canceladas: {gestor.ObtenerTotalCanceladas()}"); 
} 
catch (Exception ex) 
{ 
    Console.WriteLine($"\n ERROR CRÍTICO EN EL SISTEMA: {ex.Message}"); 
} 

Console.WriteLine("\n================================================="); 
Console.WriteLine("Presione cualquier tecla para salir..."); 
Console.ReadKey();