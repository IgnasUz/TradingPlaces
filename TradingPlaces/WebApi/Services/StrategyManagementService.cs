using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TradingPlaces.Resources;

namespace TradingPlaces.WebApi.Services
{
    internal class StrategyManagementService : TradingPlacesBackgroundServiceBase, IStrategyManagementService
    {
        private const int TickFrequencyMilliseconds = 1000;

        public StrategyManagementService(ILogger<StrategyManagementService> logger)
            : base(TimeSpan.FromMilliseconds(TickFrequencyMilliseconds), logger)
        {
        }

        protected override Task CheckStrategies()
        {
            // TODO: Check registered strategies.

            return Task.CompletedTask;
        }
    }
}
