using Structures.Models;
using Structures.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface ICovidDataService
    {
        Task InsertOrUpdateCovidDataAsync(List<LocationData> locationData, List<TimeSeriesData> timeSeriesData);

        Task<CovidDataResponse> SearchAsync(CovidDataRequest request);
    }
}