using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using SystemObject = System.Object;

namespace LuaEngine
{
    [Serializable]
    public class LuaParamGroup
    {
        public string name;
        public List<LuaParam> paramList = new List<LuaParam>();
    }

    public class LuaObjectBehaviour : LuaBinderBehaviour
    {
        [SerializeField]
        private List<LuaParam> registParams = new List<LuaParam>();
        [SerializeField]
        private List<LuaParamGroup> registParamGroups = new List<LuaParamGroup>();

        protected override void OnInitFinished()
        {
            base.OnInitFinished();

            foreach (var param in registParams)
            {
                RegistParamToTable(Table, param.name, param);
            }

            foreach (var group in registParamGroups)
            {
                if (group != null && !string.IsNullOrEmpty(group.name))
                {
                    LuaTable regTable = Env.NewTable();
                    Table.Set(group.name, regTable);
                    for (int i = 0; i < group.paramList.Count; ++i)
                    {
                        RegistParamToTable(regTable, i + 1, group.paramList[i]);
                    }
                }
                else
                {
                    Debug.LogError("LuaObjectBehaviour::OnInitFinished->the group or the name of group is null")
                }
            }
        }

        private void RegistParamToTable(LuaTable table, string name, LuaParam param)
        {
            if (table != null && !string.IsNullOrEmpty(name) && param != null)
            {
                SystemObject value = GetValueFromParam(param);
                if (value != null)
                {
                    table.Set(name, value);
                }
            }
        }

        private void RegistParamToTable(LuaTable table, int index, LuaParam param)
        {
            if (table != null && param != null)
            {
                SystemObject value = GetValueFromParam(param);
                if (value != null)
                {
                    table.Set(index, value);
                }
            }
        }

        private SystemObject GetValueFromParam(LuaParam paramValue)
        {
            if (paramValue != null)
            {
                SystemObject value = paramValue.GetValue();
                if (value != null)
                {
                    if (paramValue.paramType == LuaParamType.UObject)
                    {
                        if (value.GetType().IsSubclassOf(typeof(LuaBinderBehaviour)))
                        {
                            LuaBinderBehaviour lbBeh = value as LuaBinderBehaviour;
                            lbBeh.InitBinder();

                            value = lbBeh.Table;
                        }
                    }
                }
                return value;
            }
            return null;
        }

    }
}
