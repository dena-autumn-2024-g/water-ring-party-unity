using UnityEngine;

public class CommonInfoInitializer : MonoBehaviour
{
    private static CommonInfoInitializer _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
        InitializeCommonInfoManager();
    }

    private void InitializeCommonInfoManager()
    {
        CommonInfoManager.ROOM_ID = "";
        CommonInfoManager.NUM_PLAYER = 0;
        //CommonInfoManager.SCORE_LEFT_TIME = 0;
        CommonInfoManager.SCORE_RING_COUNT = new int[10];
        //CommonInfoManager.SCORE_POINT = 0;
        CommonInfoManager.END_GAME = false;
    }
}
