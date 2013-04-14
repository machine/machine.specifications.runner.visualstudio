Machine.VSTestAdapter
=====================

Visual Studio 2012 Test Adapter for Machine.Specifictions (MSpec)

Download the installer from the Visual Studio Gallery here http://visualstudiogallery.msdn.microsoft.com/4abcb54b-53b5-4c44-877f-0397556c5c44


# Where the test adapter stands right now
* Current version is 0.1.2
* The MSpec Visual Studio 2012 test adapter has been tested on Visual Studio 2012 Professional Edition with and without Visual Studio Update 1 and Update 2.
* Running with Visual Studio Update 1 or greater is preferable, as grouping by trait (Classname and MSpec Subject) is available.
* Only .NET 4.0 and .NET 4.5 MSpec assemblies have been tested.
* The adapter was tested on solutions with several thousand tests in multiple MSpec projects.
* The adapter is built against MSpec version 0.5.12 and has been tested against test projects built against MSpec version 0.5.0 and above.
* Support for nested types has been added.

![Alt text](https://github.com/eugeneduvenage/machine.vstestadapter/raw/dev/Misc/TestWindowScreenShot.png)

# What still needs to be done.
* Test coverage is rather low, this needs to improve soon.
* Check that behaviors work and add support if they do not.
* Add some performance improvements.
* Merge the VSRunner into the Machine.Specifications project.

# Contributing
* Source code is located at https://github.com/eugeneduvenage/machine.vstestadapter
* Please fork the dev branch and not master before making changes and submitting pull requests.
