using System;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace ChilliSource.Mobile.Logging
{
    /// <summary>
    /// Serilog Sink to output log data to the console via System.Diagnostics
    /// </summary>
    public class DiagnosticsSink : ILogEventSink
    {
        readonly ITextFormatter _textFormatter;

        public DiagnosticsSink(ITextFormatter textFormatter)
        {
            _textFormatter = textFormatter ?? throw new ArgumentNullException(nameof(textFormatter));
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException();
            }

            var renderSpace = new StringWriter();
            _textFormatter.Format(logEvent, renderSpace);

            System.Diagnostics.Debug.WriteLine(renderSpace.ToString());
        }
    }
}
