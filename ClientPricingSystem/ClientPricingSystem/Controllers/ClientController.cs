using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Methods.Client;
using ClientPricingSystem.Views.ViewModels;
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
    public async Task<IActionResult> Search(string searchText, string sortBy ,string sortDirection, int page=1)
    {

        /*ViewBag.ClientNameSortParam = String.IsNullOrEmpty(sortDirection) ? "ClientName_desc" : "";
        ViewBag.ClientAddressSortParam = sortDirection == "Address" ? "Address_desc" : "Address";
        ViewBag.ClientAddressSortParam = sortDirection == "MarkupRate" ? "MarkupRate_desc" : "MarkupRate"; */

        ClientDto clientDto = await _mediator.Send(new GetAllCleints_ToDto.Query()).ConfigureAwait(false);
        var result = new List<ClientDto>();

        //Sort
        switch ((sortBy, sortDirection))
        {
            case ("Name","desc"):
                result = clientDto.Clients.OrderByDescending(a => a.Name).ToList();
                break;
            case ("Address", "asc"):
                result = clientDto.Clients.OrderBy(a => a.Address).ToList();
                break;
            case ("Address", "desc"):
                result = clientDto.Clients.OrderByDescending(a => a.Address).ToList();
                break;
            case ("MarkupRate", "asc"):
                result = clientDto.Clients.OrderBy(a => a.MarkupRate).ToList();
                break;
            case ("MarkupRate", "desc"):
                result = clientDto.Clients.OrderByDescending(a => a.MarkupRate).ToList();
                break;
            default:
                result = clientDto.Clients.OrderBy(a => a.Name).ToList();
                break;

        }
        //Search
        if (searchText != null)
        {
            Console.WriteLine(clientDto.Clients);
            if (clientDto.Clients != null)
            {
                result = clientDto.Clients.Where(a => a.Name.ToLower().Contains(searchText.ToLower()) || a.Address.ToLower().Contains(searchText) || a.MarkupRate.ToString().Contains(searchText)).ToList();
                var tableViewSearch = new TableViewModel
                {
                    RecordsPerPage = 10,
                    Records = result,
                    CurrentPage = page
                };
                return PartialView("_TableView", tableViewSearch);
            }
           
        }
        var tableView = new TableViewModel
        {
            RecordsPerPage = 10,
            Records = result,
            CurrentPage = page
        };
        return PartialView("_TableView", tableView);
        
    }

}


