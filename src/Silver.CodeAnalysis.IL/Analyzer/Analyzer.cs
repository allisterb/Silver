namespace Silver.CodeAnalysis.IL;

public class Analyzer : Runtime
{
    public Analyzer(string fileName)
    {
		FileName = fileName;
		Host = new PeReader.DefaultHost();
		Module = this.Host.LoadUnitFrom(fileName) as IModule;

		if (Module is null || Module == Dummy.Module || Module == Dummy.Assembly)
		{
			Error("The file {0} is not a valid CLR module or assembly.", fileName);
			return;
		}
		var pdbFileName = Path.ChangeExtension(fileName, "pdb");

		if (File.Exists(pdbFileName))
		{
			PdbReader = new PdbReader(fileName, pdbFileName, this.Host, true);
		}
		Types.Initialize(Host);
		Initialized = true;
	}

	#region Properties
	public string FileName { get; init; }
	public IMetadataHost Host { get; init; }
	public IModule? Module { get; init; }
	public PdbReader? PdbReader { get; init; }

	
    #endregion

    #region Methods
    #endregion
}

