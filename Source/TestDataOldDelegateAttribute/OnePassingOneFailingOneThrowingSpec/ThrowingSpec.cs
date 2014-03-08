using Machine.Specifications;

namespace OnePassingOneFailingOneThrowingSpec
{
    public class ThrowingSpec
    {
        private static SUT sut;
        private static int a, b, result;
        private static int expectedResult;

        private Establish context = () =>
        {
            a = 2;
            b = 1;
            expectedResult = a / b;
            sut = new SUT();
        };

        private Because of = () =>
        {
            result = sut.Divide(a, b);
        };

        private It should_divide_the_numbers_correctly = () =>
        {
            result.ShouldEqual(expectedResult);
        };
    }
}