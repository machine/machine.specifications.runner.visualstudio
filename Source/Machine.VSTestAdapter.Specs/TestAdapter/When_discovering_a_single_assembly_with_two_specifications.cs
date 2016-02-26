using Machine.Fakes;
using Machine.Specifications;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using Machine.VSTestAdapter.Discovery;

namespace Machine.VSTestAdapter.Specs.TestAdapter
{
    public class When_discovering_a_single_assembly_with_two_specifications : WithFakes
    {
        private static MSpecTestAdapter testAdapter;
        private static ISpecificationDiscoverer discoverer;
        private static ISpecificationExecutor executor;
        private static IMessageLogger logger;
        private static ITestCaseDiscoverySink discoverySink;
        private static IDiscoveryContext discoveryContext;

        private Establish context = () =>
            {
                discoverer = An<ISpecificationDiscoverer>();
                executor = An<ISpecificationExecutor>();
                discoveryContext = An<IDiscoveryContext>();
                logger = An<IMessageLogger>();
                discoverySink = An<ITestCaseDiscoverySink>();

                // mock - ensure when the discoverer tests if this is a valid mspec source location that we return true
                discoverer.WhenToldTo(x => x.SourceDirectoryContainsMSpec(Param.IsAny<string>()))
                    .Return(true);
                discoverer.WhenToldTo(x => x.AssemblyContainsMSpecReference(Param.IsAny<string>()))
                    .Return(true);

                // mock - return two specifications
                discoverer.WhenToldTo(x => x.EnumerateSpecs(Param.IsAny<string>()))
                    .Return(new MSpecTestCase[] {   new MSpecTestCase() { ContextType = "Test1Type", ContextFullType ="Test1FullType", SpecificationName = "Test1SpecName"} ,
                                                    new MSpecTestCase() { ContextType = "Test2Type", ContextFullType ="Test2FullType", SpecificationName = "Test2SpecName"}
                                                });

                // mock - use the MSpecTestAdapterFactory, adapter factory, constructor overload to specify the mocked discoverer and executor
                MSpecTestAdapterFactory factory = new MSpecTestAdapterFactory(() => { return discoverer; }, () => { return executor; });
                testAdapter = new MSpecTestAdapter(factory);
            };

        private Because of = () =>
            {
                testAdapter.DiscoverTests(new List<string>() { "source1" }, discoveryContext, logger, discoverySink);
            };

        private It should_send_an_informational_notification_of_starting_discovery = () =>
            {
                logger.WasToldTo(x => x.SendMessage(TestMessageLevel.Informational, Machine.VSTestAdapter.Strings.DISCOVERER_STARTING));
            };

        private It should_send_an_informational_notification_of_intent_to_interrogate_the_source = () =>
        {
            logger.WasToldTo(x => x.SendMessage(TestMessageLevel.Informational, string.Format(Machine.VSTestAdapter.Strings.DISCOVERER_LOOKINGIN, "source1")));
        };

        private It should_send_an_informational_notification_of_completing_discovery = () =>
        {
            logger.WasToldTo(x => x.SendMessage(TestMessageLevel.Informational, string.Format(Machine.VSTestAdapter.Strings.DISCOVERER_COMPLETE, 2, 1, 1)));
        };
    }
}