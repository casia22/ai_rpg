using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class housePortal : MonoBehaviour
{
    // Start is called before the first frame update
    public int sceneNum_house;
    void Start()
    {
        
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
            

            //ȥunity build settings�ﳡ�������ֱ���buildIndex
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + sceneNum_house);
            

        }
    }
}
