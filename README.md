# Unity使用说明

## 文档介绍

Unity project中按照常用存放，在文件夹中，地图存放在Scenes文档，Prefabs中存放预制体；
添加了RPG MAKER包，主要材料和人物暂时从中提取。
Scripts中npc_dialogue存放与npc相关的代码，含有上帝之手，移动，对话场景创建等代码。
movement中存放和AStar寻路方法相关的改进。
Inventory中存放背包系统。

## 使用介绍

project现在使用主要有三部分组成，npc的上帝之手，npc的寻路AStar，以及背包系统
### 1.Astar寻路
基本使用方法参考 https://zhuanlan.zhihu.com/p/117299096
在搭载基础后，在movement中找到seekerctrl 代码搭载到npc上，可以解决寻路中碰撞停止的问题（这块解决方法可以适当改善）。
### 2.背包系统
背包系统参考https://www.bilibili.com/video/BV1LJ411X78s/?spm_id_from=333.999.0.0&vd_source=91d5babc7d10bdd3e3844b582aec01f3
### 3.上帝之手
上帝之手为unity主要代码，在运行调试时先打开python程序后开始。
将RoomMaker挂载在一块区域上（区域用square等都可），代表触发对话的区域。chatroom中需要初始化位置和事件，作为给后端LLM进行触发的prompt。注意！！位置需要与初始化程序中所含位置对应。
将GodsHand挂载在地图上任意物体上（最好不是npc），然后将chatroom拉入。
Speaker挂载在npc上，然后填写对应的npc名字，将npc的对话框挂载。

