Machine.VSTestAdapter
=====================

Visual Studio 2012 Test Adapter for Machine.Specifictions (MSpec)

Download the installer from the Visual Studio Gallery here http://visualstudiogallery.msdn.microsoft.com/

 # Where the test adapter stands right now
  * The MSpec Visual Studio 2012 test adapter has been tested on Visual Studio 2012 Professional Edition with and without Visual Studio Update 1.
  * Running with Visual Studio Update 1 is preferable, as grouping by trait (Classname and MSpec Subject) is available.
  * Only .NET 4.0 and .NET 4.5 MSpec assemblies have been tested.
  * The adapter was tested on solutions with several thousand tests in multiple MSpec projects.
  * The adapter is built against MSpec version 0.5.10 and has been tested against test projects built against MSpec version 0.5.0 and above.
 
 # What still needs to be done.
  * Test coverage is rather low, this needs to improve soon.
  * Check that behaviors work and add support if they do not.
  * Add some performance improvements.
  
 # Known issues
  * Every now and again once all the tests are successfully run, Visual Studio reports an error that an attempt has been made to access an AppDomain that has been unloaded, XUnit reported a simialr issue originally before fixing it, I need to take a deeper look.
  
 # Contributing
  * Please fork the dev branch and not master before making changes and submitting pull requests.