namespace Silver.CodeAnalysis.Cs
{
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VerifyCS = Tests.CSharpCodeFixVerifier<SmartContractAnalyzer, SilverCodeAnalysisCsCodeFixProvider>;

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
            var nulltest = @"
                using Stratis.SmartContracts;
                
                    class {|#0:TypeName|}
                    {   
                    }
                ";

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

            var expected = new[] 
            { 
                VerifyCS.Diagnostic(SyntaxAnalyzer.GetErrorDescriptor("SC0001")).WithSpan(2, 17, 2, 30).WithArguments("System"), 
                VerifyCS.Diagnostic(SyntaxAnalyzer.GetErrorDescriptor("SC0001")).WithSpan(3, 17, 3, 50).WithArguments("System.Collections.Generic")
            };

            await VerifyCS.VerifyAnalyzerAsync(nulltest);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task NamespaceDeclTest()
        {
            var test = @"
                // using System;
                // using System.Collections.Generic;
                // using Stratis.SmartContracts;

                namespace ConsoleApplication1
                {
                    class {|#0:TypeName|}
                    {   
                    }
                }";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SyntaxAnalyzer.GetErrorDescriptor("SC0002")).WithSpan(6, 7, 11, 18).WithArguments("ConsoleApplication1")
            };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ClassDeclTest()
        {
            var nulltest = @"
                using Stratis.SmartContracts;
                
                class Foo : SmartContract
                {   
                    public Foo(ISmartContractState state) : base(state) { }

                }
                ";

            var test = @"
                using Stratis.SmartContracts;
                
                class {|#0:TypeName|}
                {   
                }
                ";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SyntaxAnalyzer.GetErrorDescriptor("SC0003"))
                .WithSpan(4, 17, 6, 18).WithArguments("TypeName")
            };
            await VerifyCS.VerifyAnalyzerAsync(nulltest);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
