using System.Text.Json;

namespace FunkosShopBack_end.Controllers
{
    public class CoinGeckoApi : IDisposable
    {
        private const string API_URL = "https://api.coingecko.com/api/v3/";

        private HttpClient HttpClient {  get; init; }

        public CoinGeckoApi()
        {
            HttpClient = new HttpClient()
            {
                BaseAddress = new Uri(API_URL)
            };
        }

        public async Task<decimal> GetEthereumPriceAsync()
        {
            string json = await HttpClient.GetStringAsync("coins/ethereum");
            JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

            // Obtenemos los datos del mercado
            JsonElement jsonMarketData = jsonElement.GetProperty("market_data");

            // Obtenemos las propiedades
            JsonElement jsonPrices = jsonMarketData.GetProperty("current_price");

            // Obtenemos el precio en euros
            decimal euros = jsonPrices.GetProperty("eur").GetDecimal();
            return euros;
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }
    }
}
