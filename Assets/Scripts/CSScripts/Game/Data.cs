using XLua;
using DotEngine.Lua;

namespace KingOne.Match3.LuaDatas{
    public class HttpData : BaseData {
        public HttpData(int reference, LuaEnv luaenv) : base(reference, luaenv){}

        public string stringValue{
            get {
                return Get<string>("stringValue");
            }
            set {
                Set("stringValue",value);
            }
        }

        public short shortValue{
            get {
                return Get<short>("shortValue");
            }
            set {
                Set("shortValue",value);
            }
        }

        public int intValue{
            get {
                return Get<int>("intValue");
            }
            set {
                Set("intValue",value);
            }
        }

        public bool boolValue{
            get {
                return Get<bool>("boolValue");
            }
            set {
                Set("boolValue",value);
            }
        }

        public long longValue{
            get {
                return Get<long>("longValue");
            }
            set {
                Set("longValue",value);
            }
        }

        public byte byteValue{
            get {
                return Get<byte>("byteValue");
            }
            set {
                Set("byteValue",value);
            }
        }

        public UserType uType{
            get {
                return Get<UserType>("uType");
            }
            set {
                Set("uType",value);
            }
        }

        public LuaList<string> stringList{
            get {
                return Get<LuaList<string>>("stringList");
            }
            set {
                Set("stringList",value);
            }
        }

        public UserData userValue{
            get {
                return Get<UserData>("userValue");
            }
            set {
                Set("userValue",value);
            }
        }

        public LuaList<UserData> userList{
            get {
                return Get<LuaList<UserData>>("userList");
            }
            set {
                Set("userList",value);
            }
        }
    }
    public class UserData : BaseData {
        public UserData(int reference, LuaEnv luaenv) : base(reference, luaenv){}

        public int intValue{
            get {
                return Get<int>("intValue");
            }
            set {
                Set("intValue",value);
            }
        }
    }
    public class BaseData : LuaTable {
        public BaseData(int reference, LuaEnv luaenv) : base(reference, luaenv){}

        public int guid{
            get {
                return Get<int>("guid");
            }
            set {
                Set("guid",value);
            }
        }
    }
}
