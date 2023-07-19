using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//https://indienova.com/u/louisliu/blogread/25461

public class ObjDictInfo : MonoBehaviour {
    [System.Serializable]
    public struct StrStrStruct
    {
        public string key;
        public List<string> valueList;
    }
    [System.Serializable]
    public struct StrFloatStruct
    {
        public string key;
        public List<float> valueList;
    }
    [System.Serializable]
    public struct StrObjStruct
    {
        public string key;
        public List<GameObject> valueList;
    }
    //定义对外显示的列表：给开发者填写
    public StrStrStruct[] StrStrList;
    public StrFloatStruct[] StrFloatList;
    public StrObjStruct[] StrObjList;

    public Dictionary<string, List<string>> StrStrDict;
    public Dictionary<string, List<float>> StrFloatDict;
    public Dictionary<string, List<GameObject>> StrObjDict;

    private void Start()
    {
        StrStrDict = InitStrStrDict(new Dictionary<string, List<string>>(), StrStrList);
        StrFloatDict =InitStrFloatDict(new Dictionary<string, List<float>>(), StrFloatList);
        StrObjDict=InitStrObjDict(new Dictionary<string, List<GameObject>>(), StrObjList);
    }
    private Dictionary<string, List<string>> InitStrStrDict(Dictionary<string, List<string>> orinDict, StrStrStruct[] addList)
    {//复制使用只需要改函数头即可
        for (int i = 0; i < addList.Length; ++i)
        {
            //注意：若m_listGoods出现相同的key转换后只会导入第一次出现的数据，
            //重复key值视为bug并且没有保护(不会提示没有接收到)，请小心使用!
            if (!orinDict.ContainsKey(addList[i].key))
            {
                orinDict.Add(addList[i].key, addList[i].valueList);
            }
        }
        return orinDict;
    }
    private Dictionary<string, List<float>> InitStrFloatDict(Dictionary<string, List<float>> orinDict, StrFloatStruct[] addList)
    {//复制使用只需要改函数头即可
        for (int i = 0; i < addList.Length; ++i)
        {
            //注意：若m_listGoods出现相同的key转换后只会导入第一次出现的数据，
            //重复key值视为bug并且没有保护(不会提示没有接收到)，请小心使用!
            if (!orinDict.ContainsKey(addList[i].key))
            {
                orinDict.Add(addList[i].key, addList[i].valueList);
            }
        }
        return orinDict;
    }
    private Dictionary<string, List<GameObject>> InitStrObjDict(Dictionary<string, List<GameObject>> orinDict, StrObjStruct[] addList)
    {//复制使用只需要改函数头即可
        for (int i = 0; i < addList.Length; ++i)
        {
            //注意：若m_listGoods出现相同的key转换后只会导入第一次出现的数据，
            //重复key值视为bug并且没有保护(不会提示没有接收到)，请小心使用!
            if (!orinDict.ContainsKey(addList[i].key))
            {
                orinDict.Add(addList[i].key, addList[i].valueList);
            }
        }
        return orinDict;
    }
    public int StrStrListIdx(string key)
    {
        StrStrStruct[] list = StrStrList;//复制使用只需要改以上部分即可
        if (list.Length == 0) return -1;
        int i = 0;
        for (; i < list.Length; i++) if (list[i].key == key) break;
        return i;
    }
}
