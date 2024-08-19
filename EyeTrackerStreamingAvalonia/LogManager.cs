// Module name: EyeTrackerStreamingAvalonia
// File name: LoggerManager.cs
// Last edit: 2024-07-24 14:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc. - All rights reserved.
// 
// All information contained herein is, and remains the property of
// Inseye Inc. The intellectual and technical concepts contained herein are
// proprietary to Inseye Inc. and may be covered by U.S. and Foreign Patents, patents
// in process, and are protected by trade secret or copyright law. Dissemination
// of this information or reproduction of this material is strictly forbidden
// unless prior written permission is obtained from Inseye Inc. Access to the source
// code contained herein is hereby forbidden to anyone except current Inseye Inc.
// employees, managers or contractors who have executed Confidentiality and
// Non-disclosure agreements explicitly covering such access.

using System;
using Microsoft.Extensions.Logging;
using Splat;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace EyeTrackerStreamingAvalonia;

public class LogManager : ILogManager
{
	private ILoggerFactory LoggerFactory { get; }

	public LogManager(ILoggerFactory loggerFactory)
	{
		LoggerFactory = loggerFactory;
	}

	public IFullLogger GetLogger(Type type)
	{
		return new WrappingFullLogger(new LogConverter(LoggerFactory.CreateLogger(type.Name)));
	}

	private class LogConverter : Splat.ILogger
	{
		private Microsoft.Extensions.Logging.ILogger MsLogger { get; }

		public LogConverter(Microsoft.Extensions.Logging.ILogger logger)
		{
			MsLogger = logger;
		}

		public void Write(string message, Splat.LogLevel logLevel)
		{
			MsLogger.Log(Convert(logLevel), "{Message}", message);
		}

		public void Write(Exception exception, string message, Splat.LogLevel logLevel)
		{
			MsLogger.Log(Convert(logLevel), exception, "{Message}", message);
		}

		public void Write(string message, Type type, Splat.LogLevel logLevel)
		{
			MsLogger.Log(Convert(logLevel), "{Message}", message);
		}

		public void Write(Exception exception, string message, Type type, Splat.LogLevel logLevel)
		{
			MsLogger.Log(Convert(logLevel), exception, "{Message}", message);
		}

		public Splat.LogLevel Level => Splat.LogLevel.Debug;

		public static Microsoft.Extensions.Logging.LogLevel Convert(Splat.LogLevel logLevel)
		{
			switch (logLevel)
			{
				case Splat.LogLevel.Debug:
					return LogLevel.Debug;
				case Splat.LogLevel.Info:
					return LogLevel.Information;
				case Splat.LogLevel.Warn:
					return LogLevel.Warning;
				case Splat.LogLevel.Error:
					return LogLevel.Error;
				case Splat.LogLevel.Fatal:
					return LogLevel.Critical;
				default:
					throw new NotImplementedException();
			}
		}
	}
}