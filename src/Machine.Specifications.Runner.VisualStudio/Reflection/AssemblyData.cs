using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Machine.VSTestAdapter.Reflection
{
    public class AssemblyData
    {
        private ReadOnlyCollection<TypeData> types;

        public static AssemblyData Read(string assembly)
        {

        }

        public ReadOnlyCollection<TypeData> Types
        {
            get
            {
                if (types != null)
                {
                    return types;
                }


            }
        }
    }
}
