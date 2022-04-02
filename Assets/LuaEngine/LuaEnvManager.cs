using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace LuaEngine
{
    public class LuaEnvManager
    {
        private static LuaEnvManager manager = null;

        public static LuaEnvManager CreateInstance()
        {
            if (manager == null)
            {
                manager = new LuaEnvManager();
                manager.OnInitialize();
            }
            return manager;
        }

        public static LuaEnvManager GetInstance()
        {
            return manager;
        }

        public static void DestroyInstance()
        {
            if (manager != null)
            {
                manager.OnDestroy();
                manager = null;
            }
        }

        public LuaEnv Env { get; private set; } = null;

        public bool IsValid
        {
            get
            {
                return Env != null && Env.IsValid();
            }
        }

        public LuaTable Global
        {
            get
            {
                if (IsValid)
                {
                    return Env.Global;
                }
                return null;
            }
        }

        private LuaEnvManager()
        {
        }

        private void OnInitialize()
        {
            Env = new LuaEnv();
            Env.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
            Env.AddBuildin("pb", XLua.LuaDLL.Lua.LoadLuaProfobuf);

#if DEBUG
            Global.Set("isDebug", true);
#else
            Global.Set("isDebug", false);
#endif

            ALuaScriptLoader scriptLoader = new FileScriptLoader(Application.dataPath + "/Scripts/LuaScripts");
            Env.AddLoader(scriptLoader.LoadScript);


        }

        private void OnDestroy()
        {

        }

    }
}
