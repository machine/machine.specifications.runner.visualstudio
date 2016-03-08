using Machine.Fakes;
using Machine.Specifications;

namespace SampleSpecs
{
    /// <summary>
    /// Machine.Fakes has a binding redirect for NSubstitute
    /// </summary>
    public class AssemblyBindingSampleSpec : WithFakes
    {
        public interface ITester
        {
            bool Bla();
        }

        static bool Result;


        Establish context = () => {
            The<ITester>()
                .WhenToldTo(t => t.Bla())
                .Return(true);
        };

        Because of = () => {
            Result = The<ITester>().Bla();
        };


        It should_be_true = () => {
            Result.ShouldBeTrue();
        };
        

    }
}
