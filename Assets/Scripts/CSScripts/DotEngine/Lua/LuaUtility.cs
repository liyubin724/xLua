namespace DotEngine.Lua
{
    public static class LuaUtility
    {
        public static string GetScriptPath(string scriptAssetPath)
        {
            if (string.IsNullOrEmpty(scriptAssetPath))
            {
                return null;
            }
            return scriptAssetPath.Replace("\\", "/").Replace(LuaDefine.SCRIPT_ASSET_DIR, "").Replace(LuaDefine.SCRIPT_EXTENSION, "");
        }

        public static string GetScriptAssetPath(string scriptPath)
        {
            return $"Assets/{LuaDefine.SCRIPT_ASSET_DIR}{scriptPath}{LuaDefine.SCRIPT_EXTENSION}";
        }
    }
}
