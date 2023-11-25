using System.Collections.Generic;
using Entitas;

public class InitDefinitionSystem : IInitializeSystem
{
    public InitDefinitionSystem(Contexts contexts)
    {
        
    }
    
    public void Initialize()
    {
        GlobalDefinition.Init(new Dictionary<string, Symbol>()
        {
            {"VAL_TEST", new Symbol("Hello World!")}
        });
    }
}
