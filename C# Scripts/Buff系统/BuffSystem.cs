using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuffSystem : MonoBehaviour
{
    public static BuffSystem Instance { get; private set; }
    
    // 存储结构：[目标对象] -> [Buff列表]
    private Dictionary<GameObject, List<BuffInstance>> activeBuffs = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // 每帧更新所有Buff（生产环境建议用时间分片优化）
        foreach (var kvp in activeBuffs.ToList())
        {
            for (int i = kvp.Value.Count - 1; i >= 0; i--)
            {
                var buff = kvp.Value[i];
                buff.duration -= Time.deltaTime;
                
                if (buff.duration <= 0)
                {
                    RemoveBuffInternal(kvp.Key, buff);
                    kvp.Value.RemoveAt(i);
                }
                else if (buff.buffData.tickInterval > 0 && 
                         Time.time - buff.lastTickTime >= buff.buffData.tickInterval)
                {
                    // 触发周期效果（如DOT）
                    buff.buffData.OnTick?.Invoke(kvp.Key, buff.stacks);
                    buff.lastTickTime = Time.time;
                }
            }
        }
    }

    /// <summary>
    /// 添加Buff（自动处理叠加/刷新）
    /// </summary>
    public void AddBuff(GameObject target, BuffData buffData, int stacks = 1, GameObject caster = null)
    {
        if (target == null || buffData == null) return;
        
        if (!activeBuffs.ContainsKey(target))
            activeBuffs[target] = new List<BuffInstance>();
        
        // 检查是否已存在同类Buff
        BuffInstance existing = activeBuffs[target].FirstOrDefault(b => b.buffData == buffData);
        if (existing != null)
        {
            // 刷新持续时间 + 叠加层数（根据Buff配置）
            existing.duration = buffData.duration;
            if (buffData.canStack) existing.stacks = Mathf.Min(existing.stacks + stacks, buffData.maxStacks);
            existing.lastTickTime = Time.time;
            return;
        }
        
        // 创建新Buff实例
        BuffInstance newInstance = new BuffInstance
        {
            buffData = buffData,
            duration = buffData.duration,
            stacks = stacks,
            caster = caster,
            lastTickTime = Time.time
        };
        activeBuffs[target].Add(newInstance);
        
        // 触发初始效果
        buffData.OnApply?.Invoke(target, stacks, caster);
    }

    /// <summary>
    /// 移除指定Buff
    /// </summary>
    public void RemoveBuff(GameObject target, BuffData buffData)
    {
        if (target == null || !activeBuffs.ContainsKey(target)) return;
        var buff = activeBuffs[target].FirstOrDefault(b => b.buffData == buffData);
        if (buff != null) RemoveBuffInternal(target, buff);
    }

    /// <summary>
    /// 检查目标是否有指定Buff
    /// </summary>
    public bool HasBuff(GameObject target, string buffName)
    {
        if (target == null || !activeBuffs.ContainsKey(target)) return false;
        return activeBuffs[target].Any(b => b.buffData.buffName == buffName);
    }

    // ===== 内部方法 =====
    private void RemoveBuffInternal(GameObject target, BuffInstance buff)
    {
        buff.buffData.OnRemove?.Invoke(target, buff.stacks);
        // 可扩展：触发移除音效/VFX
    }

    // ===== 辅助类 =====
    public class BuffInstance
    {
        public BuffData buffData;
        public float duration;
        public int stacks;
        public GameObject caster;
        public float lastTickTime;
    }
}

// Buff配置数据（需创建为ScriptableObject）
[System.Serializable]
public class BuffData : ScriptableObject
{
    public string buffName;
    public Sprite icon;
    public float duration = 5f;
    public bool canStack = false;
    public int maxStacks = 5;
    public float tickInterval = 0f; // 0=无周期效果
    
    // 回调委托（在BuffSystem中触发）
    public System.Action<GameObject, int, GameObject> OnApply;  // 应用时
    public System.Action<GameObject, int> OnTick;               // 周期触发
    public System.Action<GameObject, int> OnRemove;             // 移除时
}