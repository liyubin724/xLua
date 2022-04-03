using System;
using UnityEngine;
using XLua;
using SystemObject = System.Object;

namespace DotEngine.Lua
{
    public class LuaBinderBehaviour : MonoBehaviour
    {
        [SerializeField]
        private LuaBinder m_Binder = new LuaBinder();

        public LuaTable Table { get; private set; }

        private bool m_IsInited = false;

        public LuaEnv Env
        {
            get
            {
                LuaEnvManager mgr = LuaEnvManager.GetInstance();
                if (mgr != null && mgr.IsValid)
                {
                    return mgr.Env;
                }
                return null;
            }
        }

        public bool IsValid()
        {
            if (m_IsInited && Env != null && Table != null)
            {
                return true;
            }

            return false;
        }

        public void InitBinder()
        {
            if (!m_IsInited)
            {
                m_IsInited = true;
                Table = m_Binder.GetInstance();

                OnInitFinished();
            }
        }

        protected virtual void OnInitFinished()
        {
            SetValue("gameObject", gameObject);
            SetValue("transform", transform);
        }

        protected virtual void Awake()
        {
            InitBinder();

            CallAction(LuaDefine.AWAKE_FUNCTION_NAME);
        }

        protected virtual void Start()
        {
            CallAction(LuaDefine.START_FUNCTION_NAME);
        }

        protected virtual void OnEnable()
        {
            CallAction(LuaDefine.ENABLE_FUNCTION_NAME);
        }

        protected virtual void OnDisable()
        {
            CallAction(LuaDefine.DISABLE_FUNCTION_NAME);
        }

        protected virtual void OnDestroy()
        {
            if (IsValid())
            {
                CallAction(LuaDefine.DESTROY_FUNCTION_NAME);

                Table.Dispose();
            }

            Table = null;
            m_IsInited = false;
        }

        public void SetValue<T>(string name, T value)
        {
            if (IsValid())
            {
                Table.Set(name, value);
            }
        }

        public void CallActionWith(string funcName, params LuaParam[] values)
        {
            if(!IsValid())
            {
                return;
            }

            if (values == null || values.Length == 0)
            {
                Action<LuaTable> action = Table.Get<Action<LuaTable>>(funcName);
                action?.Invoke(Table);
            }
            else
            {
                LuaFunction func = Table.Get<LuaFunction>(funcName);
                if (func == null)
                {
                    return;
                }
                SystemObject[] paramValues = new SystemObject[values.Length + 1];
                paramValues[0] = Table;
                for (int i = 0; i < values.Length; ++i)
                {
                    paramValues[i + 1] = values[i].GetValue();
                }
                func.ActionParams(paramValues);
                func.Dispose();
            }
        }

        public void CallActionWith(string funcName, params SystemObject[] values)
        {
            if (!IsValid())
            {
                return;
            }

            if (values == null || values.Length == 0)
            {
                Action<LuaTable> action = Table.Get<Action<LuaTable>>(funcName);
                action?.Invoke(Table);
            }
            else
            {
                LuaFunction func = Table.Get<LuaFunction>(funcName);
                if(func == null)
                {
                    return;
                }
                SystemObject[] paramValues = new SystemObject[values.Length + 1];
                paramValues[0] = Table;
                Array.Copy(values, 0, paramValues, 1, values.Length);
                func.ActionParams(paramValues);
                func.Dispose();
            }
        }

        public void CallAction(string funcName)
        {
            if (!IsValid())
            {
                return;
            }

            Action<LuaTable> action = Table.Get<Action<LuaTable>>(funcName);
            action?.Invoke(Table);
        }

        public void CallAction<T>(string funcName, T value)
        {
            if (!IsValid())
            {
                return;
            }

            Action<LuaTable, T> action = Table.Get<Action<LuaTable, T>>(funcName);
            action?.Invoke(Table, value);
        }

        public void CallAction<T1, T2>(string funcName, T1 value1, T2 value2)
        {
            if (!IsValid())
            {
                return;
            }

            Action<LuaTable, T1, T2> action = Table.Get<Action<LuaTable, T1, T2>>(funcName);
            action?.Invoke(Table, value1, value2);
        }

        public R CallFunc<R>(string funcName)
        {
            if (IsValid())
            {
                Func<LuaTable, R> func = Table.Get<Func<LuaTable, R>>(funcName);
                return func(Table);
            }
            return default(R);
        }

        public R CallFunc<T, R>(string funcName, T value)
        {
            if (IsValid())
            {
                Func<LuaTable, T, R> func = Table.Get<Func<LuaTable, T, R>>(funcName);
                return func(Table, value);
            }
            return default(R);
        }

        public R CallFunc<T1, T2, R>(string funcName, T1 value1, T2 value2)
        {
            if (IsValid())
            {
                Func<LuaTable, T1, T2, R> func = Table.Get<Func<LuaTable, T1, T2, R>>(funcName);
                return func(Table, value1, value2);
            }
            return default(R);
        }
    }
}
