using System.Runtime.CompilerServices;
using FolhaDePagamentoAPI.Data;
using FolhaDePagamentoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FolhaDePagamentoAPI.Controllers;

[ApiController]
[Route("api/folha")]
public class FolhaController : ControllerBase
{
    private readonly AppDataContext _ctx;
   

    public FolhaController(AppDataContext ctx)
    {
        _ctx = ctx;
    }

    //GET: api/folha/listar
    [HttpGet]
    [Route("listar")]

    public IActionResult Listar ()
    {
        try
        {
            List<Folha> folhas =
                _ctx.Folhas
                .Include(x => x.Funcionario)
                .ToList();
            return folhas.Count == 0 ? NotFound() : Ok(folhas);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
           
        }
    }

    //POST : api/funcionario/cadastrar
    [HttpPost]
    [Route("cadastrar")]

    public IActionResult Cadastrar([FromBody] Folha folha)
    {
        try
        {
            
            Funcionario? funcionario = _ctx.Funcionarios.Find(folha.FuncionarioId);
            if (folha == null)
            {
                return NotFound("Funcionário não encontrado.");
            }

            folha.Funcionario = funcionario;

            //funcao para calcular salario bruto
            double SalarioBruto = folha.Valor * folha.Quantidade;

            //calcular o Imposto IRRF
            static double CalcularImpostoRenda(double salario)
            {
                double imposto = 0;

                if (salario <= 1903.98)
                {
                    // Faixa 1: Até R$ 1.903,98, alíquota 0%
                    imposto = 0;
                }
                else if (salario <= 2826.65)
                {
                    // Faixa 2: De R$ 1.903,99 a R$ 2.826,65, alíquota 7,5%
                    imposto = (salario * 0.075) - 142.80;
                }
                else if (salario <= 3751.05)
                {
                    // Faixa 3: De R$ 2.826,66 a R$ 3.751,05, alíquota 15%
                    imposto = (salario * 0.15) - 354.80;
                }
                else if (salario <= 4664.68)
                {
                    // Faixa 4: De R$ 3.751,06 a R$ 4.664,68, alíquota 22,5%
                    imposto = (salario * 0.225) - 636.13;
                }
                else
                {
                    // Faixa 5: Acima de R$ 4.664,68, alíquota 27,5%
                    imposto = (salario * 0.275) - 869.36;
                }

                return imposto;
            }
            //calcular INSS
            static double CalcularINSS(double salario)
            {
                double inss = 0.0;

                if (salario <= 1693.72)
                {
                    inss = salario * 0.08;
                }
                else if (salario <= 2822.90)
                {
                    inss = salario * 0.09;
                }
                else if (salario <= 5645.80)
                {
                    inss = salario * 0.11;
                }
                else
                {
                    inss = 621.03;
                }

                return inss;
            }

            //Calcular FGTS
            static double CalcularFGTS(double salario)
            {
                return salario * 0.08;
            }

            double irrf = CalcularImpostoRenda(SalarioBruto);
            double inss = CalcularINSS(SalarioBruto);
            double fgts = CalcularFGTS(SalarioBruto);

            //calcular Salario Liquido
            double SalarioLiquido = SalarioBruto - irrf - inss;
            
            folha.ImpostoIrrf = irrf;
            folha.ImpostoInss = inss;
            folha.ImpostoFgts = fgts;
            folha.SalarioBruto = SalarioBruto;
            folha.SalarioLiquido = SalarioLiquido;

            _ctx.Folhas.Add(folha);
            _ctx.SaveChanges();
            return Created("", folha);
        }
        catch (Exception e)
        {
           Console.WriteLine(e);
           return BadRequest(e.Message);
        }
    }

    // GET: api/folha/buscar/{cpf}/{mes}/{ano}
    [HttpGet("buscar/{cpf}/{mes}/{ano}")]
    public IActionResult Buscar([FromRoute] string cpf, int mes, int ano)
    {
        try
        {

            Folha? folhaCadastrada =_ctx.Folhas
            .Include(x => x.Funcionario)
            .FirstOrDefault(x => x.Funcionario.Cpf == cpf && x.Mes == mes && x.Ano == ano);
            
            if (folhaCadastrada != null)
            {
                return Ok(folhaCadastrada);
            }
            else 
            {
                return NotFound($"Nenhuma folha encontrada para o CPF {cpf}, mês {mes} e ano {ano}");
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

 }


