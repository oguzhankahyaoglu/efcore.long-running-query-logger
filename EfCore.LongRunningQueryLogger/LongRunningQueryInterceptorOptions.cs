using System;
using Microsoft.Extensions.Logging;

namespace EfCore.LongRunningQueryLogger
{
    public class LongRunningQueryInterceptorOptions
    {
        internal TimeSpan _threshold;
        internal readonly LogLevel _logLevelIfThresholdExceeded;
        internal readonly LogLevel? _logLevelCommandStartedExecuting;
        internal readonly LogLevel? _logLevelCommandFinishedExecuting;

        /// <summary>
        /// Define options for the interceptor
        /// </summary>
        /// <param name="logLevelCommandStartedExecuting">If specified, it will log on any db command started</param>
        /// <param name="logLevelCommandFinishedExecuting">If specified, it will log after any db command finished</param>
        /// <param name="threshold">Threshold for db command duration for logging</param>
        /// <param name="logLevelIfThresholdExceeded">The log level for thresold exceed</param>
        public LongRunningQueryInterceptorOptions(TimeSpan threshold,
            LogLevel logLevelIfThresholdExceeded,
            LogLevel? logLevelCommandStartedExecuting = null,
            LogLevel? logLevelCommandFinishedExecuting = null)
        {
            _threshold = threshold;
            _logLevelIfThresholdExceeded = logLevelIfThresholdExceeded;
            _logLevelCommandStartedExecuting = logLevelCommandStartedExecuting;
            _logLevelCommandFinishedExecuting = logLevelCommandFinishedExecuting;
        }
    }
}