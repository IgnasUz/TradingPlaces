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
using TradingPlaces.WebApi.Validators;
using TradingPlaces.WebApi.Models;

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
            var errorMessage = strategyDetails.Validate();
            
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _logger.LogError($"Validation failed with error message '{errorMessage}'");
                throw new InvalidStrategyException(errorMessage);
            }

            var strategy = new Strategy
            {
                Id = Guid.NewGuid().ToString(),
                Ticker = strategyDetails.Ticker,
                Instruction = strategyDetails.Instruction,
                PricePoint = Calculate(strategyDetails.Instruction,
                                _tradeClient.GetQuote(strategyDetails.Ticker),
                                strategyDetails.PriceMovement),
                Quantity = strategyDetails.Quantity
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

            var uniqueTickers = allStrategies.Select(x => x.Ticker).Distinct().ToList();

            foreach (var ticker in uniqueTickers)
            {
                try
                {
                    var currentPrice = _tradeClient.GetQuote(ticker);

                    var tickerStrategies = allStrategies.Where(x => x.Ticker == ticker).ToList();

                    ExcecuteTradeDecision(currentPrice, tickerStrategies);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Handling of {ticker} ticker skipped due to the following error: '{ex}'");
                }
            }

            return Task.CompletedTask;
        }
        
        public void ExcecuteTradeDecision(decimal currentPrice, List<Strategy> strategies)
        {
            foreach (var strategy in strategies)
            {

                if (strategy.DecideToTrade(currentPrice) && strategy.Instruction == BuySell.Buy)
                {
                    _tradeClient.Buy(strategy.Ticker, strategy.Quantity);
                    Remove(strategy.Id);
                }

                if (strategy.DecideToTrade(currentPrice) && strategy.Instruction == BuySell.Sell)
                {
                    _tradeClient.Sell(strategy.Ticker, strategy.Quantity);
                    Remove(strategy.Id);
                }
            }
        }

        private decimal Calculate(BuySell instruction, decimal currentPrice, decimal priceMovement)
        {
            if (instruction == BuySell.Buy)
            {
                return currentPrice - (currentPrice * priceMovement);
            }

            if (instruction == BuySell.Sell)
            {
                return currentPrice + (currentPrice * priceMovement);
            }

            throw new InvalidStrategyException(ExceptionMessages.InvalidInstructions);
        }
    }
}
