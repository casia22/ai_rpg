using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerCtrl : MonoBehaviour
{
    GameObject[] anchor_objs;
    public Speaker speaker;

    public AIDestinationSetter dest_setter;

    private bool getHit=false;
    private Vector3 turn_axis;

    void Start()
    {
        anchor_objs = GameObject.FindGameObjectsWithTag("anchor");
        dest_setter = gameObject.GetComponent<AIDestinationSetter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (speaker.action != "对话" && speaker.action != "chat" && getHit)//被碰
        {
            //getHit = Slow_Trans2D(2f, -(vect + transform.right));
            getHit = Slow_Rotate2D(turn_axis, 60f, 0.6f);//若还没移动完则
        }
        if (speaker.action == "前往" || speaker.action == "move")//移动
        {
            if(dest_setter.target == null || speaker.args != dest_setter.target.gameObject.GetComponent<Anchor>().name)//如果speaker给了新目标
            {
                foreach (GameObject an_obj in anchor_objs)
                {
                    Anchor anchor = an_obj.GetComponent<Anchor>();
                    if (anchor != null && anchor.name == speaker.args) dest_setter.target = an_obj.transform;//找到anchor
                }
            }
            speaker.wanaChat = false;//暂时，保证在移动中不会由于碰撞生成动态房间
        }
        else
        {
            dest_setter.target = null;
            speaker.wanaChat = true;//暂时
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        getHit = true;
        Vector3 vect = collision.gameObject.transform.position - transform.position;
        vect.Normalize();
        turn_axis = vect + transform.position;
    }

    float len_sum = 0;
    float backspeed;
    float len_speed_rate = 2;
    bool Slow_Trans2D(float len, Vector3 way)
    {
        backspeed = len_speed_rate * (len - len_sum);//前面的2控制速度大小
        len_sum += backspeed * Time.deltaTime;
        if (len_sum < 0.8f * len)
        {
            //if (this_info.HP_slider.value > 0) way = new Vector3(way.x, 0, way.z);
            //else way = new Vector3(way.x, way.y / 2, way.z);
            way = new Vector3(way.x, way.y, 0);
            way.Normalize();
            transform.Translate(way * backspeed * Time.deltaTime, Space.World);
            //navMeshAgent.velocity = new Vector3(0, 0, 0);
            return true;
        }
        else
        {
            len_sum = 0;
            backspeed = 0;
            return false;
        }

    }

    float time_sum = 0;
    bool Slow_Rotate2D(Vector2 center, float angle, float time)
    {

        if (time_sum < time)
        {
            time_sum += Time.deltaTime;
            transform.RotateAround(center, Vector3.forward, angle * Time.deltaTime / time);
            transform.RotateAround(transform.position, Vector3.forward, -angle * Time.deltaTime / time);
            return true;
        }
        else
        {
            time_sum = 0;
            return false;
        }
    }
}
