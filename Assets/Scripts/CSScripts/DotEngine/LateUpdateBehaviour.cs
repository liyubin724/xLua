using System;
using UnityEngine;

namespace DotEngine
{
    public class LateUpdateBehaviour : MonoBehaviour
    {
        public event Action<float, float> LateUpdateEvent;

        private void LateUpdate()
        {
            LateUpdateEvent?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);    
        }
    }
}
