using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMaker : MonoBehaviour
{

    public Collider2D detectColider2D;
    public Speaker speaker;
    [Range(0f, 5f)]
    public float radiusIncrement = 0.5f;

    private Vector3 center;
    private Vector3 dist;
    private bool roomGenerated;
    private float detectRadius;
    private GodsHand godsHand;
    // Start is called before the first frame update
    void Start()
    {
        godsHand = FindObjectOfType<GodsHand>();
        if (detectColider2D is CircleCollider2D)
        {
            CircleCollider2D detectColider = detectColider2D as CircleCollider2D;
            detectRadius = detectColider.radius;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void ChangeRoomColider2D()
    {
        GameObject newRoom = new GameObject("DynamicRoom");
        newRoom.transform.position = center;
        CircleCollider2D RoomCC2D = newRoom.AddComponent<CircleCollider2D>();
        RoomCC2D.isTrigger = true;
        RoomCC2D.radius = detectRadius / 2f + 0.1f + radiusIncrement;
        ChatRoom chatRoom = newRoom.AddComponent<ChatRoom>();
        chatRoom.isDynamic = true;
        chatRoom.location = speaker.location;
        godsHand.rooms.Add(chatRoom);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (speaker.wanaChat == true && !speaker.inRoom)
        {
            Speaker other_speaker = other.gameObject.GetComponent<Speaker>();

            if (other_speaker && other_speaker.wanaChat && !other_speaker.inRoom && speaker.SpeakPriority > other_speaker.SpeakPriority)
            {
                Vector3 this_pos = this.gameObject.transform.position; Vector3 other_pos = other_speaker.gameObject.transform.position;
                center = (this_pos + other_pos) / 2f; center.z = this_pos.z;
                dist = new Vector3(Mathf.Abs(this_pos.x - other_pos.x), Mathf.Abs(this_pos.y - other_pos.y), 0);

                ChangeRoomColider2D();
                roomGenerated = true;
            }
        }
    }

    
}
