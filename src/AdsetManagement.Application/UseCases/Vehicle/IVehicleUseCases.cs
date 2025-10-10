using AdsetManagement.Application.DTOs.Requests;
using AdsetManagement.Application.DTOs.Responses;
using AdsetManagement.Domain.Common;

namespace AdsetManagement.Application.UseCases.Vehicle;

public interface ICreateVehicleUseCase
{
    Task<Result<VehicleResponse>> ExecuteAsync(CreateVehicleRequest request);
}

public interface IGetVehicleByIdUseCase
{
    Task<Result<VehicleResponse>> ExecuteAsync(int id);
}

public interface IGetVehiclesUseCase
{
    Task<Result<VehicleListResponse>> ExecuteAsync(VehicleFilterRequest filter);
}

public interface IUpdateVehicleUseCase
{
    Task<Result<VehicleResponse>> ExecuteAsync(int id, UpdateVehicleRequest request);
}

public interface IDeleteVehicleUseCase
{
    Task<Result> ExecuteAsync(int id);
}