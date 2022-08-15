using System.Collections.Generic;
using TradingPlaces.WebApi.Models;

namespace TradingPlaces.WebApi.DataAccess.Repositories
{
    public class StrategyRepository : IStrategyRepository
    {
        private readonly Dictionary<string, Strategy> _strategies = new Dictionary<string, Strategy>();

        public bool Add(Strategy strategy)
        {
            return _strategies.TryAdd(strategy.Id, strategy);
        }

        public bool Remove(string id)
        {
            return _strategies.Remove(id);
        }

        public IEnumerable<Strategy> GetAll()
        {
            return _strategies.Values;
        }
    }
}
