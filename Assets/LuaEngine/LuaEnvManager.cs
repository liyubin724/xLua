using System;
using UnityEngine;
using XLua;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

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

        public bool IsRunning { get; private set; } = false;
        
        public LuaTable BridgeTable { get; private set; } = null;

        private LuaEnvBehaviour m_EnvBehaviour = null;
        private Action<float, float> m_UpdateAction = null;
        private Action<float, float> m_LateUpdateAction = null;

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

            string luaDir = $"{Application.dataPath}/{LuaDefine.SCRIPT_ASSET_DIR}";
            ALuaScriptLoader scriptLoader = new FileScriptLoader(luaDir);
            Env.AddLoader(scriptLoader.LoadScript);
        }

        public void Startup(string startupScriptPath)
        {
            if(IsRunning)
            {
                return;
            }

            IsRunning = true;

            var envGameObject = new GameObject("Lua Env Behaviour");
            m_EnvBehaviour = envGameObject.AddComponent<LuaEnvBehaviour>();
            UnityObject.DontDestroyOnLoad(envGameObject);

            BridgeTable = RequireAndInstanceLocalTable(startupScriptPath);
            if (BridgeTable != null)
            {
                Action startAction = BridgeTable.Get<Action>(LuaDefine.START_FUNCTION_NAME);
                startAction?.Invoke();

                m_UpdateAction = BridgeTable.Get<Action<float,float>>(LuaDefine.UPDATE_FUNCTION_NAME);
                m_LateUpdateAction = BridgeTable.Get<Action<float, float>>(LuaDefine.LATEUPDATE_FUNCTION_NAME);
            }
        }

        public void Update(float deltaTime,float unscaleDeltaTime)
        {
            if (IsValid)
            {
                Env.Tick();

                m_UpdateAction?.Invoke(deltaTime, unscaleDeltaTime);
            }
        }

        public void LateUpdate(float deltaTime,float unscaleDeltaTime)
        {
            if(IsValid)
            {
                m_LateUpdateAction?.Invoke(deltaTime, unscaleDeltaTime);
            }
        }

        public void Shuntdown()
        {
            if(!IsRunning)
            {
                return;
            }

            IsRunning = false;
            m_UpdateAction = null;
            m_LateUpdateAction = null;

            UnityObject.Destroy(m_EnvBehaviour.gameObject);
            m_EnvBehaviour = null;

            if(BridgeTable!=null)
            {
                LuaFunction destroyLuaFunc = BridgeTable.Get<LuaFunction>(LuaDefine.DESTROY_FUNCTION_NAME);
                destroyLuaFunc?.Call();
                destroyLuaFunc.Dispose();

                BridgeTable.Dispose();
                BridgeTable = null;
            }
        }

        public void FullGC()
        {
            if (IsValid)
            {
                Env.FullGc();
            }
        }

        public void Require(string scriptPath)
        {
            if(string.IsNullOrEmpty(scriptPath))
            {
                throw new ArgumentNullException();
            }
            if(!IsValid)
            {
                throw new LuaEnvInvalidException();
            }

            Env.DoString(string.Format(LuaDefine.REQUIRE_SCRIPT_FORMAT,scriptPath));
        }

        public LuaTable RequireAndGetGlobalTable(string scriptPath)
        {
            if (string.IsNullOrEmpty(scriptPath))
            {
                throw new ArgumentNullException();
            }
            if (!IsValid)
            {
                throw new LuaEnvInvalidException();
            }

            string name = GetScriptName(scriptPath);
            LuaTable table = Global.Get<LuaTable>(name);
            if (table == null)
            {
                Env.DoString(string.Format(LuaDefine.REQUIRE_SCRIPT_FORMAT, scriptPath));
                table = Global.Get<LuaTable>(name);
            }
            return table;
        }

        public LuaTable RequireAndGetLocalTable(string scriptPath)
        {
            if (string.IsNullOrEmpty(scriptPath))
            {
                throw new ArgumentNullException();
            }
            if (!IsValid)
            {
                throw new LuaEnvInvalidException();
            }

            SystemObject[] values = Env.DoString(string.Format(LuaDefine.REQUIRE_SCRIPT_FORMAT, scriptPath));
            if (values == null || values.Length == 0)
            {
                Debug.LogError("LuaEnvManager::RequireAndGetLocalTable->value is null"); 
                return null;
            }
            return values[0] as LuaTable;
        }

        public LuaTable RequireAndInstanceLocalTable(string scriptPath)
        {
            LuaTable luaClass = RequireAndGetLocalTable(scriptPath);
            if (luaClass == null)
            {
                Debug.LogError("LuaEnvManager::RequireAndInstanceLocalTable->luaClass is null");
                return null;
            }

            LuaFunction luaFunc = luaClass.Get<LuaFunction>(LuaDefine.CONSTRUCTOR_FUNCTION_NAME);
            SystemObject[] results = luaFunc.Call();
            if (results == null || results.Length == 0)
            {
                Debug.LogError("LuaEnvManager::RequireAndGetLocalTable->value is null");
                return null;
            }

            return results[0] as LuaTable;
        }

        public LuaTable RequireAndInstanceLocalTable(string scriptPath,params SystemObject[] paramValues)
        {
            LuaTable luaClass = RequireAndGetLocalTable(scriptPath);
            if(luaClass == null)
            {
                Debug.LogError("LuaEnvManager::RequireAndInstanceLocalTable->luaClass is null");
                return null;
            }
            LuaFunction luaFunc = luaClass.Get<LuaFunction>(LuaDefine.CONSTRUCTOR_FUNCTION_NAME);
            SystemObject[] results = luaFunc.Call(paramValues);
            if(results == null || results.Length == 0)
            {
                Debug.LogError("LuaEnvManager::RequireAndGetLocalTable->value is null");
                return null;
            }

            return results[0] as LuaTable;
        }

        private string GetScriptName(string scriptPath)
        {
            string name = scriptPath;
            int index = name.LastIndexOf("/");
            if (index > 0)
            {
                name = name.Substring(index + 1);
            }
            return name;
        }

        private void OnDestroy()
        {
            if(IsRunning)
            {
                Shuntdown();
            }

            Env?.FullGc();
            Env?.Dispose();
            Env = null;
        }
    }
}
