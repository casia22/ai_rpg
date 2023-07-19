using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class mm : MonoBehaviour
{
    [DisplayOnly]
    public string state = "normal";
    [DisplayOnly]
    public float m_speed = 4f;
    public Animator animator=null;
    public Text tex;
    static int count = 0;

    // Use this for initialization
    void Start()
    {
        if(animator==null) animator = GetComponent<Animator>();
        //runPython();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == "normal")
        {
            MoveCtrl(m_speed);
            count++;
        }
    }

    void MoveCtrl(float m_speed)
    {
        int sum = 0;
        Vector3 vec = Vector3.zero;
        animator.SetInteger("girl_move", -1);
        if (Input.GetKey(KeyCode.W))
        {
            sum += 1;
            vec += Vector3.up * m_speed * Time.deltaTime;
            animator.SetInteger("girl_move", 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            sum += 1;
            vec += Vector3.down * m_speed * Time.deltaTime;
            animator.SetInteger("girl_move", 1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            sum += 1;
            vec += Vector3.left * m_speed * Time.deltaTime;
            animator.SetInteger("girl_move", 2);
        }
        if (Input.GetKey(KeyCode.D))
        {
            sum += 1;
            vec += Vector3.right * m_speed * Time.deltaTime;
            animator.SetInteger("girl_move", 3);
        }
        float rate = (sum == 2) ? 1.414f : 1;
        transform.Translate(vec / rate);
        //Debug.Log(vec);
    }
}


