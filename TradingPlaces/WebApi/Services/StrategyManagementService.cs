using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TradingPlaces.Resources;
using TradingPlaces.WebApi.DataAccess.Repositories;
using System.Linq;
using System.Collections.Generic;
using TradingPlaces.WebApi.Dtos;
using TradingPlaces.WebApi.Exceptions;
using TradingPlaces.WebApi.Constants;
using TradingPlaces.WebApi.DataAccess.ApiClients;

namespace TradingPlaces.WebApi.Services
{
    public class StrategyManagementService : TradingPlacesBackgroundServiceBase, IStrategyManagementService
    {
        private const int TickFrequencyMilliseconds = 1_000;

        private readonly IStrategyRepository _strategyRepository;
        private readonly ITradeClient _tradeClient;
        private readonly ILogger<StrategyManagementService> _logger;

        public StrategyManagementService(            
            IStrategyRepository strategyRepository,
            ITradeClient tradeClient,
            ILogger<StrategyManagementService> logger)
            : base(TimeSpan.FromMilliseconds(TickFrequencyMilliseconds), logger)
        {
            _strategyRepository = strategyRepository;
            _tradeClient = tradeClient;
            _logger = logger;
        }

        public string Add(StrategyDetailsDto strategyDetails)
        {
            var errorMessage = Validate(strategyDetails.Ticker);
            
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _logger.LogError($"Validation failed with error message '{errorMessage}'");
                throw new InvalidStrategyException(errorMessage);
            }

            var strategy = new StrategyDto
            {
                Id = Guid.NewGuid().ToString(),
                StrategyDetails = strategyDetails,
                TickerPriceWhenRegistered = _tradeClient.GetQuote(strategyDetails.Ticker)
            };

            if (!_strategyRepository.Add(strategy))
            {
                _logger.LogError($"Failed to add strategy ID: {strategy.Id}");
                throw new StrategyRegisterException(ExceptionMessages.StrategyRegistrationFailed);
            }

            return strategy.Id;
        }

        public void Remove(string id)
        {
            if(!_strategyRepository.Remove(id))
            {
                _logger.LogError($"Failed to remove strategy ID: {id}");
                throw new StrategyNotFoundException(ExceptionMessages.StrategyNotFound);
            }
        }

        protected override Task CheckStrategies()
        {
            var allStrategies = _strategyRepository.GetAll();

            var uniqueTickers = allStrategies.Select(x => x.StrategyDetails.Ticker).Distinct().ToList();

            foreach (var ticker in uniqueTickers)
            {
                try
                {
                    var currentPrice = _tradeClient.GetQuote(ticker);

                    var tickerStrategies = allStrategies.Where(x => x.StrategyDetails.Ticker == ticker).ToList();

                    ExcecuteTradeDecision(currentPrice, tickerStrategies);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Handling of {ticker} ticker skipped due to the following error: '{ex}'");
                }
            }

            return Task.CompletedTask;
        }

        public void ExcecuteTradeDecision(decimal currentPrice, List<StrategyDto> strategies)
        {
            foreach (var strategy in strategies)
            {
                var strategyDetails = strategy.StrategyDetails;
                var originalPrice = strategy.TickerPriceWhenRegistered;

                if (ShouldBuy(strategyDetails, originalPrice, currentPrice))
                {
                    _tradeClient.Buy(strategyDetails.Ticker, strategyDetails.Quantity);
                    Remove(strategy.Id);
                }

                if (ShouldSell(strategyDetails, originalPrice, currentPrice))
                {
                    _tradeClient.Sell(strategyDetails.Ticker, strategyDetails.Quantity);
                    Remove(strategy.Id);
                }
            }
        }

        public bool ShouldBuy(StrategyDetailsDto strategydetails, decimal originalPrice, decimal currentPrice)
        {
            if (strategydetails.Instruction != BuySell.Buy)
            {
                return false;
            }

            var priceToBuy = originalPrice - (originalPrice * strategydetails.PriceMovement);

            return currentPrice <= priceToBuy;
        }

        public bool ShouldSell(StrategyDetailsDto strategydetails, decimal originalPrice, decimal currentPrice)
        {
            if (strategydetails.Instruction != BuySell.Sell)
            {
                return false;
            }

            var priceToSell = originalPrice + (originalPrice * strategydetails.PriceMovement);

            return currentPrice >= priceToSell;
        }

        private string Validate(string ticker)
        {
            if (!ticker.All(char.IsLetterOrDigit) || ticker.Any(char.IsLower))
            {
                return ExceptionMessages.TickerInvalidCharacters;
            }

            if (ticker.Length < 3 || ticker.Length > 5)
            {
                return ExceptionMessages.TickerInvalidLength;
            }

            return null;
        }        
    }
}
