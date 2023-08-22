﻿using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Methods.Vendor;
using ClientPricingSystem.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClientPricingSystem.Controllers;
public class VendorController : Controller
{
    IVendorService _vendorService;
    IMediator _mediator;
    public VendorController(IVendorService vendorService, IMediator mediator) 
    {
        _vendorService = vendorService;
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
    public async Task<IActionResult> Create(VendorDto vendor)
    {
        if (vendor != null)
        {
            await _mediator.Send(new CreateVendor_FromDto.Query { VendorDto = vendor }).ConfigureAwait(false);
        }
        return RedirectToAction("Get", "Vendor", null);
    }
}
