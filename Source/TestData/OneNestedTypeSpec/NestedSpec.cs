using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace OneNestedTypeSpec
{
    public class Parent
    {
        class NestedSpec
        {
            It should_remember_that_true_is_true = () =>
            {
                true.ShouldBeTrue();
            };
        }
    }
}
