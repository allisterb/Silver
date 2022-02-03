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

    public static bool IsSmartContractMethod(this IMethodDefinition method)
        => method.ContainingTypeDefinition.IsSmartContract() && method.Visibility == TypeMemberVisibility.Public;
        
    public static string GetUniqueName(this IMethodDefinition md)
        => MemberHelper.GetMemberSignature(md, NameFormattingOptions.Signature);
    public static string GetUniqueId(this IMethodDefinition md, int id)
        => md.GetUniqueName() + "::" + id;

    public static IEnumerable<IOperation> GetILFromBody(this Instruction i, IMethodBody body) => body.Operations.Where(il => il.Offset == i.Offset);
}

