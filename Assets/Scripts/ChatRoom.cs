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
    [DisplayOnly]
    public string words;
    //can't see
    [HideInInspector]
    public bool getConv;
    public ReceiveConvFormat receiveConv = new ReceiveConvFormat();
    private bool changeMember;
    private GodsHand godsHand;


    private async Task PrintWordsOneByOne()
    {
        await Task.Delay(TimeSpan.FromSeconds(0.4f));//对话开始准备时间
        for(int i=0; i < receiveConv.lines.Count; i++)
        {
            if (getConv) break;//收到新的对话则退出，但好像没有什么效果
            var line = receiveConv.lines[i];
            Debug.Log($"into2-{receiveConv.lines.Count}{line.words}");
            if (line.words == "") continue;
            Speaker speaker = members[line.name];
            if (speaker == null) continue;//防止人物乱入 or break
            Text tx = speaker.tx; tx.text = "";
            await Task.Delay(TimeSpan.FromSeconds(0.1f));//思考(空白时间)
            if (tx != null)
            {
                speaker.onSpeaking = true;
                foreach (char c in line.words)
                {
                    //Debug.Log("into3");
                    tx.text += c;
                    await Task.Delay(TimeSpan.FromSeconds(0.1f));
                }
            }
            if (!line.action.Equals(default(ActionFormat)))
            {
                speaker.action = line.action.type;
                speaker.args = line.action.args;
            }
            godsHand.scene.ConfirmConversation(receiveConv.id, i);
            await Task.Delay(TimeSpan.FromSeconds(0.5f));//保持时间
            speaker.onSpeaking = false;
        }
        Debug.Log("out");
        receiveConv = new ReceiveConvFormat();
    }

    // Start is called before the first frame update
    void Start()
    {
        godsHand = FindObjectOfType<GodsHand>();
    }

    // Update is called once per frame
    void Update()
    {
        membersName = new List<string>(members.Keys);
        if (changeMember)
        {
            if (members.Count > 1) chatRequest = true;
            foreach (Speaker speaker in members.Values) speaker.tx.text = "";//清空泡泡
            changeMember = false;
        }
        if (getConv && receiveConv.lines != null)
        {
            Debug.Log("into1");
            words = receiveConv.lines[0].words;
            isChatting = true;
            getConv = false;
            PrintWordsOneByOne();
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
