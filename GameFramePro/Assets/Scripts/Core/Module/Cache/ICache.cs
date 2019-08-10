using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    public interface ICache
    {
        bool TryCache(object instance);

        bool GetInstanceFromCache(string assKey);
    }
}
