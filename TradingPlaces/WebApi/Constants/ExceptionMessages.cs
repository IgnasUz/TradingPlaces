namespace TradingPlaces.WebApi.Constants
{
    public static class ExceptionMessages
    {
        public const string StrategyRegistrationFailed = "Failed to register strategy, try again later.";
        public const string StrategyNotFound = "Strategy with specified ID does not exist";

        public const string TickerInvalidCharacters = "Valid ticker identifier contains only uppercase letters or numbers";
        public const string TickerInvalidLength = "Valid ticker identifier length is between 3 and 5 inclusive";

        public const string FailedToGetTickerPrices = "Failed to get ticker prices";
        public const string FailedToExecuteTrade = "Failed to execute trade";
    }
}
