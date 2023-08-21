using System.ComponentModel.DataAnnotations;

namespace ClientPricingSystem.Core.Dtos;
public class ClientDto
{
    // Document fields
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }

    [DisplayFormat(DataFormatString = "{0:0.00}")]
    public decimal MarkupRate { get; set; }
    
    // View fields
    public List<ClientDto> Clients { get; set; }
}

