using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Configuration
{
    class AdapterSpecs : WithFakes
    {
        static string configuration_xml = @"<RunSettings>
  <RunConfiguration>
    <MaxCpuCount>0</MaxCpuCount>
  </RunConfiguration>
  <MSpec>
    <DisableFullTestNameInIDE>true</DisableFullTestNameInIDE>
    <DisableFullTestNameInOutput>true</DisableFullTestNameInOutput>
  </MSpec>
</RunSettings>";

        static MSpecTestAdapterExecutor adapter;
        static MSpecTestAdapterDiscoverer discoverer;

        Establish establish = () =>
        {
            The<IRunSettings>()
                .WhenToldTo(runSettings => runSettings.SettingsXml)
                .Return(configuration_xml);

            The<IRunContext>()
                .WhenToldTo(context => context.RunSettings)
                .Return(The<IRunSettings>());

            discoverer = new MSpecTestAdapterDiscoverer(An<ISpecificationDiscoverer>());
            adapter = new MSpecTestAdapterExecutor(The<ISpecificationExecutor>(), discoverer, An<ISpecificationFilterProvider>());
        };

        Because of = () =>
            adapter.RunTests(new[] { new TestCase("a", MSpecTestAdapter.Uri, "dll") }, The<IRunContext>(), An<IFrameworkHandle>());

        It should_pick_up_disable_full_test_name_in_ide = () =>
            The<ISpecificationExecutor>()
                .WasToldTo(d => d.RunAssemblySpecifications("dll",
                    Param<VisualStudioTestIdentifier[]>.IsAnything,
                    Param<Settings>.Matches(s => s.DisableFullTestNameInIDE),
                    Param<Uri>.IsAnything,
                    Param<IFrameworkHandle>.IsAnything));

        It should_pick_up_disable_full_test_name_in_output = () =>
            The<ISpecificationExecutor>()
                .WasToldTo(d => d.RunAssemblySpecifications("dll",
                    Param<VisualStudioTestIdentifier[]>.IsAnything,
                    Param<Settings>.Matches(s => s.DisableFullTestNameInOutput),
                    Param<Uri>.IsAnything,
                    Param<IFrameworkHandle>.IsAnything));

    }
}
