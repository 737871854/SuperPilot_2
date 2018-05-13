/**
*    Copyright (c) 2015 Need co.,Ltd
*    All rights reserved

*    文件名称:    WeaponPO.cs
*    创建标识:    
*    简    介:    怪物ID
*/
using System;
using System.Collections.Generic; 
using System.Text;
using LitJson; 
    public partial class WeaponPO 
    {
        protected int m_Id;
        protected string m_Name;
        protected string m_Desc;
        protected int m_Type;
        protected string m_BornVolumeName;
        protected string m_DieVolumeName;
        protected float m_Damage;
        protected int m_IsEnermy;
        protected string m_BornEffect;
        protected string m_DieEffect;
        protected int m_DamagePlane;

        public WeaponPO(JsonData jsonNode)
        {
            m_Id = (int)jsonNode["Id"];
            m_Name = jsonNode["Name"].ToString() == "NULL" ? "" : jsonNode["Name"].ToString();
            m_Desc = jsonNode["Desc"].ToString() == "NULL" ? "" : jsonNode["Desc"].ToString();
            m_Type = (int)jsonNode["Type"];
            m_BornVolumeName = jsonNode["BornVolumeName"].ToString() == "NULL" ? "" : jsonNode["BornVolumeName"].ToString();
            m_DieVolumeName = jsonNode["DieVolumeName"].ToString() == "NULL" ? "" : jsonNode["DieVolumeName"].ToString();
            m_Damage = (float)(double)jsonNode["Damage"];
            m_IsEnermy = (int)jsonNode["IsEnermy"];
            m_BornEffect = jsonNode["BornEffect"].ToString() == "NULL" ? "" : jsonNode["BornEffect"].ToString();
            m_DieEffect = jsonNode["DieEffect"].ToString() == "NULL" ? "" : jsonNode["DieEffect"].ToString();
            m_DamagePlane = (int)jsonNode["DamagePlane"];
        }

        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public string Desc
        {
            get
            {
                return m_Desc;
            }
        }

        public int Type
        {
            get
            {
                return m_Type;
            }
        }

        public string BornVolumeName
        {
            get
            {
                return m_BornVolumeName;
            }
        }

        public string DieVolumeName
        {
            get
            {
                return m_DieVolumeName;
            }
        }

        public float Damage
        {
            get
            {
                return m_Damage;
            }
        }

        public int IsEnermy
        {
            get
            {
                return m_IsEnermy;
            }
        }

        public string BornEffect
        {
            get
            {
                return m_BornEffect;
            }
        }

        public string DieEffect
        {
            get
            {
                return m_DieEffect;
            }
        }

        public int DamagePlane
        {
            get
            {
                return m_DamagePlane;
            }
        }

    }

