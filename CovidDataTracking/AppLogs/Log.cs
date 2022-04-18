using System;

namespace AppLogs
{
    //TODO: We can use this class to use either Azure-AppInsight / AWS-CloudWatch logging or any custom logger here
    public static class Log
    {
        public static void Error(Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
