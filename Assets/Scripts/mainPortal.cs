using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class mainPortal : MonoBehaviour
{
    public int sceneNum;

    private static Vector2 savedPlayerPosition;// �洢���λ�õı���
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
            
            //ȥunity build settings�ﳡ�������ֱ���buildIndex
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+ sceneNum);
            Debug.Log("��ǰλ�ã�" + savedPlayerPosition);

        }
    }
    public void RestorePlayerPosition()
    {
        PlayerControl pc = FindObjectOfType<PlayerControl>();
        if (pc != null)
        {
            //Debug.Log("�洢λ�ã�" + savedPlayerPosition);
            // �����λ����Ϊ֮ǰ�����λ��
            Vector2 savePosition = savedPlayerPosition;
            
            savePosition.y -= 0.5f; //��ֹ���ž�ײ���ٻ�ȥ
            pc.transform.position = savePosition;
            //pc.transform.position= savedPlayerPosition;
            
        }
    }

}
