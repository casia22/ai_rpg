using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoom : MonoBehaviour
{
    public string location="";
    public string topic="";
    public Dictionary<string, Speaker> members = new Dictionary<string, Speaker>();
    public string interruptSpeech="";
    //show only, temp
    [DisplayOnly]
    public bool isDynamic = false;
    [DisplayOnly]
    public List<string> membersName;
    [DisplayOnly]
    public bool chatRequest;
    [DisplayOnly]
    public bool isChatting;
    //[DisplayOnly]
    //public string words;
    //can't see
    [HideInInspector]
    public bool getConv;
    public ReceiveConvFormat receiveConv = new ReceiveConvFormat();
    private bool changeMember;
    private GodsHand godsHand;
    [DisplayOnly]
    public bool request_wait;
    
    
    private IEnumerator PrintWordsOneByOne()
    {
        yield return new WaitForSeconds(0.4f); // 对话开始准备时间

        for (int i = 0; i < receiveConv.lines.Count; i++)
        {
            /*if (getConv)
            {
                Debug.Log("取消之前，重新生成！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！");
                break;
            }*/
            var line = receiveConv.lines[i];
            if (line.words == "") continue;
            Speaker speaker = null;
            if (members.ContainsKey(line.name))
            {
                speaker = members[line.name];
                Debug.Log($"{receiveConv.lines.Count}-{speaker.name}: {line.words}");
            } 
            else
            {
                Debug.Log($"跳过一个人物");
                continue; // 防止人物乱入 or break
            }
            Text tx = speaker.tx;
            tx.text = "";
            yield return new WaitForSeconds(0.1f); // 思考(空白时间)
            if (tx != null)
            {
                speaker.onSpeaking = true;
                foreach (char c in line.words)
                {
                    tx.text += c;
                    yield return new WaitForSeconds(0.1f);
                }
            }
            if (!line.action.Equals(default(ActionFormat)))
            {
                speaker.action = line.action.type;
                speaker.args = line.action.args;
            }
            godsHand.scene.ConfirmConversation(receiveConv.id, i);
            yield return new WaitForSeconds(0.5f); // 保持时间
            speaker.onSpeaking = false;
        }
        Debug.Log("out");
        receiveConv = new ReceiveConvFormat();
    }
    
    void recoverRequest()
    {
        request_wait = false;
    }
    void req()
    {
        if (members.Count > 0) chatRequest = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        godsHand = FindObjectOfType<GodsHand>();
        InvokeRepeating("req", 180f,180f);
        request_wait = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (pre_test != test)
        {
            StopCoroutine("HHH");
            pre_test = test;
            StartCoroutine("HHH");
        }
        _=hhh(test);*/

        membersName = new List<string>(members.Keys);
        if (changeMember && !request_wait)
        {
            if (members.Count > 0) chatRequest = true;
            foreach (Speaker speaker in members.Values) speaker.tx.text = "";//清空泡泡
            changeMember = false;
            request_wait = true;
            CancelInvoke("recoverRequest");
            Invoke("recoverRequest",3f);
        }
        if (getConv && receiveConv.lines != null)//新的有效对话
        {
            StopCoroutine("PrintWordsOneByOne");
            Debug.Log("into1");
            //words = receiveConv.lines[0].words;
            isChatting = true;
            getConv = false;
            StartCoroutine("PrintWordsOneByOne");
        }
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        Speaker other_speaker = other.gameObject.GetComponent<Speaker>();
        if (other_speaker && !members.ContainsValue(other_speaker))
        {
            members.Add(other_speaker.name, other_speaker);
            other_speaker.location = location; other_speaker.inRoom = true;
            UnityEngine.Debug.Log($"增加一个邻居：{other.gameObject}");
            changeMember = true;
            if(request_wait) Debug.Log($"等待创建对话");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Speaker other_speaker = other.gameObject.GetComponent<Speaker>();

        if (other_speaker && members.ContainsValue(other_speaker))
        {
            members.Remove(other_speaker.name);
            other_speaker.location = ""; other_speaker.tx.text = ""; other_speaker.inRoom = false;//anno: bugs might accur when chatRooms overlap
            UnityEngine.Debug.Log($"减少一个邻居：{other.gameObject}");
            changeMember = true;
            if (isDynamic && members.Count <= 1) Destroy(gameObject);
        }
    }

}
