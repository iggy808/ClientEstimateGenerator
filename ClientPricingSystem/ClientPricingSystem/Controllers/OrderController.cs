using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Enums;
using ClientPricingSystem.Core.Methods.Client;
using ClientPricingSystem.Core.Methods.Order;
using ClientPricingSystem.Core.Methods.Vendor;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClientPricingSystem.Controllers;
public class OrderController : Controller
{
    IMediator _mediator;
    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Get()
    {
        OrderDto orderDto = await _mediator.Send(new GetAllOrders_ToDto.Query()).ConfigureAwait(false);
        return View(orderDto);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        OrderDto orderDto = new OrderDto
        {
            Clients = (await _mediator.Send(new GetAllCleints_ToDto.Query()).ConfigureAwait(false))
                .Clients.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }),
            Vendors = (await _mediator.Send(new GetAllVendors_ToDto.Query()).ConfigureAwait(false))
                .Vendors.Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }),
            Sizes = Enum.GetNames(typeof(Size)).Select(s => new SelectListItem { Text = s, Value = s})
        };
        return View(orderDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderDto orderDto)
    {
        if (orderDto != null)
        {
            try
            {
                await _mediator.Send(new CreateOrder_FromDto.Command { OrderDto = orderDto }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // TODO: Figure out a better way to handle this.
                Console.WriteLine("Error occured while creating order:\n" + e.Message);
            }
        }
        return RedirectToAction("Get", "Order", null);
    }
}