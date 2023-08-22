using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.MediatRMethods.Vendor;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClientPricingSystem.Controllers;
public class VendorController : Controller
{
    IMediator _mediator;
    public VendorController(IMediator mediator) 
    {
        _mediator = mediator;
    }
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Get()
    {
        VendorDto vendorDto = await _mediator.Send(new GetAllVendors_ToDto.Query()).ConfigureAwait(false);
        return View(vendorDto);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new VendorDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create(VendorDto vendorDto)
    {
        if (vendorDto != null)
        {
            await _mediator.Send(new CreateVendor_FromDto.Command { VendorDto = vendorDto }).ConfigureAwait(false);
        }
        return RedirectToAction("Get", "Vendor", null);
    }
}
