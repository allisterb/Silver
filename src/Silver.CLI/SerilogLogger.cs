namespace Silver;

using Serilog;
using SerilogTimings;
using SerilogTimings.Extensions;

public class SerilogLogger : Logger
{
    public SerilogLogger(bool console = false, string? logFileName = null, bool debug = false)
    {
        Config = new LoggerConfiguration();
        if (console)
        {
            Config = new LoggerConfiguration().WriteTo.Console();
        }
        Config = Config.WriteTo.File(logFileName ?? "Silver.log");
        if (debug)
        {
            Config = Config.MinimumLevel.Debug();
        }
        Logger = Config.CreateLogger();
    }

    public SerilogLogger(ILogger logger)
    {
        Logger = logger;
    }

    public SerilogLogger() : this(Log.Logger) { }

    public LoggerConfiguration? Config { get; protected set; }
    public ILogger Logger { get; protected set; }

    [DebuggerStepThrough]
    public override void Info(string messageTemplate, params object[] args) => Logger.Information(messageTemplate, args);

    [DebuggerStepThrough]
    public override void Debug(string messageTemplate, params object[] args) => Logger.Debug(messageTemplate, args);

    [DebuggerStepThrough]
    public override void Error(string messageTemplate, params object[] args) => Logger.Error(messageTemplate, args);

    [DebuggerStepThrough]
    public override void Error(Exception ex, string messageTemplate, params object[] args) => Logger.Error(ex, messageTemplate, args);

    [DebuggerStepThrough]
    public override void Warn(string messageTemplate, params object[] args) => Logger.Warning(messageTemplate, args);

    [DebuggerStepThrough]
    public override void Fatal(string messageTemplate, params object[] args) => Logger.Fatal(messageTemplate, args);

    [DebuggerStepThrough]
    public override Op Begin(string messageTemplate, params object[] args)
    {
        Info(messageTemplate + "...", args);
        return new SerilogOp(this, Logger.BeginOperation(messageTemplate, args));
    }
}

public class SerilogOp : Logger.Op
{
    public SerilogOp(SerilogLogger logger, Operation op) : base(logger)
    {
        Op = op;
        this.logger = logger;
    }

    [DebuggerStepThrough]
    public override void Abandon()
    {
        Op.Abandon();
        isAbandoned = true;
    }

    [DebuggerStepThrough]
    public override void Complete()
    {
        Op.Complete();
        isCompleted = true;
    }

    [DebuggerStepThrough]
    public override void Dispose()
    {
        if (!(isAbandoned || isCompleted))
        {
            Op.Cancel();
        }
    }

    protected Operation Op;

    protected Logger logger;
}




