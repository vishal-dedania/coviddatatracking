using AppLogs;
using Microsoft.Extensions.DependencyInjection;
using Parsers;
using Persistence;
using Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CovidDataDownload
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddSingleton<IParseCovid19CsvFile, ParseCovid19CsvFile>()
                .AddSingleton<IFileDownloadService, FileDownloadService>()
                .AddSingleton<ICovidDataService, CovidDataService>()
                .AddSingleton<ISqlRepository, SqlRepository>()
                .BuildServiceProvider();

            var fileLocation = Directory.GetCurrentDirectory();
            var fileDownloadService = services.GetRequiredService<IFileDownloadService>();
            var fileUrl = AppConstants.Constants.CovidDataSourceUrl;
            var targetFileName = $"{fileLocation}\\{Guid.NewGuid()}.csv";
            if (fileDownloadService.DownloadFile(fileUrl, targetFileName))
            {
                var parser = services.GetRequiredService<IParseCovid19CsvFile>();
                var covidData = parser.Parse(targetFileName);

                var covidDataService = services.GetRequiredService<ICovidDataService>();
                await covidDataService.InsertOrUpdateCovidDataAsync(covidData.Item1, covidData.Item2);
            }
            else
            {
                Log.Error(new Exception($"Unable to download file: {fileUrl}"));
            }
        }
    }
}