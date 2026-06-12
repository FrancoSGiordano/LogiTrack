using System;
using System.Collections.Generic;
using System.Text;

namespace Fleet.Application.Exceptions
{
    public class TruckNotFoundException : Exception
    {
        public TruckNotFoundException(Guid Id) : base($"El camión con ID: {Id} no fue encontrado.") { }
    }
}
