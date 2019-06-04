using Machine.Specifications;

namespace SampleSpecs
{
    public class Parent
    {
        public class NestedSpec
        {
            It should_remember_that_true_is_true = () =>
                true.ShouldBeTrue();
        }
    }
}
