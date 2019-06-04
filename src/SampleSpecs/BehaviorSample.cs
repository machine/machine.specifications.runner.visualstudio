using Machine.Specifications;

namespace SampleSpecs
{
    [Behaviors]
    public class SampleBehavior
    {
        It sample_behavior_test = () =>
            true.ShouldBeTrue();
    }

    public class BehaviorSampleSpec
    {
        Because of = () => { };

        Behaves_like<SampleBehavior> some_behavior;
    }
}
