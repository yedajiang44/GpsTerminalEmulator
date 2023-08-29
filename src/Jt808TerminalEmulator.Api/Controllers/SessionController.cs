using Jt808TerminalEmulator.Core.Abstract;
using Jt808TerminalEmulator.Model.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jt808TerminalEmulator.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
    private readonly ITcpClientManager tcpClientManager;

    public SessionController(ITcpClientManager tcpClientManager)
    {
        this.tcpClientManager = tcpClientManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        try
        {
            return Ok(new JsonResultDto<IEnumerable<Core.Netty.ISession>>
            {
                Flag = true,
                Data = tcpClientManager.GetTcpClients().SelectMany(x => x.Sesions().Result)
            });
        }
        catch (Exception e)
        {
            return Ok(new JsonResultDto<IEnumerable<Core.Netty.ISession>>
            {
                Message = e.Message
            });
        }
    }
    [HttpGet("[action]")]
    public IActionResult Count()
    {
        try
        {
            return Ok(new JsonResultDto<long>
            {
                Flag = true,
                Data = tcpClientManager.GetTcpClients().SelectMany(x => x.Sesions().Result).Count()
            });
        }
        catch (Exception e)
        {
            return Ok(new JsonResultDto<long>
            {
                Message = e.Message
            });
        }
    }
}