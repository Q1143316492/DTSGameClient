using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CWLEngine.Core.Manager;

public class TestObjDes : MonoBehaviour
{
    void OnEnable()
    {
        Invoke("Push", 1);
    }
    
    void Push()
    {
        ObjectPoolMgr.Instance.RemoveGameObject(this.gameObject);
    }
}