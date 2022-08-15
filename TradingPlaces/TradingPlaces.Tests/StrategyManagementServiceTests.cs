using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using TradingPlaces.Resources;
using TradingPlaces.WebApi.DataAccess.ApiClients;
using TradingPlaces.WebApi.DataAccess.Repositories;
using TradingPlaces.WebApi.Dtos;
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
        public void ExecuteTradeDecision_ExecutesBuyOnce_WhenPriceMatchesStrategy()
        {
            // Arrange
            const decimal ExpectedPriceMovement = 0.02m;
            const decimal TickerPriceWhenRegistered = 100;
            var priceToBuy = TickerPriceWhenRegistered - (TickerPriceWhenRegistered * ExpectedPriceMovement);

            var strategy = new StrategyDto
            {
                Id = "SomeString",
                StrategyDetails = new StrategyDetailsDto
                {
                    Ticker = "TEST",
                    PriceMovement = ExpectedPriceMovement,
                    Instruction = BuySell.Buy
                },
                TickerPriceWhenRegistered = TickerPriceWhenRegistered
            };

            _strategyRepositoryMock.Setup(x => x.Remove(strategy.Id)).Returns(true);

            // Act
            _strategyManagementService.ExcecuteTradeDecision(priceToBuy, new List<StrategyDto>() { strategy });

            // Assert
            _tradeClientMock.Verify(x => x.Buy(strategy.StrategyDetails.Ticker, strategy.StrategyDetails.Quantity),
                Times.Once);

            _strategyRepositoryMock.Verify(x => x.Remove(strategy.Id), Times.Once);
        }

        [Fact]
        public void ExecuteTradeDecision_ExecutesSellOnce_WhenPriceMatchesStrategy()
        {
            // Arrange
            const decimal ExpectedPriceMovement = 0.02m;
            const decimal TickerPriceWhenRegistered = 100;
            var priceToSell = TickerPriceWhenRegistered + (TickerPriceWhenRegistered * ExpectedPriceMovement);

            var strategy = new StrategyDto
            {
                Id = "SomeString",
                StrategyDetails = new StrategyDetailsDto
                {
                    Ticker = "TEST",
                    PriceMovement = ExpectedPriceMovement,
                    Instruction = BuySell.Sell
                },
                TickerPriceWhenRegistered = TickerPriceWhenRegistered
            };

            _strategyRepositoryMock.Setup(x => x.Remove(strategy.Id)).Returns(true);

            // Act
            _strategyManagementService.ExcecuteTradeDecision(priceToSell, new List<StrategyDto>() { strategy });

            // Assert
            _tradeClientMock.Verify(x => x.Sell(strategy.StrategyDetails.Ticker, strategy.StrategyDetails.Quantity),
                Times.Once);

            _strategyRepositoryMock.Verify(x => x.Remove(strategy.Id), Times.Once);
        }

        [Fact]
        public void ShouldBuy_ReturnsTrue_WhenPriceMatchesStrategy()
        {
            // Arrange
            const decimal ExpectedPriceMovement = 0.02m;
            const decimal TickerPriceWhenRegistered = 100;

            var priceToBuy = TickerPriceWhenRegistered - (TickerPriceWhenRegistered * ExpectedPriceMovement);
            var strategyDetails = new StrategyDetailsDto
            {
                PriceMovement = ExpectedPriceMovement,
                Instruction = BuySell.Buy
            };

            _tradeClientMock.Setup(x => x.GetQuote(It.IsAny<string>())).Returns(priceToBuy);

            // Act
            var result = _strategyManagementService.ShouldBuy(strategyDetails,
                TickerPriceWhenRegistered,
                _tradeClientMock.Object.GetQuote(It.IsAny<string>()));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShouldSell_ReturnsTrue_WhenPriceMatchesStrategy()
        {
            // Arrange
            const decimal ExpectedPriceMovement = 0.02m;
            const decimal TickerPriceWhenRegistered = 100;

            var priceToSell = TickerPriceWhenRegistered + (TickerPriceWhenRegistered * ExpectedPriceMovement);
            var strategyDetails = new StrategyDetailsDto
            {
                PriceMovement = ExpectedPriceMovement,
                Instruction = BuySell.Sell
            };

            _tradeClientMock.Setup(x => x.GetQuote(It.IsAny<string>())).Returns(priceToSell);

            // Act
            var result = _strategyManagementService.ShouldSell(strategyDetails,
                TickerPriceWhenRegistered,
                _tradeClientMock.Object.GetQuote(It.IsAny<string>()));

            // Assert
            Assert.True(result);
        }
    }
}