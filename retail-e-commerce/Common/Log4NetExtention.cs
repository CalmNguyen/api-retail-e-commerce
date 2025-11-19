using System.Diagnostics;

namespace retail_e_commerce.Common
{
    internal class Log4NetExtention
    {
    }
    public interface ILogExtension
    {
        //Info
        bool IsInfoEnabled { get; }
        bool Info(object message, Exception? exception = null);
        //Debug
        bool IsDebugEnabled { get; }
        bool Debug(object message, Exception? exception = null);
        //Warn
        bool IsWarnEnabled { get; }
        bool Warn(object message, Exception? exception = null);
        //Error
        bool IsErrorEnabled { get; }
        bool Error(object message, Exception? exception = null);
        //Fatal
        bool IsFatalEnabled { get; }
        bool Fatal(object message, Exception? exception = null);
    }
    public class NNPNLogger : ILogExtension
    {
        private readonly log4net.ILog _logger;
        public NNPNLogger(Type type)
        {
            _logger = log4net.LogManager.GetLogger(type);
        }
        //Info
        public bool IsInfoEnabled
        {
            get { return _logger.IsInfoEnabled; }
        }
        public bool Info(object message, Exception? exception = null)
        {
            if (_logger.IsInfoEnabled)
            {
                if (exception != null)
                {
                    _logger.Info(message, exception);
                }
                else
                {
                    _logger.Info(message);
                }
                return true;
            }
            return false;
        }
        //Debug
        public bool IsDebugEnabled
        {
            get { return _logger.IsDebugEnabled; }
        }
        public bool Debug(object message, Exception? exception = null)
        {
            if (_logger.IsInfoEnabled)
            {
                if (exception != null)
                {
                    _logger.Debug(message, exception);
                }
                else
                {
                    _logger.Debug(message);
                }
                return true;
            }
            return false;
        }
        //Warn
        public bool IsWarnEnabled
        {
            get { return _logger.IsWarnEnabled; }
        }
        public bool Warn(object message, Exception? exception = null)
        {
            if (_logger.IsInfoEnabled)
            {
                if (exception != null)
                {
                    _logger.Warn(message, exception);
                }
                else
                {
                    _logger.Warn(message);
                }
                return true;
            }
            return false;
        }
        //Error
        public bool IsErrorEnabled
        {
            get { return _logger.IsErrorEnabled; }
        }
        public bool Error(object message, Exception? exception = null)
        {
            if (_logger.IsInfoEnabled)
            {
                if (exception != null)
                {
                    _logger.Error(message, exception);
                }
                else
                {
                    _logger.Error(message);
                }
                return true;
            }
            return false;
        }
        //Fatal
        public bool IsFatalEnabled
        {
            get { return _logger.IsFatalEnabled; }
        }
        public bool Fatal(object message, Exception? exception = null)
        {
            if (_logger.IsInfoEnabled)
            {
                if (exception != null)
                {
                    _logger.Fatal(message, exception);
                }
                else
                {
                    _logger.Fatal(message);
                }
                return true;
            }
            return false;
        }
    }
    public static class LogExtension
    {
        public static ILogExtension InitLogger()
        {
            var stack = new StackTrace();
            var frame = stack.GetFrame(1);
            return new NNPNLogger(frame.GetMethod().DeclaringType);
        }
        public static ILogExtension InitLogger(Type type)
        {
            return new NNPNLogger(type);
        }
    }
}
