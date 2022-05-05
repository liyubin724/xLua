using System;
#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif


namespace XLua
{
    public enum PanicActionType
    {
        Continue = 0,
        Abort = 1,
    }

    public static class LuaEnv_Extension
    {
        private static PanicActionType sm_PanicActionType = PanicActionType.Continue;
        public static void SetCustomPanic(this LuaEnv env,PanicActionType panicActionType)
        {
            sm_PanicActionType = panicActionType;
            LuaAPI.lua_atpanic(env.L, Panic);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        private static int Panic(RealStatePtr L)
        {
            string reason = String.Format("unprotected error in call to Lua API ({0})", LuaAPI.lua_tostring(L, -1));
            UnityEngine.Debug.LogError(reason);
            return (int)sm_PanicActionType;
        }

        public static float GetTotalMemory(this LuaEnv env)
        {
            int memoryInK = LuaAPI.lua_gc(env.L, LuaGCOptions.LUA_GCCOUNT, 0);
            int memoryInB = LuaAPI.lua_gc(env.L, LuaGCOptions.LUA_GCCOUNTB, 0);
            
            return memoryInK + (memoryInB / 1024.0f);
        }

        public static bool IsValid(this LuaEnv luaEnv)
        {
            return luaEnv.L != RealStatePtr.Zero;
        }
    }
}
