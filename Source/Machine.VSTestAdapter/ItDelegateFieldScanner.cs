namespace Machine.VSTestAdapter
{
    public class ItDelegateFieldScanner : IDelegateFieldScanner
    {
        public bool ProcessFieldDefinition(Mono.Cecil.FieldDefinition fieldToProcess)
        {
            if (fieldToProcess.FieldType.FullName == "Machine.Specifications.It")
            {
                return true;
            }

            return false;
        }
    }
}