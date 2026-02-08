using MongoDB.Entities;
using SearchService.DTOs;

namespace SearchService;

public class AuctionSvcHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<Item>> GetItemsForSearchDb()
    {
        string lastUpdated = null;
        var count = await DB.CountAsync<Item>();
        if (count > 0)
        {
            lastUpdated = await DB.Find<Item, string>()
                .Sort(x => x.Descending(x => x.UpdatedAt))
                .Project(x => x.UpdatedAt.ToString())
                .ExecuteFirstAsync();
        }

        var url = _config["AuctionServiceUrl"] + "/api/auctions";
        if (!string.IsNullOrEmpty(lastUpdated))
            url += "?date=" + lastUpdated;

        var dtos = await _httpClient.GetFromJsonAsync<List<AuctionHttpDto>>(url) ?? new List<AuctionHttpDto>();

        return dtos.Select(dto => new Item
        {
            ID = dto.Id.ToString(),
            ReservePrice = dto.ReservePrice,
            Seller = dto.Seller,
            Winner = dto.Winner,
            SoldAmount = dto.SoldAmount,
            CurrentHighBid = dto.CurrentHighBid,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            AuctionEnd = dto.AuctionEnd,
            Status = dto.Status,
            Make = dto.Make,
            Model = dto.Model,
            Year = dto.Year,
            Color = dto.Color,
            Mileage = dto.Mileage,
            ImageUrl = dto.ImageUrl
        }).ToList();
    }
}