

using UnityEngine;

public class Test
{
    private Rigidbody rb;
    public void Tset()
    {
        // Mathf
        // LayerMask
        rb.AddForce(Vector3.one, ForceMode.VelocityChange);
        ObjectsPool.Instance.ReturnObjectToPool(new GameObject(), (obj) =>
        {
            
        });
    }
}