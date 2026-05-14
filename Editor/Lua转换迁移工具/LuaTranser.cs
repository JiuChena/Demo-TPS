using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LuaTranser : Editor
{
    // 配置路径
    public static string LuaFilePath = Application.dataPath + "/XLua Scripts/Lua/";
    public static string LuaPre_UpdateFilePath = Application.dataPath + "/XLua Scripts/Pre-Update/";
    public static string TxtFilePath = Application.dataPath + "/XLua Scripts/Txt/";

    [MenuItem("Assets/Create/Lua Script", false, 15)]
    public static void CreateLuaFile()
    {
        UnityEngine.Object selectedObject = Selection.activeObject;

        // 2. 确定保存路径
        string path;

        // 如果选中的是文件夹，则直接在文件夹内创建
        if (selectedObject != null && AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(selectedObject)))
        {
            path = AssetDatabase.GetAssetPath(selectedObject);
        }
        // 如果选中的是文件，则在文件所在的文件夹内创建
        else if (selectedObject != null)
        {
            path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selectedObject));
        }
        // 如果没选中任何东西，默认在 Assets 根目录
        else
        {
            path = "Assets";
        }

        // 3. 生成完整的文件路径 (文件名固定为 New Lua.lua，如有重名会自动处理)
        // 使用 AssetDatabase.GenerateUniqueAssetPath 可以自动处理重名 (例如: New Lua 1.lua)
        string fullPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, "New Lua.lua"));

        // 4. 定义默认模板内容
        string templateContent = @"require('MainLua')";

        // 5. 写入文件
        File.WriteAllText(fullPath, templateContent);

        // 6. 刷新资源数据库
        AssetDatabase.Refresh();

        // 7. (可选) 自动选中并打开新创建的文件
        // 加载新生成的资源对象
        TextAsset newAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(fullPath);
        
        if (newAsset != null)
        {
            // 在 Project 窗口中高亮显示
            Selection.activeObject = newAsset;
            EditorGUIUtility.PingObject(newAsset);
            
            // 如果你想自动打开它进行编辑，可以取消下面这行的注释
            // AssetDatabase.OpenAsset(newAsset); 
        }
        
        Debug.Log($"成功创建 Lua 脚本: {fullPath}");
    }

    /// <summary>
    /// 功能1：将 Pre-Update 文件夹中的 .lua 文件转换为 .lua.txt 并移动到 Txt 文件夹
    /// 转换成功后，自动删除源目录下的 .lua 文件及其 .meta 文件
    /// </summary>
    [MenuItem("XLua/将Pre-Update中的lua文件转为txt文件")]
    public static void LuaToTxtForPre_Update()
    {
        if (!Directory.Exists(LuaPre_UpdateFilePath))
        {
            Debug.LogError($"[XLua迁移] 源路径不存在: {LuaPre_UpdateFilePath}");
            return;
        }

        if (!Directory.Exists(TxtFilePath))
        {
            Directory.CreateDirectory(TxtFilePath);
            Debug.Log($"[XLua迁移] 已创建目标文件夹: {TxtFilePath}");
        }

        string[] allFiles = Directory.GetFiles(LuaPre_UpdateFilePath, "*", SearchOption.AllDirectories);
        int successCount = 0;
        int deleteCount = 0;

        foreach (string filePath in allFiles)
        {
            // 过滤：只处理 .lua 结尾，排除 .meta 和 .lua.txt
            if (filePath.EndsWith(".lua") && 
                !filePath.EndsWith(".meta") && 
                !filePath.EndsWith(".lua.txt"))
            {
                string fileName = Path.GetFileName(filePath); 
                string targetFileName = fileName + ".txt";    
                string targetPath = Path.Combine(TxtFilePath, targetFileName);

                try
                {
                    // 复制
                    File.Copy(filePath, targetPath, true);
                    
                    // 删除源文件 (.lua)
                    File.Delete(filePath);
                    deleteCount++;

                    // 删除源 Meta
                    string metaPath = filePath + ".meta";
                    if (File.Exists(metaPath))
                    {
                        File.Delete(metaPath);
                        deleteCount++;
                    }

                    successCount++;
                    Debug.Log($"[XLua迁移] 成功: {fileName} -> {targetFileName} (源文件及Meta已删除)");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[XLua迁移] 处理失败 {fileName}: {e.Message}");
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"[XLua迁移] 全部完成！成功转移 {successCount} 个文件，清理源文件及Meta共 {deleteCount} 个。");
        
        if (successCount == 0)
        {
            Debug.LogWarning("[XLua迁移] 未找到任何符合条件的 .lua 文件。");
        }
    }

    /// <summary>
    /// 功能2：
    /// 1. 对比 LuaFilePath 和 TxtFilePath，删除那些在 Lua 目录中已经不存在的对应 txt 文件
    /// 2. 【新增】检查 Txt 文件夹，删除所有后缀非 .lua.txt 的杂质文件
    /// </summary>
    [MenuItem("XLua/将已经删除的lua文件对比txt删除并清理杂质")]
    public static void RemoveDeleted()
    {
        if (!Directory.Exists(TxtFilePath))
        {
            Debug.Log($"[XLua清理] 目标文件夹不存在，无需清理: {TxtFilePath}");
            return;
        }

        int totalDeleted = 0;

        // --- 步骤 1: 对比删除过期的 .lua.txt ---
        if (Directory.Exists(LuaFilePath))
        {
            string[] txtFiles = Directory.GetFiles(TxtFilePath, "*.lua.txt", SearchOption.AllDirectories);
            
            foreach (string txtPath in txtFiles)
            {
                string txtFileName = Path.GetFileName(txtPath); 
                if (!txtFileName.EndsWith(".txt")) continue;

                // 还原 lua 文件名
                string correspondingLuaName = txtFileName.Substring(0, txtFileName.Length - 4); 
                
                // 计算相对路径以支持子目录
                string relativePath = Path.GetRelativePath(TxtFilePath, txtPath); 
                string relativeDir = Path.GetDirectoryName(relativePath);
                
                string targetLuaPath;
                if (string.IsNullOrEmpty(relativeDir) || relativeDir == ".")
                {
                    targetLuaPath = Path.Combine(LuaFilePath, correspondingLuaName);
                }
                else
                {
                    targetLuaPath = Path.Combine(LuaFilePath, relativeDir, correspondingLuaName);
                }

                // 如果源文件不存在，删除该 txt
                if (!File.Exists(targetLuaPath))
                {
                    try
                    {
                        File.Delete(txtPath);
                        string metaPath = txtPath + ".meta";
                        if (File.Exists(metaPath)) File.Delete(metaPath);
                        
                        totalDeleted++;
                        Debug.Log($"[XLua清理] 检测到 Lua 源文件缺失，已删除过期文件: {txtFileName}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"[XLua清理] 删除失败 {txtFileName}: {e.Message}");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"[XLua清理] 源 Lua 文件夹不存在 ({LuaFilePath})，跳过过期文件对比步骤。");
        }

        // --- 步骤 2: 【新增】清理所有非 .lua.txt 的杂质文件 ---
        string[] allFilesInTxt = Directory.GetFiles(TxtFilePath, "*", SearchOption.AllDirectories);
        int junkCount = 0;

        foreach (string filePath in allFilesInTxt)
        {
            string fileName = Path.GetFileName(filePath);

            // 跳过 .meta 文件 (因为 .meta 文件是依附于主文件的，主文件删了它会在下次刷新或手动清理时处理，
            // 或者我们这里逻辑是：如果主文件不是 .lua.txt，那它的 meta 也是垃圾)
            if (fileName.EndsWith(".meta"))
            {
                // 检查对应的主体文件是否存在且合法
                string mainFilePath = filePath.Substring(0, filePath.Length - 5); // 去掉 .meta
                if (File.Exists(mainFilePath))
                {
                    // 如果主体文件存在，主体文件的逻辑会在下面判断。
                    // 如果主体文件不合法被删了，这个 meta 会在下面被连带删除，或者留到下次。
                    // 为了简单，我们主要判断非 meta 文件。
                    continue; 
                }
                else
                {
                    // 主体文件都不在了，这个孤立的 meta 直接删
                    try { File.Delete(filePath); junkCount++; } catch {}
                    continue;
                }
            }

            // 核心判断：如果不是以 .lua.txt 结尾，视为杂质
            if (!fileName.EndsWith(".lua.txt"))
            {
                try
                {
                    File.Delete(filePath);
                    
                    // 尝试删除伴随的 meta
                    string metaPath = filePath + ".meta";
                    if (File.Exists(metaPath))
                    {
                        File.Delete(metaPath);
                        junkCount++;
                    }

                    junkCount++; // 主文件计数
                    Debug.Log($"[XLua清理] 发现非法文件格式，已删除杂质: {fileName}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[XLua清理] 删除杂质失败 {fileName}: {e.Message}");
                }
            }
        }

        totalDeleted += junkCount;
        AssetDatabase.Refresh();
        
        Debug.Log($"[XLua清理] 全部完成！共清理 {totalDeleted} 个文件 (其中杂质文件: {junkCount})。");
    }
    
    /// <summary>
    /// 功能3：将 LuaFilePath 路径下所有的 .lua 文件备份为 .lua.txt 到 TxtFilePath
    /// 1. 不删除原 lua 脚本
    /// 2. 自动处理 Meta 文件（通过刷新让 Unity 重新生成）
    /// 3. 支持子目录结构同步
    /// </summary>
    [MenuItem("XLua/将Lua目录下的文件备份为txt(不删除源文件)")]
    public static void BackupLuaToTxt()
    {
        // 1. 检查源目录
        if (!Directory.Exists(LuaFilePath))
        {
            Debug.LogError($"[XLua备份] 源路径不存在: {LuaFilePath}");
            return;
        }

        // 2. 确保目标目录存在
        if (!Directory.Exists(TxtFilePath))
        {
            Directory.CreateDirectory(TxtFilePath);
            Debug.Log($"[XLua备份] 已创建目标文件夹: {TxtFilePath}");
        }

        // 3. 获取所有 .lua 文件 (包含子目录)
        string[] allLuaFiles = Directory.GetFiles(LuaFilePath, "*.lua", SearchOption.AllDirectories);
        
        int successCount = 0;
        int skipCount = 0;

        foreach (string sourcePath in allLuaFiles)
        {
            // 排除 meta 文件 (虽然搜索过滤了，但双重保险)
            if (sourcePath.EndsWith(".meta")) continue;

            // 4. 计算目标路径
            // 目的：保持目录结构一致。
            // 例如: Assets/.../Lua/Module/Test.lua -> Assets/.../Txt/Module/Test.lua.txt
            
            // 获取相对路径 (去掉 LuaFilePath 部分)
            string relativePath = sourcePath.Substring(LuaFilePath.Length);
            
            // 组合新的目标路径
            string targetPath = Path.Combine(TxtFilePath, relativePath + ".txt");

            // 确保目标文件夹存在 (如果是子目录，需要先创建文件夹)
            string targetDir = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            try
            {
                // 5. 复制文件
                // File.Copy(源, 目标, 是否覆盖)
                // 这里设置为 true，如果 txt 已存在则覆盖，保证内容同步
                File.Copy(sourcePath, targetPath, true);
                
                successCount++;
                // 可选：如果文件太多，可以注释掉下面这行以减少控制台 spam
                // Debug.Log($"[XLua备份] 备份成功: {relativePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[XLua备份] 备份失败: {relativePath}, 错误: {e.Message}");
                skipCount++;
            }
        }

        // 6. 刷新资源数据库
        // 这一步非常关键：它会告诉 Unity 重新扫描文件夹
        // Unity 会自动为新的 .txt 文件生成全新的 .meta 文件
        AssetDatabase.Refresh();

        Debug.Log($"[XLua备份] 全部完成！\n成功备份: {successCount} 个文件\n失败: {skipCount} 个\n\n源文件未删除，Meta 文件已自动重新生成。");
    }
}