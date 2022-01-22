using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Silver.CodeAnalysis.Cs;
using VerifyCS = Silver.CodeAnalysis.Cs.Tests.CSharpCodeFixVerifier<Silver.CodeAnalysis.Cs.SmartContractSyntaxValidator,
    Silver.CodeAnalysis.Cs.SilverCodeAnalysisCsCodeFixProvider>;

namespace Silver.CodeAnalysis.Cs
{
    [TestClass]
    public class SyntaxTests
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task UsingDirectiveNullTest()
        {
            var test = @"
                using Stratis.SmartContracts;
                namespace ConsoleApplication1
                {
                    class {|#0:TypeName|}
                    {   
                    }
                }"; 
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task UsingDirectiveTest()
        {
            var test = @"
                using System;
                using System.Collections.Generic;
                using Stratis.SmartContracts;

                namespace ConsoleApplication1
                {
                    class {|#0:TypeName|}
                    {   
                    }
                }";

            /*
            var fixtest = @"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;
                using System.Diagnostics;

                namespace ConsoleApplication1
                {
                    class TYPENAME
                    {   
                    }
                }";
            */
            var syntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(test);
            var expected = new[] { VerifyCS.Diagnostic(SmartContractSyntaxValidator.GetErrorDescriptor("SC0001"))
                .WithSpan(2, 17, 2, 30).WithArguments("System"), VerifyCS.Diagnostic(SmartContractSyntaxValidator.GetErrorDescriptor("SC0001"))
                .WithSpan(3, 17, 3, 50).WithArguments("System.Collections.Generic")};
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
