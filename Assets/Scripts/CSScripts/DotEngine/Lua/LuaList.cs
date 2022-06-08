using System;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace DotEngine.Lua
{
    public static class LuaTableUtility
    {
        private static bool m_IsInit = false;
        public static Action<LuaTable> ClearAction;
        public static Func<LuaTable, object,bool> ContainsValueFunc;
        public static Func<LuaTable, object, int> IndexOfFunc;
        public static Action<LuaTable, int, object> InsertAction;
        public static Func<LuaTable, object, bool> RemoveValueFunc;
        public static Action<LuaTable, int> RemoveAtAction;

        public static void Init()
        {
            if(m_IsInit)
            {
                return;
            }

            m_IsInit = true;
            var envMgr = LuaEnvManager.GetInstance();
            LuaTable tableUtility = envMgr.RequireAndGetLocalTable("DotLua/Utility/TableUtility");
            ClearAction = tableUtility.Get<Action<LuaTable>>("Clear");
            ContainsValueFunc = tableUtility.Get<Func<LuaTable, object, bool>>("ContainsValue");
            IndexOfFunc = tableUtility.Get<Func<LuaTable, object, int>>("IndexOf");
            RemoveValueFunc = tableUtility.Get<Func<LuaTable, object, bool>>("RemoveValue");
            tableUtility.Dispose();

            LuaTable table = envMgr.Global.Get<LuaTable>("table");
            InsertAction = table.Get<Action<LuaTable, int, object>>("insert");
            RemoveAtAction = table.Get<Action<LuaTable, int>>("remove");
            table.Dispose();
        }
    }

    public class LuaList<T> : LuaTable, IList<T> 
    {
        private LuaTable m_CachedData = null;
        public LuaList(LuaTable table):base(table.LuaReference,table.Env)
        {
           
            //m_CachedData = table;
            LuaTableUtility.Init();
        }

        public LuaList(int reference, LuaEnv luaenv) : base(reference, luaenv)
        {
            LuaTableUtility.Init();
        }


        public T this[int index] 
        { 
            get
            {
                return Get<int,T>(index+1);
            }
            set
            {
                Set<int, T>(index+1, value);
            }
        }

        public int Count => Length;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            Set(Length + 1, item);
        }

        public void Clear()
        {
            LuaTableUtility.ClearAction(this);
        }

        public bool Contains(T item)
        {
            return LuaTableUtility.ContainsValueFunc(this, item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            return LuaTableUtility.IndexOfFunc(this, item);
        }

        public void Insert(int index, T item)
        {
            LuaTableUtility.InsertAction(this, index+1, item);
        }

        public bool Remove(T item)
        {
            return LuaTableUtility.RemoveValueFunc(this, item);
        }

        public void RemoveAt(int index)
        {
            LuaTableUtility.RemoveAtAction(this, index+1);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
