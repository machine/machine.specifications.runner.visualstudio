using System;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Configuration;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Execution;
using Machine.VSTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Configuration
{
    public class When_adapter_runs_tests : WithFakes
    {
        static string ConfigurationXml = @"<RunSettings>
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
                .Return(ConfigurationXml);

            The<IRunContext>()
                .WhenToldTo(context => context.RunSettings)
                .Return(The<IRunSettings>());

            discoverer = new MSpecTestAdapterDiscoverer(An<ISpecificationDiscoverer>());
            adapter = new MSpecTestAdapterExecutor(The<ISpecificationExecutor>(), discoverer, An<ISpecificationFilterProvider>());
        };

        Because of = () => adapter.RunTests(new[] { new TestCase("a", MSpecTestAdapter.Uri, "dll") }, The<IRunContext>(), An<IFrameworkHandle>());

        It should_pick_up_DisableFullTestNameInIDE = () =>
            The<ISpecificationExecutor>()
                .WasToldTo(d => d.RunAssemblySpecifications("dll",
                    Param<VisualStudioTestIdentifier[]>.IsAnything,
                    Param<Settings>.Matches(s => s.DisableFullTestNameInIDE),
                    Param<Uri>.IsAnything,
                    Param<IFrameworkHandle>.IsAnything));

        It should_pick_up_DisableFullTestNameInOutput = () =>
            The<ISpecificationExecutor>()
                .WasToldTo(d => d.RunAssemblySpecifications("dll",
                    Param<VisualStudioTestIdentifier[]>.IsAnything,
                    Param<Settings>.Matches(s => s.DisableFullTestNameInOutput),
                    Param<Uri>.IsAnything,
                    Param<IFrameworkHandle>.IsAnything));

    }
}
