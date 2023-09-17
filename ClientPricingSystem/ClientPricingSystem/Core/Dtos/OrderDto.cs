using ClientPricingSystem.Core.Documents;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClientPricingSystem.Core.Dtos;
public class OrderDto
{
    // Document fields
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public ClientDocument Client { get; set; }
    public decimal ArtistFee { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemDocument> Items { get; set; }

    // View fields
    public List<OrderDto> Orders { get; set; }
    public IEnumerable<SelectListItem> Clients { get; set; }
    public IEnumerable<SelectListItem> Sizes { get; set; }
    public IEnumerable<SelectListItem> Vendors { get; set; }
    public OrderItemDocument Item { get; set; }
    public string? ItemsJson { get; set; }
}

