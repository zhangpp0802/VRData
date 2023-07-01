using System;
using UnityEngine;

public class DataClient: MonoBehaviour
{
    private DataRequest dataRequestor;

    private void Start() => InitializeServer();

    public void InitializeServer()
    {
        dataRequestor = new DataRequest();
        dataRequestor.Start();
    }

    public void Predict(float[] input, Action<int[]> onOutputReceived, Action<Exception> fallback)
    {
        dataRequestor.SetOnTextReceivedListener(onOutputReceived, fallback);
        dataRequestor.SendInput(input);
    }

    private void OnDestroy()
    {
        dataRequestor.Stop();
    }
}