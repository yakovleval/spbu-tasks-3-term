namespace MyThreadPool;

public class ThreadPoolShutDownException : Exception
{
    public ThreadPoolShutDownException(string message = "") : base(message) { }
}
