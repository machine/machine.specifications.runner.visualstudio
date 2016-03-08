using Machine.Specifications;

namespace SampleSpecs
{
    [AssertDelegate]
    public delegate void They();

    [ActDelegate]
    public delegate void WhenDoing();

    public class CustomActAssertDelegateSpec
    {
        private static string a;
        private static string b;

        private static int resultA;
        private static int resultB;

        private Establish context = () =>
        {
            a = "foo";
            b = "foo";
        };

        private WhenDoing of = () =>
        {
            resultA = a.GetHashCode();
            resultB = b.GetHashCode();
        };
         
        private They should_have_the_same_hash_code = () => resultA.ShouldEqual(resultB);
    }
}