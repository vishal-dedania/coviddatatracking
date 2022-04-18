using AppLogs;
using RestSharp;
using System;
using System.IO;

namespace Services
{
    public class FileDownloadService : IFileDownloadService
    {
        public bool DownloadFile(string url, string storageLocation)
        {
            var writer = File.OpenWrite(storageLocation);
            try
            {
                var uri = new Uri(url);
                var client = new RestClient($"{uri.Scheme}://{uri.Host}");
                var request = new RestRequest(uri.PathAndQuery, Method.GET)
                {
                    ResponseWriter = responseStream =>
                    {
                        using (responseStream)
                        {
                            responseStream.CopyTo(writer);
                        }
                    }
                };

                client.DownloadData(request);

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                writer.Close();
            }

            return false;
        }
    }
}