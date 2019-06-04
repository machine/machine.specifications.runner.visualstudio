using Machine.Specifications;

namespace SampleSpecs
{
    [AssertDelegate]
    public delegate void They();

    [ActDelegate]
    public delegate void WhenDoing();

    public class CustomActAssertDelegateSpec
    {
        static string a;
        static string b;
        static int resultA;
        static int resultB;

        Establish context = () =>
        {
            a = "foo";
            b = "foo";
        };

        WhenDoing of = () =>
        {
            resultA = a.GetHashCode();
            resultB = b.GetHashCode();
        };
         
        They should_have_the_same_hash_code = () =>
            resultA.ShouldEqual(resultB);
    }
}
