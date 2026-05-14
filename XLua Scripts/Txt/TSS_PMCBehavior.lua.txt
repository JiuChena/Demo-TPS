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

function Solider_HG_AttackHitPlayer(attackerData, defencerData)
    remain = defencerData.curHealth - 100
    Debug.Log(remain)
    defencerData.curHealth = Mathf.Max(0, remain)
end

function Solider_HG_AttackHitEnemy(attackerData, defencerData)

end