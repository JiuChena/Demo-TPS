--1.生成开火特效
--2.生成子弹

require("MainLua")

function Solider_HG_Attack(context)
    --播放开火特效
    context.particles[0]:Play()
    --播放开火音效
    AudioManager.Instance:SetAudio(context.audioClips[0], context.objects[0])
    --生成子弹,初始化
    ObjectsPool.Instance:GetObjectFromPool("Enemy_Bullet", nil, function(obj)
        obj.transform.position = context.particles[0].transform.position
        obj.transform.rotation = context.particles[0].transform.rotation
        obj:GetComponent("BulletDriverModule"):Init(40, context.actualData, "Solider_HG")
    end)
end

function Solider_HG_AttackHitPlayer(attackerData, defencerData, target)
    BatchSkipCharCachePool.Instance:PushSkipChar(target, 100)
    defencerData.curHealth = Mathf.Max(0, defencerData.curHealth - 100)
end

function Solider_HG_AttackHitEnemy(attackerData, defencerData, target)
    
end

local playerInSoliderBurstSL = false

local function BurstSkill(context)
    --协程方法
    --生成一个手雷球体抓在手上
    local obj
    ObjectsPool.Instance:GetObjectFromPool(context.objects[1], context.transforms[0], function(go)
        obj = go
        obj.transform.localPosition = Vector3.zero
    end)

    coroutine.yield()
    --第二次调用时父对象为null并且给予力扔出去
    if obj ~= nil then
        obj.transform:SetParent(nil)
        --确定一下目标位置，根据目标位置，自身位置   y = 
        local rb = obj:GetComponent("Rigidbody")
        local playerPos = PlayerControlModule.Instance.transform.position
        local selfPos = context.transforms[0].position
        local y = math.max(selfPos.y - playerPos.y, 0.1)
        local t = math.sqrt((2 * y) / 9.8)
        playerPos.y = 0
        selfPos.y = 0
        local xDis = math.min(Vector3.Distance(playerPos, selfPos), 10)
        local v = xDis / t
        local dir = Vector3.Normalize(Vector3.ProjectOnPlane(playerPos - selfPos, Vector3.up))
        rb.useGravity = true
        rb.isKinematic = false
        rb:AddForce(dir * v, CS.UnityEngine.ForceMode.VelocityChange);

        --倒计时三秒之后爆炸并且播放爆炸特效，范围伤害等执行
        local areaDetector = obj:GetComponent("AreaDetector_Sphere")
        areaDetector:Init("Sloider_HG_Burst_SL")
        
        --获取区域检测脚本，播放特效，造成伤害等
        TimerEventManager.Instance:AddTimerEvent(3, function()
            rb.isKinematic = true
            obj.transform:Find("BoomEffect"):GetComponent("ParticleSystem"):Play()
            obj.transform:Find("Sphere").gameObject:SetActive(false)
            if playerInSoliderBurstSL then
                local dis = Vector3.Distance(PlayerControlModule.Instance.transform.position, obj.transform.position)
                if dis > 1 and dis < 2.2 then
                    PlayerControlModule.Instance.GetCHActualDataPanel.curHealth = PlayerControlModule.Instance.GetCHActualDataPanel.curHealth - 400
                elseif dis <= 1 then
                    PlayerControlModule.Instance.GetCHActualDataPanel.curHealth = PlayerControlModule.Instance.GetCHActualDataPanel.curHealth - 800
                end
            end
            
            --归还对象到对象池并将参数初始化
            TimerEventManager.Instance:AddTimerEvent(1, function()
                ObjectsPool.Instance:ReturnObjectToPool(obj, function(go)
                    go:GetComponent("Rigidbody").useGravity = false
                    go.transform:Find("Sphere").gameObject:SetActive(true)
                end)
            end)
        end)
    end
end

local coBurst = coroutine.create(BurstSkill)

function Solider_HG_Burst(context)
    if coroutine.status(coBurst) == "dead" then coBurst = coroutine.create(BurstSkill) end
    coroutine.resume(coBurst, context) 
end

function Sloider_HG_Burst_SL_TriggerFilter(other)
    if other.gameObject.layer == LayerMask.NameToLayer("Player") then
        return true
    else return false
    end
end

function Sloider_HG_Burst_SL_TriggerEnter(other)
    playerInSoliderBurstSL = true
    Debug.Log("Enter")
end

function Sloider_HG_Burst_SL_TriggerStay(other)

end

function Sloider_HG_Burst_SL_TriggerExit(other)
    playerInSoliderBurstSL = false
    Debug.Log("Exit")
end