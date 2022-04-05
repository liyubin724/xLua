using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace DotEditor.Lua
{
    public class GenConfig : ScriptableObject,ISerializationCallbackReceiver
    {
        public Dictionary<string, List<string>> SelectedAssemblyDic = new Dictionary<string, List<string>>();

        [SerializeField]
        private List<string> m_SelectedAssemblyNames = new List<string>();
        [SerializeField]
        private List<List<string>> m_SelectedNSNames = new List<List<string>>();

        public List<string> AssemblyNames = new List<string>();
        
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
            m_SelectedAssemblyNames.Clear();
            m_SelectedNSNames.Clear();
            foreach(var kvp in SelectedAssemblyDic)
            {
                m_SelectedAssemblyNames.Add(kvp.Key);
                List<string> nsList = new List<string>();
                m_SelectedNSNames.Add(nsList);
                foreach(var ns in kvp.Value)
                {
                    nsList.Add(ns);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            for(int i =0;i<m_SelectedAssemblyNames.Count;i++)
            {
                SelectedAssemblyDic.Add(m_SelectedAssemblyNames[i], m_SelectedNSNames[i]);
            }
        }
    }
}
