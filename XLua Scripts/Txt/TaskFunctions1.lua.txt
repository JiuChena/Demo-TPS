function StoreTask1(bullets)
    local data = GenericDataContainer()
    data:LoadData("Data/TaskData/", "StoreTask1")
    local boughtBullets = data:GetDataAt(0)
    
    if boughtBullets == nil then boughtBullets = 0 end

    boughtBullets = boughtBullets + bullets

    if boughtBullets >= 1000 then
        --发放奖励，移除监听
        Debug.Log("发放奖励，移除监听")
        TaskSystem.Instance:RemoveTask("Store Task 1")
    else
        --缓存数据
        data:PushData(boughtBullets)
        data:SaveData("Data/TaskData/", "StoreTask1")
    end
end

function StoreTask1ForListDisplay(textComponent)
    local data = GenericDataContainer()
    data:LoadData("Data/TaskData/", "StoreTask1")
    local boughtBullets = data:GetDataAt(0)

    if boughtBullets == nil then boughtBullets = 0 end
    
    textComponent.text = boughtBullets .. " / 1000"
end

function MoneyTask1()
    
end

function InteractionTask1()
    
end

function FightTask1()
    
end

--在购买按键上加一个事件监听的触发，触发传入参数为本次购买数量