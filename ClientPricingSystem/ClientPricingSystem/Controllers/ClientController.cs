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

    public async Task<IActionResult> Index()
    {
        ClientDto clientDto = await _mediator.Send(new GetAllCleints_ToDto.Query()).ConfigureAwait(false);
        return View(clientDto);
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
            try
            {
                await _mediator.Send(new CreateClient_FromDto.Command { ClientDto = clientDto }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // TODO: Figure out a better way to handle this.
                Console.WriteLine("Error occured while creating client:\n" + e.Message);
            }
        }
        return RedirectToAction("Get", "Client", null);
    }
}


