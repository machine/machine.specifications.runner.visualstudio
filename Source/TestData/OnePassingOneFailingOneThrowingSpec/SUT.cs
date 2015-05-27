using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePassingOneFailingOneThrowingSpec
{
    public class SUT
    {
        /// <summary>
        /// Method that acts correctly
        /// </summary>
        public int Add(int a, int b)
        {
            return a + b;
        }

        /// <summary>
        /// Method that does not act correctly
        /// </summary>
        public int Subtract(int a, int b)
        {
            return Add(a, b);
        }

        /// <summary>
        /// Method that will throw an exception
        /// </summary>
        public int Divide(int a, int b)
        {
            b = 0;
            return a / b;
        }
    }
}