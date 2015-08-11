using Destructurama;
using HarshPoint.Diagnostics;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Display;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit.Abstractions;

namespace HarshPoint.Tests
{
    public abstract class SeriloggedTest : IDisposable
    {
        private const String CaptureCorrelationIdKey = "SeriloggedTest_CaptureCorrelationId";

        private static readonly Subject<LogEvent> LogEventSubject = new Subject<LogEvent>();
        private static readonly MessageTemplateTextFormatter Formatter = new MessageTemplateTextFormatter(
                "{Timestamp:HH:mm:ss.fff} [{Level}] {SourceContext} {Message}{NewLine}{Exception}", null);

        static SeriloggedTest()
        {
            Log.Logger = new LoggerConfiguration()
                .Destructure.With<ExpressionScalarConversionPolicy>()
                .Destructure.UsingAttributes()
                .MinimumLevel.Verbose()
                .WriteTo.Observers(
                    observable => observable.Subscribe(logEvent => LogEventSubject.OnNext(logEvent))
                )
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        private readonly Guid _captureId = Guid.NewGuid();
        private readonly HarshDisposableBag _disposables = new HarshDisposableBag();

        public SeriloggedTest(ITestOutputHelper output)
        {
            _disposables.Add(
                LogEventSubject
                .Where(IsMyCorrelationId)
                .Subscribe(logEvent =>
                {
                    using (var writer = new StringWriter())
                    {
                        Formatter.Format(logEvent, writer);
                        output.WriteLine(writer.ToString());
                    }
                })
            );

            _disposables.Add(
                LogContext.PushProperty(CaptureCorrelationIdKey, _captureId)
            );
        }

        public virtual void Dispose()
            => _disposables.Dispose();

        private Boolean IsMyCorrelationId(LogEvent logEvent)
        {
            LogEventPropertyValue value;

            if (logEvent.Properties.TryGetValue(CaptureCorrelationIdKey, out value))
            {
                return Guid.Parse(value.ToString()).Equals(_captureId);
            }

            return false;
        }
    }
}