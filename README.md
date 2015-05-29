Machine.VSTestAdapter
=====================

Visual Studio 2012, 2013, 2015 Test Adapter for Machine.Specifications (MSpec)

Download the installer from the Visual Studio Gallery here http://visualstudiogallery.msdn.microsoft.com/4abcb54b-53b5-4c44-877f-0397556c5c44


# Where the test adapter stands right now
* Current Version 0.1.6 Added support for Visual Studio 2015
* Version 0.1.5 (minor bug-fix, relating to traits no longer working in VS 2013 with version 0.1.4, VS2012 was unaffected)
* The MSpec Visual Studio 2012 test adapter has been tested on :
	* Visual Studio 2012 including Visual Studio Updates 1, 2 and 3 
	* Visual Studio 2013 
* Running with Visual Studio Update 1 or greater is preferable, as grouping by trait is possible - either by Classname or MSpec Subject (MSpec Tags can also be used from version 0.1.4).
* Only .NET 4.0 and .NET 4.5 MSpec assemblies have been tested.
* The adapter was tested on solutions with several thousand tests in multiple MSpec projects.
* The adapter is built against MSpec version 0.7.0 and has been tested against test projects built against MSpec version 0.5.0 and above.
* Support for nested types has been added.
* Support for discovering MSpec Custom Delegates has been added, including the new style AssertDelegateAttribute (since version 0.1.4).
* Support for Visual Studio 2013 has been added.

![Alt text](https://github.com/jonathanwilkins/machine.vstestadapter/raw/dev/Misc/TestWindowScreenShot.png)

# What still needs to be done.
* Test coverage is rather low, this needs to improve soon.
* Check that behaviors work and add support if they do not.
* Add some performance improvements.
* Merge the VSRunner into the Machine.Specifications project.

# Contributing
* Source code is located at https://github.com/eugeneduvenage/machine.vstestadapter
* Please fork the dev branch and not master before making changes and submitting pull requests.
