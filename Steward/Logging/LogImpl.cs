namespace Steward.Logging
{
    internal class LogImpl : ILog
    {
        private readonly log4net.ILog log;

        internal LogImpl(log4net.ILog log)
        {
            this.log = log;
        }

        void ILog.Debug(object message)
        {
            log.Debug(message);
        }

        void ILog.Error(object message)
        {
            log.Error(message);
        }
    }
}