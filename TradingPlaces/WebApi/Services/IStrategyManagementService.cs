using Microsoft.Extensions.Hosting;
using TradingPlaces.WebApi.Dtos;

namespace TradingPlaces.WebApi.Services
{
    public interface IStrategyManagementService : IHostedService
    {
        string Add(StrategyDetailsDto strategyDetails);
        void Remove(string id);
    }
}