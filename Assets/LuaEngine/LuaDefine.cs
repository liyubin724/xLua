namespace LuaEngine
{
    public static class LuaDefine
    {
        public const string AWAKE_FUNCTION_NAME = "DoAwake";
        public const string ENABLE_FUNCTION_NAME = "DoEnable";
        public const string START_FUNCTION_NAME = "DoStart";
        public const string UPDATE_FUNCTION_NAME = "DoUpdate";
        public const string LATEUPDATE_FUNCTION_NAME = "DoLateUpdate";
        public const string DISABLE_FUNCTION_NAME = "DoDisable";
        public const string DESTROY_FUNCTION_NAME = "DoDestroy";

        public const string CONSTRUCTOR_FUNCTION_NAME = "new";

        public const string SCRIPT_ASSET_DIR = "Scripts/LuaScripts";
        public const string SCRIPT_EXTENSION = ".txt";

        public const string REQUIRE_SCRIPT_FORMAT = "require(\"{0}\")";
    }
}
