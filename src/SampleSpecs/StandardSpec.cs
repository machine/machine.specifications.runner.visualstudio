using System;
using System.Linq;
using Machine.Specifications;

namespace SampleSpecs
{

    public class StandardSpec
    {
        Because of = () => {
            
        };

        It should_pass = () => {
            1.ShouldEqual(1);
        
        };

        [Ignore("reason")]
        It should_be_ignored = () => {
        
        };

        It should_fail = () => {
            1.ShouldEqual(2);
        };


        It unhandled_exception = () => {
            throw new NotImplementedException();
        };

        
        #pragma warning disable CS0169

        It not_implemented;
        #pragma warning restore CS0169
    }
}
