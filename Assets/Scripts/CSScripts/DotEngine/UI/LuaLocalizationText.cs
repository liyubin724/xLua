﻿using DotEngine.Lua;
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
                    envMgr.OnLanguageChanged -= OnLanguageChanged;
                    envMgr.OnLanguageChanged += OnLanguageChanged;

                    text = envMgr.GetLocalizationText(localizationName);
                }
            }

            base.Awake();
        }

        private void OnLanguageChanged()
        {
            LuaEnvManager envMgr = LuaEnvManager.GetInstance();
            if (envMgr != null)
            {
                text = envMgr.GetLocalizationText(localizationName);
            }
        }

        protected override void OnDestroy()
        {
            LuaEnvManager envMgr = LuaEnvManager.GetInstance();
            if (envMgr != null)
            {
                envMgr.OnLanguageChanged -= OnLanguageChanged;
            }

            base.OnDestroy();
        }
    }
}
