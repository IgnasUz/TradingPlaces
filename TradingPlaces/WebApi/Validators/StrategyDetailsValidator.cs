using System.Linq;
using TradingPlaces.Resources;
using TradingPlaces.WebApi.Constants;
using TradingPlaces.WebApi.Dtos;

namespace TradingPlaces.WebApi.Validators
{
    public static class StrategyDetailsValidator
    {
        public static string Validate(this StrategyDetailsDto strategyDetails)
        {
            var ticker = strategyDetails.Ticker;

            if (!ticker.All(char.IsLetterOrDigit) || ticker.Any(char.IsLower))
            {
                return ExceptionMessages.TickerInvalidCharacters;
            }

            if (ticker.Length < 3 || ticker.Length > 5)
            {
                return ExceptionMessages.TickerInvalidLength;
            }

            if(strategyDetails.Instruction != BuySell.Buy && strategyDetails.Instruction != BuySell.Sell)
            {
                return ExceptionMessages.InvalidInstructions;
            }

            return null;
        }
    }
}
