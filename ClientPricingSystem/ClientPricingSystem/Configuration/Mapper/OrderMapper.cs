using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using Newtonsoft.Json;
using Riok.Mapperly.Abstractions;

namespace ClientPricingSystem.Configuration.Mapper;
[Mapper]
public static partial class OrderMapper
{
    public static partial OrderDto MapOrderDocument_OrderDto(OrderDocument orderDocument);
    [MapProperty(nameof(OrderDto.ItemsJson), nameof(OrderDocument.Items))]
    public static partial OrderDocument MapOrderDto_OrderDocument(OrderDto orderDto);
    public static List<OrderItemDocument>? ItemsJsonToItems(string itemsJson) => JsonConvert.DeserializeObject<List<OrderItemDocument>?>(itemsJson);
}