using TradingPlaces.Resources;

namespace TradingPlaces.WebApi.Models
{
    public class Strategy
    {
        public string Id { get; set; }
        public string Ticker { get; set; }
        public BuySell Instruction { get; set; }
        public decimal PricePoint { get; set; }
        public int Quantity { get; set; }

        public bool DecideToTrade(decimal currentPrice)
        {
            if (Instruction == BuySell.Buy)
            {
                return currentPrice <= PricePoint;
            }

            if (Instruction == BuySell.Sell)
            {
                return currentPrice >= PricePoint;
            }

            return false;
        }
    }
}
