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
            // �ڼ��س��������RestorePlayerPosition����
            teleport.RestorePlayerPosition();
            enabled = false; // λ�ûָ�����øýű�
        }
    }
}
