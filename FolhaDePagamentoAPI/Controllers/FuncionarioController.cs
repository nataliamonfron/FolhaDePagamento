using FolhaDePagamentoAPI.Data;
using FolhaDePagamentoAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FolhaDePagamentoAPI.Controllers;

[ApiController]
[Route("api/funcionario")]
public class FuncionarioController : ControllerBase
{
    private readonly AppDataContext _ctx;
      
    public FuncionarioController(AppDataContext ctx)
    {
        _ctx = ctx;
    }

    //Get: api/funcionario/listar
    [HttpGet]
    [Route("listar")]

    public IActionResult Listar ()
    {
        try
        {
            List<Funcionario> funcionarios = _ctx.Funcionarios.ToList();
            return funcionarios.Count == 0 ? NotFound() : Ok(funcionarios);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
           
        }
    }

    //Post : api/funcionario/cadastrar
    [HttpPost]
    [Route("cadastrar")]

    public IActionResult Cadastrar([FromBody] Funcionario funcionario)
    {
        try
        {
            
            _ctx.Funcionarios.Add(funcionario);
            _ctx.SaveChanges();
            return Created("", funcionario);
        }
        catch (Exception e)
        {
           Console.WriteLine(e);
           return BadRequest(e.Message);
        }
    }

    //Post : api/folha/cadastrar

 }


