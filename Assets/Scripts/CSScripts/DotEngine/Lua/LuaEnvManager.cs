using System;
using UnityEngine;
using XLua;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace DotEngine.Lua
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

        private GameObject m_EnvGameObject = null;
        private Action<float, float> m_UpdateAction = null;
        private Action<float, float> m_LateUpdateAction = null;

        private string m_Language = null;
        public string Language
        {
            get
            {
                return m_Language;
            }
        }
        private LuaLocalization m_Localization = new LuaLocalization();

        public event Action OnLanguageChanged;
        public void SetLocalizationText(string language,LuaTable languageTable)
        {
            m_Language = language;
            m_Localization.ChangeLanguage(languageTable);
            OnLanguageChanged?.Invoke();
        }

        public string GetLocalizationText(string locName)
        {
            return m_Localization?.GetText(locName);
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
            Env.SetCustomPanic(PanicActionType.Abort);
#else
            Global.Set("isDebug", false);
            Env.SetCustomPanic(PanicActionType.Continue);
#endif
            string scriptRootDir = string.Empty;
#if UNITY_EDITOR
            scriptRootDir = $"{Application.dataPath}/{LuaDefine.SCRIPT_ASSET_DIR}";
#else
            scriptRootDir = $"{Application.persistentDataPath}/{LuaDefine.SCRIPT_ASSET_DIR}";
#endif
            Global.Set("ScriptRootDir", scriptRootDir);
            ALuaScriptLoader scriptLoader = new FileScriptLoader(scriptRootDir);
            Env.AddLoader(scriptLoader.LoadScript);
        }

        public void Startup(string startupScriptPath)
        {
            if(IsRunning)
            {
                return;
            }

            IsRunning = true;

            m_EnvGameObject = new GameObject("Lua Env Behaviour");
            UnityObject.DontDestroyOnLoad(m_EnvGameObject);

            var updateBehaviour = m_EnvGameObject.AddComponent<UpdateBehaviour>();
            updateBehaviour.UpdateEvent += DoUpdate;

            var lateUpdateBehavoiur = m_EnvGameObject.AddComponent<LateUpdateBehaviour>();
            lateUpdateBehavoiur.LateUpdateEvent += DoLateUpdate;

            BridgeTable = RequireAndGetLocalTable(startupScriptPath);
            if (BridgeTable != null)
            {
                Action startAction = BridgeTable.Get<Action>(LuaDefine.START_FUNCTION_NAME);
                startAction?.Invoke();

                m_UpdateAction = BridgeTable.Get<Action<float,float>>(LuaDefine.UPDATE_FUNCTION_NAME);
                m_LateUpdateAction = BridgeTable.Get<Action<float, float>>(LuaDefine.LATEUPDATE_FUNCTION_NAME);
            }
        }

        private void DoUpdate(float deltaTime,float unscaleDeltaTime)
        {
            if (IsValid)
            {
                Env.Tick();

                m_UpdateAction?.Invoke(deltaTime, unscaleDeltaTime);
            }
        }

        private void DoLateUpdate(float deltaTime,float unscaleDeltaTime)
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

            UnityObject.Destroy(m_EnvGameObject);
            m_EnvGameObject = null;

            m_Localization?.Dispose();
            m_Localization = null;

            if (BridgeTable!=null)
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
