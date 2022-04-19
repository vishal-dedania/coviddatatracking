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

        public async Task InsertOrUpdateCovidDataAsync(List<LocationData> locationData,
            List<TimeSeriesData> timeSeriesData)
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

        public async Task<CovidDailyBreakDownDataResponse> GetDailyBreakDownDataAsync(CovidDataRequest request)
        {
            var records = await _sqlRepository.QueryAsync<CovidRecordsDaily>(QueryBuilderDailyBreakDown(request));

            var response = new CovidDailyBreakDownDataResponse
            {
                Records = records,
                Summary = CalculateDailySummary(request, records.FirstOrDefault())
            };

            return response;
        }

        public async Task<CovidDataResponse> SearchAsync(CovidDataRequest request)
        {
            var records = await _sqlRepository.QueryAsync<CovidRecords>(QueryBuilder(request));

            var response = new CovidDataResponse
            {
                Records = records,
                Summary = CalculateSummary(request, records.FirstOrDefault())
            };

            return response;
        }   

        private Summary CalculateSummary(CovidDataRequest request, CovidRecords record)
        {
            if (record == null)
            {
                return new Summary();
            }

            var summary = new Summary
            {
                AverageDailyCases = (record.TotalCases / record.TotalCount).RoundOffNearestTen(),
                NumberOfCases = new NumberOfCases
                {
                    Maximum = new CasesDetail
                    {
                        Date = record.MaxCasesOn,
                        Total = record.MaxCases
                    },
                    Minimum = new CasesDetail
                    {
                        Date = record.MinCasesOn,
                        Total = record.MinCases
                    }
                },
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = record.TotalCount,
                TotalPages = record.TotalCount > 0
                    ? (long)Math.Ceiling(record.TotalCount / (double)request.PageSize)
                    : record.TotalCount
            };

            summary.HasNext = request.PageNumber < summary.TotalPages;
            summary.HasPrevious = summary.CurrentPage > 1;

            return summary;
        }

        private string QueryBuilder(CovidDataRequest request)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@$";WITH Main_CTE AS(
                        select ld.Admin2 as County, ld.Province_State as State, 
                        ld.Lat as Latitude, ld.Long as Longitude, td.Date, td.Value as TodayCases
                        from LocationData ld with (nolock)
                        inner join TimeSeriesData td with (nolock)
                        on ld.UID = td.LocationUId
						 {BuildWhereClause(request)}
						)
                            ,Count_CTE AS (
                                SELECT COUNT(*) AS [TotalCount]
                                FROM Main_CTE
                            )
							,Max_Cases as (
                                SELECT top 1 TodayCases as MaxCases, Date AS [MaxCasesOn]
                                FROM Main_CTE order by TodayCases desc
                            )
							,Min_Cases as (
                                SELECT top 1 TodayCases as MinCases, Date AS [MinCasesOn]
                                FROM Main_CTE 
								where TodayCases > 0
								order by TodayCases asc
                            )
							,Total_Cases as (
                                SELECT sum(TodayCases) as TotalCases
                                FROM Main_CTE 
                            )
                            SELECT *
                            FROM Main_CTE, Count_CTE, Max_Cases, Min_Cases,Total_Cases
                            ORDER BY Main_CTE.TodayCases desc
                            offset {request.PageSize * (request.PageNumber - 1)} rows fetch next {request.PageSize} rows only");

            return sb.ToString();
        }

        private string QueryBuilderDailyBreakDown(CovidDataRequest request)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@$"WITH PreviousDayCases AS (
                              SELECT 
                                ld.Province_State as State, 
                                ld.Admin2 as County, 
                                td.Value as TodayCases, 
                                td.Date, 
                                ld.Lat as Latitude, 
                                ld.Long as Longitude, 
                                LAG(td.Value) OVER(
                                  PARTITION BY ld.Admin2, 
                                  ld.Province_State 
                                  ORDER BY 
                                    td.date
                                ) AS PreviousDayCases 
                              FROM 
                                LocationData ld with (nolock) 
                                inner join TimeSeriesData td with (nolock) on ld.UID = td.LocationUId 
                              {BuildWhereClause(request)}
                            ), 
                            PercentChanges AS (
                              SELECT 
                                *, 
                                COALESCE(
                                  (
                                    (
                                      PreviousDayCases.TodayCases - PreviousDayCases
                                    ) * 100
                                  ) / (PreviousDayCases.TodayCases), 
                                  0
                                ) AS PercentChange 
                              FROM 
                                PreviousDayCases
                            ),
Count_CTE AS (
                                SELECT COUNT(*) AS [TotalCount]
                                FROM PreviousDayCases
                            ) 
                            SELECT 
                              *, 
                              CASE WHEN PercentChange > 0 THEN 'Increase' WHEN PercentChange = 0 THEN 'No Changes' ELSE 'Decrease' END AS Trend 
                            FROM 
                              PercentChanges,Count_CTE
                            order by 
	                            State,
                              County, 
                              Date asc
                                offset {request.PageSize * (request.PageNumber - 1)} rows fetch next {request.PageSize} rows only");

            return sb.ToString();
        }

        private SummaryDailyBreakDown CalculateDailySummary(CovidDataRequest request, CovidRecordsDaily record)
        {
            if (record == null)
            {
                return new SummaryDailyBreakDown();
            }

            var summary = new SummaryDailyBreakDown
            {
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = record.TotalCount,
                TotalPages = record.TotalCount > 0
                    ? (long)Math.Ceiling(record.TotalCount / (double)request.PageSize)
                    : record.TotalCount
            };

            summary.HasNext = request.PageNumber < summary.TotalPages;
            summary.HasPrevious = summary.CurrentPage > 1;

            return summary;
        }

        private string BuildWhereClause(CovidDataRequest request)
        {
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

            if (!string.IsNullOrWhiteSpace(request.State))
            {
                sqlWhere.Append($" and ld.Province_State = '{request.State}'");
            }

            sqlWhere.Append($" and td.Value > 0");

            return sqlWhere.ToString();
        }

        
    }
}