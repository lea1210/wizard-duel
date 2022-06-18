using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class debugDisplay : MonoBehaviour
{
    private bool pressed;
    public XRNode inputSource;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;

    public Text display;
    private string displayText = "";
    private int LogCounter = 0;
    private int firstLogIndex = 0;
    public MovementRecognizer recognizer;

    private Dictionary<string,string > log = new Dictionary<string,string>();
    private Dictionary<int, string> keyLog = new Dictionary<int, string>();

    public void Start()
    {
        display.text = "Log Text ...";

    }

    public void Update()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);
        pressed = isPressed;

        if(pressed){
            if (log.Count > 9)
            {
                foreach (int key in keyLog.Keys)
                {

                    if (key == firstLogIndex)
                    {
                        log.Remove(keyLog[key]);
                    }
                }
                firstLogIndex++;
            }
            Application.logMessageReceived += HandleLog;

            display.text = "";
            foreach (string key in log.Keys)
            {
                display.text = display.text + "\n" + key + ": " + log[key];
            }
        }


    }

    void onEnable()
    {
        
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        
        if (type == LogType.Log)
        {
            if (logString.Contains(":")){
                string[] parts = logString.Split(':');
                log[parts[0]] = parts[1];
                keyLog[LogCounter] = parts[0];
                LogCounter++;
            }

        }
    }

}
