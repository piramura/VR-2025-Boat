using System;
using UnityEngine;

public class SerialSend : MonoBehaviour
{
    public SerialHandler serialHandler;
    int i = 0;

    void FixedUpdate()
    {
        i++;

        // 状態確認用の間引きログ（必要なら回数を調整）
        if (i % 100 == 0)
        {
            Debug.LogFormat("[SerialSend] FixedUpdate tick: i={0}, time={1:F3}", i, Time.time);
        }

        if (i > 999)
        {
            if (serialHandler == null)
            {
                Debug.LogError("[SerialSend] serialHandler is null. Assign the SerialHandler in the Inspector.");
            }
            else
            {
                try
                {
                    Debug.Log("[SerialSend] Sending: \"1\"");
                    serialHandler.Write("1");
                    Debug.Log("[SerialSend] Sent successfully.");
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("[SerialSend] Write failed: {0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
            i = 0;
        }
    }
}