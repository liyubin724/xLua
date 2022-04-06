using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace DotEditor.Lua
{
    [Serializable]
    public class GenAssemblyInfo
    {
        public string AssemblyName;
        public List<string> NameSpaceNames = new List<string>();
    }

    public class GenConfig : ScriptableObject,ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<GenAssemblyInfo> m_SelectedAssemblyInfos = new List<GenAssemblyInfo>();
        public Dictionary<string, GenAssemblyInfo> SelectedAssemblyInfoDic = new Dictionary<string, GenAssemblyInfo>();

        public List<string> LuaCallCSharpTypes = new List<string>();
        public List<string> CSharpCallLuaTypes = new List<string>();
        public List<string> GCOptimizeTypes = new List<string>();

        public List<string> LuaCallCSharpGenericTypes = new List<string>();
        public List<string> CSharpCallLuaGenericTypes = new List<string>();

        public List<string> blackDatas = new List<string>();

        private static string GEN_CONFIG_ASSET_PATH = "Assets/xlua_gen_config.asset";
        public static GenConfig GetConfig(bool createIfNotExist = true)
        {
            GenConfig genConfig = AssetDatabase.LoadAssetAtPath<GenConfig>(GEN_CONFIG_ASSET_PATH);
            if (genConfig == null && createIfNotExist)
            {
                genConfig = ScriptableObject.CreateInstance<GenConfig>();
                AssetDatabase.CreateAsset(genConfig, GEN_CONFIG_ASSET_PATH);
                AssetDatabase.ImportAsset(GEN_CONFIG_ASSET_PATH);
            }
            return genConfig;
        }

        public void OnBeforeSerialize()
        {
            m_SelectedAssemblyInfos.Clear();
            m_SelectedAssemblyInfos.AddRange(SelectedAssemblyInfoDic.Values);
        }

        public void OnAfterDeserialize()
        {
            SelectedAssemblyInfoDic.Clear();
            foreach(var info in m_SelectedAssemblyInfos)
            {
                SelectedAssemblyInfoDic.Add(info.AssemblyName, info);
            }
        }
    }
}
