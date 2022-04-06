using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Lua
{
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

        private List<GenAssemblyInfo> m_AssemblyInfos = new List<GenAssemblyInfo>();
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
            "log4net",
            "Unity.Legacy.NRefactory",
            "Unity.Plastic.Antlr3.Runtime",
            "Unity.Plastic.Newtonsoft.Json",
            "UnityEngine.ClothModule",
            "UnityEngine.IMGUIModule",
            "UnityEngine.TerrainModule",
            "UnityEngine.TerrainPhysicsModule",
        };
        private List<string> m_IgnoreNSNames = new List<string>()
        {
        };
        private List<string> m_IgnoreNSContainNames = new List<string>()
        {
            "Editor",
            "Test",
            "Tutorial",
            "Microsoft",
            "Mono"
        };

        public Action ClosedCallback { get; set; }

        private void OnEnable()
        {
            List<Assembly> assemblies = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                         let assemblyName = assembly.GetName().Name
                                         where assemblyName.IndexOf("Editor") < 0 
                                                && assemblyName.IndexOf("Test") <0
                                                && m_IgnoreAssemblyNames.IndexOf(assemblyName) < 0
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

                GenAssemblyInfo assemblyInfo = new GenAssemblyInfo();
                assemblyInfo.AssemblyName = assembly.GetName().Name;
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
                        assemblyInfo.NameSpaceNames.Add(nsName);
                    }
                }

                assemblyInfo.NameSpaceNames.Sort((item1, item2) =>
                {
                    return item1.CompareTo(item2);
                });
                m_AssemblyInfos.Add(assemblyInfo);
            }
        }

        private Vector2 scrollPos = Vector2.zero;
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox);
            {
                foreach (var assemblyInfo in m_AssemblyInfos)
                {
                    Rect assemblyRect = GUILayoutUtility.GetRect(new GUIContent(assemblyInfo.AssemblyName), EGUIStyles.BoxedHeaderStyle, GUILayout.ExpandWidth(true));
                    EditorGUI.LabelField(assemblyRect, GUIContent.none, EGUIStyles.BoxedHeaderStyle);

                    GenAssemblyInfo selectedAssemblyInfo = null;
                    bool assemblyIsSelected = false;
                    if (m_GenConfig.SelectedAssemblyInfoDic.TryGetValue(assemblyInfo.AssemblyName, out selectedAssemblyInfo))
                    {
                        assemblyIsSelected = true;
                    }
                    bool tempAssemblyIsSelected = EditorGUI.ToggleLeft(assemblyRect, assemblyInfo.AssemblyName, assemblyIsSelected);
                    if (tempAssemblyIsSelected != assemblyIsSelected)
                    {
                        if (tempAssemblyIsSelected)
                        {
                            selectedAssemblyInfo = new GenAssemblyInfo()
                            {
                                AssemblyName = assemblyInfo.AssemblyName
                            };
                            m_GenConfig.SelectedAssemblyInfoDic.Add(assemblyInfo.AssemblyName, selectedAssemblyInfo);
                        }
                        else
                        {
                            m_GenConfig.SelectedAssemblyInfoDic.Remove(assemblyInfo.AssemblyName);
                            selectedAssemblyInfo = null;
                        }
                    }

                    foreach (var nsName in assemblyInfo.NameSpaceNames)
                    {
                        Rect nsRect = GUILayoutUtility.GetRect(new GUIContent(nsName), EGUIStyles.BoxedHeaderStyle, GUILayout.ExpandWidth(true));
                        EditorGUI.LabelField(nsRect, GUIContent.none, EGUIStyles.BoxedHeaderStyle);
                        nsRect.x += 40;
                        nsRect.width -= 40;
                        bool nsIsSelected = selectedAssemblyInfo != null && selectedAssemblyInfo.NameSpaceNames.IndexOf(nsName) >= 0;
                        bool tempNSIsSelected = EditorGUI.ToggleLeft(nsRect, nsName, nsIsSelected);
                        if (tempNSIsSelected != nsIsSelected)
                        {
                            if (tempNSIsSelected)
                            {
                                if (selectedAssemblyInfo == null)
                                {
                                    selectedAssemblyInfo = new GenAssemblyInfo()
                                    {
                                        AssemblyName = assemblyInfo.AssemblyName
                                    };
                                    m_GenConfig.SelectedAssemblyInfoDic.Add(assemblyInfo.AssemblyName, selectedAssemblyInfo);
                                }
                                selectedAssemblyInfo.NameSpaceNames.Add(nsName);
                            }
                            else
                            {
                                if (selectedAssemblyInfo != null)
                                {
                                    selectedAssemblyInfo.NameSpaceNames.Remove(nsName);
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
