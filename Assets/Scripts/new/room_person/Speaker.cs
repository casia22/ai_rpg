using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speaker : MonoBehaviour
{

    public string name;
    [DisplayOnly]
    public string location;
    public Text tx=null;
    [DisplayOnly]
    public bool wanaChat=true;
    [DisplayOnly]
    public bool inRoom;
    [DisplayOnly]
    public bool onSpeaking;
    [DisplayOnly]
    public float SpeakPriority;
    [DisplayOnly]
    public string action;
    [DisplayOnly]
    public string args;

    // Start is called before the first frame update
    void Start()
    {
        SpeakPriority = (float)(General.random.NextDouble());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
