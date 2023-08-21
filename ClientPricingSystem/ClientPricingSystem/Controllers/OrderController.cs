using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Enums;
using ClientPricingSystem.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClientPricingSystem.Controllers;
public class OrderController : Controller
{
    IClientService _clientService;
    IOrderService _orderService;
    IVendorService _vendorService;

    public OrderController(IOrderService orderService, IClientService clientService, IVendorService vendorService)
    {
        _clientService = clientService;
        _orderService = orderService;
        _vendorService = vendorService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Get()
    {
        List<OrderDocument> orders = await _orderService.GetAllOrdersAsync().ConfigureAwait(false);
        OrderDto orderDto = new OrderDto
        {
            Orders = orders.Select(o => OrderMapper.MapOrderDocument_OrderDto(o)).ToList(),
        };
        return View(orderDto);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        OrderDto orderDto = new OrderDto
        {
            Clients = (await _clientService.GetAllClientsAsync().ConfigureAwait(false))
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }),
            Vendors = (await _vendorService.GetAllVendorsAsync().ConfigureAwait(false))
                .Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }),
            Sizes = Enum.GetNames(typeof(Size)).Select(s => new SelectListItem { Text = s, Value = s})
        };
        return View(orderDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderDto order)
    {
        if (order != null)
        {
            await _orderService.CreateOrderAsync(
                OrderMapper.MapOrderDto_OrderDocument(order)).ConfigureAwait(false);
        }
        return RedirectToAction("Get", "Order", null);
    }
}