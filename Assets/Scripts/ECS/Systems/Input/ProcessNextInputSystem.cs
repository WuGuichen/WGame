using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class ProcessNextInputSystem : IExecuteSystem
{
    private readonly InputContext _inputContext;
    public ProcessNextInputSystem(Contexts contexts)
    {
        _inputContext = contexts.input;
    }

    public void Execute()
    {
        
    }
}
