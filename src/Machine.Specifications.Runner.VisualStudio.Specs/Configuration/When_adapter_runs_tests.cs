using System;
using System.Linq;
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

        static MSpecTestAdapter Adapter;

        Establish establish = () => {
            The<IRunSettings>().WhenToldTo(runSettings => runSettings.SettingsXml).Return(ConfigurationXml);
            The<IRunContext>().WhenToldTo(context => context.RunSettings).Return(The<IRunSettings>());

            Adapter = new MSpecTestAdapter(An<ISpecificationDiscoverer>(), The<ISpecificationExecutor>());
        };


        Because of = () => {
            Adapter.RunTests(new[] { "dll" }, The<IRunContext>(), An<IFrameworkHandle>());
        };


        It should_pick_up_DisableFullTestNameInIDE = () => {
            The<ISpecificationExecutor>().WasToldTo(d => d.RunAssembly("dll",
                                                                       Param<Settings>.Matches(s => s.DisableFullTestNameInIDE == true),
                                                                       Param<Uri>.IsAnything,
                                                                       Param<IFrameworkHandle>.IsAnything));
        };

        It should_pick_up_DisableFullTestNameInOutput = () => {
            The<ISpecificationExecutor>().WasToldTo(d => d.RunAssembly("dll",
                                                                       Param<Settings>.Matches(s => s.DisableFullTestNameInOutput == true),
                                                                       Param<Uri>.IsAnything,
                                                                       Param<IFrameworkHandle>.IsAnything));
        };
        
    }
}
