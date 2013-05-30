mvbacore-thirdparty ReadMe
===

## DESCRIPTION

mvbacore-thirdparty is a collection of common classes that we use in our projects at MVBA. The classes used to be a part of MvbaCore, but we moved them so that MvbaCore would not have any third party dependencies. This library depends on NewtonSoft.Json and Lucene.Net

It has the following features:

* Utility classes to support serialization and deserialization to/from Json.
* Utility classes to support interacting with Lucene.Net.


## HOW TO BUILD

The build script requires Ruby with rake installed.

1. Run `InstallGems.bat` to get the ruby dependencies (only needs to be run once per computer)
1. open a command prompt to the root folder and type `rake` to execute rakefile.rb

If you do not have ruby:

1. You need to create a src\CommonAssemblyInfo.cs file. Go.bat will copy src\CommonAssemblyInfo.cs.default to src\CommonAssemblyInfo.cs
1. open src\MvbaCore.ThirdParty.sln with Visual Studio and Build the solution

## License		

[MIT License][mitlicense]

This project is part of [MVBA Law Commons][mvbalawcommons].

[mvbalawcommons]: http://code.google.com/p/mvbalaw-commons/
[mitlicense]: http://www.opensource.org/licenses/mit-license.php
