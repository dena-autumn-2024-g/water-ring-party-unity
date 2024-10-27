using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FluidSimulation : MonoBehaviour {

    /// <summary>物体X</summary>
    [SerializeField] private GameObject nsobjectOriginal = null;
    /// <summary>Xサイズ</summary>
    [SerializeField] private int WX = 12;
    /// <summary>Yサイズ</summary>
    [SerializeField] private int WY = 12;
    /// <summary>Zサイズ</summary>
    [SerializeField] private int WZ = 12;
    /// <summary>デルタT</summary>
    [SerializeField] private float delta_t = 0.2f;
    /// <summary>レイノルズ数。大きいほど粘性が少ない</summary>
    [SerializeField] private float Re = 1000000.0f;
    /// <summary>SOR法で使う加速係数</summary>
    [SerializeField] private float omega = 1.8f;
    /// <summary>浮遊粒子数。これは流体の上に流れている質量０の点(dot)</summary>
    [SerializeField] private int rys_num = 1024;
    /// <summary>最小サイズ</summary>
    [SerializeField] private float min_size = 0.1f;
    /// <summary>最大倍率</summary>
    [SerializeField] private float size_mul = 0.1f;
    /// <summary>倍率</summary>
    [SerializeField] private Vector3 scale = Vector3.one;
    /// <summary>オフセットX</summary>
    [SerializeField] private Vector3 offset = Vector3.zero;
    /// <summary>外力ベクトル</summary>
    public Vector3[] powers = new Vector3[5];

    public Vector3Int[] block_positions;

    /// <summary>X速度</summary>
    private double[,,] vx;
    /// <summary>Y速度</summary>
    private double[,,] vy;
    /// <summary>Z速度</summary>
    private double[,,] vz;
    /// <summary>変化後X速度</summary>
    private double[,,] vx_after;
    /// <summary>変化後Y速度</summary>
    private double[,,] vy_after;
    /// <summary>変化後Z速度</summary>
    private double[,,] vz_after;
    /// <summary>発散値</summary>
    private double[,,] s;
    /// <summary>圧力</summary>
    private double[,,] p;
    /// <summary>変化後圧力</summary>
    private double[,,] p_after;
    /// <summary>粒子の座標</summary>
    private Vector3[] rys;
    /// <summary>可視化オブジェクト</summary>
    private GameObject[] nsobject;

    void Start() {

        vx = new double[WX + 1, WY, WZ];
        vy = new double[WX, WY + 1, WZ];
        vz = new double[WX, WY, WZ + 1];
        vx_after = new double[WX + 1, WY, WZ];
        vy_after = new double[WX, WY + 1, WZ];
        vz_after = new double[WX, WY, WZ + 1];
        s = new double[WX, WY, WZ];
        p = new double[WX, WY, WZ];
        p_after = new double[WX, WY, WZ];
        rys = new Vector3[rys_num];
        nsobject = new GameObject[rys_num];
        for (var i=0; i<rys_num; i++) {
            rys[i] = new Vector3(
                Random.value * (1.0f * (float)(WX - 2)) + 1.0f,
                Random.value * (1.0f * (float)(WY - 2)) + 1.0f,
                Random.value * (1.0f * (float)(WZ - 2)) + 1.0f
            );
            nsobject[i] = Instantiate(nsobjectOriginal);
            nsobject[i].transform.position = rys[i];
        }
    }

    private void Update() {
        adve(); // 移流（一時風上差分）
        viscosity(); // 粘性

        // 別の力をスペースキーで制御
        ApplyForce(new Vector3(3, 3, WZ/2), powers[0]);
        ApplyForce(new Vector3(9, 3, WZ/2), powers[1]);
        ApplyForce(new Vector3(14, 3, WZ/2), powers[2]);
        ApplyForce(new Vector3(19, 3, WZ/2), powers[3]);
        ApplyForce(new Vector3(25, 3, WZ/2), powers[4]);

        set(); // 壁速度0に固定
        div(); // ダイバージェンス計算
        poisson(); // ポアソン方程式の項
        rhs(); // 修正
        div(); // ダイバージェンス計算
        view(); // 可視化
    }

    private void ApplyForce(Vector3 position, Vector3 force) {
        int x = Mathf.Clamp(Mathf.RoundToInt(position.x), 1, WX-2);
        int y = Mathf.Clamp(Mathf.RoundToInt(position.y), 1, WY-2);
        int z = Mathf.Clamp(Mathf.RoundToInt(position.z), 1, WZ-2);

        vx[x, y, z] += force.x * delta_t;
        vy[x, y, z] += force.y * delta_t;
        vz[x, y, z] += force.z * delta_t;
    }

    /// <summary>
    /// 外力を与える
    /// </summary>
    public void Gairyoku(Vector3 position, Vector3 force) {
        var sx = (int)Mathf.Clamp(position.x + WX * 0.5f, 0.0f, 1.0f * WX - 1.1f);
        var sy = (int)Mathf.Clamp(position.y, 1.0f, 1.0f * WY - 1.1f);
        var sz = (int)Mathf.Clamp(position.z + WZ * 0.5f, 0.0f, 1.0f * WZ - 1.1f);
        vx[sx, sy, sz] = force.x;
        vy[sx, sy, sz] = force.y;
        vz[sx, sy, sz] = force.z;
    }

    

    /// <summary>
    /// 移流（一時風上差分）
    /// </summary>
    private void adve() {

        //まずはvxの更新から
        for (var x = 1; x < WX - 1; x++) {
            for (var y = 1; y < WY - 1; y++) {
                for (var z = 1; z < WZ - 1; z++) {

                    var u = vx[x, y, z];
                    var v = (vy[x-1, y, z] + vy[x, y, z] + vy[x-1, y+1, z] + vy[x, y+1, z]) * 0.25;
                    var w = (vz[x-1, y, z] + vz[x, y, z] + vz[x-1, y, z+1] + vz[x, y, z+1]) * 0.25;

                    //( ｕ>=0かつｖ>=0かつ w >= 0の場合 )
                    if (u >= 0.0 && v >= 0.0 && w >= 0.0) {
                        vx_after[x, y, z] = vx[x, y, z] - u * (vx[x, y, z] - vx[x-1, y, z]) * delta_t - v * (vx[x, y, z] - vx[x, y-1, z]) * delta_t - w * (vx[x, y, z] - vx[x, y, z-1]) * delta_t;
                    }
                    //( ｕ<0かつｖ>=0かつ w >= 0の場合 )
                    if (u < 0.0 && v >= 0.0 && w >= 0.0) {
                        vx_after[x, y, z] = vx[x, y, z] - u * (vx[x+1, y, z] - vx[x, y, z]) * delta_t - v * (vx[x, y, z] - vx[x, y-1, z]) * delta_t - w * (vx[x, y, z] - vx[x, y, z-1]) * delta_t;
                    }
                    //( ｕ>=0かつｖ<0かつ w >= 0の場合 )
                    if (u >= 0.0 && v < 0.0 && w >= 0.0) {
                        vx_after[x, y, z] = vx[x, y, z] - u * (vx[x, y, z] - vx[x-1, y, z]) * delta_t - v * (vx[x, y+1, z] - vx[x, y, z]) * delta_t - w * (vx[x, y, z] - vx[x, y, z-1]) * delta_t;
                    }
                    //( ｕ<0かつｖ<0かつ w => 0の場合 )
                    if (u < 0.0 && v < 0.0 && w >= 0.0) {
                        vx_after[x, y, z] = vx[x, y, z] - u * (vx[x+1, y, z] - vx[x, y, z]) * delta_t - v * (vx[x, y+1, z] - vx[x, y, z]) * delta_t - w * (vx[x, y, z] - vx[x, y, z-1]) * delta_t;
                    }

                    //( ｕ>=0かつｖ>=0かつ w < 0の場合 )
                    if (u >= 0.0 && v >= 0.0 && w < 0.0) {
                        vx_after[x, y, z] = vx[x, y, z] - u * (vx[x, y, z] - vx[x-1, y, z]) * delta_t - v * (vx[x, y, z] - vx[x, y-1, z]) * delta_t - w * (vx[x, y, z+1] - vx[x, y, z]) * delta_t;
                    }
                    //( ｕ<0かつｖ>=0かつ w < 0の場合 )
                    if (u < 0.0 && v >= 0.0 && w < 0.0) {
                        vx_after[x, y, z] = vx[x, y, z] - u * (vx[x+1, y, z] - vx[x, y, z]) * delta_t - v * (vx[x, y, z] - vx[x, y-1, z]) * delta_t - w * (vx[x, y, z+1] - vx[x, y, z]) * delta_t;
                    }
                    //( ｕ>=0かつｖ<0かつ w < 0の場合 )
                    if (u >= 0.0 && v < 0.0 && w < 0.0) {
                        vx_after[x, y, z] = vx[x, y, z] - u * (vx[x, y, z] - vx[x-1, y, z]) * delta_t - v * (vx[x, y+1, z] - vx[x, y, z]) * delta_t - w * (vx[x, y, z+1] - vx[x, y, z]) * delta_t;
                    }
                    //( ｕ<0かつｖ<0かつ w < 0の場合 )
                    if (u < 0.0 && v < 0.0 && w < 0.0) {
                        vx_after[x, y, z] = vx[x, y, z] - u * (vx[x+1, y, z] - vx[x, y, z]) * delta_t - v * (vx[x, y+1, z] - vx[x, y, z]) * delta_t - w * (vx[x, y, z+1] - vx[x, y, z]) * delta_t;
                    }

                }
            }
        }
        //次にvyの更新
        for (var x = 1; x < WX - 1; x++) {
            for (var y = 1; y < WY - 1; y++) {
                for (var z = 1; z < WZ - 1; z++) {

                    var u = (vx[x, y - 1, z] + vx[x + 1, y - 1, z] + vx[x, y, z] + vx[x + 1, y, z]) * 0.25;
                    var v = vy[x, y, z];
                    var w = (vz[x, y - 1, z] + vz[x, y - 1, z + 1] + vz[x, y, z] + vz[x, y, z + 1]) * 0.25;

                    //( ｕ>=0かつｖ>=0かつ w >= 0の場合 )
                    if (u >= 0.0 && v >= 0.0 && w >= 0.0) {
                        vy_after[x, y, z] = vy[x, y, z] - u * (vy[x, y, z] - vy[x - 1, y, z]) * delta_t - v * (vy[x, y, z] - vy[x, y - 1, z]) * delta_t - w * (vy[x, y, z] - vy[x, y, z - 1]) * delta_t;
                    }
                    //( ｕ<0かつｖ>=0かつ w >= 0の場合 )
                    if (u < 0.0 && v >= 0.0 && w >= 0.0) {
                        vy_after[x, y, z] = vy[x, y, z] - u * (vy[x + 1, y, z] - vy[x, y, z]) * delta_t - v * (vy[x, y, z] - vy[x, y - 1, z]) * delta_t - w * (vy[x, y, z] - vy[x, y, z - 1]) * delta_t;
                    }
                    //( ｕ>=0かつｖ<0かつ w >= 0の場合 )
                    if (u >= 0.0 && v < 0.0 && w >= 0.0) {
                        vy_after[x, y, z] = vy[x, y, z] - u * (vy[x, y, z] - vy[x - 1, y, z]) * delta_t - v * (vy[x, y + 1, z] - vy[x, y, z]) * delta_t - w * (vy[x, y, z] - vy[x, y, z - 1]) * delta_t;
                    }
                    //( ｕ<0かつｖ<0かつ w => 0の場合 )
                    if (u < 0.0 && v < 0.0 && w >= 0.0) {
                        vy_after[x, y, z] = vy[x, y, z] - u * (vy[x + 1, y, z] - vy[x, y, z]) * delta_t - v * (vy[x, y + 1, z] - vy[x, y, z]) * delta_t - w * (vy[x, y, z] - vy[x, y, z - 1]) * delta_t;
                    }

                    //( ｕ>=0かつｖ>=0かつ w < 0の場合 )
                    if (u >= 0.0 && v >= 0.0 && w < 0.0) {
                        vy_after[x, y, z] = vy[x, y, z] - u * (vy[x, y, z] - vy[x - 1, y, z]) * delta_t - v * (vy[x, y, z] - vy[x, y - 1, z]) * delta_t - w * (vy[x, y, z + 1] - vy[x, y, z]) * delta_t;
                    }
                    //( ｕ<0かつｖ>=0かつ w < 0の場合 )
                    if (u < 0.0 && v >= 0.0 && w < 0.0) {
                        vy_after[x, y, z] = vy[x, y, z] - u * (vy[x + 1, y, z] - vy[x, y, z]) * delta_t - v * (vy[x, y, z] - vy[x, y - 1, z]) * delta_t - w * (vy[x, y, z + 1] - vy[x, y, z]) * delta_t;
                    }
                    //( ｕ>=0かつｖ<0かつ w < 0の場合 )
                    if (u >= 0.0 && v < 0.0 && w < 0.0) {
                        vy_after[x, y, z] = vy[x, y, z] - u * (vy[x, y, z] - vy[x - 1, y, z]) * delta_t - v * (vy[x, y + 1, z] - vy[x, y, z]) * delta_t - w * (vy[x, y, z + 1] - vy[x, y, z]) * delta_t;
                    }
                    //( ｕ<0かつｖ<0かつ w < 0の場合 )
                    if (u < 0.0 && v < 0.0 && w < 0.0) {
                        vy_after[x, y, z] = vy[x, y, z] - u * (vy[x + 1, y, z] - vy[x, y, z]) * delta_t - v * (vy[x, y + 1, z] - vy[x, y, z]) * delta_t - w * (vy[x, y, z + 1] - vy[x, y, z]) * delta_t;
                    }
                }
            }
        }

        //次にvzの更新
        for (var x = 1; x < WX - 1; x++) {
            for (var y = 1; y < WY - 1; y++) {
                for (var z = 1; z < WZ - 1; z++) {

                    var u = (vx[x, y, z - 1] + vx[x + 1, y, z - 1] + vx[x, y, z] + vx[x + 1, y, z]) * 0.25;
                    var v = (vy[x, y - 1, z] + vy[x, y - 1, z + 1] + vy[x, y, z] + vy[x, y, z + 1]) * 0.25;
                    var w = vz[x, y, z];

                    //( ｕ>=0かつｖ>=0かつ w >= 0の場合 )
                    if (u >= 0.0 && v >= 0.0 && w >= 0.0) {
                        vz_after[x, y, z] = vz[x, y, z] - u * (vz[x, y, z] - vz[x - 1, y, z]) * delta_t - v * (vz[x, y, z] - vz[x, y - 1, z]) * delta_t - w * (vz[x, y, z] - vz[x, y, z - 1]) * delta_t;
                    }
                    //( ｕ<0かつｖ>=0かつ w >= 0の場合 )
                    if (u < 0.0 && v >= 0.0 && w >= 0.0) {
                        vz_after[x, y, z] = vz[x, y, z] - u * (vz[x + 1, y, z] - vz[x, y, z]) * delta_t - v * (vz[x, y, z] - vz[x, y - 1, z]) * delta_t - w * (vz[x, y, z] - vz[x, y, z - 1]) * delta_t;
                    }
                    //( ｕ>=0かつｖ<0かつ w >= 0の場合 )
                    if (u >= 0.0 && v < 0.0 && w >= 0.0) {
                        vz_after[x, y, z] = vz[x, y, z] - u * (vz[x, y, z] - vz[x - 1, y, z]) * delta_t - v * (vz[x, y + 1, z] - vz[x, y, z]) * delta_t - w * (vz[x, y, z] - vz[x, y, z - 1]) * delta_t;
                    }
                    //( ｕ<0かつｖ<0かつ w => 0の場合 )
                    if (u < 0.0 && v < 0.0 && w >= 0.0) {
                        vz_after[x, y, z] = vz[x, y, z] - u * (vz[x + 1, y, z] - vz[x, y, z]) * delta_t - v * (vz[x, y + 1, z] - vz[x, y, z]) * delta_t - w * (vz[x, y, z] - vz[x, y, z - 1]) * delta_t;
                    }

                    //( ｕ>=0かつｖ>=0かつ w < 0の場合 )
                    if (u >= 0.0 && v >= 0.0 && w < 0.0) {
                        vz_after[x, y, z] = vz[x, y, z] - u * (vz[x, y, z] - vz[x - 1, y, z]) * delta_t - v * (vz[x, y, z] - vz[x, y - 1, z]) * delta_t - w * (vz[x, y, z + 1] - vz[x, y, z]) * delta_t;
                    }
                    //( ｕ<0かつｖ>=0かつ w < 0の場合 )
                    if (u < 0.0 && v >= 0.0 && w < 0.0) {
                        vz_after[x, y, z] = vz[x, y, z] - u * (vz[x + 1, y, z] - vz[x, y, z]) * delta_t - v * (vz[x, y, z] - vz[x, y - 1, z]) * delta_t - w * (vz[x, y, z + 1] - vz[x, y, z]) * delta_t;
                    }
                    //( ｕ>=0かつｖ<0かつ w < 0の場合 )
                    if (u >= 0.0 && v < 0.0 && w < 0.0) {
                        vz_after[x, y, z] = vz[x, y, z] - u * (vz[x, y, z] - vz[x - 1, y, z]) * delta_t - v * (vz[x, y + 1, z] - vz[x, y, z]) * delta_t - w * (vz[x, y, z + 1] - vz[x, y, z]) * delta_t;
                    }
                    //( ｕ<0かつｖ<0かつ w < 0の場合 )
                    if (u < 0.0 && v < 0.0 && w < 0.0) {
                        vz_after[x, y, z] = vz[x, y, z] - u * (vz[x + 1, y, z] - vz[x, y, z]) * delta_t - v * (vz[x, y + 1, z] - vz[x, y, z]) * delta_t - w * (vz[x, y, z + 1] - vz[x, y, z]) * delta_t;
                    }
                }
            }
        }
        Array.Copy(vx_after, vx, vx.Length);
        Array.Copy(vy_after, vy, vy.Length);
        Array.Copy(vz_after, vz, vz.Length);
    }

    /// <summary>
    /// 粘性
    /// </summary>
    void viscosity() {
        for (var x = 1; x < WX - 1; x++) {
            for (var y = 1; y < WY - 1; y++) {
                for (var z = 1; z < WZ - 1; z++) {
                    vx_after[x, y, z] = vx[x, y, z] - 1.0 / Re * (vx[x + 1, y, z] + vx[x, y + 1, z] + vx[x, y, z + 1] + vx[x - 1, y, z] + vx[x, y - 1, z] + vx[x, y, z - 1]) * delta_t;
                    vy_after[x, y, z] = vy[x, y, z] - 1.0 / Re * (vy[x + 1, y, z] + vy[x, y + 1, z] + vy[x, y, z + 1] + vy[x - 1, y, z] + vy[x, y - 1, z] + vy[x, y, z - 1]) * delta_t;
                    vz_after[x, y, z] = vz[x, y, z] - 1.0 / Re * (vz[x + 1, y, z] + vz[x, y + 1, z] + vz[x, y, z + 1] + vz[x - 1, y, z] + vz[x, y - 1, z] + vz[x, y, z - 1]) * delta_t;
                }
            }
        }
        Array.Copy(vx_after, vx, vx.Length);
        Array.Copy(vy_after, vy, vy.Length);
        Array.Copy(vz_after, vz, vz.Length);
    }

    /// <summary>
    /// 壁速度0に固定
    /// </summary>
    void set() {
        for (var x = 0; x < WX; x++) {
            for (var y = 0; y < WY; y++) {
                for (var z = 0; z < WZ; z++) {
                    if (x == 0 || x == (WX - 1) || y == 0 || y == (WY - 1) || z == 0 || z == (WZ - 1)) {
                        vx[x, y, z] = 0.0;
                        vx[x + 1, y, z] = 0.0;
                        vy[x, y, z] = 0.0;
                        vy[x, y + 1, z] = 0.0;
                        vz[x, y, z] = 0.0;
                        vz[x, y, z + 1] = 0.0;
                    }
                }
            }
        }
        for (var i = 0; i < block_positions.Length; i++) {
            var x = block_positions[i].x;
            var y = block_positions[i].y;
            var z = block_positions[i].z;
            vx[x, y, z] = 0.0;
            vx[x + 1, y, z] = 0.0;
            vy[x, y, z] = 0.0;
            vy[x, y + 1, z] = 0.0;
            vz[x, y, z] = 0.0;
            vz[x, y, z + 1] = 0.0;
        }
    }

    /// <summary>
    /// ダイバージェンス計算
    /// </summary>
    void div() {
        for (var x = 1; x < WX - 1; x++) {
            for (var y = 1; y < WY - 1; y++) {
                for (var z = 0; z < WZ - 1; z++) {
                    s[x, y, z] = (-vx[x, y, z] - vy[x, y, z] - vz[x, y, z] + vx[x + 1, y, z] + vy[x, y + 1, z] + vz[x, y, z + 1]) / delta_t;
                }
            }
        }
    }

    /// <summary>
    /// ポアソン方程式の項
    /// </summary>
    void poisson() {
        for (var c = 0; c < 10; c++) {
            for (var x = 1; x < WX - 1; x++) {
                for (var y = 1; y < WY - 1; y++) {
                    for (var z = 1; z < WZ - 1; z++) {
                        //もし壁なら、ijの圧力を代入
                        if (x == 1) p[x - 1, y, z] = p[x, y, z];
                        if (x == WX - 2) p[x + 1, y, z] = p[x, y, z];
                        if (y == 1) p[x, y - 1, z] = p[x, y, z];
                        if (y == WY - 2) p[x, y + 1, z] = p[x, y, z];
                        if (z == 1) p[x, y, z - 1] = p[x, y, z];
                        if (z == WZ - 2) p[x, y, z + 1] = p[x, y, z];
                        //ここがSOR
                        p[x, y, z] = (1.0 - omega) * p[x, y, z] + omega / 6.0 * (p[x - 1, y, z] + p[x + 1, y, z] + p[x, y - 1, z] + p[x, y + 1, z] + p[x, y, z - 1] + p[x, y, z + 1] - s[x, y, z]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 修正
    /// </summary>
    void rhs() {
        for (var x = 1; x < WX - 1; x++) {
            for (var y = 1; y < WY - 1; y++) {
                for (var z = 1; z < WZ - 1; z++) {
                    vx[x, y, z] -= (p[x, y, z] - p[x - 1, y, z]) * delta_t;
                    vy[x, y, z] -= (p[x, y, z] - p[x, y - 1, z]) * delta_t;
                    vz[x, y, z] -= (p[x, y, z] - p[x, y, z - 1]) * delta_t;
                }
            }
        }
    }

    private int counter = 0;

    /// <summary>
    /// 可視化
    /// </summary>
    void view() {
        // 新しい粒子の生成
        for (var i = 0; i < 1; i++) {
            rys[counter] = new Vector3(
                UnityEngine.Random.value * (1.0f * (float)(WX - 2)) + 1.0f,
                UnityEngine.Random.value * (1.0f * (float)(WY - 2)) + 1.0f,
                UnityEngine.Random.value * (1.0f * (float)(WZ - 2)) + 1.0f
            );
            counter = (counter + 1) % rys_num;
        }
        
        // 粒子の更新
        for (var i = 0; i < rys_num; i++) {
            Vector3 velocity = CalculateVelocity(rys[i]);
            
            rys[i] += velocity;

            if (IsOutOfBounds(rys[i])) {
                rys[i] = GenerateNewParticle();
            }

            UpdateParticleVisual(i, velocity);
        }
    }

    /// <summary>
    /// 指定された位置での速度を計算する
    /// </summary>
    /// <param name="position">計算する位置</param>
    /// <returns>計算された速度ベクトル</returns>
    public Vector3 CalculateVelocity(Vector3 position) {
        var xx = (double)Mathf.Clamp(position.x, 0.0f, 1.0f * WX - 1.1f);
        var yy = (double)Mathf.Clamp(position.y, 0.0f, 1.0f * WY - 1.1f);
        var zz = (double)Mathf.Clamp(position.z, 0.0f, 1.0f * WZ - 1.1f);
        var ixx = (int)xx;
        var iyy = (int)yy;
        var izz = (int)zz;
        var sxx = xx - ixx;
        var syy = yy - iyy;
        var szz = zz - izz;
        var im1 = (ixx + 1) % WX;
        var jm1 = (iyy + 1) % WY;
        var km1 = (izz + 1) % WZ;

        // 速度情報の線形補完。自分のいる座標での正確な速度を計算
        var xsp = (
            (((vx[ixx, iyy, izz] * (1.0 - sxx)) + (vx[im1, iyy, izz] * sxx)) * (1.0 - syy) + ((vx[ixx, jm1, izz] * (1.0 - sxx)) + (vx[im1, jm1, izz] * sxx)) * syy) * (1.0 - szz) +
            (((vx[ixx, iyy, km1] * (1.0 - sxx)) + (vx[im1, iyy, km1] * sxx)) * (1.0 - syy) + ((vx[ixx, jm1, km1] * (1.0 - sxx)) + (vx[im1, jm1, km1] * sxx)) * syy) * szz
        ) * delta_t;

        var ysp = (
            (((vy[ixx, iyy, izz] * (1.0 - sxx)) + (vy[im1, iyy, izz] * sxx)) * (1.0 - syy) + ((vy[ixx, jm1, izz] * (1.0 - sxx)) + (vy[im1, jm1, izz] * sxx)) * syy) * (1.0 - szz) +
            (((vy[ixx, iyy, km1] * (1.0 - sxx)) + (vy[im1, iyy, km1] * sxx)) * (1.0 - syy) + ((vy[ixx, jm1, km1] * (1.0 - sxx)) + (vy[im1, jm1, km1] * sxx)) * syy) * szz
        ) * delta_t;

        var zsp = (
            (((vz[ixx, iyy, izz] * (1.0 - sxx)) + (vz[im1, iyy, izz] * sxx)) * (1.0 - syy) + ((vz[ixx, jm1, izz] * (1.0 - sxx)) + (vz[im1, jm1, izz] * sxx)) * syy) * (1.0 - szz) +
            (((vz[ixx, iyy, km1] * (1.0 - sxx)) + (vz[im1, iyy, km1] * sxx)) * (1.0 - syy) + ((vz[ixx, jm1, km1] * (1.0 - sxx)) + (vz[im1, jm1, km1] * sxx)) * syy) * szz
        ) * delta_t;

        return new Vector3((float)xsp, (float)ysp, (float)zsp);
    }

    /// <summary>
    /// 粒子が境界外にあるかどうかをチェック
    /// </summary>
    private bool IsOutOfBounds(Vector3 position) {
        return (position.x >= (1.0 * WX - 1.1)) ||
               (position.y >= (1.0 * WY - 1.1)) ||
               (position.z >= (1.0 * WZ - 1.1)) ||
               (position.x < 1.1) ||
               (position.y < 1.1) ||
               (position.z < 1.1);
    }

    /// <summary>
    /// 新しい粒子の位置を生成
    /// </summary>
    private Vector3 GenerateNewParticle() {
        return new Vector3(
            Random.value * (1.0f * (float)(WX - 2)) + 1.0f,
            Random.value * (1.0f * (float)(WY - 2)) + 1.0f,
            Random.value * (1.0f * (float)(WZ - 2)) + 1.0f
        );
    }

    /// <summary>
    /// 粒子の視覚的な更新
    /// </summary>
    /// <summary>
    /// Transform.positionから計算用Vector3に変換する
    /// </summary>
    public Vector3 TransformPositionToCalculationVector3(Vector3 transformPosition)
    {
        return new Vector3(
            (transformPosition.x / scale.x) + (WX * 0.5f) - offset.x,
            (transformPosition.y / scale.y) + (WY * 0.5f) - offset.y,
            (transformPosition.z / scale.z) + (WZ * 0.5f) - offset.z
        );
    }

    /// <summary>
    /// 計算用Vector3からTransform.positionに変換する
    /// </summary>
    public Vector3 CalculationVector3ToTransformPosition(Vector3 calculationVector3)
    {
        return new Vector3(
            (calculationVector3.x - (WX * 0.5f) + offset.x) * scale.x,
            (calculationVector3.y - (WY * 0.5f) + offset.y) * scale.y,
            (calculationVector3.z - (WZ * 0.5f) + offset.z) * scale.z

        );
    }

    /// <summary>
    /// 粒子の視覚的な更新
    /// </summary>
    private void UpdateParticleVisual(int index, Vector3 velocity) {
        var speed = velocity.magnitude;
        nsobject[index].transform.position = CalculationVector3ToTransformPosition(rys[index]);
        nsobject[index].transform.localScale = Vector3.one * (min_size + speed * size_mul);
    }
    

}
