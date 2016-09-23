using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public sealed class FPSMeter : MonoBehaviour
{

    Text text = null;
    float updateInterval = 0.5F;
    float accum = 0;
    int frames = 0;
    float timeleft;
    public float FPS { get; private set; }

    void Awake()
    {
        text = GetComponent<Text>();
    }

    public FPSMeter(float updateInterval = .5f)
    {
        this.updateInterval = updateInterval;
    }

    public void Update()
    {
        timeleft -= Time.unscaledDeltaTime;
        accum += 1f / Time.unscaledDeltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            FPS = accum / frames;
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
            text.text = FPS.ToString("0.0");
        }
    }
}
