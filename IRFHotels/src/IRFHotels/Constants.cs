//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="BME MIT" />
//-----------------------------------------------------------------------
/*
 * Name:    LogCategories, LogEventIds
 * Author:  Szabó Patrik
 * Date:    2015.04.25.
 * Desc     Structs and enums containing constants for logging
 */

namespace IRFHotels
{
    internal struct LogCategories
    {
        internal const string ProgramFail = "IRFHotelsFail";
        internal const string ProgramInfo = "IRFHotelInfo"; 
    }

    internal struct LogEventIds
    {
        #region Succes events
        internal const int HostOpenSuccesful = 102;
        internal const int HostOpenSuccesfulPrio = 2;
        internal const int HotelManagement = 103;
        internal const int HotelManagementPrio = 3;
        #endregion
        #region Failure events
        internal const int HostOpenFailed = 101;
        internal const int HostOpenFailedPrio = 1;
        #endregion
    }
}
