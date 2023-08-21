using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientPricingSystem.Controllers;
public class ClientController : Controller
{
    IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    public IActionResult Index()
    {
        return View();
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
            await _clientService.CreateClientAsync(
                ClientMapper.MapClientDto_ClientDocument(clientDto)).ConfigureAwait(false);
        }
        return RedirectToAction("Get", "Client", null);
    }

    public async Task<IActionResult> Get()
    {
        List<ClientDocument> clientDocuments = await _clientService.GetAllClientsAsync().ConfigureAwait(false);
        ClientDto clientDto = new ClientDto
        {
            Clients = clientDocuments.Select(c => ClientMapper.MapCleintDocument_ClientDto(c)).ToList()
        };
        return View(clientDto);
    }
}


