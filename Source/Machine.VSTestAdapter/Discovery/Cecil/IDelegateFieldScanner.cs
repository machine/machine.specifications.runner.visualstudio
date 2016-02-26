using Mono.Cecil;

namespace Machine.VSTestAdapter.Discovery.Cecil
{
    public interface IDelegateFieldScanner
    {
        bool ProcessFieldDefinition(FieldDefinition fieldToProcess);
    }
}