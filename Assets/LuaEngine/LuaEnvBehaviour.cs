using UnityEngine;

namespace LuaEngine
{
    public class LuaEnvBehaviour : MonoBehaviour
    {
        private void Update()
        {
            LuaEnvManager.GetInstance()?.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}
