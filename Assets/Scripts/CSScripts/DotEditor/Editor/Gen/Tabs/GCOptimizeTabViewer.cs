using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static DotEditor.Lua.GenSelectionWindow;

namespace DotEditor.Lua
{
    internal class GCOptimizeTabViewer : GenTabViewer
    {
        private Vector2 scrollPos = Vector2.zero;
        public GCOptimizeTabViewer(GenConfig config, List<AssemblyTypeData> data) : base(config, data)
        {
        }

        protected internal override void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox);
                {
                    EGUILayout.DrawBoxHeader("Assembly Type List", GUILayout.ExpandWidth(true));
                    foreach (var typeData in assemblyTypes)
                    {
                        if(!typeData.IsStruct() && !typeData.IsEnum())
                        {
                            continue;
                        }
                        string fullName = typeData.Type.FullName;
                        if (!string.IsNullOrEmpty(searchText) && fullName.ToLower().IndexOf(searchText.ToLower()) < 0)
                        {
                            continue;
                        }
                        if (genConfig.LuaCallCSharpTypes.IndexOf(fullName)<0 && genConfig.CSharpCallLuaTypes.IndexOf(fullName)<0)
                        {
                            continue;
                        }
                        
                        bool isSelected = genConfig.GCOptimizeTypes.IndexOf(fullName) >= 0;
                        bool tempIsSelected = EditorGUILayout.ToggleLeft(typeData.Type.FullName, isSelected);
                        if (tempIsSelected != isSelected)
                        {
                            if (tempIsSelected)
                            {
                                genConfig.GCOptimizeTypes.Add(fullName);
                            }
                            else
                            {
                                genConfig.GCOptimizeTypes.Remove(fullName);
                            }
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }
    }
}
