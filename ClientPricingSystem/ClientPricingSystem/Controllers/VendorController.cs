using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientPricingSystem.Controllers;
public class VendorController : Controller
{
    IVendorService _vendorService;
    public VendorController(IVendorService vendorService) 
    {
        _vendorService = vendorService;
    }
    public IActionResult Index()
    {
        return View();
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
            await _vendorService.CreateVendorAsync(
                VendorMapper.MapVendorDto_VendorDocument(vendorDto)).ConfigureAwait(false);
        }
        return RedirectToAction("Get", "Vendor", null);
    }

    public async Task<IActionResult> Get()
    {
        List<VendorDocument> vendors = await _vendorService.GetAllVendorsAsync().ConfigureAwait(false);
        VendorDto vendorDto = new VendorDto
        {
            Vendors = vendors.Select(v => VendorMapper.MapVendorDocument_VendorDto(v)).ToList()
        };
        return View(vendorDto);
    }
}
