using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.MediatRMethods.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClientPricingSystem.Controllers;
public class ClientController : Controller
{
    IMediator _mediator;
    public ClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Get()
    {
        ClientDto clientDto = await _mediator.Send(new GetAllCleints_ToDto.Query()).ConfigureAwait(false);
        return View(clientDto);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ClientDto clientDto)
    {
        if (clientDto != null)
        {
            await _mediator.Send(new CreateClient_FromDto.Query { ClientDto = clientDto }).ConfigureAwait(false);
        }
        return RedirectToAction("Get", "Client", null);
    }
}


