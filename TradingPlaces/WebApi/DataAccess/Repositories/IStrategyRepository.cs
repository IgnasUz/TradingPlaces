using System.Collections.Generic;
using TradingPlaces.WebApi.Models;

namespace TradingPlaces.WebApi.DataAccess.Repositories
{
    public interface IStrategyRepository
    {
        bool Add(Strategy strategy);
        bool Remove(string id);
        IEnumerable<Strategy> GetAll();
    }
}
