using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EfCore.LongRunningQueryLogger
{

    public class LongRunningQueryInterceptor : IDbCommandInterceptor
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly ILogger<LongRunningQueryInterceptor> _logger;
        private readonly LongRunningQueryInterceptorOptions _options;

        public LongRunningQueryInterceptor(ILogger<LongRunningQueryInterceptor> logger, LongRunningQueryInterceptorOptions options)
        {
            _logger = logger;
            _options = options;
        }

        public InterceptionResult<DbCommand> CommandCreating(CommandCorrelatedEventData eventData, InterceptionResult<DbCommand> result) => result;

        public DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            return result;
        }

        public InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            CommandExecuting(command);
            return result;
        }

        public InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            CommandExecuting(command);
            return result;
        }

        public InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            CommandExecuting(command);
            return result;
        }

        public Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CommandExecuting(command);
            return Task.FromResult(result);
        }

        public Task<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CommandExecuting(command);
            return Task.FromResult(result);
        }

        public Task<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CommandExecuting(command);
            return Task.FromResult(result);
        }

        public DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            CommandExecuted(command, eventData);
            return result;
        }

        public object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            CommandExecuted(command, eventData);
            return result;
        }

        public int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            CommandExecuted(command, eventData);
            return result;
        }

        public Task<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CommandExecuted(command, eventData);
            return Task.FromResult(result);
        }

        public Task<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CommandExecuted(command, eventData);
            return Task.FromResult(result);
        }

        public Task<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CommandExecuted(command, eventData);
            return Task.FromResult(result);
        }

        public void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            LogIfError(command, eventData);
        }

        public Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            LogIfError(command, eventData);
            return Task.CompletedTask;
        }

        public InterceptionResult DataReaderDisposing(DbCommand command, DataReaderDisposingEventData eventData, InterceptionResult result)
        {
            return result;
        }

        private void CommandExecuting(DbCommand command)
        {
            _stopwatch.Restart();
            if (_options._logLevelCommandStartedExecuting != null)
            {
                _logger.Log(_options._logLevelCommandStartedExecuting.Value, "[LongRunnnigQuery] Started executing {0}", command.CommandText);
            }
        }

        private void CommandExecuted(DbCommand command, CommandExecutedEventData eventData)
        {
            _stopwatch.Stop();
            if (_options._logLevelCommandFinishedExecuting != null)
            {
                _logger.Log(_options._logLevelCommandFinishedExecuting.Value,
                    $"[LongRunnnigQuery] Finished executing, took {_stopwatch.ElapsedMilliseconds:N0}ms {command.CommandText}");
            }

            LogIfTooSlow(command, _stopwatch.Elapsed);
        }

        private void LogIfError(DbCommand command, CommandErrorEventData eventData)
        {
            if (eventData.Exception != null)
            {
                _logger.LogError(eventData.Exception, "Command  failed with exception, query: {0}, error:{1}", command.CommandText,
                    eventData.Exception);
            }
        }

        private void LogIfTooSlow(DbCommand command, TimeSpan completionTime)
        {
            if (completionTime.TotalMilliseconds > _options._threshold.Milliseconds)
            {
                _logger.Log(_options._logLevelIfThresholdExceeded, "Query time ({0}ms) exceeded the threshold of {1}ms. Command: {2}",
                    completionTime.TotalMilliseconds.ToString("N0"),
                    _options._threshold.TotalMilliseconds.ToString("N0"), command.CommandText);
            }
        }
    }
}