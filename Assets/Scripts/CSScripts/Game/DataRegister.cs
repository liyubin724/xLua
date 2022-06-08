
        using XLua;
        using static XLua.ObjectTranslator;
using DotEngine.Lua;

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
            public partial class ObjectTranslator
            {
                static KingOne.Match3.LuaDatas.DataAutoRegister s_data_reg_dumb_obj = new KingOne.Match3.LuaDatas.DataAutoRegister();
                static KingOne.Match3.LuaDatas.DataAutoRegister data_reg_dumb_obj { get { return s_data_reg_dumb_obj; } }
            }
        }

        namespace KingOne.Match3.LuaDatas
        {
            public class DataAutoRegister
            {
                static DataAutoRegister()
                {
                    XLua.LuaEnv.AddIniter(Init);
                }

                static void Init(LuaEnv luaenv, ObjectTranslator translator)
                {
        
                    CheckFunc<HttpData> HttpData_Checker = (RealStatePtr L, int idx) =>{
                        return LuaAPI.lua_isnil(L, idx) || LuaAPI.lua_istable(L, idx) || (LuaAPI.lua_type(L, idx) == LuaTypes.LUA_TUSERDATA && translator.SafeGetCSObj(L, idx) is LuaTable);
                    };
                    translator.RegisterChecker<HttpData>(HttpData_Checker);
                    GetFunc<HttpData> HttpData_Handler = (RealStatePtr L, int idx, out HttpData obj) =>{
                        obj = null;
                        if (!LuaAPI.lua_istable(L, idx)){return;}
                        obj = new HttpData(LuaAPI.luaL_ref(L), translator.luaEnv);
                    };
                    translator.RegisterCaster<HttpData>(HttpData_Handler);
                    CheckFunc<UserData> UserData_Checker = (RealStatePtr L, int idx) =>{
                        return LuaAPI.lua_isnil(L, idx) || LuaAPI.lua_istable(L, idx) || (LuaAPI.lua_type(L, idx) == LuaTypes.LUA_TUSERDATA && translator.SafeGetCSObj(L, idx) is LuaTable);
                    };
                    translator.RegisterChecker<UserData>(UserData_Checker);
                    GetFunc<UserData> UserData_Handler = (RealStatePtr L, int idx, out UserData obj) =>{
                        obj = null;
                        if (!LuaAPI.lua_istable(L, idx)){return;}
                        obj = new UserData(LuaAPI.luaL_ref(L), translator.luaEnv);
                    };
                    translator.RegisterCaster<UserData>(UserData_Handler);
                    CheckFunc<BaseData> BaseData_Checker = (RealStatePtr L, int idx) =>{
                        return LuaAPI.lua_isnil(L, idx) || LuaAPI.lua_istable(L, idx) || (LuaAPI.lua_type(L, idx) == LuaTypes.LUA_TUSERDATA && translator.SafeGetCSObj(L, idx) is LuaTable);
                    };
                    translator.RegisterChecker<BaseData>(BaseData_Checker);
                    GetFunc<BaseData> BaseData_Handler = (RealStatePtr L, int idx, out BaseData obj) =>{
                        obj = null;
                        if (!LuaAPI.lua_istable(L, idx)){return;}
                        obj = new BaseData(LuaAPI.luaL_ref(L), translator.luaEnv);
                    };
                    translator.RegisterCaster<BaseData>(BaseData_Handler);
                    CheckFunc<LuaList<string>> list_string_Checker = (RealStatePtr L, int idx) =>{
                        return LuaAPI.lua_isnil(L, idx) || LuaAPI.lua_istable(L, idx) || (LuaAPI.lua_type(L, idx) == LuaTypes.LUA_TUSERDATA && translator.SafeGetCSObj(L, idx) is LuaTable);
                    };
                    translator.RegisterChecker<LuaList<string>>(list_string_Checker);
                    GetFunc<LuaList<string>> list_string_Handler = (RealStatePtr L, int idx, out LuaList<string> obj) =>{
                        obj = null;
                        if (!LuaAPI.lua_istable(L, idx)){return;}
                        obj = new LuaList<string>(LuaAPI.luaL_ref(L), translator.luaEnv);
                    };
                    translator.RegisterCaster<LuaList<string>>(list_string_Handler);
                    CheckFunc<LuaList<UserData>> list_UserData_Checker = (RealStatePtr L, int idx) =>{
                        return LuaAPI.lua_isnil(L, idx) || LuaAPI.lua_istable(L, idx) || (LuaAPI.lua_type(L, idx) == LuaTypes.LUA_TUSERDATA && translator.SafeGetCSObj(L, idx) is LuaTable);
                    };
                    translator.RegisterChecker<LuaList<UserData>>(list_UserData_Checker);
                    GetFunc<LuaList<UserData>> list_UserData_Handler = (RealStatePtr L, int idx, out LuaList<UserData> obj) =>{
                        obj = null;
                        if (!LuaAPI.lua_istable(L, idx)){return;}
                        obj = new LuaList<UserData>(LuaAPI.luaL_ref(L), translator.luaEnv);
                    };
                    translator.RegisterCaster<LuaList<UserData>>(list_UserData_Handler);
         
                }
            }
        }
        
