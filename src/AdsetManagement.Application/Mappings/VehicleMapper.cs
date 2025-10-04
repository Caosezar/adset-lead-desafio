using AdsetManagement.Application.DTOs.Requests;
using AdsetManagement.Application.DTOs.Responses;
using AdsetManagement.Domain.Entities;

namespace AdsetManagement.Application.Mappings;

public static class VehicleMapper
{
    public static Vehicle Map(CreateVehicleRequest request)
    {
        return new Vehicle
        {
            Imagens = request.Imagens,
            Marca = request.Marca,
            Modelo = request.Modelo,
            Ano = request.Ano,
            Placa = request.Placa,
            Km = request.Km,
            Cor = request.Cor,
            Preco = request.Preco,
            OtherOptions = request.OtherOptions != null ? new OtherOptions
            {
                ArCondicionado = request.OtherOptions.ArCondicionado,
                Alarme = request.OtherOptions.Alarme,
                Airbag = request.OtherOptions.Airbag,
                ABS = request.OtherOptions.ABS
            } : null,
            PacoteICarros = request.PacoteICarros,
            PacoteWebMotors = request.PacoteWebMotors
        };
    }

    public static void Map(Vehicle vehicle, UpdateVehicleRequest request)
    {
        vehicle.Imagens = request.Imagens;
        vehicle.Marca = request.Marca;
        vehicle.Modelo = request.Modelo;
        vehicle.Ano = request.Ano;
        vehicle.Placa = request.Placa;
        vehicle.Km = request.Km;
        vehicle.Cor = request.Cor;
        vehicle.Preco = request.Preco;
        vehicle.OtherOptions = request.OtherOptions != null ? new OtherOptions
        {
            ArCondicionado = request.OtherOptions.ArCondicionado,
            Alarme = request.OtherOptions.Alarme,
            Airbag = request.OtherOptions.Airbag,
            ABS = request.OtherOptions.ABS
        } : null;
        vehicle.PacoteICarros = request.PacoteICarros;
        vehicle.PacoteWebMotors = request.PacoteWebMotors;
        vehicle.UpdateDate = DateTime.UtcNow;
    }

    public static void Map(Vehicle vehicle, UpdatePacotesRequest request)
    {
        vehicle.PacoteICarros = request.PacoteICarros;
        vehicle.PacoteWebMotors = request.PacoteWebMotors;
        vehicle.UpdateDate = DateTime.UtcNow;
    }

    public static VehicleResponse Map(Vehicle vehicle)
    {
        return new VehicleResponse
        {
            Id = vehicle.Id,
            Imagens = vehicle.Imagens,
            Marca = vehicle.Marca,
            Modelo = vehicle.Modelo,
            Ano = vehicle.Ano,
            Placa = vehicle.Placa,
            Km = vehicle.Km,
            Cor = vehicle.Cor,
            Preco = vehicle.Preco,
            OtherOptions = vehicle.OtherOptions != null ? new OtherOptionsResponse
            {
                ArCondicionado = vehicle.OtherOptions.ArCondicionado,
                Alarme = vehicle.OtherOptions.Alarme,
                Airbag = vehicle.OtherOptions.Airbag,
                ABS = vehicle.OtherOptions.ABS
            } : null,
            PacoteICarros = vehicle.PacoteICarros,
            PacoteWebMotors = vehicle.PacoteWebMotors,
            CreationDate = vehicle.CreationDate,
            UpdateDate = vehicle.UpdateDate,
            CreateUserId = vehicle.CreateUserId,
            UpdateUserId = vehicle.UpdateUserId
        };
    }

    public static VehicleListResponse Map(IEnumerable<Vehicle> vehicles, int totalItems, int page, int pageSize)
    {
        return new VehicleListResponse
        {
            Data = vehicles.Select(Map).ToList(),
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
        };
    }
}