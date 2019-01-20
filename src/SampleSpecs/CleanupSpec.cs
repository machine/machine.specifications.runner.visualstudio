using Machine.Specifications;

namespace SampleSpecs
{
    public class CleanupSpec
    {
        static int cleanup_count;

        Because of = () => { };

        Cleanup after = () =>
            cleanup_count++;

        It should_not_increment_cleanup = () =>
            cleanup_count.ShouldEqual(0);

        It should_have_no_cleanups = () =>
            cleanup_count.ShouldEqual(0);
    }
}
