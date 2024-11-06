using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingController : MonoBehaviour
{
    public FluidSimulation fluidSimulation;
    [SerializeField] private float forceMultiplier = 100000f; // 力の倍率を調整するパラメータ
    private Rigidbody rb;

    public GameObject child1;
    public GameObject child2;

    public int TeamId { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetTeamId(int teamId)
    {
        TeamId = teamId;
        Renderer renderer = gameObject.GetComponent<Renderer>();
        renderer.material.color = IconColor.Colors[teamId];
        // エミッションの設定
        renderer.material.EnableKeyword("_EMISSION");
        renderer.material.SetColor("_EmissionColor", IconColor.Colors[teamId] * 0.8f);
    }
    
    void FixedUpdate()
    {
        // x軸とy軸の回転を0に固定
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        var pos1 = child1.transform.position;
        var pos2 = child2.transform.position;
        // 現在位置を計算用座標系に変換
        var inputVector1 = fluidSimulation.TransformPositionToCalculationVector3(pos1);
        var inputVector2 = fluidSimulation.TransformPositionToCalculationVector3(pos2);
        
        // その位置での速度を計算
        var velocity1 = fluidSimulation.CalculateVelocity(inputVector1);
        var velocity2 = fluidSimulation.CalculateVelocity(inputVector2);
        
        // 速度をワールド座標系に変換
        var worldVelocity1 = fluidSimulation.CalculationVector3ToTransformPosition(velocity1) - fluidSimulation.CalculationVector3ToTransformPosition(Vector3.zero);
        var worldVelocity2 = fluidSimulation.CalculationVector3ToTransformPosition(velocity2) - fluidSimulation.CalculationVector3ToTransformPosition(Vector3.zero);
        // Rigidbodyに力を加える（倍率を適用）
        rb.AddForceAtPosition(worldVelocity1 * forceMultiplier, pos1, ForceMode.Acceleration);
        rb.AddForceAtPosition(worldVelocity2 * forceMultiplier, pos2, ForceMode.Acceleration);
    }
}
