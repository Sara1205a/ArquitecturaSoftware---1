using System;
using System.Collections.Generic;
using System.Text;

namespace DentaCare.Legacy
{
    public class Odontologo
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public int EspecialidadId { get; set; } // 1=Ortodoncia, 2=Endodoncia, 3=Cirugia, 4=Odontopediatria
        public bool EstaDisponible { get; set; }
    }
}