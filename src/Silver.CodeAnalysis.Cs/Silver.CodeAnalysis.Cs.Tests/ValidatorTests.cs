using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Silver.CodeAnalysis.Cs.Test
{
    using VerifyCS = Tests.CSharpCodeFixVerifier<SmartContractAnalyzer, SilverCodeAnalysisCsCodeFixProvider>;

    [TestClass]
    public class ValidatorTests
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task NullTest()
        {
            var test = @"
                using Stratis.SmartContracts;
                
                public class Foo : SmartContract
                {   
                    public Foo(ISmartContractState state) : base(state) {}
                }
                "; 
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

            var expected = new[] 
            { 
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0001", DiagnosticSeverity.Error)).WithSpan(2, 17, 2, 30).WithArguments("System"), 
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0001", DiagnosticSeverity.Error)).WithSpan(3, 17, 3, 50).WithArguments("System.Collections.Generic")
            };

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
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0002", DiagnosticSeverity.Error)).WithSpan(6, 7, 11, 18).WithArguments("ConsoleApplication1")
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
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0003", DiagnosticSeverity.Error))
                .WithSpan(4, 23, 4, 31).WithArguments("TypeName")
            };
            await VerifyCS.VerifyAnalyzerAsync(nulltest);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ConstructorDeclTest()
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
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0003", DiagnosticSeverity.Error))
                .WithSpan(4, 23, 4, 31).WithArguments("TypeName")
            };
            await VerifyCS.VerifyAnalyzerAsync(nulltest);
            //await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AssertConditionAlwaysTrueTest()
        {
            const string test = @"
using Stratis.SmartContracts;

public class Foo : SmartContract
{   
    public Foo(ISmartContractState state, Address market) : base(state)
    {
        Assert(true, ""This is a pointless assert"");
    }
}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0010", DiagnosticSeverity.Warning))
                                             .WithSpan(8, 16, 8, 20)
                                             .WithArguments(true)
            };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AssertConditionAlwaysTrueConstTest()
        {
            const string test = @"
using Stratis.SmartContracts;

public class Foo : SmartContract
{   
    public Foo(ISmartContractState state, Address market) : base(state)
    {
        const bool condition = true;
        Assert(condition, ""This will always fail"");
    }
}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0010", DiagnosticSeverity.Warning))
                    .WithSpan(9, 16, 9, 25)
                    .WithArguments(true)
            };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AssertConditionAlwaysFalseTest()
        {
            const string test = @"
using Stratis.SmartContracts;

public class Foo : SmartContract
{   
    public Foo(ISmartContractState state, Address market) : base(state)
    {
        Assert(false, ""This will always fail"");
    }
}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0010", DiagnosticSeverity.Warning))
                    .WithSpan(8, 16, 8, 21)
                    .WithArguments(false)
            };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AssertConditionAlwaysFalseConstTest()
        {
            const string test = @"
using Stratis.SmartContracts;

public class Foo : SmartContract
{   
    public Foo(ISmartContractState state, Address market) : base(state)
    {
        const bool condition = false;
        Assert(condition, ""This will always fail"");
    }
}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0010", DiagnosticSeverity.Warning))
                    .WithSpan(9, 16, 9, 25)
                    .WithArguments(false)
            };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AssertMessageNotProvidedTest()
        {
            const string test = @"
using Stratis.SmartContracts;

public class Foo : SmartContract
{   
    public Foo(ISmartContractState state, Address market) : base(state)
    {
        Assert(market != Address.Zero);
    }
}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0011", DiagnosticSeverity.Info))
                    .WithSpan(8, 9, 8, 39)
            };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [DataTestMethod]
        [DataRow("\"\"")]
        [DataRow("string.Empty")]
        public async Task AssertMessageEmptyTest(string assertMessage)
        {
            var test = @$"
using Stratis.SmartContracts;

public class Foo : SmartContract
{{   
    public Foo(ISmartContractState state, Address market) : base(state)
    {{
        Assert(market != Address.Zero, {assertMessage});
    }}
}}";

            const int startColumn = 40;
            var expected = new[]
            {
                VerifyCS.Diagnostic(Validator.GetDescriptor("SC0012", DiagnosticSeverity.Info))
                        .WithSpan(8, startColumn, 8, startColumn + assertMessage.Length)
            };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
