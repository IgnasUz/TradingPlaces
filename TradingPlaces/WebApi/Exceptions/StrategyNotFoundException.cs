using System;

namespace TradingPlaces.WebApi.Exceptions
{
    public class StrategyNotFoundException: Exception
    {
        public StrategyNotFoundException(string message)
            : base(message)
        {
        }
    }
}
