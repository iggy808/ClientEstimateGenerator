using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Methods.Client;

namespace ClientPricingSystem.Views.ViewModels

{
    public class TableViewModel
    {
        public List<ClientDto> Records { get; set; }
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(Records.Count() / (double)RecordsPerPage));
        }
        public List<ClientDto> PaginatedRecords()
        {
            int start = (CurrentPage - 1) * RecordsPerPage;
            return Records.Skip(start).Take(RecordsPerPage).ToList();
        }
    }
}
