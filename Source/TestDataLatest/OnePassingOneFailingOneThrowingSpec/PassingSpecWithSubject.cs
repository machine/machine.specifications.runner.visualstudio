using Machine.Specifications;

namespace OnePassingOneFailingOneThrowingSpec
{
    [Subject("ATESTSUBJECT")]
    public class PassingSpecWithSubject
    {
        private static SUT sut;
        private static int a, b, result;
        private static int expectedResult;

        Establish context = () =>
            {
                a = 1;
                b = 1;
                expectedResult = a + b;
                sut = new SUT();
            };

        Because of = () =>
            {
                result = sut.Add(a, b);
            };

        It should_add_the_numbers_correctly = () =>
            {
                result.ShouldEqual(expectedResult);
            };
    }

}