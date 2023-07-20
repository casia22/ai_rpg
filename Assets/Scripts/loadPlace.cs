using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadPlace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    private void Update()
    {
        mainPortal teleport = FindObjectOfType<mainPortal>();
        if (teleport != null)
        {
            // 在加载场景后调用RestorePlayerPosition方法
            teleport.RestorePlayerPosition();
            enabled = false; // 位置恢复后禁用该脚本
        }
    }
}
