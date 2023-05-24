using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationSettings : MonoBehaviour
{
    [Header("Video")] [SerializeField] private int m_ScreenWidth;
    [SerializeField] private int m_ScreenHeight;
    [SerializeField] private FullScreenMode m_ScreenFullscreenMode;
    [SerializeField] private int m_TargetFrameRate;
    [SerializeField] private float m_ScreenBrightness;
    [SerializeField] private int m_SleepTimeout;
    [SerializeField] private bool m_AutorotatePortrait;
    [SerializeField] private bool m_AutorotateLandscapeLeft;
    [SerializeField] private bool m_AutorotateLandscapeRight;

    private const string EXIST_SETTINGS_PROP = "ExistSettings";

    private const string SCREEN_WIDTH_PROP = "ScreenWidth";
    private const string SCREEN_HEIGHT_PROP = "ScreenHeight";
    private const string SCREEN_FULLSCREEN_MODE_PROP = "ScreenFullScreenMode";
    private const string SCREEN_TARGET_FRAME_RATE_PROP = "TargetFrameRate";
    private const string SCREEN_BRIGHTNESS_PROP = "ScreenBrightness";
    private const string SCREEN_SLEEP_TIMEOUT_PROP = "SleepTimeout";
    private const string SCREEN_AUTOROTATE_PORTRAIT_PROP = "AutorotatePortrait";
    private const string SCREEN_AUTOROTATE_LANDSCAPE_LEFT_PROP = "AutorotateLandscapeLeft";
    private const string SCREEN_AUTOROTATE_LANDSCAPE_RIGHT_PROP = "AutorotateLandscapeRight";

    private static bool s_bAlreadyCalled = false;

    void Start()
    {
        if (s_bAlreadyCalled == true)
            return;

        s_bAlreadyCalled = true;

#if UNITY_EDITOR
        // 에디터 실행인 경우, 무조건 inspector에서 설정한 설정으로 덮어씁니다.
        SaveSettings();
#else
        // 그렇지 않은 경우, 초기 실행 시 1회에 한해 현재 설정 값으로 설정을 저장합니다.
        bool _existSettings = PlayerPrefs.GetInt(EXIST_SETTINGS_PROP) != 0;
        if (_existSettings == false)
        {
            OverwriteFromCurrentContext();
            SaveSettings();
        }
#endif

        LoadSettings();
        ApplySettings();
    }

    public void OverwriteFromCurrentContext()
    {
        m_ScreenWidth = Screen.width;
        m_ScreenHeight = Screen.height;
        m_ScreenFullscreenMode = Screen.fullScreenMode;
        m_TargetFrameRate = Application.targetFrameRate;
        m_ScreenBrightness = Screen.brightness;
        m_SleepTimeout = Screen.sleepTimeout;
        m_AutorotatePortrait = Screen.autorotateToPortrait;
        m_AutorotateLandscapeLeft = Screen.autorotateToLandscapeLeft;
        m_AutorotateLandscapeRight = Screen.autorotateToLandscapeRight;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt(EXIST_SETTINGS_PROP, 1);

        PlayerPrefs.SetInt(SCREEN_WIDTH_PROP, m_ScreenWidth);
        PlayerPrefs.SetInt(SCREEN_HEIGHT_PROP, m_ScreenHeight);
        PlayerPrefs.SetInt(SCREEN_FULLSCREEN_MODE_PROP, (int)m_ScreenFullscreenMode);
        PlayerPrefs.SetInt(SCREEN_TARGET_FRAME_RATE_PROP, m_TargetFrameRate);
        PlayerPrefs.SetFloat(SCREEN_BRIGHTNESS_PROP, m_ScreenBrightness);
        PlayerPrefs.SetInt(SCREEN_SLEEP_TIMEOUT_PROP, m_SleepTimeout);
        PlayerPrefs.SetInt(SCREEN_AUTOROTATE_PORTRAIT_PROP, m_AutorotatePortrait ? 1 : 0);
        PlayerPrefs.SetInt(SCREEN_AUTOROTATE_LANDSCAPE_LEFT_PROP, m_AutorotateLandscapeLeft ? 1 : 0);
        PlayerPrefs.SetInt(SCREEN_AUTOROTATE_LANDSCAPE_RIGHT_PROP, m_AutorotateLandscapeRight ? 1 : 0);
    }

    public void LoadSettings()
    {
        m_ScreenWidth = PlayerPrefs.GetInt(SCREEN_WIDTH_PROP);
        m_ScreenHeight = PlayerPrefs.GetInt(SCREEN_HEIGHT_PROP);
        m_ScreenFullscreenMode = (FullScreenMode)PlayerPrefs.GetInt(SCREEN_FULLSCREEN_MODE_PROP);
        m_TargetFrameRate = PlayerPrefs.GetInt(SCREEN_TARGET_FRAME_RATE_PROP);
        m_ScreenBrightness = PlayerPrefs.GetFloat(SCREEN_BRIGHTNESS_PROP);
        m_SleepTimeout = PlayerPrefs.GetInt(SCREEN_SLEEP_TIMEOUT_PROP);
        m_AutorotatePortrait = PlayerPrefs.GetInt(SCREEN_AUTOROTATE_PORTRAIT_PROP) != 0;
        m_AutorotateLandscapeLeft = PlayerPrefs.GetInt(SCREEN_AUTOROTATE_LANDSCAPE_LEFT_PROP) != 0;
        m_AutorotateLandscapeRight = PlayerPrefs.GetInt(SCREEN_AUTOROTATE_LANDSCAPE_RIGHT_PROP) != 0;
    }

    public void ApplySettings()
    {
        Screen.SetResolution(m_ScreenWidth, m_ScreenHeight, m_ScreenFullscreenMode, m_TargetFrameRate);
        Screen.brightness = m_ScreenBrightness;
        Screen.sleepTimeout = m_SleepTimeout;
        Screen.autorotateToPortrait = m_AutorotatePortrait;
        Screen.autorotateToLandscapeLeft = m_AutorotateLandscapeLeft;
        Screen.autorotateToLandscapeRight = m_AutorotateLandscapeRight;
    }

    public override string ToString()
    {
        return
            $"{base.ToString()}, {nameof(m_ScreenWidth)}: {m_ScreenWidth}, {nameof(m_ScreenHeight)}: {m_ScreenHeight}, {nameof(m_ScreenFullscreenMode)}: {m_ScreenFullscreenMode}, {nameof(m_TargetFrameRate)}: {m_TargetFrameRate}, {nameof(m_ScreenBrightness)}: {m_ScreenBrightness}, {nameof(m_SleepTimeout)}: {m_SleepTimeout}, {nameof(m_AutorotatePortrait)}: {m_AutorotatePortrait}, {nameof(m_AutorotateLandscapeLeft)}: {m_AutorotateLandscapeLeft}, {nameof(m_AutorotateLandscapeRight)}: {m_AutorotateLandscapeRight}";
    }
}