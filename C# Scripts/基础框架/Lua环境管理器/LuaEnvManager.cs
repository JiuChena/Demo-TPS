using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using XLua;

public class LuaEnvManager
{
    private static LuaEnvManager instance;

    public static LuaEnvManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LuaEnvManager();
                instance.InitLuaEnv();
            }
            return instance;
        }
    }

    private LuaEnv env;

    public void InitLuaEnv()
    {
        if (env == null)
        {
            env = new LuaEnv();
            env.AddLoader(CustomLuaLoader);
            env.AddLoader(LuaAddressableLoader);
        }
    }

    /// <summary>
    /// 获取_G表
    /// </summary>
    public LuaTable Global
    {
        get
        {
            return env.Global;
        }
    }

    public void DoLua(string fileName)
    {
        string str = string.Format("require('{0}')", fileName);
        env.DoString(str);
    }

    public void DoString(string str)
    {
        if (env == null)
        {
            Debug.Log("解析器未初始化");
            return;
        }
        env.DoString(str);
    }

    public void Tick()
    {
        if (env == null)
        {
            Debug.Log("解析器未初始化");
            return;
        }
        env.Tick();
    }

    public void Dispose()
    {
        if (env == null)
        {
            Debug.Log("解析器未初始化");
            return;
        }
        env.Dispose();
        env = null;
    }
    
    private byte[] CustomLuaLoader(ref string fileName)
    {
        string path = Application.dataPath + "/Lua/" + fileName + ".lua";

        if (File.Exists(path))
        {
            return File.ReadAllBytes(path);
        }
        else
        {
            Debug.LogWarning(fileName + ".Lua文件找不到");
        }
        
        return null;
    }

    // private bool haveLoadedLua = false;
    // private AssetBundle ab;
    private byte[] LuaAddressableLoader(ref string fileName)
    {
        // if (!haveLoadedLua)
        // {
        //     string path = Application.streamingAssetsPath + "/lua";
        //     ab = AssetBundle.LoadFromFile(path);
        //     haveLoadedLua = true;
        // }
        // TextAsset tx = ab.LoadAsset<TextAsset>(fileName + ".lua");
        
        TextAsset luaText = Addressables.LoadAssetAsync<TextAsset>(fileName + ".lua").WaitForCompletion();
        
        return luaText.bytes;
    }
}
