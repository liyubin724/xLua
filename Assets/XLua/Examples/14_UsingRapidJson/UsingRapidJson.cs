using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class UsingRapidJson : MonoBehaviour
{
    public TextAsset luaScript;

    void Start()
    {
        LuaEnv luaEnv = new LuaEnv();
        luaEnv.DoString(luaScript.text, "LuaTestScript");
        luaEnv.Dispose();
    }

    
}
