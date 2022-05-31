namespace Silver;

public abstract class Logger
{ 
    public abstract class Op : IDisposable
    {
        public Op(Logger l)
        {
            L = l;
        }

        public Logger L;

        protected bool isCompleted = false;

        protected bool isAbandoned = false;

        public abstract void Complete();

        public abstract void Abandon();

        public abstract void Dispose();
    }

    public bool IsConfigured { get; protected set; } = false;

    public bool IsDebug { get; protected set; } = false;

    public abstract void Info(string messageTemplate, params object[] args);

    public abstract void Debug(string messageTemplate, params object[] args);

    public abstract void Error(string messageTemplate, params object[] args);

    public abstract void Error(Exception ex, string messageTemplate, params object[] args);

    public abstract void Warn(string messageTemplate, params object[] args);

    public abstract void Fatal(string messageTemplate, params object[] args);

    public abstract Op Begin(string messageTemplate, params object[] args);
}

#region Console logger
public class ConsoleOp : Logger.Op
{
    public ConsoleOp(ConsoleLogger l, string opName) : base(l) 
    { 
        this.opName = opName;
        timer.Start();
        L.Info(opName + "...");
    }

    public override void Complete()
    {
        timer.Stop();
        L.Info($"{opName} completed in {timer.ElapsedMilliseconds}ms.");
        isCompleted = true;
    }

    public override void Abandon()
    {
        timer.Stop();
        isAbandoned = true;
        L.Error($"{opName} abandoned after {timer.ElapsedMilliseconds}ms.");
    }

    public override void Dispose()
    {
        if (timer.IsRunning) timer.Stop();
        if (!(isCompleted || isAbandoned))
        {
            L.Error($"{opName} abandoned after {timer.ElapsedMilliseconds}ms.");
        }
    }

    string opName;
    Stopwatch timer = new Stopwatch();
}

public class ConsoleLogger : Logger
{
    public ConsoleLogger(bool debug = false):base() 
    { 
        IsDebug = debug; 
    }
    
    public override void Info(string messageTemplate, params object[] args) => Console.WriteLine("[INFO] " + messageTemplate, args);

    public override void Debug(string messageTemplate, params object[] args)
    {
        if (IsDebug) 
        { 
            Console.WriteLine("[DEBUG] " + messageTemplate, args); 
        }
    }

    public override void Error(string messageTemplate, params object[] args) => Console.WriteLine("[ERROR] " + messageTemplate, args);

    public override void Error(Exception ex, string messageTemplate, params object[] args) => Console.WriteLine("[ERROR] " + messageTemplate, args);

    public override void Warn(string messageTemplate, params object[] args) => Console.WriteLine("[WARN] " + messageTemplate, args);

    public override void Fatal(string messageTemplate, params object[] args) => Console.WriteLine("[FATAL] " + messageTemplate, args);

    public override Op Begin(string messageTemplate, params object[] args) => new ConsoleOp(this, String.Format(messageTemplate, args));
}
#endregion