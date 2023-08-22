using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace ClientPricingSystem.Configuration.Mapper;

[Mapper]
public static partial class ClientMapper
{
    public static partial ClientDto MapClientDocument_ClientDto(ClientDocument clientDocument);
    public static partial ClientDocument MapClientDto_ClientDocument(ClientDto clientDto);
}