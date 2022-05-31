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

public class ConsoleOp : Logger.Op
{
    public ConsoleOp(ConsoleLogger l) : base(l) { }

    public override void Complete()
    {
        L.Info("Complete.");
        isCompleted = true;
    }

    public override void Abandon()
    {
        isAbandoned = true;
        L.Error("Abandoned.");
    }

    public override void Dispose()
    {
        if (!(isCompleted || isAbandoned))
        {
            L.Error("Abandoned.");
        }
    }
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

    public override Op Begin(string messageTemplate, params object[] args) => new ConsoleOp(this);
}
