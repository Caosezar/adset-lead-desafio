namespace AdsetManagement.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class VehicleNotFoundException : DomainException
{
    public VehicleNotFoundException(int id) : base($"Veículo com ID {id} não foi encontrado.") { }
}

public class InvalidVehicleDataException : DomainException
{
    public InvalidVehicleDataException(string message) : base(message) { }
}