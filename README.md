# Silver
<p align="center">
  <img src="https://static.wikia.nocookie.net/zelda_gamepedia_en/images/5/5a/BotW_Silver_Shield_Model.png/revision/latest/scale-to-width-down/400?cb=20210118165032&format=original" />
</p>

## About
Silver is a [static analysis](https://en.wikipedia.org/wiki/Static_program_analysis) and [formal verification](https://en.wikipedia.org/wiki/Formal_verification) 
tool for Stratis smart contracts. Silver can analyze both C# source code using a Roslyn diagnostic analyzer and CIL code in a bytecode assembly 
and can run both inside Visual Studio and on the command line.

![type not allowed from namespace](https://dm2301files.storage.live.com/y4mtdREUkjcGF6gKDRZjHDPQ1s0NU53LLENRXrni2IXbOeNblTZ4z7xMATD2woY3RdyoZvto0VlnKjW80e6tUISj2YO2t4JifQJdj0tRIwK5YDt5XIuLSWo-fBbwl6iWcF7jQGuJ0zlhvk7_uYfoflzmJSp7E612_O6O5KREX3vWTYcEJHpGO4kYHC6r6309vJx?width=1424&height=1015&cropmode=none)

Silver can [validate](https://github.com/allisterb/Silver/blob/master/src/Silver.CodeAnalysis.Cs/Silver.CodeAnalysis.Cs/Validator.cs) C# code using a Roslyn diagnostic analyzer according to the same [rules](https://github.com/stratisproject/StratisFullNode/blob/master/Documentation/Features/SmartContracts/Clr-execution-and-validation.md) for types and members used by the Stratis CLR VM for smart contracts. In the screenshot above the class generates a diagnostic with the code _SC0002_ when a smart contract class does not inherit from the base **Stratis.SmartContracts.SmartContract** class. All the [validation policies](https://github.com/stratisproject/StratisFullNode/blob/master/src/Stratis.SmartContracts.CLR.Validation/DeterminismPolicy.cs) currently in use will be ported to the Roslyn analyzer.

Silver can also statically analyze CIL code in a .NET assembly using the [Analysis.Net](https://github.com/edgardozoppi/analysis-net/tree/master) framework e.g. the following shows a [call-graph](https://en.wikipedia.org/wiki/Call_graph) analysis of the methods in the [Address Mapper](https://github.com/stratisproject/CirrusSmartContracts/tree/master/Mainnet/AddressMapper) contract.
![img](https://dm2301files.storage.live.com/y4mLu9yA4qSBuSATzoJqXQtKfaJCMsDx11duBmqvmt5ZDMgvXMJhvPVIurq9har4_VC2vza5GKYWXYhOReBYPW3g-xS1iDWmYiEjEqLfxzSZMzrfXTS51oDOEml0oT3Y_MuL8OLc8Bvm8VWVqToi37DxrXBTBiyfRwRU09k57lEK8riBf_OvJGxdiVNWwl-lH84?width=1916&height=1023&cropmode=none)

Silver can [output graphs](https://github.com/allisterb/Silver/tree/master/src/Silver.Drawing) in different formats like PNG images.
![img](https://dm2301files.storage.live.com/y4mRkO7wiNlaUapDiUxbW_hLwNXWrXOhhyE3fTSHLoelnaD3GIvKMRUv97clPiiyW__NfobAAzSNuNUT4Frk3sIluCe9uhcds2vA0z0nVMOYd2C6xz6cXcnBwo0g3YbYH-CC8SxLDdGRhZHGOUTdxuYmptpXMojwcJQc_fgGJPgfurMuqF3ATuTSO359j3o-39M?width=2000&height=612&cropmode=none)

Silver can also generate graphs in the DGML format which are natively supported in Visual Studio. The following screenshot shows the call-graph in DGML format being manipulated and analyzed by the Visual Studio DGML editor.

![img](https://dm2301files.storage.live.com/y4mOhd7isx7dRXOsuYjaZk1o88mkSv7sjqVzuGyTdhGRa9mYHLB2ziQQXbkyE-pdv5I4zqgYFgoXOgvZY88YBAOvs7I41I77KB1lw_9rZ9-ZSxHBWOutiBUZDYMGLnmGmaZJYGv9azJD3I9v0GTARJIIysAD4UJqoFZrQURyXfmE0HZXI1kSZIOtHAy9-H7JtEz?width=1904&height=946&cropmode=none)



## Building

### Requirements
* [NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* Mono (on *nix/MacOs)
* libgdiplus (on *nix/MacOs, for graph drawing)

### Known issues
The verifier is currently broken on non-Windows as the Spec# verifier depends on some Windows specific code in the compiler to write .PDB files which are needed to verify an assembly. Everything else should work cross-platform including the analyzers and compiler.

### Steps
1. Ensure requirements are installed
2. Clone this git repo and submodules: `git clone https://github.com/allisterb/Silver.git --recurse-submodules`
3. Run .`/build` or `build.cmd` in the root repo directory. Build should complete without errors.
4. Run `./silver install` to download and install the external tools needed.
5. Compile and verify one of the example projects e.g. `./silver compile examples\AddressMapper\AddressMapper.csproj --verify` and ` silver verify examples\AddressMapper\bin\Debug\netcoreapp2.1\ssc\AddressMapper.dll`

### Usage
See `silver help` for the different commands and actions.
