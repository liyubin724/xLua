using UnityEngine;

namespace LuaEngine
{
    public abstract class ALuaScriptLoader
    {
        public byte[] LoadScript(ref string scriptPath)
        {
            byte[] scriptBytes = ReadScriptBytes(scriptPath, out scriptPath);
            if (scriptBytes == null || scriptBytes.Length == 0)
            {
                Debug.LogError($"load luaScript failed.scriptPath = {scriptPath}");
                return null;
            }

            return scriptBytes;
        }

        protected abstract byte[] ReadScriptBytes(string path, out string fullPath);
    }
}

