namespace Silver.CodeAnalysis.IL;

using Backend.ThreeAddressCode.Instructions;

public static class Extensions
{
    public static bool HaseBaseClass(this ITypeDefinition t) => t.BaseClasses is not null && t.BaseClasses.Any();

    public static string GetName(this ITypeDefinition t) => TypeHelper.GetTypeName(t.ResolvedType);

    public static bool IsSmartContract(this ITypeDefinition t)
    {
        return t.HaseBaseClass() && t.BaseClasses.Any(t => t.ToString() == "Stratis.SmartContracts.SmartContract");
    }

    public static bool IsDeployedSmartContract(this ITypeDefinition t)
    {
        return t.IsSmartContract() && t.Attributes is not null && t.Attributes.Any(a => TypeHelper.GetTypeName(a.Type) == "Stratis.SmartContracts.DeployAttribute");
    }

    public static bool IsDummyName(this ITypeDefinitionMember method) =>
        method.Name.ToString() == "Microsoft.Cci.DummyName";

    public static bool IsSmartContractMethod(this IMethodDefinition method)
        => method.ContainingTypeDefinition.IsSmartContract() && method.Visibility == TypeMemberVisibility.Public;

    public static bool IsDeployedSmartContractMethod(this IMethodDefinition method)
       => method.ContainingTypeDefinition.IsDeployedSmartContract() && method.Visibility == TypeMemberVisibility.Public;

    public static string GetUniqueName(this IMethodDefinition md)
        => !md.IsDummyName() ? MemberHelper.GetMemberSignature(md, NameFormattingOptions.Signature) : MemberHelper.GetMemberSignature(md, NameFormattingOptions.Signature).TrimStart('.');
    public static string GetUniqueId(this IMethodDefinition md, int id)
        => md.GetUniqueName() + "::" + id;

    public static string GetName(this ITypeReference tr)
        => TypeHelper.GetTypeName(tr);//!td.IsDummyName() ? MemberHelper.GetMemberSignature(md, NameFormattingOptions.Signature) : MemberHelper.GetMemberSignature(md, NameFormattingOptions.Signature).TrimStart('.');

    public static IEnumerable<IOperation> GetILFromBody(this Instruction i, IMethodBody body) => body.Operations.Where(il => il.Offset == i.Offset);

    public static int GasCost(this IEnumerable<IOperation> ops)
    {
        int cost = 0;
        foreach (var op in ops)
        {
            if (op.OperationCode == OperationCode.Call || op.OperationCode == OperationCode.Callvirt)
            {
                cost += 5;
            }
            else
            {
                cost += 1;
            }
        }
        return cost;
    }

    //public static bool ReadsState(this IEnumerable<IOperation> ops) 
    //{ ops.Any(op => (op.OperationCode == OperationCode.Call || op.OperationCode== OperationCode.Callvirt) &&
    //    );
    //}
}

