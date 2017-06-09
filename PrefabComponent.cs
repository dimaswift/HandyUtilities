using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandyUtilities
{
    public abstract class PrefabComponent<T> : Prefab where T : Component
    {
        public T instanceType { get; protected set; }

        public override void Spawn()
        {
            if (spawned) return;
            base.Spawn();
            instanceType = instance.GetComponent<T>();
        }
    }
}
