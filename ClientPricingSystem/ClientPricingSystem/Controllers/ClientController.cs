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

    public async Task<IActionResult> Sort(string sortOrder)
    {
        ViewBag.ClientNameSortParam = String.IsNullOrEmpty(sortOrder) ? "ClientName_desc" : "";
        ViewBag.ClientAddressSortParam = sortOrder == "Address" ? "Address_desc" : "Address";
        ViewBag.ClientAddressSortParam = sortOrder == "MarkupRate" ? "MarkupRate_desc" : "MarkupRate";

        ClientDto clientDto = await _mediator.Send(new GetAllCleints_ToDto.Query()).ConfigureAwait(false);
        var result = new List<ClientDto>();
        switch (sortOrder)
        {
            case "ClientName_desc":
                result = clientDto.Clients.OrderByDescending(a => a.Name).ToList();
                break;
            case "Address":
                result = clientDto.Clients.OrderBy(a => a.Address).ToList();
                break;
            case "Address_desc":
                result = clientDto.Clients.OrderByDescending(a => a.Address).ToList();
                break;
            case "MarkupRate":
                result = clientDto.Clients.OrderBy(a => a.MarkupRate).ToList();
                break;
            case "MarkupRate_desc":
                result = clientDto.Clients.OrderByDescending(a => a.MarkupRate).ToList();
                break;

        }

        return PartialView("_TableView", result);

    }
}


