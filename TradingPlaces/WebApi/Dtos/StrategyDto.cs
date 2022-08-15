namespace TradingPlaces.WebApi.Dtos
{
    public class StrategyDto
    {
        public string Id { get; set; }        
        public StrategyDetailsDto StrategyDetails { get; set; }
        public decimal TickerPriceWhenRegistered { get; set; }
    }
}
