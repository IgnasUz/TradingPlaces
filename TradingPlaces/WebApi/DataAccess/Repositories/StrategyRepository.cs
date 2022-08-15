using System.Collections.Generic;
using TradingPlaces.WebApi.Dtos;

namespace TradingPlaces.WebApi.DataAccess.Repositories
{
    public class StrategyRepository : IStrategyRepository
    {
        private readonly Dictionary<string, StrategyDto> _strategies = new Dictionary<string, StrategyDto>();

        public bool Add(StrategyDto strategy)
        {
            return _strategies.TryAdd(strategy.Id, strategy);
        }

        public bool Remove(string id)
        {
            return _strategies.Remove(id);
        }

        public IEnumerable<StrategyDto> GetAll()
        {
            return _strategies.Values;
        }
    }
}
