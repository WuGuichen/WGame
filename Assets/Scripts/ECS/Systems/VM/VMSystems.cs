public class VMSystems : Feature
{
    public VMSystems(Contexts contexts)
    {
        Add(new InitDefinitionSystem(contexts));
        Add(new ProcessCodeStringSystem(contexts));
    }
}
