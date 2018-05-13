using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*------------ SplineBend 类为javascript类，将其放入Plusins文件夹才能调用----------------*/

namespace Assets.Scripts.Editor
{
    [CustomEditor(typeof(PathInfo))]
    public class GenPathEditor : UnityEditor.Editor
    {
        private static Transform _helicopter;
        private GUIContent _helicopterGuiContent = new GUIContent("Helicopter");
        private static float _locate = 0;
        protected Vector3 NextPoint;
        protected Vector3 CurPoint;
        protected const float Step = 0.001f;
        public override void OnInspectorGUI()
        {
            PathInfo pathInfo = target as PathInfo;
            _helicopter = EditorGUILayout.ObjectField(_helicopterGuiContent, _helicopter, typeof(Transform)) as Transform;
            float temp = EditorGUILayout.FloatField("Locate", _locate*100)*0.01f;
            if (!_locate.Equals(temp))
            {
                _locate = temp;
                if (_helicopter != null)
                {
                    if (pathInfo.Path == null)
                        GenPath(pathInfo);
                    CurPoint = pathInfo.GetPos(_locate);
                    NextPoint = pathInfo.GetPos(_locate + Step);
                    _helicopter.position = CurPoint;
                    if (_helicopter.position != NextPoint)
                    {
                        Vector3 upPoint = pathInfo.GetUpPos(_locate);
                        _helicopter.LookAt(NextPoint, upPoint - CurPoint);
                    }
                }
            }
            temp = GUILayout.HorizontalSlider(_locate*100, 0, 100)*0.01f;
            if (!_locate.Equals(temp))
            {
                _locate = temp;
                if (_helicopter != null)
                {
                    if (pathInfo.Path == null)
                        GenPath(pathInfo);
                    CurPoint = pathInfo.GetPos(_locate);
                    NextPoint = pathInfo.GetPos(_locate + Step);
                    _helicopter.position = CurPoint;
                    if (_helicopter.position != NextPoint)
                    {
                        Vector3 upPoint = pathInfo.GetUpPos(_locate);
                        _helicopter.LookAt(NextPoint, upPoint - CurPoint);
                    }
                }
            }

            pathInfo.ShowPath = EditorGUILayout.Toggle("Show Path", pathInfo.ShowPath);
            pathInfo.TestPercent = GUILayout.HorizontalSlider(pathInfo.TestPercent * 100, 0, 100) * 0.01f;
            if (GUILayout.Button("Move to view"))
            {
                GameObject testGameObject = new GameObject("Test");
                testGameObject.transform.position = pathInfo.GetPos(pathInfo.TestPercent);
                Selection.activeGameObject = testGameObject;
            }
            if (GUILayout.Button("Gen Path"))
            {
                GenPath(pathInfo);
            }
            if (GUILayout.Button("Align Markers"))
            {
                SplineBend splineBend = pathInfo.GetComponent<SplineBend>();
                for (int i = 0; i < splineBend.markers.Length - 1; i++)
                {
                    splineBend.markers[i].transform.LookAt(splineBend.markers[i+1].transform.position);
                }
            }
        }

        private void GenPath(PathInfo pathInfo)
        {
            Vector3 basePos = pathInfo.transform.position;
            MeshFilter meshFilter = pathInfo.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh != null)
            {
                Vector3[] vertices = mesh.vertices;
                pathInfo.Path = new Vector3[vertices.Length / 4];
                pathInfo.PathUp = new Vector3[vertices.Length / 4];
                pathInfo.PathRight = new Vector3[vertices.Length / 4];
                for (int i = 0; i < pathInfo.Path.Length; ++i)
                {
                    pathInfo.Path[i] = (vertices[i * 4] + vertices[i * 4 + 1]) / 2 + basePos;
                    Vector3 right = vertices[i * 4 + 1] + basePos;
                    Vector3 cur;
                    Vector3 next;
                    if (i == pathInfo.Path.Length-1)
                    {
                        cur = (vertices[(i-1) * 4] + vertices[(i-1) * 4 + 1]) / 2 + basePos;
                        next = pathInfo.Path[i];
                    }
                    else
                    {
                        cur = pathInfo.Path[i];
                        next = (vertices[(i+1) * 4] + vertices[(i+1) * 4 + 1]) / 2 + basePos;
                    }
                    pathInfo.PathUp[i] = pathInfo.Path[i] + (Quaternion.AngleAxis(90, next - cur) * (right - pathInfo.Path[i])).normalized *0.1f;
                    pathInfo.PathRight[i] = right;
                }
            }
            pathInfo.PathLength = iTween.PathLength(pathInfo.Path);
            pathInfo.Path = iTween.PathControlPointGenerator(pathInfo.Path);
            pathInfo.PathUp = iTween.PathControlPointGenerator(pathInfo.PathUp);
            pathInfo.PathRight = iTween.PathControlPointGenerator(pathInfo.PathRight);

            CreatePools(pathInfo);
        }

        private void CreatePools(PathInfo pathInfo)
        {
            // 不仅只存在于内存当中，还存在与项目里面
            GameObjectPathList pathList = ScriptableObject.CreateInstance<GameObjectPathList>();

            pathList.PathLength = pathInfo.PathLength;
            pathList.Path = pathInfo.Path;
            pathList.PathUp = pathInfo.PathUp;
            pathList.PathRight = pathInfo.PathRight;

            AssetDatabase.CreateAsset(pathList, "Assets/Resources/landpath.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
