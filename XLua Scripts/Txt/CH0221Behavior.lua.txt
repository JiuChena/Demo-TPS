require("MainLua")

local targets = {}

local function AttackCoroutine(context)
    Debug.Log("dwgauy")
    --播放开火特效
    context.particles[0]:Play()
    --播放开火音效
    AudioManager.Instance:SetAudio(context.audioClips[0], context.objects[1])
    --生成子弹，初始化

    for i=1,3,1 do
        ObjectsPool.Instance:GetObjectFromPool(context.objects[0], nil, function(obj)
            obj.transform.position = context.particles[0].transform.position
            obj.transform.rotation = context.particles[0].transform.rotation
            obj:GetComponent("BulletDriverModule"):Init(30, context.actualData, "CH0221")
            context.actualData.bulletCount = context.actualData.bulletCount - 1
        end)
        if i < 3 then coroutine.yield() end
    end
end

local coAttack = coroutine.create(AttackCoroutine)

function CH0221_Attack(context)
    if coroutine.status(coAttack) == "dead" then coAttack = coroutine.create(AttackCoroutine) end
    coroutine.resume(coAttack, context)
end

function CH0221_AttackHit(attackerData, defencerData, target)
    BatchSkipCharCachePool.Instance:PushSkipChar(target, 100 + Random.Range(10,100))
    defencerData.curHealth = Mathf.Max(0, defencerData.curHealth - 100)
end

function CH0221_Talent(context)
    --播放特效
    context.particles[2]:Play()
    
    actualDatas = PlayerControlModule.Instance.CHActualDataPanels
    
    for k,v in pairs(actualDatas) do
        local targetHealth = v.curHealth + 300
        v.curHealth = Mathf.Min(targetHealth, v.maxHealth)
    end
end

local burstEffectTable = 
{ 
    [1] = function() 
        Debug.Log("暴击伤害提高20%，效果10s")
        PlayerControlModule.Instance.bonusPanel.critDamageBonus = PlayerControlModule.Instance.bonusPanel.critDamageBonus + 0.2-
        TimerEventManager.Instance:AddTimerEvent(10, function()
            PlayerControlModule.Instance.bonusPanel.critDamageBonus = PlayerControlModule.Instance.bonusPanel.critDamageBonus - 0.2
        end)
    end ,
    
    [2] = function()
        Debug.Log("攻击力提高30%，效果10s")
        PlayerControlModule.Instance.bonusPanel.attackBonus = PlayerControlModule.Instance.bonusPanel.attackBonus + 0.3
        TimerEventManager.Instance:AddTimerEvent(10, function()
            PlayerControlModule.Instance.bonusPanel.attackBonus = PlayerControlModule.Instance.bonusPanel.attackBonus - 0.3
        end)
    end,
    
    [3] = function()
        Debug.Log("防御力提高40%，效果10s")
        PlayerControlModule.Instance.bonusPanel.defenceBonus = PlayerControlModule.Instance.bonusPanel.defenceBonus + 0.4
        TimerEventManager.Instance:AddTimerEvent(10, function()
            PlayerControlModule.Instance.bonusPanel.defenceBonus = PlayerControlModule.Instance.bonusPanel.defenceBonus - 0.4
        end)
    end
}

function BurstSkill(context)
    --每0.5秒放一次波，每次波随机提供爆伤/攻击/防御Buff
    context.particles[3]:Play()
    
    local res = math.floor(Random.Range(1, 4))
    burstEffectTable[res]()
    coroutine.yield()
    
    res = math.floor(Random.Range(1, 4))
    burstEffectTable[res]()
    coroutine.yield()
    
    res = math.floor(Random.Range(1, 4))
    burstEffectTable[res]()
    
    context.particles[3]:Stop()
end

local coBurst = coroutine.create(BurstSkill)

function CH0221_Burst(context)
    if coroutine.status(coBurst) == "dead" then coBurst = coroutine.create(BurstSkill) end
    coroutine.resume(coBurst, context)
end