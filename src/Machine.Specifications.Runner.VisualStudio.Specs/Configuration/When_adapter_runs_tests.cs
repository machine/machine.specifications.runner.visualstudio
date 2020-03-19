using System;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Configuration;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Execution;
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

        static MSpecTestAdapter adapter;

        Establish establish = () =>
        {
            The<IRunSettings>()
                .WhenToldTo(runSettings => runSettings.SettingsXml)
                .Return(ConfigurationXml);

            The<IRunContext>()
                .WhenToldTo(context => context.RunSettings)
                .Return(The<IRunSettings>());

            adapter = new MSpecTestAdapter(An<ISpecificationDiscoverer>(), The<ISpecificationExecutor>(), An<ISpecificationFilterProvider>());
        };

        Because of = () =>
            adapter.RunTests(new[] { "dll" }, The<IRunContext>(), An<IFrameworkHandle>());

        It should_pick_up_DisableFullTestNameInIDE = () =>
            The<ISpecificationExecutor>()
                .WasToldTo(d => d.RunAssembly("dll",
                    Param<Settings>.Matches(s => s.DisableFullTestNameInIDE),
                    Param<Uri>.IsAnything,
                    Param<IFrameworkHandle>.IsAnything));

        It should_pick_up_DisableFullTestNameInOutput = () =>
            The<ISpecificationExecutor>()
                .WasToldTo(d => d.RunAssembly("dll",
                    Param<Settings>.Matches(s => s.DisableFullTestNameInOutput),
                    Param<Uri>.IsAnything,
                    Param<IFrameworkHandle>.IsAnything));
    }
}
