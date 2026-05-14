using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaTest_CS : MonoBehaviour
{
    public LuaEnv env;
    private void Start()
    {
        // env = new LuaEnv();
        //
        // string filePath = Application.dataPath + "/Resources/MainLua.lua";
        //
        // if (File.Exists(filePath))
        // {
        //     string luaCode = File.ReadAllText(filePath);
        //     
        //     // 2. 直接执行这段代码字符串
        //     env.DoString(luaCode);
        //     
        //     Debug.Log("✅ Lua 文件读取并执行成功！");
        // }
        // else
        // {
        //     Debug.LogError("❌ 错误：找不到文件！路径是：" + filePath);
        // }
        
        // LuaEnvManager.Instance.DoLua("MainLua");
    }

    private void Update()
    {
        
    }
}
