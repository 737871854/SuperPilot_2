using UnityEngine;

public class PathInfo : MonoBehaviour
{
    public Vector3[] Path;
    public Vector3[] PathUp;
    public Vector3[] PathRight;
    public float PathLength;
    public bool ShowPath;
    public float TestPercent = 0;

    private void OnDrawGizmos()
    {
        if (ShowPath)
        {
            if (Path != null)
            {
                iTween.DrawPath(Path);
                Gizmos.DrawSphere(GetPos(TestPercent), 0.1f);
            }
            if (PathUp != null)
            {
                iTween.DrawPath(PathUp);
                Gizmos.DrawSphere(GetUpPos(TestPercent), 0.1f);
            }
        }
    }

    //public void InitPath()
    //{
    //    Vector3 basePos         = transform.position;
    //    MeshFilter meshFilter   = GetComponent<MeshFilter>();
    //    Mesh mesh               = meshFilter.sharedMesh;
    //    if (mesh != null)
    //    {
    //        Vector3[] vertices = mesh.vertices;
    //        Path = new Vector3[vertices.Length / 4];
    //        PathUp = new Vector3[vertices.Length / 4];
    //        PathRight = new Vector3[vertices.Length / 4];
    //        for (int i = 0; i < Path.Length; ++i)
    //        {
    //            Path[i] = (vertices[i * 4] + vertices[i * 4 + 1]) / 2 + basePos;
    //            Vector3 right = vertices[i * 4 + 1] + basePos;
    //            Vector3 cur;
    //            Vector3 next;
    //            if (i == Path.Length-1)
    //            {
    //                cur = (vertices[(i-1) * 4] + vertices[(i-1) * 4 + 1]) / 2 + basePos;
    //                next = Path[i];
    //            }
    //            else
    //            {
    //                cur = Path[i];
    //                next = (vertices[(i+1) * 4] + vertices[(i+1) * 4 + 1]) / 2 + basePos;
    //            }
    //            PathUp[i] = Path[i] + (Quaternion.AngleAxis(90, next - cur) * (right - Path[i])).normalized *0.1f;
    //            PathRight[i] = right;
    //        }
    //    }

    //    PathLength  = iTween.PathLength(Path);
    //    Path = iTween.PathControlPointGenerator(Path);
    //    PathUp = iTween.PathControlPointGenerator(PathUp);
    //    PathRight = iTween.PathControlPointGenerator(PathRight);
    //}

    public Vector3 GetPos(float percent)
    {
        percent = Mathf.Repeat(percent, 1);
        return iTween.J3PointOnPath(Path, percent);
    }

    public Vector3 GetUpPos(float percent)
    {
        percent = Mathf.Repeat(percent, 1);
        return iTween.J3PointOnPath(PathUp, percent);
    }
    public Vector3 GetRightPos(float percent)
    {
        percent = Mathf.Repeat(percent, 1);
        return iTween.J3PointOnPath(PathRight, percent);
    }
}
