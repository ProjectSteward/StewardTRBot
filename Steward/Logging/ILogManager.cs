using System;

namespace Steward.Logging
{
    internal interface ILogManager
    {
        ILog GetLogger(Type type);
    }
}
