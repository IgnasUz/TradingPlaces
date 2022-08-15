using TradingPlaces.WebApi.Dtos;
using System.Collections.Generic;

namespace TradingPlaces.WebApi.DataAccess.Repositories
{
    public interface IStrategyRepository
    {
        bool Add(StrategyDto strategy);        
        bool Remove(string id);
        IEnumerable<StrategyDto> GetAll();
    }
}
