using Machine.Specifications;
using System;

namespace CustomDelegateSpec
{
    [Subject(typeof(String), "Equality")]
    public class When_getting_the_hash_code_of_equal_strings_with_custom_assert_delegate
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

        private Because of = () =>
        {
            resultA = a.GetHashCode();
            resultB = b.GetHashCode();
        };
         
        private They should_have_the_same_hash_code = () => resultA.ShouldEqual(resultB);

        private It should_result_in_the_same_hash_code = () => resultA.ShouldEqual(resultB);
    }
}