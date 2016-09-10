/*==== EasyFramerateCounter.cs ====================================================
 * Class for handling multi-line, multi-color debugging messages with the new UI.
 * Author: Alterego Games
 * Version 0.5 July 14th, 2015
 *
 * How To Use:
 * Drop the prefab "Easy Debug Console" into the root of your scene and press "Play".
 * To Debug, Use the standard Debug.Log() and Debug.LogWarning, like you are used to!
 * 
 * EXAMPLE SCENE
 * 
 * - INTERFACE FUNCTIONS -
 * 
 * isVisible (true,false)  Toggles the visibility of the output. 
 * 
 * Clear() Clears all messages
 * 
 * Collapse() Collapses the same messages
 * 
 *  - INSPECTOR TOGGLES -
 * 
 * KeyCode Default key to toggle visibility
 * 
 * Visible (Toggle) Toggles the visibility of the output.
 * 
 * Collapse (Toggle) Toggles the Collapse functionality
 * 
 * =========================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EasyFramerateCounter : MonoBehaviour
{
    [Header("Settings")]
    //Settings
    public bool PcSpecs = true;				// PC Specs bool
    public bool QualitySpecs = true;		// Quality Settings bool
    public bool visible = true;				// visiblity bool
    public float targetFrameRate = 30f;		// Target Frame Rate

    [Header("Toggle Key")]
    //Header
    public KeyCode keyCode;					// Custom visibility toggle

    [Header("UI Elements")]
    //UI
    public Text FramerateCounterText;					// UI text Element

    //Privates
    private float deltaTime = 0.0f;
    private float averageCounter = 0;
    private int averageFps = 0;
    private int maxFps = 0;
    private int minFps = 0;
    private int count = 0;
    private string pcSpecs;
    private string qualitySettings;

    //Quality Settings
    private string[] qualityLevels;

    void Start()
    {
        Initialisation();
    }

    void Update()
    {
        KeyToggle();

        if (visible)
        {
            Build();
        }
    }

    //---------- void Initialisation() ------
    //Set PC & Quality specs
    //--------------------------------------------------------------
    void Initialisation()
    {
        pcSpecs = "CPU: " + SystemInfo.processorType + " " + SystemInfo.processorCount + " cores\n";
        pcSpecs += "RAM: " + SystemInfo.systemMemorySize + " MB\n";
        pcSpecs += "GPU: " + SystemInfo.graphicsDeviceName + "VRAM :" + SystemInfo.graphicsMemorySize;

        qualityLevels = QualitySettings.names;
        qualitySettings = "Quality Settings: " + qualityLevels[QualitySettings.GetQualityLevel()] + "\n";
        qualitySettings += "Display: " + Screen.currentResolution;
    }


    //---------- void Build() ------
    //Build the string (runs in Update)
    //--------------------------------------------------------------
    private void Build()
    {
        int _fps = (int)GetFrameRate();
        SetOtherFPS(_fps);

        Display(GetSettings(), _fps);
    }

    //---------- void GetFrameRate() ----------------------
    // Get current Framerate
    //--------------------------------------------------------------
    private float GetFrameRate()
    {
        //FPS
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float _fps = 1.0f / deltaTime;
        return _fps;
    }

    //---------- void SetOtherFPS() ----------------------
    // Set min,max & average FPS
    //--------------------------------------------------------------
    private void SetOtherFPS(int _fps)
    {
        count++;
        //Wait for proper Initialisation
        if (count == 50)
        {
            minFps = _fps;
        }
        if (count > 10)
        {
            averageCounter += _fps;

            if (_fps > maxFps)
            {
                maxFps = _fps;
            }
            else if (_fps < minFps)
            {
                minFps = _fps;
            }

        }
        else
        {
            averageCounter += _fps;
        }

        //Reset FPS for int maximum
        if (averageFps > 1000000000)
        {
            averageFps = 0;
            count = 0;
        }
        averageFps = (int)(averageCounter / count);
    }

    //---------- void GetSettings() ----------------------
    // Get settings 
    //--------------------------------------------------------------
    private string GetSettings()
    {
        string _toDisplay = "";
        if (PcSpecs)
        {
            _toDisplay += pcSpecs;
        }
        if (QualitySpecs)
        {
            _toDisplay += "\n\n" + qualitySettings + "\n\n";
        }
        return _toDisplay;
    }

    //---------- void Display() ----------------------
    // Display everything
    //--------------------------------------------------------------
    private void Display(string _toDisplay, int _fps)
    {
        //Display
        string _color;
        if (_fps < targetFrameRate)
        {
            _color = "<color=#ff0000ff>";
        }
        else if (_fps == targetFrameRate)
        {
            _color = "<color=#ffff00ff>";
        }
        else
        {
            _color = "<color=#00ff00ff>";
        }

        _toDisplay += "FPS: " + _color + "" + _fps + "</color> AVG: " + _color + "" + averageFps + "</color>\n" + "MAX: " + _color + "" + maxFps + "</color> MIN: " + _color + "" + minFps + "</color>";
        FramerateCounterText.text = _toDisplay;
    }

    //---------- void KeyToggle() ----------------------
    // Toggle visibility On/Off with custom keybutton See: keyCode
    //--------------------------------------------------------------
    private void KeyToggle()
    {
        if (Input.GetKeyUp(keyCode))
        {
            if (!visible)
            {
                visible = true;
                Initialisation();
            }
            else
            {
                visible = false;
                FramerateCounterText.text = "";
            }
        }
    }
}