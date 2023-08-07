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
        yield return new WaitForSeconds(0.4f); // �Ի���ʼ׼��ʱ��

        for (int i = 0; i < receiveConv.lines.Count; i++)
        {
            /*if (getConv)
            {
                Debug.Log("ȡ��֮ǰ���������ɣ���������������������������������������������������������������������������������������");
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
                Debug.Log($"����һ������");
                continue; // ��ֹ�������� or break
            }
            Text tx = speaker.tx;
            tx.text = "";
            yield return new WaitForSeconds(0.1f); // ˼��(�հ�ʱ��)
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
            yield return new WaitForSeconds(0.5f); // ����ʱ��
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
            foreach (Speaker speaker in members.Values) speaker.tx.text = "";//�������
            changeMember = false;
            request_wait = true;
            CancelInvoke("recoverRequest");
            Invoke("recoverRequest",3f);
        }
        if (getConv && receiveConv.lines != null)//�µ���Ч�Ի�
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
            UnityEngine.Debug.Log($"����һ���ھӣ�{other.gameObject}");
            changeMember = true;
            if(request_wait) Debug.Log($"�ȴ������Ի�");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Speaker other_speaker = other.gameObject.GetComponent<Speaker>();

        if (other_speaker && members.ContainsValue(other_speaker))
        {
            members.Remove(other_speaker.name);
            other_speaker.location = ""; other_speaker.tx.text = ""; other_speaker.inRoom = false;//anno: bugs might accur when chatRooms overlap
            UnityEngine.Debug.Log($"����һ���ھӣ�{other.gameObject}");
            changeMember = true;
            if (isDynamic && members.Count <= 1) Destroy(gameObject);
        }
    }

}
