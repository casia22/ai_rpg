import socket
import sys
import time
import _thread as thread
from chat import NPC
import threading
import json

name_set=set()
name_response_dict={}
pre_name_response_dict={}
name_thread_dict={}
name_ai_dict={}
name_adress_dict={}

class MyThread(threading.Thread):#为什么要定义类：为了将线程加入字典
    def __init__(self, name, content,timeout=5.0):
        super(MyThread, self).__init__()
        self.name = name
        self.content = content
        self.condition=True
        self.is_running = True
        self.timeout=timeout

    def run(self):
        timer = threading.Timer(self.timeout, self.stop)
        timer.start()
        print("线程 {} 开始执行".format(self.name))
        self.genContent(self.name,self.content)
        if not self.condition:
            self.stop()

        print("线程 {} 执行结束".format(self.name))
    def genContent(self,name,content):
        #time.sleep(5)
        clist=content.split('|')
        for c in clist:
            if c and (len(c)-c.index('/'))>2:
                if c[:c.index('/')]=="我": name_ai_dict[name].listen(c,role="assistant")
                else: name_ai_dict[name].listen(c,role="user")
        print("**********************************************")
        name_response_dict[name]=name_ai_dict[name].say()#没有start则这里运行不了，显示未完成
    def stop(self):
        self.is_running = False

def init_server():
    # 创建套接字并绑定本地IP地址和端口号
    ip_address = '127.0.0.1'
    port = 8648

    # 创建UDP服务器套接字
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    RECV_BUFFER_SIZE = 2 * 1024 * 1024 #2MB
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_RCVBUF, RECV_BUFFER_SIZE)

    server_socket.bind((ip_address, port))
    
    return server_socket

def listen():#监听并做
    while True:
        data, client_address = server_socket.recvfrom(RECV_BUFFER_SIZE)
        data=data.decode('utf-8')
        try:
            parsed_data = json.loads(data)  # 解析JSON
            print("Received data:", parsed_data)
        except json.JSONDecodeError:
            print("Invalid JSON data:", data)
        # 基本信息
        name_set.add(data[name])
        name_adress_dict[name]=client_address
        # 创建并启动线程
        my_thread = MyThread(name,"请随意讲一个故事")
        my_thread.start()
        name_thread_dict[name]=my_thread

def send():
    time.sleep(1000)
    for name in name_set:
        if pre_name_response_dict[name]!=name_response_dict[name]:
            server_socket.sendto(f'{name}改变'.encode('utf-8'), name_adress_dict[name])
    pre_name_response_dict=name_response_dict

if __name__=="__main__":
    try:
        # 创建套接字并绑定本地IP地址和端口号
        server_socket=init_server()
        listen_thread = threading.Thread(target=listen); listen_thread.start()
        while True:
            send()
        server_socket.close()
    except Exception as e:
        import traceback
        traceback.print_exc()
        f=open('error.txt','w')
        f.write(f"Error occurred: {e}")
        f.close()
        server_socket.close()
    
    