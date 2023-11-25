using System.Collections.Generic;
using Entitas;

public class ProcessCodeStringSystem : ReactiveSystem<VMEntity>
{
    public ProcessCodeStringSystem(Contexts contexts) : base(contexts.vM)
    {
        
    }

    protected override ICollector<VMEntity> GetTrigger(IContext<VMEntity> context)
    {
        return context.CreateCollector(VMMatcher.CodeString);
    }

    protected override bool Filter(VMEntity entity)
    {
        return entity.hasCodeString && entity.hasVMService;
    }

    protected override void Execute(List<VMEntity> entities)
    {
        entities.ForEach(DoCode);
    }
    
    protected static void DoCode(VMEntity entity)
    {
        entity.vMService.service.DoString(entity.codeString.code);
    }
}
