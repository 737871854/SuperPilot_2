using UnityEngine;
using Need.Mx;

public class PathEffect : SingletonBehaviour<PathEffect>
{
    public ParticleSystem EffectSystem;
    public int Segments;
    public int Emission;
    public float Tolerance;
    public Vector3[] EffectLeftPositions;
    public Vector3[] EffectRightPositions;
    private int _length = 1;
    private bool _play = false;

    public bool ShowPosition = false;
    public Color ColorFrom;
    public Color ColorTo;

    private float Percent
    {
        get
        {
            return ioo.gameMode.GetPilotPrecent;
        }
    }

    // Use this for initialization
    void Start()
    {
        GenEffectPositions();
        _length = EffectLeftPositions.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (_play)
        {
            float percent = Percent;
            int curId = (int)(percent * _length);
            for (int i = 0; i < Emission; i++)
            {
                int idOffset = Random.Range(0, Segments);
                int id = (curId + idOffset) % _length;
                EffectSystem.Emit(GetRandomPos(EffectLeftPositions[id], Tolerance), Vector3.zero, Random.Range(EffectSystem.startSize * 0.5f, EffectSystem.startSize),
                    EffectSystem.startLifetime, Color.Lerp(ColorFrom, ColorTo, Random.Range(0f, 1f)));
                idOffset = Random.Range(0, Segments);
                id = (curId + idOffset) % _length;
                EffectSystem.Emit(GetRandomPos(EffectRightPositions[id], Tolerance), Vector3.zero, Random.Range(EffectSystem.startSize * 0.5f, EffectSystem.startSize),
                    EffectSystem.startLifetime, Color.Lerp(ColorFrom, ColorTo, Random.Range(0f, 1f)));
                float lerp = Random.Range(0f, 1f);
                EffectSystem.Emit(GetRandomPos(Vector3.Lerp(EffectLeftPositions[id], EffectRightPositions[id], lerp), Tolerance), Vector3.zero, Random.Range(EffectSystem.startSize * 0.5f, EffectSystem.startSize),
                    EffectSystem.startLifetime, Color.Lerp(ColorFrom, ColorTo, Random.Range(0f, 1f)));
                //EffectSystem.Emit(GetRandomPos(Vector3.Lerp(EffectLeftPositions[id], EffectRightPositions[id], lerp), Tolerance), Vector3.zero, Random.Range(EffectSystem.startSize * 0.5f, EffectSystem.startSize),
                //EffectSystem.startLifetime, EffectSystem.startColor);
            }
        }
    }

    private Vector3 GetRandomPos(Vector3 pos, float offset)
    {
        float x = Random.Range(-offset, offset);
        float y = Random.Range(-offset, offset);
        float z = Random.Range(-offset, offset);
        return new Vector3(x, y, z) + pos;
    }

    public void StartPathEffect()
    {
        _play = true;
    }
    public void StopPathEffect()
    {
        _play = false;
    }

    private void OnDrawGizmos()
    {
        if (ShowPosition && EffectLeftPositions != null && EffectRightPositions != null)
        {
            for (int i = 0; i < EffectLeftPositions.Length; i++)
            {
                Gizmos.DrawSphere(EffectLeftPositions[i], 5);
                Gizmos.DrawSphere(EffectRightPositions[i], 5);
            }
        }
    }

    private void GenEffectPositions()
    {
        Vector3 basePos = transform.position;
        MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        if (mesh != null)
        {
            Vector3[] vertices = mesh.vertices;
            EffectLeftPositions = new Vector3[vertices.Length / 4];
            EffectRightPositions = new Vector3[vertices.Length / 4];
            for (int i = 0; i < EffectRightPositions.Length; ++i)
            {
                EffectLeftPositions[i] = vertices[i * 4] + basePos;
                EffectRightPositions[i] = vertices[i * 4 + 1] + basePos;
            }
        }
    }
}
