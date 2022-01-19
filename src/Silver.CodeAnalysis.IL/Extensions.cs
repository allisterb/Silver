using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.CodeAnalysis.IL
{
    internal static class Extensions
    {
        public static bool HaseBaseClass(this ITypeDefinition t) => TypeHelper.BaseClass(t) is not null;

        public static string GetName(this ITypeDefinition t) => TypeHelper.GetTypeName(t.ResolvedType);
        
        public static bool IsSmartContract(this ITypeDefinition t) => 
            t.HaseBaseClass() && TypeHelper.BaseClass(t).GetName() == "Stratis.SmartContract";
    }
}
