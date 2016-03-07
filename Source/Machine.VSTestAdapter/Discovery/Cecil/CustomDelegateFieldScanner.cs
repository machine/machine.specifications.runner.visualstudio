using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine.VSTestAdapter.Discovery.Cecil
{
    public class CustomDelegateFieldScanner : IDelegateFieldScanner
    {
        public bool ProcessFieldDefinition(Mono.Cecil.FieldDefinition fieldToProcess)
        {
            TypeDefinition typeDefinition = fieldToProcess.FieldType as TypeDefinition;
            if (typeDefinition != null)
            {
                IEnumerable<CustomAttribute> fieldsWithDelgateAttributes = typeDefinition.CustomAttributes.Where(x => x.AttributeType.FullName == "Machine.Specifications.DelegateUsageAttribute");

                if (fieldsWithDelgateAttributes.Count() > 0)
                {
                    bool hasAssertAttributes = fieldsWithDelgateAttributes.Any(x => x.ConstructorArguments.Any(y => y.Type.FullName == "Machine.Specifications.DelegateUsage" && (int)y.Value == 3));
                    return hasAssertAttributes;
                }

                fieldsWithDelgateAttributes = typeDefinition.CustomAttributes.Where(x => x.AttributeType.FullName == "Machine.Specifications.AssertDelegateAttribute");
                if (fieldsWithDelgateAttributes.Count() > 0)
                {
                    return true;
                }

            }
            return false;
        }
    }
}
