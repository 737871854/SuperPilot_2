/**
* 	Copyright (c) 2015 Need co.,Ltd
*	All rights reserved

*    文件名称:    PropsData.cs
*    创建标志:    
*    简    介:    怪物ID
*/
using System;
using System.Collections.Generic; 
using LitJson; 
    public partial class PropsData 
    {
        protected static PropsData instance;
        protected Dictionary<int,PropsPO> m_dictionary;

        public static PropsData Instance
        {
            get{
                if(instance == null)
                {
                    instance = new PropsData();
                }
                return instance;
            }
        }

        protected PropsData()
        {
            m_dictionary = new Dictionary<int,PropsPO>();
        }

        public PropsPO GetPropsPO(int key)
        {
            if(m_dictionary.ContainsKey(key) == false)
            {
                return null;
            }
            return m_dictionary[key];
        }

        static public void LoadHandler(LoadedData data)
        {
            JsonData jsonData = JsonMapper.ToObject(data.Value.ToString());
            if (!jsonData.IsArray)
            {
                return;
            }
            for (int index = 0; index < jsonData.Count; index++)
            {
                JsonData element = jsonData[index];
                PropsPO po = new PropsPO(element);
                PropsData.Instance.m_dictionary.Add(po.Id, po);
            }
        }
    }
