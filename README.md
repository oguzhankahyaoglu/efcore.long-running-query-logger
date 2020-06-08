# EFCore.long-running-query-logger

An useful EntityFrameworkCore interceptor to find long running queries against the database. Apart from using any profiler, it will log an error (or anything depending on the configuration) if the specified threshold exceeded.

# Installation

Install nuget package: EFCore.LongRunningQueriesLogger

# Usage

Add interceptor to db context on either OnConfigure of DbContext, or at startup.cs

````
var logger = IocManager.Instance.Resolve<ILogger<LongRunningQueryInterceptor>>();
options.DbContextOptions.AddInterceptors(new LongRunningQueryInterceptor(logger, new LongRunningQueryInterceptorOptions(
    TimeSpan.FromMilliseconds(10),
    LogLevel.Error,
    LogLevel.Debug,
    LogLevel.Debug
)));
````