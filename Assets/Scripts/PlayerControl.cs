using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float speed = 5f;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 position = transform.position;
        position.x += moveX * speed * Time.deltaTime;
        position.y += moveY * speed * Time.deltaTime;
        transform.position = position;
        //Debug.Log("µ±«∞Œª÷√£∫" + position);
    }
    void Update()
    {
        

    }


}
