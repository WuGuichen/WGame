using CrashKonijn.Goap.Configs.Interfaces;
using UnityEngine;

namespace CrashKonijn.Goap.Behaviours
{
    public abstract class GoapSetFactoryBase : MonoBehaviour
    {
        public abstract IGoapSetConfig Create();
    }
}