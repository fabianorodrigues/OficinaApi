using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.DTO.CatalogoEstoque;
using Oficina.Application.UseCases.CatalogoEstoque;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/pecas")]
[Authorize]
public class PecasController : ControllerBase
{
    private readonly CadastrarPecaUseCase _cadastrar;

    public PecasController(CadastrarPecaUseCase cadastrar) => _cadastrar = cadastrar;

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarPecaRequest req, CancellationToken ct)
    {
        var id = await _cadastrar.Executar(req.PrecoUnitario, ct);
        return CreatedAtAction(nameof(Cadastrar), new { id }, new { id });
    }
}
