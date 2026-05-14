--公共类
ObjectsPool = CS.ObjectsPool
Debug = CS.UnityEngine.Debug
AudioManager = CS.AudioManager
Mathf = CS.UnityEngine.Mathf
LayerMask = CS.UnityEngine.LayerMask
TimerEventManager = CS.TimerEventManager
PlayerControlModule = CS.PlayerControlModule
Vector3 = CS.UnityEngine.Vector3
Random = CS.UnityEngine.Random
GenericDataContainer = CS.GenericDataContainer
TaskSystem = CS.TaskSystem
BatchSkipCharCachePool = CS.BatchSkipCharCachePool
PanelManager = CS.PanelManager


--公共方法
function Contains(table, value)
    for _, v in ipairs(table) do
        if v == value then
            return true
        end
    end
    return false
end

---- 【核心引擎类】
--Debug = cs.UnityEngine.Debug
--Time = cs.UnityEngine.Time
--GameObject = cs.UnityEngine.GameObject
--Transform = cs.UnityEngine.Transform

--Vector2 = cs.UnityEngine.Vector2
--Quaternion = cs.UnityEngine.Quaternion
--Color = cs.UnityEngine.Color
--Mathf = cs.UnityEngine.Mathf
--
----【组件类】
--Rigidbody = cs.UnityEngine.Rigidbody
--Animator = cs.UnityEngine.Animator
--Renderer = cs.UnityEngine.Renderer
--BoxCollider = cs.UnityEngine.BoxCollider
--
----【UI 类 (UGUI)】
--Canvas = cs.UnityEngine.Canvas
--RectTransform = cs.UnityEngine.RectTransform
--Button = cs.UnityEngine.UI.Button
--Text = cs.UnityEngine.UI.Text
--Image = cs.UnityEngine.UI.Image
--Slider = cs.UnityEngine.UI.Slider
--InputField = cs.UnityEngine.UI.InputField
--Toggle = cs.UnityEngine.UI.Toggle
--
----【资源与场景】
--Resources = cs.UnityEngine.Resources
--SceneManager = cs.UnityEngine.SceneManagement.SceneManager
--Application = cs.UnityEngine.Application
--
----【输入】
--Input = cs.UnityEngine.Input
--
----【C# 原生工具库】
--String = cs.System.String
--StringBuilder = cs.System.Text.StringBuilder
--List = cs.System.Collections.Generic.List
--Dictionary = cs.System.Collections.Generic.Dictionary
--Path = cs.System.IO.Path
--DateTime = cs.System.DateTime