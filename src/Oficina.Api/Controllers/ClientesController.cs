using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.DTO.Cadastro;
using Oficina.Application.UseCases.Cadastro;
using Oficina.Domain.Cadastro.ValueObjects;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly CadastrarClienteUseCase _cadastrar;
    private readonly ObterClienteUseCase _obter;

    public ClientesController(CadastrarClienteUseCase cadastrar, ObterClienteUseCase obter)
    {
        _cadastrar = cadastrar;
        _obter = obter;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarClienteRequest req, CancellationToken ct)
    {
        var id = await _cadastrar.Executar(req.CpfCnpj, req.Nome, req.Email, req.Telefone, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var cliente = await _obter.Executar(id, ct);
        return Ok(
            new 
            { 
                cliente.Id, 
                cpfCnpj = cliente.Documento.Valor, 
                nome = cliente.Nome,  
                contato = new { email = cliente.Contato.Email, telefone = cliente.Contato.Telefone }
            });
    }
}
