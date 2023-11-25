using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraService
{
    Transform Root { get; }
    Transform Pivot { get; }
    Transform Camera { get; }
}
