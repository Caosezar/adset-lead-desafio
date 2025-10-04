using Microsoft.AspNetCore.Mvc;
using AdsetManagement.Application.DTOs.Requests;
using AdsetManagement.Application.DTOs.Responses;
using AdsetManagement.Application.Interfaces;

namespace AdsetManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpPost]
    public async Task<ActionResult<VehicleResponse>> CreateVehicle([FromBody] CreateVehicleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _vehicleService.CreateVehicleAsync(request);
        return CreatedAtAction(nameof(GetVehicleById), new { id = response.Id }, response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleResponse>> GetVehicleById(int id)
    {
        var response = await _vehicleService.GetVehicleByIdAsync(id);
        
        if (response == null)
            return NotFound($"Veículo com ID {id} não encontrado");

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<VehicleListResponse>> GetVehicles([FromQuery] VehicleFilterRequest filter)
    {
        var response = await _vehicleService.GetVehiclesAsync(filter);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<VehicleResponse>> UpdateVehicle(int id, [FromBody] UpdateVehicleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _vehicleService.UpdateVehicleAsync(id, request);
        if (response == null)
            return NotFound($"Veículo com ID {id} não encontrado");

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteVehicle(int id)
    {
        var result = await _vehicleService.DeleteVehicleAsync(id);
        if (!result)
            return NotFound($"Veículo com ID {id} não encontrado");

        return Ok(new { Message = "Veículo excluído com sucesso", Id = id });
    }

    [HttpPut("{id:int}/pacotes")]
    public async Task<ActionResult<VehicleResponse>> UpdateVehiclePacotes(int id, [FromBody] UpdatePacotesRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _vehicleService.UpdateVehiclePacotesAsync(id, request);
        if (response == null)
            return NotFound($"Veículo com ID {id} não encontrado");

        return Ok(response);
    }
}