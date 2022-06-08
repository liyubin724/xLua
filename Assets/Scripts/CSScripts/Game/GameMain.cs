using DotEngine.Lua;
using DotEngine.UPool;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public enum A
{
    a = 0,
    b,
    c
}

public enum UserType
{
    Guest = 0,
    Facebook
}


public class GameMain : MonoBehaviour
{
    public static KingOne.Match3.LuaDatas.HttpData httpData = null;

    private void Awake()
    {
        var envMgr = LuaEnvManager.CreateInstance();
        envMgr.Startup("Launcher");

        //LuaTable httpDataTable = envMgr.Global.Get<LuaTable>("httpData");
        //KingOne.Match3.LuaDatas.HttpData d = new KingOne.Match3.LuaDatas.HttpData(httpDataTable);

        //for(int i =0;i<d.userList.Count;i++)
        //{
        //    var userData = d.userList[i];
        //    Debug.Log(userData.guid);
        //}

        httpData = envMgr.Global.Get<KingOne.Match3.LuaDatas.HttpData>("httpData");

        UGOPoolManager.CreateInstance();
    }

    void Start()
    {
        
    }

    private int i = 0;
    // Update is called once per frame
    void Update()
    {
        System.GC.Collect();
        i++;
        if (i == 300)
        {
            var data = httpData.userValue;
            i = 0;
        }
    }
}
