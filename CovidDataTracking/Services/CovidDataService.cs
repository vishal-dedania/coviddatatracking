using Extensions;
using Persistence;
using Structures.Models;
using Structures.ViewModels;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CovidDataService : ICovidDataService
    {
        private readonly ISqlRepository _sqlRepository;

        public CovidDataService(ISqlRepository sqlRepository)
        {
            _sqlRepository = sqlRepository;
        }

        public async Task InsertOrUpdateCovidDataAsync(List<LocationData> locationData, List<TimeSeriesData> timeSeriesData)
        {
            foreach (var location in locationData)
            {
                //TODO: Improvement : Use batch insert using table valued parameter
                var sql = @$"
                            BEGIN
                                    IF NOT EXISTS (SELECT * FROM [dbo].[LocationData] 
                                               WHERE UID = {location.Uid})
                                    BEGIN
                                    INSERT INTO [dbo].[LocationData]
                                                           ([UID]
                                                           ,[ISO2]
                                                           ,[ISO3]
                                                           ,[Code3]
                                                           ,[FIPS]
                                                           ,[Admin2]
                                                           ,[Province_State]
                                                           ,[Country_Region]
                                                           ,[Lat]
                                                           ,[Long]
                                                           ,[Combined_Key]
                                                           ,[CreatedOn])
                                                     VALUES
                                                           ({location.Uid}
                                                           ,'{location.Iso2}'
                                                           ,'{location.Iso3}'
                                                           ,'{location.Code3}'
                                                           ,'{location.FIPS}'
                                                           ,'{location.Admin2.IncludeSingleQuote()}'
                                                           ,'{location.ProvinceState}'
                                                           ,'{location.CountryRegion}'
                                                           ,'{location.Lat}'
                                                           ,'{location.Long}'
                                                           ,'{location.CombinedKey.IncludeSingleQuote()}'
                                                           ,'{location.CreatedOn}')
                                    END
                                END;";

                await _sqlRepository.InsertAsync(sql);
            }

            foreach (var timeSeries in timeSeriesData)
            {
                //TODO: Improvement: Use batch insert using table valued parameter
               var sql = $@"BEGIN
                                    IF NOT EXISTS (SELECT * FROM [dbo].[TimeSeriesData] 
                                               WHERE LocationUID = {timeSeries.LocationUid}
                                                    AND Date = '{timeSeries.Date.ToString("M/d/yy")}')
                                    BEGIN
                                    INSERT INTO [dbo].[TimeSeriesData]
                                       ([LocationUID]
                                       ,[Date]
                                       ,[Value]
                                       ,[RecordUpdatedOn])
                                     VALUES
                                           ({timeSeries.LocationUid}
                                           ,'{timeSeries.Date}'
                                           ,'{timeSeries.Value}'
                                           ,'{timeSeries.RecordUpdatedOn}')
                                    END
                                END;";

                await _sqlRepository.InsertAsync(sql);
            }
        }

        public async Task<IList<CovidDataResponse>> SearchAsync(CovidDataRequest request)
        {
            return await _sqlRepository.QueryAsync<CovidDataResponse>(QueryBuilder(request));
        }

        private string QueryBuilder(CovidDataRequest request)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from LocationData");
            return "";
        }
    }
}
