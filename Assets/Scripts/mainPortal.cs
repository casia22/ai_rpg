using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class mainPortal : MonoBehaviour
{
    public int sceneNum;

    private static Vector2 savedPlayerPosition;// 存储玩家位置的变量
    // Start is called before the first frame update

    void Start()
    {
        
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        PlayerControl pc = other.GetComponent<PlayerControl>();
        if (pc != null)
        {
            savedPlayerPosition = pc.transform.position;
            
            //去unity build settings里场景后数字便是buildIndex
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+ sceneNum);
            Debug.Log("当前位置：" + savedPlayerPosition);

        }
    }
    public void RestorePlayerPosition()
    {
        PlayerControl pc = FindObjectOfType<PlayerControl>();
        if (pc != null)
        {
            //Debug.Log("存储位置：" + savedPlayerPosition);
            // 将玩家位置设为之前保存的位置
            Vector2 savePosition = savedPlayerPosition;
            
            savePosition.y -= 0.5f; //防止出门就撞到再回去
            pc.transform.position = savePosition;
            //pc.transform.position= savedPlayerPosition;
            
        }
    }

}
