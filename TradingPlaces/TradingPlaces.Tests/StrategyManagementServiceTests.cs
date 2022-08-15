using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using TradingPlaces.Resources;
using TradingPlaces.WebApi.DataAccess.ApiClients;
using TradingPlaces.WebApi.DataAccess.Repositories;
using TradingPlaces.WebApi.Dtos;
using TradingPlaces.WebApi.Models;
using TradingPlaces.WebApi.Services;
using Xunit;

namespace TradingPlaces.Tests
{
    public class StrategyManagementServiceTests
    {        
        private readonly Mock<IStrategyRepository> _strategyRepositoryMock;
        private readonly Mock<ITradeClient> _tradeClientMock;
        
        private readonly StrategyManagementService _strategyManagementService;

        public StrategyManagementServiceTests()
        {
            _strategyRepositoryMock = new Mock<IStrategyRepository>();
            _tradeClientMock = new Mock<ITradeClient>();

            _strategyManagementService = new StrategyManagementService(
                _strategyRepositoryMock.Object,
                _tradeClientMock.Object,
                new Mock<ILogger<StrategyManagementService>>().Object);
        }

        [Fact]
        public void Add_RegistersStrategy_WhenNoExceptionsAreThrown()
        {
            // Arrange
            var strategyDetails = new StrategyDetailsDto
            {
                Ticker = "TEST",
                PriceMovement = 0.01m,
                Quantity = 1,
                Instruction = BuySell.Sell,
            };

            _tradeClientMock.Setup(x => x.GetQuote(It.IsAny<string>())).Returns(100);
            _strategyRepositoryMock.Setup(x => x.Add(It.IsAny<Strategy>())).Returns(true);

            // Act
            var exception = Record.Exception(() => _strategyManagementService.Add(strategyDetails));

            // Arrange
            Assert.Null(exception);

            _strategyRepositoryMock.Verify(x => x.Add(It.IsAny<Strategy>()), Times.Once);
        }

        [Fact]
        public void Remove_UnRegistersStrategy_WhenNoExceptionsAreThrown()
        {
            // Arrange
            const string STRATEGY_ID = "TestID";

            _strategyRepositoryMock.Setup(x => x.Remove(It.IsAny<string>())).Returns(true);

            // Act
            var exception = Record.Exception(() => _strategyManagementService.Remove(STRATEGY_ID));

            // Arrange
            Assert.Null(exception);

            _strategyRepositoryMock.Verify(x => x.Remove(STRATEGY_ID), Times.Once);
        }


        [Fact]
        public void ExecuteTradeDecision_CallsBuyOnce_WhenCurrentPriceMatchesPricePoint()
        {
            // Arrange
            const decimal PRICE_TO_BUY = 100;

            var strategy = new Strategy
            {
                Id = "TestID",
                Ticker = "TEST",
                Instruction = BuySell.Buy,
                PricePoint = PRICE_TO_BUY
            };

            _strategyRepositoryMock.Setup(x => x.Remove(strategy.Id)).Returns(true);

            // Act
            _strategyManagementService.ExcecuteTradeDecision(PRICE_TO_BUY, new List<Strategy>() { strategy });

            // Assert
            _tradeClientMock.Verify(x => x.Buy(strategy.Ticker, strategy.Quantity),
                Times.Once);

            _strategyRepositoryMock.Verify(x => x.Remove(strategy.Id), Times.Once);
        }

        [Fact]
        public void ExecuteTradeDecision_CallsSellOnce_WhenCurrentPriceMatchesPricePoint()
        {
            // Arrange
            const decimal PRICE_TO_SELL = 100;

            var strategy = new Strategy
            {
                Id = "TestID",
                Ticker = "TEST",
                Instruction = BuySell.Sell,
                PricePoint = PRICE_TO_SELL
            };

            _strategyRepositoryMock.Setup(x => x.Remove(strategy.Id)).Returns(true);

            // Act
            _strategyManagementService.ExcecuteTradeDecision(PRICE_TO_SELL, new List<Strategy>() { strategy });

            // Assert
            _tradeClientMock.Verify(x => x.Sell(strategy.Ticker, strategy.Quantity),
                Times.Once);

            _strategyRepositoryMock.Verify(x => x.Remove(strategy.Id), Times.Once);
        }
    }
}