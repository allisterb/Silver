namespace Silver.Verifier.Models;
#nullable disable
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", ElementName = "boogie", IsNullable = false)]
public partial class BoogieResult
{
    private BoogieFile fileField;

    private string versionField;

    private string commandLineField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("version")]
    public string Version
    {
        get
        {
            return this.versionField;
        }
        set
        {
            this.versionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("commandLine")]
    public string CommandLine
    {
        get
        {
            return this.commandLineField;
        }
        set
        {
            this.commandLineField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElement("file")]
    public BoogieFile File
    {
        get
        {
            return this.fileField;
        }
        set
        {
            this.fileField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class BoogieFile
{

    private BoogieFileMethod[] methodField;

    private string nameField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElement("method")]
    public BoogieFileMethod[] Method
    {
        get
        {
            return this.methodField;
        }
        set
        {
            this.methodField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttribute("name")]
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
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class BoogieFileMethod
{

    private BoogieFileMethodError[] errorField;

    private BoogieFileMethodConclusion conclusionField;

    private string nameField;

    private string startTimeField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElement("error")]
    public BoogieFileMethodError[] Error
    {
        get
        {
            return this.errorField;
        }
        set
        {
            this.errorField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttribute("conclusion")]
    public BoogieFileMethodConclusion Conclusion
    {
        get
        {
            return this.conclusionField;
        }
        set
        {
            this.conclusionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttribute("name")]
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
    [System.Xml.Serialization.XmlAttribute("startTime")]
    public string StartTime
    {
        get
        {
            return this.startTimeField;
        }
        set
        {
            this.startTimeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class BoogieFileMethodError
{

    private BoogieFileMethodErrorTraceNode[] traceField;

    private string messageField;

    private string fileField;

    private byte lineField;

    private bool lineFieldSpecified;

    private byte columnField;

    private bool columnFieldSpecified;

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItem("traceNode", IsNullable = false)]
    public BoogieFileMethodErrorTraceNode[] Trace
    {
        get
        {
            return this.traceField;
        }
        set
        {
            this.traceField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttribute("message")]
    public string Message
    {
        get
        {
            return this.messageField;
        }
        set
        {
            this.messageField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttribute("file")]
    public string File
    {
        get
        {
            return this.fileField;
        }
        set
        {
            this.fileField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("line")]
    public byte Line
    {
        get
        {
            return this.lineField;
        }
        set
        {
            this.lineField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool lineSpecified
    {
        get
        {
            return this.lineFieldSpecified;
        }
        set
        {
            this.lineFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("column")]
    public byte Column
    {
        get
        {
            return this.columnField;
        }
        set
        {
            this.columnField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool columnSpecified
    {
        get
        {
            return this.columnFieldSpecified;
        }
        set
        {
            this.columnFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class BoogieFileMethodErrorTraceNode
{

    private string labelField;

    private string fileField;

    private byte lineField;

    private bool lineFieldSpecified;

    private byte columnField;

    private bool columnFieldSpecified;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("label")]
    public string Label
    {
        get
        {
            return this.labelField;
        }
        set
        {
            this.labelField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("file")]
    public string File
    {
        get
        {
            return this.fileField;
        }
        set
        {
            this.fileField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("line")]
    public byte Line
    {
        get
        {
            return this.lineField;
        }
        set
        {
            this.lineField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool lineSpecified
    {
        get
        {
            return this.lineFieldSpecified;
        }
        set
        {
            this.lineFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("column")]
    public byte Column
    {
        get
        {
            return this.columnField;
        }
        set
        {
            this.columnField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool columnSpecified
    {
        get
        {
            return this.columnFieldSpecified;
        }
        set
        {
            this.columnFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class BoogieFileMethodConclusion
{

    private string endTimeField;

    private decimal durationField;

    private string outcomeField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("endTime")]
    public string EndTime
    {
        get
        {
            return this.endTimeField;
        }
        set
        {
            this.endTimeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("duration")]
    public decimal Duration
    {
        get
        {
            return this.durationField;
        }
        set
        {
            this.durationField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("outcome")]
    public string Outcome
    {
        get
        {
            return this.outcomeField;
        }
        set
        {
            this.outcomeField = value;
        }
    }
}
#nullable restore
