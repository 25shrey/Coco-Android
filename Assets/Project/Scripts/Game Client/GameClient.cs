using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameClient : MonoBehaviour
{
    [Header("Game Client Data")]
    [SerializeField] private bool isDevelopment = false;
    [Divider]
    [SerializeField] private bool useDiagnostics = false;
    [Divider]
    [SerializeField] private List<GameObject> objectsToDisable;
    
    [Divider("Diagnostics")]
    [Header("Diagnostics Canvas References")]
    [SerializeField] private Canvas diagnosticsCanvas;
    [SerializeField] private GraphicRaycaster diagnosticsGraphicRaycaster;
    [Divider]
    [Header("Diagnostics Text References")]
    [SerializeField] private TextMeshProUGUI GameTimeText;
    [SerializeField] private TextMeshProUGUI CPUNameText;
    [SerializeField] private TextMeshProUGUI GPUNameText;
    [SerializeField] private TextMeshProUGUI FPSText;
    [SerializeField] private TextMeshProUGUI FPSAverageText;
    [SerializeField] private TextMeshProUGUI GPUUsageText;
    [SerializeField] private TextMeshProUGUI RAMUsageText;
    private float updateInterval = 1.0f;
    private float lastInterval; // Last interval end time
    private float frames = 0; // Frames over current interval
    private float framesAveragetick = 0;
    private float framesAverage = 0.0f;
    private float bytesToMbConvertionVal = 0.000001f;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        if (!isDevelopment)
        {
            diagnosticsCanvas.enabled = false;
            diagnosticsGraphicRaycaster.enabled = false;
            // Production
            foreach (GameObject objects in objectsToDisable)
            {
                objects.SetActive(false);
            }

            Debug.unityLogger.logEnabled = false;
        }
        else
        {
            if (useDiagnostics)
            {
                diagnosticsCanvas.enabled = true;
                diagnosticsGraphicRaycaster.enabled = true;
            }
            Debug.unityLogger.logEnabled = true;
            CPUNameText.text = "CPU : "+ SystemInfo.processorType;
            GPUNameText.text = "GPU : " + SystemInfo.graphicsDeviceName;
        }
    }


    private void Update()
    {
        if (!isDevelopment) return;

        ++frames;

        var timeNow = Time.realtimeSinceStartup;

        if (timeNow > lastInterval + updateInterval)
        {
            float fps = frames / (timeNow - lastInterval);
            float ms = 1000.0f / Mathf.Max(fps, 0.00001f);

            ++framesAveragetick;
            framesAverage += fps;
            float fpsAv = framesAverage / framesAveragetick;

            int ramUsage = Convert.ToInt32((Profiler.GetTotalAllocatedMemoryLong() * bytesToMbConvertionVal) / (Profiler.GetTotalReservedMemoryLong() * bytesToMbConvertionVal) * 100);

            int gpuUsage = Convert.ToInt32((Profiler.GetAllocatedMemoryForGraphicsDriver() * bytesToMbConvertionVal / SystemInfo.graphicsMemorySize) * 100);

            GameTimeText.text = "Time : " + ms;
            FPSText.text = "FPS : " + fps;
            FPSAverageText.text = "AverageFPS : " + fpsAv;
            RAMUsageText.text = "RamMemoryUsage : " + ramUsage + "%";
            GPUUsageText.text = "GPUMemoryUsage: " + gpuUsage + "%";
            frames = 0;
            lastInterval = timeNow;
        }
    }
}
