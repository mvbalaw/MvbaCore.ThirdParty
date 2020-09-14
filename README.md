MvbaCoreThirdParty ReadMe
===
### Description

MvbaCore.ThirdParty is a collection of common classes that we use in our projects at MVBA. The classes used to be a part of MvbaCore, but we moved them so that MvbaCore would not have any third party dependencies. This library depends on NewtonSoft.Json and Lucene.Net

It has the following features:

* Utility classes to support serialization and deserialization to/from Json.
* Utility classes to support interacting with Lucene.Net.


### How To Build:

The build script requires Ruby with rake installed.

1. Run `InstallGems.bat` to get the ruby dependencies (only needs to be run once per computer)
1. open a command prompt to the root folder and type `rake` to execute rakefile.rb

If you do not have ruby:

1. open src\MvbaCore.ThirdParty.sln with Visual Studio and build the solution

### License

[MIT License][mitlicense]

This project is part of [MVBA's Open Source Projects][MvbaLawGithub].

If you have questions or comments about this project, please contact us at <mailto:opensource@mvbalaw.com>.

[MvbaLawGithub]: http://mvbalaw.github.io/
[mitlicense]: http://www.opensource.org/licenses/mit-license.php
