﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyLogger
{
    public class MyLogger<T> : ILogger<T>
    {
        private readonly string _logfile;

        public MyLogger(string logfile)
        {
            _logfile = logfile;

            new FileStream(_logfile, FileMode.Create, FileAccess.Write).Dispose();
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        bool ILogger.IsEnabled(LogLevel logLevel) =>
            true;


        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(_logfile, FileMode.Append, FileAccess.Write)))
            {
                
            }
        }
    }
}