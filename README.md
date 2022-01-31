# Silver
<p align="center">
  <img src="https://static.wikia.nocookie.net/zelda_gamepedia_en/images/5/5a/BotW_Silver_Shield_Model.png/revision/latest/scale-to-width-down/400?cb=20210118165032&format=original" />
</p>

## About
Silver is a [static analysis](https://en.wikipedia.org/wiki/Static_program_analysis) and [formal verification](https://en.wikipedia.org/wiki/Formal_verification) 
tool for Stratis smart contracts. Silver can analyze both C# source code using a Roslyn diagnostic analyzer and CIL code in a bytecode assembly 
and can run both inside Visual Studio and on the command line.

![type not allowed from namespace](https://dm2301files.storage.live.com/y4mtdREUkjcGF6gKDRZjHDPQ1s0NU53LLENRXrni2IXbOeNblTZ4z7xMATD2woY3RdyoZvto0VlnKjW80e6tUISj2YO2t4JifQJdj0tRIwK5YDt5XIuLSWo-fBbwl6iWcF7jQGuJ0zlhvk7_uYfoflzmJSp7E612_O6O5KREX3vWTYcEJHpGO4kYHC6r6309vJx?width=1424&height=1015&cropmode=none)

Silver can [validate](https://github.com/allisterb/Silver/blob/master/src/Silver.CodeAnalysis.Cs/Silver.CodeAnalysis.Cs/Validator.cs) C# code using a Roslyn diagnostic analyzer according to the same [validation policies](https://github.com/stratisproject/StratisFullNode/blob/master/src/Stratis.SmartContracts.CLR.Validation/DeterminismPolicy.cs) for types and members used by the Stratis CLR VM for smart contracts. In the screenshot above the class generates a diagnostic with the code _SC0002_ when a smart contract class does not inherit from the base **Stratis.SmartContracts.SmartContract** class.
![img](https://challengepost-s3-challengepost.netdna-ssl.com/photos/production/software_photos/001/814/415/datas/original.png)



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
