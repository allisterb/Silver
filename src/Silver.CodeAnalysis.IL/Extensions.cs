namespace Silver.CodeAnalysis.IL
{
    public static class Extensions
    {
        public static bool HaseBaseClass(this ITypeDefinition t) => t.BaseClasses is not null && t.BaseClasses.Any();

        public static string GetName(this ITypeDefinition t) => TypeHelper.GetTypeName(t.ResolvedType);

        public static bool IsSmartContract(this ITypeDefinition t)
        {
            return t.HaseBaseClass() && t.BaseClasses.Any(t => t.ToString() == "Stratis.SmartContracts.SmartContract");
        }

        public static string GetUniqueId(this IMethodDefinition md, int id)
            => md.ContainingTypeDefinition.GetName() + "::" + md.Name + "::" + id; 
    }
}
