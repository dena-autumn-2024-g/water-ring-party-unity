using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PollRingResultModelController : MonoBehaviour
{
    [SerializeField]
    private GameObject _pollModel;

    [SerializeField]
    private GameObject _ringPrefab;

    [SerializeField]
    private Camera _renderTextureCamera;

    public RenderTexture RenderTexture { get; private set; }

    public static int RESULT_MODEL_VIEW_LAYER = 6;

    public static int CONTROLLER_NUM = 0;


    public void Awake()
    {
        transform.position = new Vector3(100 * CONTROLLER_NUM, 0, 0);
        CreateRenderTexture();
        CONTROLLER_NUM++;
        DoPollBounceAnimation();
    }

    private void CreateRenderTexture()
    {
        RenderTexture = new RenderTexture(256, 512, 16);
        RenderTexture.format = RenderTextureFormat.ARGB32;
        RenderTexture.Create();
        _renderTextureCamera.targetTexture = RenderTexture;
    }

    private void DoPollBounceAnimation()
    {
        Vector3 startPos = _pollModel.transform.position + new Vector3(0, 1, 0);
        _pollModel.transform.position = startPos;

        _pollModel.transform
            .DOMoveY(_pollModel.transform.position.y - 1, 0.7f)
            .SetEase(Ease.OutBounce)
            .SetLoops(1, LoopType.Restart);
    }

    public void AddRing(int playerId)
    {
        var ring = Instantiate(_ringPrefab, transform).GetComponent<RingController>();
        ring.gameObject.layer = RESULT_MODEL_VIEW_LAYER;
        ring.SetPlayerId(playerId);

        float noiseRange = 0.2f;
        float randomX = Random.Range(-noiseRange, noiseRange);
        float randomZ = Random.Range(-noiseRange, noiseRange);

        ring.transform.localPosition = new Vector3(0 + randomX, 4, randomZ);
    }
}
