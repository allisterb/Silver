# Silver
<p align="center">
  <img src="https://static.wikia.nocookie.net/zelda_gamepedia_en/images/5/5a/BotW_Silver_Shield_Model.png/revision/latest/scale-to-width-down/335?cb=20210118165032&format=original" />
</p>

[![img](https://img.shields.io/nuget/v/Silver.CodeAnalysis?style=plastic)](https://www.nuget.org/packages/Silver.CodeAnalysis/)

## About

Silver is a [static analysis](https://en.wikipedia.org/wiki/Static_program_analysis) and [formal verification](https://en.wikipedia.org/wiki/Formal_verification) 
tool for Stratis smart contracts. Silver can analyze both C# source code using a Roslyn diagnostic analyzer and CIL code in a .NET bytecode assembly 
and can run both inside Visual Studio and on the command line.

![type not allowed from namespace](https://dm2301files.storage.live.com/y4mtdREUkjcGF6gKDRZjHDPQ1s0NU53LLENRXrni2IXbOeNblTZ4z7xMATD2woY3RdyoZvto0VlnKjW80e6tUISj2YO2t4JifQJdj0tRIwK5YDt5XIuLSWo-fBbwl6iWcF7jQGuJ0zlhvk7_uYfoflzmJSp7E612_O6O5KREX3vWTYcEJHpGO4kYHC6r6309vJx?width=1424&height=1015&cropmode=none)

Silver can [validate](https://github.com/allisterb/Silver/blob/master/src/Silver.CodeAnalysis.Cs/Silver.CodeAnalysis.Cs/Validator.cs) C# code using a Roslyn diagnostic analyzer according to the same [rules](https://github.com/stratisproject/StratisFullNode/blob/master/Documentation/Features/SmartContracts/Clr-execution-and-validation.md) for types and members used by the Stratis CLR VM for smart contracts. All the [validation policies](https://github.com/stratisproject/StratisFullNode/blob/master/src/Stratis.SmartContracts.CLR.Validation/DeterminismPolicy.cs) currently in use will be ported to the Roslyn analyzer.

Silver can disassemble smart contract CIL code in a .NET bytecode assembly:

![Silver disassembler](https://dm2301files.storage.live.com/y4mCr82EmE3ovpdLVj9Xihx2Oa1TeLDD3SEvnNTlvC2JRws1ka1X2G09KKVk1XfTxjai4AFZlWZjhrXNT0TYdVLF-ofmBqtmWmTX10YLKZcCO-lIXw6UvpOs1ikO6r_bEMHGxpx5h3CX8dkTqILCNYYz9Alp43dH5-q_aQu1rQfLkbZFWdT3i8k6h6lOv8ITjpS?width=1920&height=945&cropmode=none)
and statically analyze it using the [Analysis.Net](https://github.com/edgardozoppi/analysis-net/tree/master) framework e.g. the following is a [call-graph](https://en.wikipedia.org/wiki/Call_graph) analysis of the methods in the [Address Mapper](https://github.com/stratisproject/CirrusSmartContracts/tree/master/Mainnet/AddressMapper) contract.

![img](https://dm2301files.storage.live.com/y4mLu9yA4qSBuSATzoJqXQtKfaJCMsDx11duBmqvmt5ZDMgvXMJhvPVIurq9har4_VC2vza5GKYWXYhOReBYPW3g-xS1iDWmYiEjEqLfxzSZMzrfXTS51oDOEml0oT3Y_MuL8OLc8Bvm8VWVqToi37DxrXBTBiyfRwRU09k57lEK8riBf_OvJGxdiVNWwl-lH84?width=1916&height=1023&cropmode=none)

Silver can [output graphs](https://github.com/allisterb/Silver/tree/master/src/Silver.Drawing) in different formats like PNG images:

![img](https://dm2301files.storage.live.com/y4mRkO7wiNlaUapDiUxbW_hLwNXWrXOhhyE3fTSHLoelnaD3GIvKMRUv97clPiiyW__NfobAAzSNuNUT4Frk3sIluCe9uhcds2vA0z0nVMOYd2C6xz6cXcnBwo0g3YbYH-CC8SxLDdGRhZHGOUTdxuYmptpXMojwcJQc_fgGJPgfurMuqF3ATuTSO359j3o-39M?width=2000&height=612&cropmode=none)
or in the DGML format which are natively supported in Visual Studio:

![img](https://dm2301files.storage.live.com/y4mOhd7isx7dRXOsuYjaZk1o88mkSv7sjqVzuGyTdhGRa9mYHLB2ziQQXbkyE-pdv5I4zqgYFgoXOgvZY88YBAOvs7I41I77KB1lw_9rZ9-ZSxHBWOutiBUZDYMGLnmGmaZJYGv9azJD3I9v0GTARJIIysAD4UJqoFZrQURyXfmE0HZXI1kSZIOtHAy9-H7JtEz?width=1904&height=946&cropmode=none)

Silver can formally verify smart contracts in C# using the Spec# compiler from Microsoft Research:

![Silver verifier](https://dm2301files.storage.live.com/y4m1bPIN0-HBrPvmt-Aq62K-m3zlUQWs28zmJqtCsBRbLm4sTvK8sbR2Z1-9BEFh24LKD1WEJpn1g67tGJvP63bi6ng1VloHBFMnYXdTK6ceqrCPnM01t_CTFDEGvLkOJcodXkmpWJVcSZheLaJh-6X4oUVKBJ98dyQYdxh4hwoMK5vu1mtjBdRUPsXsfW0_78s?width=860&height=475&cropmode=none)

See the [wiki](https://github.com/allisterb/Silver/wiki) for more in-depth technical information and documentation.

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
5. Compile and analyze one of the example projects e.g. `./silver compile examples\AddressMapper\AddressMapper.csproj` and ` silver dis examples/AddressMapper/bin/Debug/netcoreapp2.1/AddressMapper.dll`
6. On Windows you can verify one of the example projects e.g. ` silver verify examples\SimpleVerifiableContracts\SimpleVerifiableContracts.csproj` or ` silver compile examples\SimpleVerifiableContracts\SimpleVerifiableContracts.csproj --verify`

### Usage
See `silver help` for the different commands and actions.
