using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Configuration
{
    public class When_adapter_runs_tests : WithFakes
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

        static MSpecTestAdapter adapter;

        Establish context = () =>
        {
            The<IRunSettings>()
                .WhenToldTo(x => x.SettingsXml)
                .Return(configuration_xml);

            The<IRunContext>()
                .WhenToldTo(x => x.RunSettings)
                .Return(The<IRunSettings>());

            adapter = new MSpecTestAdapter(An<ISpecificationDiscoverer>(), The<ISpecificationExecutor>());
        };

        Because of = () =>
            adapter.RunTests(new[] { "dll" }, The<IRunContext>(), An<IFrameworkHandle>());

        It should_pick_up_DisableFullTestNameInIDE = () =>
            FakeApi.WasToldTo(The<ISpecificationExecutor>(), d => d.RunAssembly("dll",
                    Param<Settings>.Matches(x => x.DisableFullTestNameInIDE),
                    Param<Uri>.IsAnything,
                    Param<IFrameworkHandle>.IsAnything));

        It should_pick_up_DisableFullTestNameInOutput = () =>
            FakeApi.WasToldTo(The<ISpecificationExecutor>(), x => x.RunAssembly("dll",
                    Param<Settings>.Matches(s => s.DisableFullTestNameInOutput),
                    Param<Uri>.IsAnything,
                    Param<IFrameworkHandle>.IsAnything));
    }
}
