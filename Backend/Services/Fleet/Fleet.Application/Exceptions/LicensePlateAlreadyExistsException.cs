using System;
using System.Collections.Generic;
using System.Text;

namespace Fleet.Application.Exceptions
{
    public class LicensePlateAlreadyExistsException : Exception
    {
        public LicensePlateAlreadyExistsException(string Plate) : base($"Ya existe un camión con la patente \"{Plate}\".") { }
    }
}
