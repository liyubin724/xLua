using DotEngine.Lua;
using DotEngine.UPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private void Awake()
    {
        var envMgr = LuaEnvManager.CreateInstance();
        envMgr.Startup("Launcher");

        UGOPoolManager.CreateInstance();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
