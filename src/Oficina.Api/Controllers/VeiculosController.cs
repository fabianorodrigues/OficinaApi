using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.DTO.Cadastro;
using Oficina.Application.UseCases.Cadastro;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/veiculos")]
[Authorize]
public class VeiculosController : ControllerBase
{
    private readonly CadastrarVeiculoUseCase _cadastrar;
    private readonly AtualizarVeiculoUseCase _atualizar;
    private readonly ObterVeiculoUseCase _obter;
    private readonly ListarVeiculosPorClienteUseCase _listarPorCliente;

    public VeiculosController(
        CadastrarVeiculoUseCase cadastrar,
        AtualizarVeiculoUseCase atualizar,
        ObterVeiculoUseCase obter,
        ListarVeiculosPorClienteUseCase listarPorCliente)
    {
        _cadastrar = cadastrar;
        _atualizar = atualizar;
        _obter = obter;
        _listarPorCliente = listarPorCliente;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarVeiculoRequest req, CancellationToken ct)
    {
        var id = await _cadastrar.Executar(req.ClienteId, req.Placa, req.Renavam, req.Modelo, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var v = await _obter.Executar(id, ct);
        return Ok(new
        {
            v.Id,
            v.ClienteId,
            placa = v.Placa.Valor,
            renavam = v.Renavam.Valor,
            modelo = new { descricao = v.Modelo.Descricao, marca = v.Modelo.Marca, ano = v.Modelo.Ano }
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarVeiculoRequest req, CancellationToken ct)
    {
        await _atualizar.Executar(id, req.Placa, req.Renavam, req.Modelo, ct);
        return NoContent();
    }

    [HttpGet("por-cliente/{clienteId:guid}")]
    public async Task<IActionResult> ListarPorCliente(Guid clienteId, CancellationToken ct)
    {
        var lista = await _listarPorCliente.Executar(clienteId, ct);
        return Ok(lista.Select(v => new
        {
            v.Id,
            placa = v.Placa.Valor,
            renavam = v.Renavam.Valor,
            modelo = new { descricao = v.Modelo.Descricao, marca = v.Modelo.Marca, ano = v.Modelo.Ano }
        }));
    }
}
