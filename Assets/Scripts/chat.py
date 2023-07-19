import openai
import zhipuai
import re,os
# 本代码不用翻墙也可以跑
zhipuai.api_key = '3fe121b978f1f456cfac1d2a1a9d8c06.iQsBvb1F54iFYfZq'
openai.api_key = "sk-8p38chfjXbbL1RT943B051229a224a8cBdE1B53b5e2c04E2"
#openai.api_key = "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"#买的
openai.api_base = "https://api.ai-yyds.com/v1"



class NPC():
    def __init__(self, name, desc, model="gpt-3.5-turbo", temperature=0.1,top_p=0.2, mood="平静"):
        # model
        self.model = model
        self.temperature = temperature
        self.top_p = top_p
        # NPC
        self.name = name
        self.desc = desc
        self.memeory = []
        self.location = "星露谷小镇村口的石头广场，每天中午有玩耍的孩子，现在有2个小孩;广场上有茶桌，麻将桌；天上有只会飞的大象"
        self.mood = mood
        self.memeory.extend([
            {"role": "system", "content": rf"""
            请你扮演角色'{self.name}', 人物特点为'{self.desc}',心情是'{self.mood}',
            下面扮演该角色进行符合口语交流习惯的对话。
            仅生成单轮对话，不要添加多轮对话。
            仅直接生成角色嘴说出的话，不输出环境描述，不要说明是谁说的话；
            不要添加特殊符号，如：换行符、空格、制表符、转义符等。
            单次回复例子：
            <动机>[语言内容]
            <想要喝一杯茶>[你好，可以给我一杯茶吗？]
            <想要找到凶手>[我们必须为这起案件找到凶手！不让凶手逍遥法外！]
            <EOC>
            """},
                             ])

    def call_openai(self):
        response = openai.ChatCompletion.create(model=self.model,
                                                messages=self.memeory)
        words = response["choices"][0]["message"]["content"].strip()
        words = re.sub(r'(\\)+("|\'|\\)', '', words)
        return words

    def call_zhipuai(self):
        response = zhipuai.model_api.invoke(
            model=self.model,
            prompt=self.memeory,
            temperature=self.temperature,
            top_p=self.top_p,
        )
        #print(response)
        words = response['data']['choices'][0]['content'].strip()
        words = re.sub(r'(\\)+("|\'|\\)', '', words)
        return words

    def append_memory(self, memory):
        self.memeory.append(memory)

    def listen(self, content, npc=None,role="user"):
        content = re.sub(r'(\\)+("|\'|\\)', '', content)
        #self.memeory.append({"role": "system", "content": '    (记住：'+self.desc+'。  不要重复听到的话，不要同意听到的话，不要感谢听到的话。  要从听到的话中推断出有价值的信息，尽量生成确定性的推论，或者推断出相关的疑问。 不要告别，当听到“再见”时要将对话扯回原问题。)'})
        #response_template = rf"{npc.name}在‘{npc.location}’说: {content}"
        response_template=content
        self.memeory.append({"role": role, "content": response_template})
        #if len(self.memeory)>1: self.memeory[-1]={"role": "user", "content": response_template}
        #else: self.memeory.append({"role": "system", "user": response_template})

    def say(self):
        f=open(f'{self.name}_memory.txt','w')
        f.write(str(self.memeory))
        f.close()
        assert self.memeory[-1]["role"] == "user", rf"{self.name}:请先让对方NPC说话"
        try:
            if self.model.startswith("gpt"):
                words = self.call_openai()
            elif self.model.startswith("chatglm"):
                words = self.call_zhipuai()
            #self.memeory.append({"role": "assistant", "content": words})
            return words
        except Exception as e:
            import traceback
            traceback.print_exc()
            print(f"Error occurred: {e}")
            return "对不起，我现在无法回答你的问题。"


if __name__ == "__main__":
    # 创建两个NPC
    male = NPC(name="李大爷", desc="李大爷住在村口的小木屋#1里，每天中午都喜欢出来喝茶，平常喜欢下棋打麻将，并且非常古板喜欢讽刺。李大爷曾经偷过村长的一只黄金西瓜，但是村长并不知道。", model="chatglm_130b")#gpt-3.5-turbo
    female = NPC(name="王大妈", desc="王大妈住在村口的小木屋#2里，每天中午都喜欢出来煮茶，平常喜欢看麻将，讨厌下棋，讨厌喝茶，并且喜欢较真，是村长的老婆。在寻找黄金西瓜。", model="chatglm_130b") # chatglm_130b,
    # 添加种子对话;大妈先跟大爷说话
    seed = "<寻找自己丢失的西瓜>[中午好啊!我的黄金西瓜不见了，你见过吗]"
    female.listen(seed, female)
    print(f"{female.name}:{seed}")
    male.listen(seed, female)
    male_words = male.say()
    print(f"{male.name}:{male_words}")
    print("--------------------")

    for i in range(10):
        female.listen(male_words, male)
        female_words = female.say()

        male.listen(female_words, female)
        male_words = male.say()
        # print对话
        print(f"{female.name}:", female_words)
        print(f"{male.name}:", male_words)
        print("-"*20)
        if female_words == "<对话结束>" or male_words == "<对话结束>":
            break