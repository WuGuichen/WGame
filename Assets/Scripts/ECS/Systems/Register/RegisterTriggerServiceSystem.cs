// using Entitas;
//
// public class RegisterTriggerServiceSystem : IInitializeSystem
// {
//     private readonly ITriggerService _triggerService;
//     private readonly MetaContext _metaContext;
//     
//     public RegisterTriggerServiceSystem(Contexts contexts, ITriggerService service)
//     {
//         _metaContext = contexts.meta;
//         _triggerService = service;
//     }
//     
//     public void Initialize()
//     {
//         _metaContext.ReplaceTriggerService(_triggerService);
//     }
// }
