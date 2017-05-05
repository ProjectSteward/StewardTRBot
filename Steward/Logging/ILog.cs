namespace Steward.Logging
{
    internal interface ILog
    {
        void Debug(object message);
        void Error(object message);
    }
}
