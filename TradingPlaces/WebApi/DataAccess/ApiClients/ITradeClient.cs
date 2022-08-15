namespace TradingPlaces.WebApi.DataAccess.ApiClients
{
    public interface ITradeClient
    {
        decimal GetQuote(string ticker);
        void Buy(string ticker, int quantity);
        void Sell(string ticker, int quantity);
    }
}
