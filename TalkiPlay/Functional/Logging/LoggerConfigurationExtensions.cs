#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ChilliSource.Mobile.Core;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;


namespace ChilliSource.Mobile.Logging
{
    /// <summary>
    /// Logger configuration extensions.
    /// </summary>
    public static class LoggerConfigurationExtensions
    {
        const string DefaultLogOutputTemplate = "[{Level}] {Message:l}{NewLine:l}{Exception:l}";


		/// <summary>
		/// Returns the specified <paramref name="configuration"/> enriched with app information from <paramref name="information"/>
		/// </summary>
		/// <returns>the updated configuration</returns>
		/// <param name="configuration">The original configuration</param>
		/// <param name="information"><see cref="IEnvironmentInformation"/> instance with app data</param>
		/// <param name="userKeyRetriever">Function to retrieve the user key for API authentication.</param>
		public static LoggerConfiguration WithApplicationInformation(this LoggerConfiguration configuration, IEnvironmentInformation information, Func<string> userKeyRetriever = null)
        {
            return configuration.Enrich.With(new ApplicationInformationEnricher(information, userKeyRetriever));
        }

        /// <summary>
        /// Creates a new <see cref="Core.ILogger"/> instance from the specified <paramref name="configuration"/>
        /// </summary>
        /// <returns>The logger.</returns>
        /// <param name="configuration">Config.</param>
        public static Core.ILogger BuildLogger(this LoggerConfiguration configuration)
        {
            configuration.CreateLogger().Register();
            return new LoggerProxy();
        }

        public static LoggerConfiguration Diagnostics(this LoggerSinkConfiguration sinkConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultLogOutputTemplate,
            IFormatProvider formatProvider = null)
        {

            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            if (outputTemplate == null)
            {
                throw new ArgumentNullException(nameof(outputTemplate));
            }

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.Sink(new DiagnosticsSink(formatter), restrictedToMinimumLevel);
        }
    }
}
