using System.Collections;
using UnityEngine;

public static class CommonInfoManager
{
    public static string ROOM_ID;
    public static string ROOM_URL = "https://github.com/dena-autumn-2024-g/protobuf/tree/main";
    public static WaterRingStreamClient CLIENT = new();
    public static int NUM_PLAYER;

    // スコアがここに格納される
    public static bool END_GAME = false;
    public static int SCORE_LEFT_TIME;
    public static int SCORE_RING_COUNT;
    public static int SCORE_POINT;
}