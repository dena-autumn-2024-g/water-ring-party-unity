using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.UI;
using UnityEngine;

public class CreateTorus : MonoBehaviour
{

    [SerializeField]
    private Material _material;
    [SerializeField]
    private Mesh _mesh;

    [SerializeField] private float _r1;
    [SerializeField] private float _r2;
    [SerializeField] private int _n;

    void Awake()
    {
        var r1 = _r1;
        var r2 = _r2;
        var n = _n;

        _mesh = new Mesh();

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var normals = new List<Vector3>();

        // (1) �g�[���X�̌v�Z
        for (int i = 0; i <= n; i++)
        {
            var phi = Mathf.PI * 2.0f * i / n;
            var tr = Mathf.Cos(phi) * r2;
            var y = Mathf.Sin(phi) * r2;

            for (int j = 0; j <= n; j++)
            {
                var theta = 2.0f * Mathf.PI * j / n;
                var x = Mathf.Cos(theta) * (r1 + tr);
                var z = Mathf.Sin(theta) * (r1 + tr);

                vertices.Add(new Vector3(x, y, z));
                // (2) �@���̌v�Z
                normals.Add(new Vector3(tr * Mathf.Cos(theta), y, tr * Mathf.Sin(theta)));
            }
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                var count = (n + 1) * j + i;
                // (3) ���_�C���f�b�N�X���w��
                triangles.Add(count);
                triangles.Add(count + n + 2);
                triangles.Add(count + 1);

                triangles.Add(count);
                triangles.Add(count + n + 1);
                triangles.Add(count + n + 2);
            }
        }

        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        _mesh.normals = normals.ToArray();

        _mesh.RecalculateBounds();
    }

    void Update()
    {
        Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _material, 0);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 100, 50), "Save Mesh"))
        {
            SaveMesh();
        }
    }

    private void SaveMesh()
    {
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(_mesh, "Assets/torus.asset");
        UnityEditor.AssetDatabase.SaveAssets();
        #endif
    }
}