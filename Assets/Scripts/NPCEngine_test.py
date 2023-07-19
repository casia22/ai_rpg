import socket
import json
import threading


class Game:
    def __init__(self, target_url="::1", target_port=8199, listen_port=8084):
        self.target_url = target_url
        self.target_port = target_port
        self.listen_port = listen_port
        self.sock = socket.socket(socket.AF_INET6, socket.SOCK_DGRAM)
        self.listen_thread = threading.Thread(target=self.listen)
        self.listen_thread.start()
        self.exit_flag=False

    def listen(self):
        self.sock.bind(('::1', self.listen_port))
        while True:
            data, addr = self.sock.recvfrom(1024)
            try:
                json_data = json.loads(data.decode())
                print(json_data)
            except json.JSONDecodeError:
                pass

    def init_engine(self):
        init_data = {
            "func": "init",
            "npc": [
                {"name": "李大爷", "desc": "是个好人", "mood": "正常", "location": "李大爷家", "memory": []},
                {"name": "王大妈", "desc": "是个好人", "mood": "焦急", "location": "王大妈家", "memory": []}
            ],
            "knowledge": {
                "actions": ["stay", "move", "chat"],
                "place": ["李大爷家", "王大妈家", "广场", "瓜田", "酒吧", "警局"],
                "moods": ["正常", "焦急", "严肃", "开心", "伤心"],
                "people": ["李大爷", "王大妈", "村长", "警长"]
            }
        }
        self.send_data(init_data)

    def generate_conversation(self, npc, location, topic, iterrupt_speech):
        conversation_data = {
            "func": "conversation",
            "npc": npc,
            "location": location,
            "topic": topic,
            "iterrupt_speech": iterrupt_speech
        }
        self.send_data(conversation_data)
        return conversation_data

    def confirm_conversation(self, conversation_id, index):
        confirm_data = {
            "func": "confirm_conversation_line",
            "conversation_id": conversation_id,
            "index": index
        }
        self.send_data(confirm_data)

    def send_data(self, data):
        self.sock.sendto(json.dumps(data).encode(), (self.target_url, self.target_port))
   
game = Game()
game.init_engine()
res = game.generate_conversation(["李大爷", "王大妈", "村长"], "酒吧", "村长的紫色内裤", "你好我是玩家，你们在干什么？")
game.confirm_conversation("4310cad1-d42f-4392-a860-42475cb63a12", 24)   
