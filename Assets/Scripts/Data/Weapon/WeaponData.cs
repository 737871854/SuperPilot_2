/**
* 	Copyright (c) 2015 Need co.,Ltd
*	All rights reserved

*    文件名称:    WeaponData.cs
*    创建标志:    
*    简    介:    怪物ID
*/
using System;
using System.Collections.Generic; 
using LitJson; 
    public partial class WeaponData 
    {
        protected static WeaponData instance;
        protected Dictionary<int,WeaponPO> m_dictionary;

        public static WeaponData Instance
        {
            get{
                if(instance == null)
                {
                    instance = new WeaponData();
                }
                return instance;
            }
        }

        protected WeaponData()
        {
            m_dictionary = new Dictionary<int,WeaponPO>();
        }

        public WeaponPO GetWeaponPO(int key)
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
                WeaponPO po = new WeaponPO(element);
                WeaponData.Instance.m_dictionary.Add(po.Id, po);
            }
        }
    }
