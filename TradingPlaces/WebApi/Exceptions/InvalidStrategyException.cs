using System;

namespace TradingPlaces.WebApi.Exceptions
{
    public class InvalidStrategyException: Exception
    {
        public InvalidStrategyException(string message)
            : base(message)
        {
        }
    }
}
