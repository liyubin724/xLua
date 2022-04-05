using System;
using UnityEngine;

namespace DotEngine
{
    public class FixedUpdateBehaviour : MonoBehaviour
    {
        public event Action<float, float> FixedUpdateEvent;

        private void FixedUpdate()
        {
            FixedUpdateEvent?.Invoke(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
        }
    }
}
