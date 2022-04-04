using DotEngine.Lua;
using UnityEngine;
using UnityEngine.UI;

namespace DotEngine.UI
{
    public class LuaLocalizationText : Text
    {
        public string localizationName = null;

        protected override void Awake()
        {
            if(Application.isPlaying && !string.IsNullOrEmpty(localizationName))
            {
                LuaEnvManager envMgr = LuaEnvManager.GetInstance();
                if(envMgr!=null)
                {
                    text = envMgr.GetLocalizationText(localizationName);
                }
            }
            base.Awake();
        }
    }
}
