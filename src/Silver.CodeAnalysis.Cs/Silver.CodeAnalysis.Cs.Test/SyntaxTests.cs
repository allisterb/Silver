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
        public async Task TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {   
        }
    }";

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

            var expected = VerifyCS.Diagnostic("SC0001");
            var ff = new Microsoft.CodeAnalysis.Testing.DiagnosticResult[6];
            System.Array.Fill(ff, expected);
            await VerifyCS.VerifyCodeFixAsync(test, ff, fixtest);
        }
    }
}
