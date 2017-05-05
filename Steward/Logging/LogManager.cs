using System;

namespace Steward.Logging
{
    internal class LogManager : ILogManager
    {
        ILog ILogManager.GetLogger(Type type)
        {
            return new LogImpl(log4net.LogManager.GetLogger(type));
        }
    }
}