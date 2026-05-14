using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDDistributor
{
    private static IDDistributor instance = new IDDistributor();
    public static IDDistributor Instance => instance;

    private ulong ID = 0;

    public ulong GetID()
    {
        return this.ID;
        ID++;
    }
}
