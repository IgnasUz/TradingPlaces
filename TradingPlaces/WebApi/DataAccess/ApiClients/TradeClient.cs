using Microsoft.Extensions.Logging;
using Reutberg;
using TradingPlaces.WebApi.Constants;

namespace TradingPlaces.WebApi.DataAccess.ApiClients
{
    public class TradeClient: ITradeClient
    {
        private readonly IReutbergService _reutbergService;
        private readonly ILogger<TradeClient> _logger;

        public TradeClient(IReutbergService reutbergService, ILogger<TradeClient> logger)
        {
            _reutbergService = reutbergService;
            _logger = logger;
        }

        public decimal GetQuote(string ticker)
        {
            try
            {
                return _reutbergService.GetQuote(ticker);
            }
            catch (QuoteException ex)
            {
                _logger.LogError(ExceptionMessages.FailedToGetTickerPrices, ex);

                throw;
            }
        }

        public void Buy(string ticker, int quantity)
        {
            try
            {
                _reutbergService.Buy(ticker, quantity);
            }
            catch (TradeException ex)
            {
                _logger.LogError(ExceptionMessages.FailedToExecuteTrade, ex);

                throw;
            }
        }        

        public void Sell(string ticker, int quantity)
        {
            try
            {
                _reutbergService.Sell(ticker, quantity);
            }
            catch (TradeException ex)
            {
                _logger.LogError(ExceptionMessages.FailedToExecuteTrade, ex);

                throw;
            }
        }
    }
}
