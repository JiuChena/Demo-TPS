using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XLua;

[RequireComponent(typeof(BoxCollider))]
public class AreaDetector_Box : MonoBehaviour
{
    public BoxCollider co;
    
    private void Start()
    {
        co = this.GetComponent<BoxCollider>();
    }

    [CSharpCallLua]
    public delegate bool TriggerFliterDel(Collider other);
    
    private TriggerFliterDel TriggerFilter;
    private UnityAction TriggerEnter;
    private UnityAction TriggerStay;
    private UnityAction TriggerExit;

    public void Init(string skillName)
    {
        TriggerFilter = LuaEnvManager.Instance.Global.Get<TriggerFliterDel>(skillName + "_TriggerFilter");
        TriggerEnter = LuaEnvManager.Instance.Global.Get<UnityAction>(skillName + "_TriggerEnter");
        TriggerStay = LuaEnvManager.Instance.Global.Get<UnityAction>(skillName + "_TriggerStay");
        TriggerExit = LuaEnvManager.Instance.Global.Get<UnityAction>(skillName + "_TriggerExit");

        if (TriggerFilter == null || TriggerEnter == null || TriggerStay == null || TriggerExit == null)
        {
            Debug.Log(165324);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (TriggerFilter?.Invoke(other) ?? false)
        {
            TriggerEnter?.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (TriggerFilter?.Invoke(other) ?? false)
        {
            TriggerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (TriggerFilter?.Invoke(other) ?? false)
        {
            TriggerEnter?.Invoke();
        }
    }
}
