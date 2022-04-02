using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace LuaEngine
{
    [Serializable]
    public class LuaBinder
    {
        [SerializeField]
        private string scriptPath = null;
        [SerializeField]
        private List<LuaParam> paramValues = new List<LuaParam>();

        public LuaBinder()
        { }

        public LuaBinder(string scriptPath)
        {
            this.scriptPath = scriptPath;
        }

        public LuaTable GetInstance()
        {
            LuaEnvManager envManager = LuaEnvManager.GetInstance();
            if (envManager == null)
            {
                return null;
            }

            if (paramValues.Count == 0)
            {
                return envManager.RequireAndInstanceLocalTable(scriptPath);
            }
            else
            {
                return envManager.RequireAndInstanceLocalTable(scriptPath, paramValues.ToArray());
            }
        }
    }
}
