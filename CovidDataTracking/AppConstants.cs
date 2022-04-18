using System;

public static class AppConstants
{
    //TODO: For production, we will use environment variable or secrets manager
    public static string DefaultConnection =
        @"data source=coviddata.cjsofghrgupv.us-east-1.rds.amazonaws.com;initial catalog=coviddata;persist security info=True;user id=admin;password=nbRAkAAmcCmcPSywq61w;MultipleActiveResultSets=True;";
}
