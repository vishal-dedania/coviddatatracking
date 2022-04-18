using Structures.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Structures.ViewModels;

namespace Services
{
    public interface ICovidDataService
    {
        Task InsertOrUpdateCovidDataAsync(List<LocationData> locationData, List<TimeSeriesData> timeSeriesData);

        Task<IList<CovidDataResponse>> SearchAsync(CovidDataRequest request);
    }
}