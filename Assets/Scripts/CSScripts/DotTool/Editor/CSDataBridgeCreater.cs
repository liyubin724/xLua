using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using DotEngine.Core.Extensions;

public class BaseData
{
    public int guid;
}

public class HttpData : BaseData
{
    public string stringValue;
    public short shortValue;
    public int intValue;
    public bool boolValue;
    public long longValue;
    public byte byteValue;

    public UserType uType;

    public List<string> stringList;

    public UserData userValue;
    public List<UserData> userList;

}

public class UserData : BaseData
{
    public int intValue;
}

public class CSDataBridgeCreater
{
    private static Type[] m_DataTypes = new Type[]
    {
        typeof(HttpData),
        typeof(UserData)
    };


    [MenuItem("Test/Bridge Creater")]
    public static void BridgeCreater()
    {
        int indent = 0;

        StringBuilder contentBuilder = new StringBuilder();
        contentBuilder.AppendLine("using XLua;");
        contentBuilder.AppendLine("using DotEngine.Lua;");
        contentBuilder.AppendLine();
        contentBuilder.AppendLine("namespace KingOne.Match3.LuaDatas{");
        {
            indent++;
            {
                Type[] types = GetExportTypes(m_DataTypes,out _);
                foreach (var type in types)
                {
                    WriteClass(contentBuilder, type, indent);
                }
            }
            indent--;
        }
        contentBuilder.AppendLine($"{GetIndent(indent)}}}");

        File.WriteAllText(@"G:\KingSoft\JFSpace\xLua\Assets\Scripts\CSScripts\Game\Data.cs", contentBuilder.ToString());

        #region
        StringBuilder registerBuilder = new StringBuilder();
        registerBuilder.AppendLine(@"
        using XLua;
        using static XLua.ObjectTranslator;

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
        ");
        {
            indent = 5;
            Type[] types = GetExportTypes(m_DataTypes,out var listTypes);
            foreach (var type in types)
            {
                WriteRegisterChecker(registerBuilder, type, indent);
                WriteRegisterCaster(registerBuilder, type, indent);
            }

            foreach (var type in listTypes)
            {
                WriteRegisterListChecker(registerBuilder, type, indent);
                WriteRegisterListCaster(registerBuilder, type, indent);
            }

        }
        registerBuilder.AppendLine(@"         
                }
            }
        }
        ");
        File.WriteAllText(@"G:\KingSoft\JFSpace\xLua\Assets\Scripts\CSScripts\Game\DataRegister.cs", registerBuilder.ToString());
        #endregion
    }

    private static void WriteRegisterListChecker(StringBuilder builder, Type type, int indent)
    {
        Type elementType = type.GetElementTypeInArrayOrList();

        builder.AppendLine($"{GetIndent(indent)}CheckFunc<LuaList<{GetTypeName(elementType)}>> list_{GetTypeName(elementType)}_Checker = (RealStatePtr L, int idx) =>{{");
        {
            indent++;
            {
                builder.AppendLine($"{GetIndent(indent)}return LuaAPI.lua_isnil(L, idx) || LuaAPI.lua_istable(L, idx) || (LuaAPI.lua_type(L, idx) == LuaTypes.LUA_TUSERDATA && translator.SafeGetCSObj(L, idx) is LuaTable);");
            }
            indent--;
        }
        builder.AppendLine($"{GetIndent(indent)}}};");
        builder.AppendLine($"{GetIndent(indent)}translator.RegisterChecker<LuaList<{GetTypeName(elementType)}>>(list_{GetTypeName(elementType)}_Checker);");
    }

    private static void WriteRegisterListCaster(StringBuilder builder, Type type, int indent)
    {
        Type elementType = type.GetElementTypeInArrayOrList();
        builder.AppendLine($"{GetIndent(indent)}GetFunc<LuaList<{GetTypeName(elementType)}>> list_{GetTypeName(elementType)}_Handler = (RealStatePtr L, int idx, out LuaList<{GetTypeName(elementType)}> obj) =>{{");
        {
            indent++;
            { 
                builder.AppendLine($"{GetIndent(indent)}obj = null;");
                builder.AppendLine($"{GetIndent(indent)}if (!LuaAPI.lua_istable(L, idx)){{return;}}");
                builder.AppendLine($"{GetIndent(indent)}obj = new LuaList<{GetTypeName(elementType)}>(LuaAPI.luaL_ref(L), translator.luaEnv);");
            }
            indent--;
        }
        builder.AppendLine($"{GetIndent(indent)}}};");
        builder.AppendLine($"{GetIndent(indent)}translator.RegisterCaster<LuaList<{GetTypeName(elementType)}>>(list_{GetTypeName(elementType)}_Handler);");
    }

    private static void WriteRegisterChecker(StringBuilder builder, Type type,int indent)
    {
        builder.AppendLine($"{GetIndent(indent)}CheckFunc<{type.Name}> {type.Name}_Checker = (RealStatePtr L, int idx) =>{{");
        {
            indent++;
            {
                builder.AppendLine($"{GetIndent(indent)}return LuaAPI.lua_isnil(L, idx) || LuaAPI.lua_istable(L, idx) || (LuaAPI.lua_type(L, idx) == LuaTypes.LUA_TUSERDATA && translator.SafeGetCSObj(L, idx) is LuaTable);");
            }
            indent--;
        }
        builder.AppendLine($"{GetIndent(indent)}}};");
        builder.AppendLine($"{GetIndent(indent)}translator.RegisterChecker<{type.Name}>({type.Name}_Checker);");
    }

    private static void WriteRegisterCaster(StringBuilder builder,Type type,int indent)
    {
        builder.AppendLine($"{GetIndent(indent)}GetFunc<{type.FullName}> {type.Name}_Handler = (RealStatePtr L, int idx, out {type.FullName} obj) =>{{");
        {
            indent++;
            {
                builder.AppendLine($"{GetIndent(indent)}obj = null;");
                builder.AppendLine($"{GetIndent(indent)}if (!LuaAPI.lua_istable(L, idx)){{return;}}");
                builder.AppendLine($"{GetIndent(indent)}obj = new {type.FullName}(LuaAPI.luaL_ref(L), translator.luaEnv);");
            }
            indent--;
        }
        builder.AppendLine($"{GetIndent(indent)}}};");
        builder.AppendLine($"{GetIndent(indent)}translator.RegisterCaster<{type.Name}>({type.Name}_Handler);");
    }

    private static void WriteClass(StringBuilder builder, Type type, int indent)
    {
        string baseTypeName = (type.BaseType == typeof(System.Object)) ? "LuaTable" : type.BaseType.Name;

        builder.AppendLine($"{GetIndent(indent)}public class {type.Name} : {baseTypeName} {{");
        {
            indent++;
            {
                if(type.BaseType == typeof(object))
                {
                    //builder.AppendLine($"{GetIndent(indent)}public {type.Name}(LuaTable table):base(table.LuaReference,table.Env){{}}");
                    builder.AppendLine($"{GetIndent(indent)}public {type.Name}(int reference, LuaEnv luaenv) : base(reference, luaenv){{}}");
                }
                else
                {
                    //builder.AppendLine($"{GetIndent(indent)}public {type.Name}(LuaTable table):base(table){{}}");
                    builder.AppendLine($"{GetIndent(indent)}public {type.Name}(int reference, LuaEnv luaenv) : base(reference, luaenv){{}}");
                }

                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (fields != null && fields.Length > 0)
                {
                    foreach (var field in fields)
                    {
                        builder.AppendLine();

                        if (IsValueType(field.FieldType))
                        {
                            WriteValueField(builder, field, indent);
                        }
                        else
                        {
                            WriteClassField(builder, field, indent);
                        }
                    }
                }
            }
            indent--;
        }
        builder.AppendLine($"{GetIndent(indent)}}}");
    }

    private static void WriteValueField(StringBuilder builder, FieldInfo fieldInfo, int indent)
    {
        builder.AppendLine($"{GetIndent(indent)}public {GetTypeName(fieldInfo.FieldType)} {fieldInfo.Name}{{");
        {
            indent++;
            {
                builder.AppendLine($"{GetIndent(indent)}get {{");
                {
                    indent++;
                    {
                        builder.AppendLine($"{GetIndent(indent)}return Get<{GetTypeName(fieldInfo.FieldType)}>(\"{fieldInfo.Name}\");");
                    }
                    indent--;
                }
                builder.AppendLine($"{GetIndent(indent)}}}");

                builder.AppendLine($"{GetIndent(indent)}set {{");
                {
                    indent++;
                    {
                        builder.AppendLine($"{GetIndent(indent)}Set(\"{fieldInfo.Name}\",value);");
                    }
                    indent--;
                }
                builder.AppendLine($"{GetIndent(indent)}}}");
            }
            indent--;
        }
        builder.AppendLine($"{GetIndent(indent)}}}");
    }

    private static void WriteClassField(StringBuilder builder, FieldInfo fieldInfo, int indent)
    {
        string priFieldName = $"m_{fieldInfo.Name}";
        //builder.AppendLine($"{GetIndent(indent)}private {GetTypeName(fieldInfo.FieldType)} {priFieldName};");

        builder.AppendLine($"{GetIndent(indent)}public {GetTypeName(fieldInfo.FieldType)} {fieldInfo.Name}{{");
        {
            indent++;
            {
                builder.AppendLine($"{GetIndent(indent)}get {{");
                {
                    indent++;
                    {
                        //builder.AppendLine($"{GetIndent(indent)}if({priFieldName} == null){{");
                        //{
                        //    indent++;
                        //    {
                        //        //builder.AppendLine($"{GetIndent(indent)}var t = Get<LuaTable>(\"{fieldInfo.Name}\");");
                        //        //builder.AppendLine($"{GetIndent(indent)}{priFieldName} = new {GetTypeName(fieldInfo.FieldType)}(t);");
                        //    }
                        //    indent--;
                        //}
                        //builder.AppendLine($"{GetIndent(indent)}}}");

                        //builder.AppendLine($"{GetIndent(indent)}return {priFieldName};");
                        builder.AppendLine($"{GetIndent(indent)}return Get<{GetTypeName(fieldInfo.FieldType)}>(\"{fieldInfo.Name}\");");
                    }
                    indent--;
                }
                builder.AppendLine($"{GetIndent(indent)}}}");

                builder.AppendLine($"{GetIndent(indent)}set {{");
                {
                    indent++;
                    {
                        //builder.AppendLine($"{GetIndent(indent)}{priFieldName}?.Dispose();");
                        //builder.AppendLine($"{GetIndent(indent)}{priFieldName} = null;");

                        builder.AppendLine($"{GetIndent(indent)}Set(\"{fieldInfo.Name}\",value);");
                    }
                    indent--;
                }
                builder.AppendLine($"{GetIndent(indent)}}}");
            }
            indent--;
        }
        builder.AppendLine($"{GetIndent(indent)}}}");
    }

    private static bool IsValueType(Type type)
    {
        if (type.IsValueType && type.IsPrimitive)
        {
            return true;
        }
        if (type.IsEnum)
        {
            return true;
        }
        if (type == typeof(string))
        {
            return true;
        }
        return false;
    }

    private static string GetTypeName(Type type)
    {
        if (type == typeof(string))
        {
            return "string";
        }
        else if (type == typeof(int))
        {
            return "int";
        }
        else if (type == typeof(short))
        {
            return "short";
        }
        else if (type == typeof(long))
        {
            return "long";
        }
        else if (type == typeof(byte))
        {
            return "byte";
        }
        else if (type == typeof(bool))
        {
            return "bool";
        }
        else if(type.IsListType())
        {
            var elementLType = type.GetElementTypeInArrayOrList();
            return $"LuaList<{GetTypeName(elementLType)}>";
        }
        else
        {
            return type.Name;
        }
    }

    private static Type[] GetExportTypes(Type[] dataTypes,out Type[] listTypes)
    {
        List<Type> originTypes = new List<Type>(dataTypes);
        List<Type> results = new List<Type>();
        List<Type> listResults = new List<Type>();
        while (originTypes.Count > 0)
        {
            var type = originTypes[0];
            originTypes.RemoveAt(0);
            results.Add(type);

            var baseTypes = GetBaseTypes(type, typeof(System.Object));
            foreach (var baseType in baseTypes)
            {
                if (results.Contains(baseType) || originTypes.Contains(baseType))
                {
                    continue;
                }
                originTypes.Add(baseType);
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                if (fieldType.IsClassType())
                {
                    if (!results.Contains(fieldType) && !originTypes.Contains(fieldType))
                    {
                        originTypes.Add(fieldType);
                    }
                    continue;
                }

                if (fieldType.IsListType())
                {
                    listResults.Add(field.FieldType);
                    fieldType = field.FieldType.GetElementTypeInArrayOrList();
                    if (fieldType.IsClassType())
                    {
                        if (!results.Contains(fieldType) && !originTypes.Contains(fieldType))
                        {
                            originTypes.Add(fieldType);
                        }
                    }
                    continue;
                }
            }
        }

        results = (from type in results where !type.IsAbstract && !type.IsInterface && (type != typeof(object)) select type).ToList();
        listTypes = listResults.Distinct().ToArray();

        return results.Distinct().ToArray();
    }

    private static string GetIndent(int indent)
    {
        return new string(' ', indent * 4);
    }

    public static Type[] GetBaseTypes(Type type, Type suspendType = null)
    {
        if (type == suspendType)
        {
            return new Type[0];
        }
        suspendType = suspendType ?? typeof(object);

        var types = new List<Type>() { type };
        while (types.Last().BaseType != suspendType)
        {
            types.Add(types.Last().BaseType);
        }
        types.Reverse();

        return types.ToArray();
    }
}
