using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//https://indienova.com/u/louisliu/blogread/25461

public class test : MonoBehaviour
{
    [System.Serializable]
    public class StrObjStruct<T>
    {
        public string key;
        public List<T> valueList;
    }

    Type objListType = typeof(StrObjStruct<string>[]);

    //���������ʾ���б�����������д
    public StrObjStruct<GameObject>[] StrObjList;
    public StrObjStruct<float>[] StrFloatList2;

    private void Start()
    {

    }
    
}
