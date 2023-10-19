using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Methods.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
        return RedirectToAction("Index", "Client", null);
    }
    [HttpGet]
    public async Task<IActionResult> Search(string searchText)
    {
        ClientDto clientDto = await _mediator.Send(new GetAllCleints_ToDto.Query()).ConfigureAwait(false);
        if (searchText != null)
        {
            Console.WriteLine(clientDto.Clients);
            if (clientDto.Clients != null)
            {
                var result = clientDto.Clients.Where(a => a.Name.ToLower().Contains(searchText.ToLower()) || a.Address.ToLower().Contains(searchText) || a.MarkupRate.ToString().Contains(searchText)).ToList();
                return PartialView("_TableView", result);
            }
           
        }
        return PartialView("_TableView", clientDto.Clients);
        
    }
}


