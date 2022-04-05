using System;
using UnityEngine;

namespace DotEngine
{
    public class UpdateBehaviour : MonoBehaviour
    {
        public event Action<float, float> UpdateEvent;

        private void Update()
        {
            UpdateEvent?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}
