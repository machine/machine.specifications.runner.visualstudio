using Machine.Fakes;
using Machine.Specifications;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using Machine.VSTestAdapter.Discovery;

namespace Machine.VSTestAdapter.Specs.TestAdapter
{
    public class When_discovering_a_single_assembly_that_is_not_an_mspec_project : WithFakes
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

            // mock - ensure when the discoverer tests if this is NOT a valid mspec source location that we return FALSE
            discoverer.WhenToldTo(x => x.SourceDirectoryContainsMSpec(Param.IsAny<string>()))
                .Return(false);

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

        private It should_not_send_an_informational_notification_of_intent_to_interrogate_the_source = () =>
        {
            logger.WasNotToldTo(x => x.SendMessage(TestMessageLevel.Informational, string.Format(Machine.VSTestAdapter.Strings.DISCOVERER_LOOKINGIN, "source1")));
        };

        private It should_not_attempt_to_enumerate_specs_in_the_assembly = () =>
        {
            discoverer.WasNotToldTo(x => x.EnumerateSpecs(Param.IsAny<string>()));
        };

        private It should_send_an_informational_notification_of_completing_discovery_with_adjusted_count = () =>
        {
            logger.WasToldTo(x => x.SendMessage(TestMessageLevel.Informational, string.Format(Machine.VSTestAdapter.Strings.DISCOVERER_COMPLETE, 0, 1, 0)));
        };
    }
}