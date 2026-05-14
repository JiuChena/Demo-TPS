require("MainLua")

local targets = {}

local function AttackCoroutine(context)
    --播放开火特效
    context.particles[0]:Play()
    context.particles[1]:Play()
    --播放开火音效
    AudioManager.Instance:SetAudio(context.audioClips[0], context.objects[1])
    --生成子弹，初始化
    for i=1,2,1 do
        ObjectsPool.Instance:GetObjectFromPool(context.objects[0], nil, function(obj)
            obj.transform.position = context.particles[0].transform.position
            obj.transform.rotation = context.particles[0].transform.rotation
            obj:GetComponent("BulletDriverModule"):Init(30, context.actualData, "CH0167")
            context.actualData.bulletCount = context.actualData.bulletCount - 2
        end)
        if i < 2 then coroutine.yield() end
    end
end

local coAttack = coroutine.create(AttackCoroutine)

function CH0167_Attack(context)
    if coroutine.status(coAttack) == "dead" then coAttack = coroutine.create(AttackCoroutine) end
    coroutine.resume(coAttack, context)
end

function CH0167_AttackHit(attackerData, defencerData, target)
    BatchSkipCharCachePool.Instance:PushSkipChar(target, 300 + Random.Range(10,100))
    defencerData.curHealth = Mathf.Max(0, defencerData.curHealth - 300)
end

function CH0167_Talent(context)
    --播放特效
    context.particles[2]:Play()
    local areaDetector = context.particles[2]:GetComponent("AreaDetector_Sphere")
    areaDetector:Init("CH0167_Talent")
    areaDetector.co.radius = 3
    PlayerControlModule.Instance.bonusPanel.attackBonus = PlayerControlModule.Instance.bonusPanel.attackBonus + 0.2
    Debug.Log("全队攻击力提高20%")

    TimerEventManager.Instance:AddTimerEvent(5, function()
        areaDetector.co.radius = 0;
        PlayerControlModule.Instance.bonusPanel.attackBonus = PlayerControlModule.Instance.bonusPanel.attackBonus - 0.2
        Debug.Log("全队攻击力提高20%Buff结束")
    end)
end

local function BurstSkill(context)
    local areaDetector = context.objects[2]:GetComponent("AreaDetector_Box")
    areaDetector:Init("CH0167_Burst_Repel")
    
    context.particles[3]:Play()
    TimerEventManager.Instance:AddTimerEvent(5, function()
        context.particles[3]:Stop()
    end)
    
    context.objects[2]:SetActive(true)
    coroutine.yield()
    if targets == nil then Debug.Log("weikong") end
    for key,value in pairs(targets) do
        Debug.Log(key)
        if value ~= nil then
            local dir = Vector3.Normalize(Vector3.ProjectorOnPlane(context.objects[2].transform.forward, Vector3.up))
            value:GetComponent("CharacterController"):Move(dir)
        end
    end

    CH0167_Attack(context)
    Debug.Log("第一次开火")
    coroutine.yield()
    CH0167_Attack(context)
    Debug.Log("第二次开火")
    coroutine.yield()
    AudioManager.Instance:SetAudio(context.audioClips[1], context.objects[1])
    coroutine.yield()
    CH0167_Attack(context)
    Debug.Log("第三次开火")

    context.objects[2]:SetActive(false)
end

function CH0167BurstAttack(context)
    --播放开火特效
    context.particles[0]:Play()
    context.particles[1]:Play()
    --播放开火音效
    AudioManager.Instance:SetAudio(context.audioClips[0], context.objects[1])
    --生成子弹，初始化
    ObjectsPool.Instance:GetObjectFromPool(context.objects[0], nil, function(obj)
        obj.transform.position = context.particles[0].transform.position
        obj.transform.rotation = context.particles[0].transform.rotation
        obj:GetComponent("BulletDriverModule"):Init(30, context.actualData, "CH0167Burst", true, 2)
    end)
end

function CH0167Burst_AttackHit(attackerData, defencerData, target)
    BatchSkipCharCachePool.Instance:PushSkipChar(target, 800 + Random.Range(10,100))
    defencerData.curHealth = Mathf.Max(0, defencerData.curHealth - 800)
end

local coBurst = coroutine.create(BurstSkill)

function CH0167_Burst(context)
    if coroutine.status(coBurst) == "dead" then coBurst = coroutine.create(BurstSkill) end
    coroutine.resume(coBurst, context)
end

function CH0167_Talent_TriggerFilter(other)
    if other.gameObject.layer == LayerMask.NameToLayer("Player") then
        return true
    else return false
    end
end

function CH0167_Talent_TriggerEnter(other)
    
end

function CH0167_Talent_TriggerStay(other)
    
end

function CH0167_Talent_TriggerExit(other)
    
end

function CH0167_Burst_Repel_TriggerFilter(other)
    if other.gameObject.layer == LayerMask.NameToLayer("Enemy") then
        return true
    else return false
    end
end

function CH0167_Burst_Repel_TriggerEnter(other)
    targets[other.gameObject.name] = other.gameObject
end

function CH0167_Burst_Repel_TriggerStay(other)

end

function CH0167_Burst_Repel_TriggerExit(other)
    targets[other.gameObject.name] = nil
end