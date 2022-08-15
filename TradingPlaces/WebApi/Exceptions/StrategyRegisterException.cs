using System;

namespace TradingPlaces.WebApi.Exceptions
{
    public class StrategyRegisterException: Exception
    {
        public StrategyRegisterException(string message)
            : base(message)
        {
        }
    }
}
