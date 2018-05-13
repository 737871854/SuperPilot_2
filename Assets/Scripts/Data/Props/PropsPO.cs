/**
*    Copyright (c) 2015 Need co.,Ltd
*    All rights reserved

*    文件名称:    PropsPO.cs
*    创建标识:    
*    简    介:    怪物ID
*/
using System;
using System.Collections.Generic; 
using System.Text;
using LitJson; 
    public partial class PropsPO 
    {
        protected int m_Id;
        protected string m_Name;
        protected string m_Desc;
        protected int m_Type;
        protected int m_Score;
        protected string m_BornVolumeName;
        protected string m_DieVolumeName;
        protected float m_Damage;
        protected float m_MinPrecent;
        protected float m_MaxPrecent;
        protected int m_MinNumber;
        protected int m_MaxNumber;
        protected int m_Offset;
        protected int m_IsEnermy;
        protected int m_DamagePlane;
        protected string m_BornEffect;
        protected string m_DieEffect;

        public PropsPO(JsonData jsonNode)
        {
            m_Id = (int)jsonNode["Id"];
            m_Name = jsonNode["Name"].ToString() == "NULL" ? "" : jsonNode["Name"].ToString();
            m_Desc = jsonNode["Desc"].ToString() == "NULL" ? "" : jsonNode["Desc"].ToString();
            m_Type = (int)jsonNode["Type"];
            m_Score = (int)jsonNode["Score"];
            m_BornVolumeName = jsonNode["BornVolumeName"].ToString() == "NULL" ? "" : jsonNode["BornVolumeName"].ToString();
            m_DieVolumeName = jsonNode["DieVolumeName"].ToString() == "NULL" ? "" : jsonNode["DieVolumeName"].ToString();
            m_Damage = (float)(double)jsonNode["Damage"];
            m_MinPrecent = (float)(double)jsonNode["MinPrecent"];
            m_MaxPrecent = (float)(double)jsonNode["MaxPrecent"];
            m_MinNumber = (int)jsonNode["MinNumber"];
            m_MaxNumber = (int)jsonNode["MaxNumber"];
            m_Offset = (int)jsonNode["Offset"];
            m_IsEnermy = (int)jsonNode["IsEnermy"];
            m_DamagePlane = (int)jsonNode["DamagePlane"];
            m_BornEffect = jsonNode["BornEffect"].ToString() == "NULL" ? "" : jsonNode["BornEffect"].ToString();
            m_DieEffect = jsonNode["DieEffect"].ToString() == "NULL" ? "" : jsonNode["DieEffect"].ToString();
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

        public int Score
        {
            get
            {
                return m_Score;
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

        public float MinPrecent
        {
            get
            {
                return m_MinPrecent;
            }
        }

        public float MaxPrecent
        {
            get
            {
                return m_MaxPrecent;
            }
        }

        public int MinNumber
        {
            get
            {
                return m_MinNumber;
            }
        }

        public int MaxNumber
        {
            get
            {
                return m_MaxNumber;
            }
        }

        public int Offset
        {
            get
            {
                return m_Offset;
            }
        }

        public int IsEnermy
        {
            get
            {
                return m_IsEnermy;
            }
        }

        public int DamagePlane
        {
            get
            {
                return m_DamagePlane;
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

    }

