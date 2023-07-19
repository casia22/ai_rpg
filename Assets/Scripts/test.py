'''import sys
import time 
player_hp = sys.argv[1]  # 接收传递的生命值参数
player_mp = sys.argv[2]  # 接收传递的魔法值参数
sys.stdout.reconfigure(encoding='utf-8')

def get_string():
    s="这里是python脚本，收到player_hp：{}，player_mp：{}".format(player_hp,player_mp)
    return s

time.sleep(3)
print(get_string())
sys.stdout.flush()
'''

import socket
import sys
import time
import _thread as thread
from chat import NPC
import threading

result=''
preres=''
arguments=''
name_response_dict={}
name_thread_dict={}
name_ai_dict={}

def genContent1(name,content):
    
    time.sleep(5)
    print("**********************************************")
    adress_response_dict[client_address]=name+'已处理的 '+content


# 自定义线程类
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


try:
    # 创建套接字并绑定本地IP地址和端口号
    ip_address = '127.0.0.1'
    port = 8648

    # 创建UDP服务器套接字
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    RECV_BUFFER_SIZE = 2 * 1024 * 1024 #2MB
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_RCVBUF, RECV_BUFFER_SIZE)


    server_socket.bind((ip_address, port))
    print("等待连接...")

    while True:
        # 接收C#端发送的数据
        data, client_address = server_socket.recvfrom(RECV_BUFFER_SIZE)
        arguments=data.decode('utf-8')
        arguments=arguments.replace(';;',';').split(';')
        try:
            name=arguments[0];want=arguments[1];content=arguments[2]
        except Exception as e:
            name='';want='';content=''
        print("收到C#脚本的请求：", arguments)
        
        # 向C#端发送响应数据
        if want=='start':
            name_ai_dict[name]=NPC(name=name, desc=content, model="gpt-3.5-turbo") # chatglm_130b,gpt-3.5-turbo
            server_socket.sendto('已初始化'.encode('utf-8'), client_address)
        elif want=='请接收':
            if name_response_dict.get(name,''):
                del name_response_dict[name]
            if name_thread_dict.get(name,''):
                name_thread_dict[name].stop()
                del name_thread_dict[name]
            
            #thread.start_new_thread(genContent,(name,content))
            # 创建并启动线程
            my_thread = MyThread(name,content)
            my_thread.start()
            name_thread_dict[name]=my_thread
            server_socket.sendto('已启动'.encode('utf-8'), client_address)
        elif want=='在等待':
            t=name_response_dict.get(name,'')
            if t:
                server_socket.sendto(t.encode('utf-8'), client_address)
                del name_response_dict[name]
            elif name_thread_dict.get(name,None):
                server_socket.sendto('未完成'.encode('utf-8'), client_address)
            else:
                server_socket.sendto('无任务'.encode('utf-8'), client_address)
        elif want=='关闭':server_socket.close();break
        else: #不需要
            server_socket.sendto('?'.encode('utf-8'), client_address)
    server_socket.close()
except Exception as e:
    import traceback
    traceback.print_exc()
    f=open('error.txt','w')
    f.write(f"Error occurred: {e}")
    f.close()
    server_socket.close()