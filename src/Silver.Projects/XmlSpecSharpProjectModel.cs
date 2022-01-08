namespace Silver.Projects.Models;


#nullable disable
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class VisualStudioProject
{

    private VisualStudioProjectXEN xENField = new();

    /// <remarks/>
    public VisualStudioProjectXEN XEN
    {
        get
        {
            return this.xENField;
        }
        set
        {
            this.xENField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class VisualStudioProjectXEN
{

    private VisualStudioProjectXENBuild buildField;

    private VisualStudioProjectXENFiles filesField = new VisualStudioProjectXENFiles();

    private string projectTypeField;

    private decimal schemaVersionField;

    private string nameField;

    private string projectGuidField;

    /// <remarks/>
    public VisualStudioProjectXENBuild Build
    {
        get
        {
            return this.buildField;
        }
        set
        {
            this.buildField = value;
        }
    }

    /// <remarks/>
    public VisualStudioProjectXENFiles Files
    {
        get
        {
            return this.filesField;
        }
        set
        {
            this.filesField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ProjectType
    {
        get
        {
            return this.projectTypeField;
        }
        set
        {
            this.projectTypeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal SchemaVersion
    {
        get
        {
            return this.schemaVersionField;
        }
        set
        {
            this.schemaVersionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ProjectGuid
    {
        get
        {
            return this.projectGuidField;
        }
        set
        {
            this.projectGuidField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class VisualStudioProjectXENBuild
{

    private VisualStudioProjectXENBuildSettings settingsField;

    private VisualStudioProjectXENBuildReference[] referencesField;

    /// <remarks/>
    public VisualStudioProjectXENBuildSettings Settings
    {
        get
        {
            return this.settingsField;
        }
        set
        {
            this.settingsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Reference", IsNullable = false)]
    public VisualStudioProjectXENBuildReference[] References
    {
        get
        {
            return this.referencesField;
        }
        set
        {
            this.referencesField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class VisualStudioProjectXENBuildSettings
{

    private VisualStudioProjectXENBuildSettingsConfig[] configField;

    private string applicationIconField;

    private string assemblyNameField;

    private string outputTypeField;

    private string rootNamespaceField;

    private string startupObjectField;

    private string standardLibraryLocationField;

    private string targetPlatformField;

    private string targetPlatformLocationField;

    private string shadowedAssemblyField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Config")]
    public VisualStudioProjectXENBuildSettingsConfig[] Config
    {
        get
        {
            return this.configField;
        }
        set
        {
            this.configField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ApplicationIcon
    {
        get
        {
            return this.applicationIconField;
        }
        set
        {
            this.applicationIconField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string AssemblyName
    {
        get
        {
            return this.assemblyNameField;
        }
        set
        {
            this.assemblyNameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string OutputType
    {
        get
        {
            return this.outputTypeField;
        }
        set
        {
            this.outputTypeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string RootNamespace
    {
        get
        {
            return this.rootNamespaceField;
        }
        set
        {
            this.rootNamespaceField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string StartupObject
    {
        get
        {
            return this.startupObjectField;
        }
        set
        {
            this.startupObjectField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string StandardLibraryLocation
    {
        get
        {
            return this.standardLibraryLocationField;
        }
        set
        {
            this.standardLibraryLocationField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string TargetPlatform
    {
        get
        {
            return this.targetPlatformField;
        }
        set
        {
            this.targetPlatformField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string TargetPlatformLocation
    {
        get
        {
            return this.targetPlatformLocationField;
        }
        set
        {
            this.targetPlatformLocationField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ShadowedAssembly
    {
        get
        {
            return this.shadowedAssemblyField;
        }
        set
        {
            this.shadowedAssemblyField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class VisualStudioProjectXENBuildSettingsConfig
{

    private string nameField;

    private string allowUnsafeBlocksField;

    private uint baseAddressField;

    private string checkForOverflowUnderflowField;

    private string configurationOverrideFileField;

    private string defineConstantsField;

    private string documentationFileField;

    private string debugSymbolsField;

    private ushort fileAlignmentField;

    private string incrementalBuildField;

    private string optimizeField;

    private string outputPathField;

    private string registerForComInteropField;

    private bool removeIntegerChecksField;

    private string treatWarningsAsErrorsField;

    private byte warningLevelField;

    private string runProgramVerifierField;

    private string referenceTypesAreNonNullByDefaultField;

    private string programVerifierCommandLineOptionsField;

    private string runProgramVerifierWhileEditingField;

    private string allowPointersToManagedStructuresField;

    private string checkContractAdmissibilityField;

    private string checkPurityField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string AllowUnsafeBlocks
    {
        get
        {
            return this.allowUnsafeBlocksField;
        }
        set
        {
            this.allowUnsafeBlocksField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint BaseAddress
    {
        get
        {
            return this.baseAddressField;
        }
        set
        {
            this.baseAddressField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string CheckForOverflowUnderflow
    {
        get
        {
            return this.checkForOverflowUnderflowField;
        }
        set
        {
            this.checkForOverflowUnderflowField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ConfigurationOverrideFile
    {
        get
        {
            return this.configurationOverrideFileField;
        }
        set
        {
            this.configurationOverrideFileField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string DefineConstants
    {
        get
        {
            return this.defineConstantsField;
        }
        set
        {
            this.defineConstantsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string DocumentationFile
    {
        get
        {
            return this.documentationFileField;
        }
        set
        {
            this.documentationFileField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string DebugSymbols
    {
        get
        {
            return this.debugSymbolsField;
        }
        set
        {
            this.debugSymbolsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ushort FileAlignment
    {
        get
        {
            return this.fileAlignmentField;
        }
        set
        {
            this.fileAlignmentField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string IncrementalBuild
    {
        get
        {
            return this.incrementalBuildField;
        }
        set
        {
            this.incrementalBuildField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Optimize
    {
        get
        {
            return this.optimizeField;
        }
        set
        {
            this.optimizeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string OutputPath
    {
        get
        {
            return this.outputPathField;
        }
        set
        {
            this.outputPathField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string RegisterForComInterop
    {
        get
        {
            return this.registerForComInteropField;
        }
        set
        {
            this.registerForComInteropField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool RemoveIntegerChecks
    {
        get
        {
            return this.removeIntegerChecksField;
        }
        set
        {
            this.removeIntegerChecksField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string TreatWarningsAsErrors
    {
        get
        {
            return this.treatWarningsAsErrorsField;
        }
        set
        {
            this.treatWarningsAsErrorsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte WarningLevel
    {
        get
        {
            return this.warningLevelField;
        }
        set
        {
            this.warningLevelField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string RunProgramVerifier
    {
        get
        {
            return this.runProgramVerifierField;
        }
        set
        {
            this.runProgramVerifierField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ReferenceTypesAreNonNullByDefault
    {
        get
        {
            return this.referenceTypesAreNonNullByDefaultField;
        }
        set
        {
            this.referenceTypesAreNonNullByDefaultField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ProgramVerifierCommandLineOptions
    {
        get
        {
            return this.programVerifierCommandLineOptionsField;
        }
        set
        {
            this.programVerifierCommandLineOptionsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string RunProgramVerifierWhileEditing
    {
        get
        {
            return this.runProgramVerifierWhileEditingField;
        }
        set
        {
            this.runProgramVerifierWhileEditingField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string AllowPointersToManagedStructures
    {
        get
        {
            return this.allowPointersToManagedStructuresField;
        }
        set
        {
            this.allowPointersToManagedStructuresField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string CheckContractAdmissibility
    {
        get
        {
            return this.checkContractAdmissibilityField;
        }
        set
        {
            this.checkContractAdmissibilityField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string CheckPurity
    {
        get
        {
            return this.checkPurityField;
        }
        set
        {
            this.checkPurityField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class VisualStudioProjectXENBuildReference
{

    private string nameField;

    private string assemblyNameField;

    private bool privateField;

    private string hintPathField;

    private string projectField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string AssemblyName
    {
        get
        {
            return this.assemblyNameField;
        }
        set
        {
            this.assemblyNameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool Private
    {
        get
        {
            return this.privateField;
        }
        set
        {
            this.privateField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string HintPath
    {
        get
        {
            return this.hintPathField;
        }
        set
        {
            this.hintPathField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Project
    {
        get
        {
            return this.projectField;
        }
        set
        {
            this.projectField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class VisualStudioProjectXENFiles
{

    private VisualStudioProjectXENFilesFile[] includeField = Array.Empty<VisualStudioProjectXENFilesFile>();

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("File", IsNullable = false)]
    public VisualStudioProjectXENFilesFile[] Include
    {
        get
        {
            return this.includeField;
        }
        set
        {
            this.includeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class VisualStudioProjectXENFilesFile
{

    private string relPathField;

    private string subTypeField;

    private string buildActionField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string RelPath
    {
        get
        {
            return this.relPathField;
        }
        set
        {
            this.relPathField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string SubType
    {
        get
        {
            return this.subTypeField;
        }
        set
        {
            this.subTypeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string BuildAction
    {
        get
        {
            return this.buildActionField;
        }
        set
        {
            this.buildActionField = value;
        }
    }
}
#nullable restore

