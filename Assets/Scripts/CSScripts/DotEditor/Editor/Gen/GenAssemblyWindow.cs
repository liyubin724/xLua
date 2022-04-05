using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Lua
{
    [Serializable]
    public class GenAssemblyData
    {
        public string Name;
        public List<string> NSNames = new List<string>();
    }

    public class GenAssemblyWindow : EditorWindow
    {
        [MenuItem("Game/XLua/1 Gen Assembly Setting", priority = 1)]
        public static GenAssemblyWindow ShowWin()
        {
            var win = GetWindow<GenAssemblyWindow>();
            win.titleContent = new GUIContent("Assembly Setting");
            win.Show();
            return win;
        }

        private List<GenAssemblyData> m_AssemblyDatas = new List<GenAssemblyData>();
        private GenConfig m_GenConfig = null;

        private List<string> m_IgnoreAssemblyNames = new List<string>()
        {
            "nunit.framework",
            "System.Windows.Forms",
            "unityplastic",
            "UnityEngine.WindModule",
            "UnityEngine.XRModule",
            "UnityEngine.VRModule",
            "UnityEngine.TestRunner",
            "UnityEngine.ARModule",
            "System.Drawing",
            "System.Data",
            "netstandard",
            "Mono.Posix",
            "Mono.Security",
            "ExCSS.Unity",
            "log4net"
        };
        private List<string> m_IgnoreNSNames = new List<string>()
        {
        };
        private List<string> m_IgnoreNSContainNames = new List<string>()
        {
            "Editor",
            "Test",
            "Tutorial",
            "Microsoft"
        };

        public Action ClosedCallback { get; set; }

        private void OnEnable()
        {
            List<Assembly> assemblies = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                         where assembly.GetName().Name.IndexOf("Editor") < 0 && m_IgnoreAssemblyNames.IndexOf(assembly.GetName().Name) < 0
                                         select assembly).ToList();
            assemblies.Sort((item1, item2) =>
            {
                return item1.GetName().Name.CompareTo(item2.GetName().Name);
            });

            m_GenConfig = GenConfig.GetConfig();

            List<string> namespaceList = new List<string>();
            foreach (var assembly in assemblies)
            {
                var nsNames = (from type in assembly.GetTypes()
                               where !type.IsNotPublic && !string.IsNullOrEmpty(type.Namespace)
                                       && m_IgnoreNSNames.IndexOf(type.Namespace) < 0
                               select type.Namespace).Distinct().ToList();

                GenAssemblyData assemblyData = new GenAssemblyData();
                assemblyData.Name = assembly.FullName;
                assemblyData.NSNames = new List<string>();
                foreach (var nsName in nsNames)
                {
                    bool isValid = true;
                    foreach (var nsIgnoreNmae in m_IgnoreNSContainNames)
                    {
                        if (nsName.Contains(nsIgnoreNmae))
                        {
                            isValid = false;
                            continue;
                        }
                    }
                    if (isValid)
                    {
                        assemblyData.NSNames.Add(nsName);
                    }
                }

                assemblyData.NSNames.Sort((item1, item2) =>
                {
                    return item1.CompareTo(item2);
                });
                m_AssemblyDatas.Add(assemblyData);
            }
        }

        private Vector2 scrollPos = Vector2.zero;
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox);
            {
                foreach (var assemblyData in m_AssemblyDatas)
                {
                    Rect assemblyRect = GUILayoutUtility.GetRect(new GUIContent(assemblyData.Name), EGUIStyles.BoxedHeaderStyle, GUILayout.ExpandWidth(true));
                    EditorGUI.LabelField(assemblyRect, GUIContent.none, EGUIStyles.BoxedHeaderStyle);
                    bool assemblyIsSelected = m_GenConfig.SelectedAssemblyDic.ContainsKey(assemblyData.Name);
                    bool tempAssemblyIsSelected = EditorGUI.ToggleLeft(assemblyRect, assemblyData.Name, assemblyIsSelected);
                    if (tempAssemblyIsSelected != assemblyIsSelected)
                    {
                        if (tempAssemblyIsSelected)
                        {
                            m_GenConfig.SelectedAssemblyDic.Add(assemblyData.Name, new List<string>());
                        }
                        else
                        {
                            m_GenConfig.SelectedAssemblyDic.Remove(assemblyData.Name);
                        }
                    }

                    foreach (var nsName in assemblyData.NSNames)
                    {
                        Rect nsRect = GUILayoutUtility.GetRect(new GUIContent(nsName), EGUIStyles.BoxedHeaderStyle, GUILayout.ExpandWidth(true));
                        EditorGUI.LabelField(nsRect, GUIContent.none, EGUIStyles.BoxedHeaderStyle);
                        nsRect.x += 40;
                        nsRect.width -= 40;
                        bool nsIsSelected = m_GenConfig.SelectedAssemblyDic.ContainsKey(assemblyData.Name) && m_GenConfig.SelectedAssemblyDic[assemblyData.Name].IndexOf(nsName) >= 0;
                        bool tempNSIsSelected = EditorGUI.ToggleLeft(nsRect, nsName, nsIsSelected);
                        if (tempNSIsSelected != nsIsSelected)
                        {
                            if (tempNSIsSelected)
                            {
                                if (!m_GenConfig.SelectedAssemblyDic.TryGetValue(assemblyData.Name, out var nsNames))
                                {
                                    nsNames = new List<string>();
                                    m_GenConfig.SelectedAssemblyDic.Add(assemblyData.Name, nsNames);
                                }
                                nsNames.Add(nsName);
                            }
                            else
                            {
                                if (m_GenConfig.SelectedAssemblyDic.TryGetValue(assemblyData.Name, out var nsNames))
                                {
                                    nsNames.Remove(nsName);
                                }
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();


            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_GenConfig);
            }
        }

        private void OnDestroy()
        {
            ClosedCallback?.Invoke();
        }

        private void OnLostFocus()
        {
            if (ClosedCallback != null)
            {
                Close();
            }
        }
    }
}
