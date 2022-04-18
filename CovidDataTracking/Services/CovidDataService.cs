using Extensions;
using Persistence;
using Structures.Models;
using Structures.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<CovidDataResponse> SearchAsync(CovidDataRequest request)
        {
            var records = await _sqlRepository.QueryAsync<CovidRecords>(QueryBuilder(request));

            var response = new CovidDataResponse
            {
                Records = records,
                Summary = CalculateSummary(records)
            };

            return response;
        }

        private Summary CalculateSummary(IList<CovidRecords> records)
        {
            var maxCases = records.OrderByDescending(w => w.TotalCases).FirstOrDefault();
            var minCases = records.Where(w => w.TotalCases > 0).OrderBy(w => w.TotalCases).FirstOrDefault();

            var summary = new Summary
            {
                AverageDailyCases = (records.Sum(w => w.TotalCases) / records.Count).RoundOffNearestTen(),
                NumberOfCases = new NumberOfCases
                {
                    Maximum = new CasesDetail
                    {
                        Date = maxCases.Date,
                        Total = maxCases.TotalCases
                    },
                    Minimum = new CasesDetail
                    {
                        Date = minCases.Date,
                        Total = minCases.TotalCases
                    }
                }
            };

            return summary;
        }

        private string QueryBuilder(CovidDataRequest request)
        {
            StringBuilder sb = new StringBuilder();
            var sqlWhere = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(request.County))
            {
                sqlWhere.Append($" and Admin2 = '{request.County.IncludeSingleQuote()}'");
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                sqlWhere.Append($" and Province_State = '{request.State.IncludeSingleQuote()}'");
            }

            if (request.From != DateTime.MinValue)
            {
                sqlWhere.Append($" and td.Date >= '{request.From:d}'");
            }

            if (request.To != DateTime.MinValue)
            {
                sqlWhere.Append($" and td.Date <= '{request.To:d}'");
            }

            //TODO: Improvement - implement db side pagination
            sb.Append(@$"select ld.Admin2 as County, ld.Province_State as State, 
                        ld.Lat as Latitude, ld.Long as Longitude, td.Date, td.Value as TotalCases
                        from LocationData ld with (nolock)
                        inner join TimeSeriesData td with (nolock)
                        on ld.UID = td.LocationUId where 1 = 1
                            {sqlWhere} order by td.Date desc");

            return sb.ToString();
        }
    }
}
