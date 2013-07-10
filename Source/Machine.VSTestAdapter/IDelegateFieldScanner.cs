using Mono.Cecil;

namespace Machine.VSTestAdapter
{
    public interface IDelegateFieldScanner
    {
        bool ProcessFieldDefinition(FieldDefinition fieldToProcess);
    }
}