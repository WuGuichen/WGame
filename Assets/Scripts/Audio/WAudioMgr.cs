using System;
using System.Collections.Generic;
using UnityEngine;
using WGame.Runtime;

public class WAudioMgr : SingletonMono<WAudioMgr>
{
    private AkTerminator akTerminator;
    private GameObject _globalObject;
    private AkAudioListener _akAudioListener;

    private GameObject _emitterMusic;
    private GameObject _emitterVoice;
    private GameObject _emitterSound;
    
    #region VolumeControl
    private const float minVolume = 0.0f;
    private const float maxVolume = 100.0f;
    public static string SoundOn = "Sound_On";
    public static string SoundOff = "Sound_Off";
    public static string VoiceOn = "Voice_On";
    public static string VoiceOff = "Voice_Off";
    public static string MusicOn = "Music_On";
    public static string MusicOff = "Music_Off";
    public struct VolumeParam
    {
        public static string SoundVolume = "Sound_Volume";
        public static string VoiceVolume = "Voice_Volume";
        public static string MusicVolume = "Music_Volume";
    }
    public void SetSoundVolume(float volume)
    {
        volume = Mathf.Clamp(volume, minVolume, maxVolume);
        AkSoundEngine.SetRTPCValue(VolumeParam.SoundVolume, volume);
    }
    public void SetVoiceVolume(float volume)
    {
        volume = Mathf.Clamp(volume, minVolume, maxVolume);
        AkSoundEngine.SetRTPCValue(VolumeParam.VoiceVolume, volume);
    }
    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp(volume, minVolume, maxVolume);
        AkSoundEngine.SetRTPCValue(VolumeParam.MusicVolume, volume);
    }
    
    #endregion

    private void Awake()
    {
        _globalObject = GameObject.Find("WwiseGlobal");
        _emitterMusic = CreateEmitter("EmitterMusic");
        _emitterVoice = CreateEmitter("EmitterVoice");
        _emitterSound = CreateEmitter("EmitterSound");

        _akAudioListener = Camera.main.gameObject.GetComponent<AkAudioListener>();
        
        AkBankManager.LoadBank("Main", false, false);
        
        AddBasePath();
    }

    private GameObject CreateEmitter(string name)
    {
        if (_globalObject == null || string.IsNullOrEmpty(name))
            return null;
        var obj = new GameObject(name);
        obj.transform.parent = _globalObject.transform;
        return obj;
    }
    private void AddBasePath()
    {
#if UNITY_IPHONE || UNITY_ANDROID
        string fileNameBase = Application.persistentDataPath + "/" + "Audio/GeneratedSoundBanks" + "/";
        //#if UNITY_IPHONE
        //        fileNameBase += "iOS";
        //#elif UNITY_ANDROID
        //        fileNameBase += "Android";
        //#endif
        var result = AkSoundEngine.AddBasePath(fileNameBase);
        BDebug.Log($"添加WwiseBasePath:{fileNameBase}，添加结果:{result}");
#endif
    }
    void Callback(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        switch (in_type)
        {
            case AkCallbackType.AK_EndOfEvent:
                if (in_cookie != null)
                {
                    AudioCtrl.EventCallback cb = (AudioCtrl.EventCallback)in_cookie;
                    cb();
                }
                break;
            case AkCallbackType.AK_Marker:
                AkMarkerCallbackInfo info = in_info as AkMarkerCallbackInfo;
                Debug.Log(info.strLabel);
                break;
            case AkCallbackType.AK_MusicSyncBeat: //互动音乐节拍点事件

                break;
            default:
                //AkSoundEngine.LogError("Callback Type not march.");
                break;
        }
    }

    public uint PlaySound(string soundEvent)
    {
        uint playingID = AkSoundEngine.PostEvent(soundEvent, _globalObject);
        return playingID;
    }
    
    public uint PlaySound(string soundEvent, GameObject obj)
    {
        uint playingID = AkSoundEngine.PostEvent(soundEvent, obj);
        return playingID;
    }
    
    public uint PlaySound(uint soundEventId, GameObject obj)
    {
        uint playingID = AkSoundEngine.PostEvent(soundEventId, obj);
        return playingID;
    }
    public uint PlaySound(string eventName, AudioCtrl.EventCallback cb, GameObject gameObj = null)
    {
        uint playingID = AkSoundEngine.AK_INVALID_PLAYING_ID;
        if (gameObj == null)
            gameObj = _globalObject;
        if (!string.IsNullOrEmpty(eventName))
            playingID = AkSoundEngine.PostEvent(eventName, gameObj, (uint)AkCallbackType.AK_EndOfEvent, Callback, cb);
        return playingID;
    }
    
    public AKRESULT StopSound(string eventName, GameObject gameObj = null, int transitionDuration = 300, AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear)
    {
        if (!string.IsNullOrEmpty(eventName))
        {
            transitionDuration = Mathf.Clamp(transitionDuration, 0, 10000);
            if (gameObj == null)
                gameObj = _globalObject;
            if (gameObj != null)
            {
                AKRESULT result = AkSoundEngine.ExecuteActionOnEvent(eventName, AkActionOnEventType.AkActionOnEventType_Stop, gameObj, transitionDuration, curveInterpolation);
                return result;
            }
        }
        return AKRESULT.AK_Fail;
    }

    public AKRESULT StopSound(uint eventID, GameObject gameObj = null, int transitionDuration = 300,
         AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear)
    {
        transitionDuration = Mathf.Clamp(transitionDuration, 0, 10000);
        if (gameObj == null)
            gameObj = _globalObject;
        AKRESULT result = AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Stop, gameObj, transitionDuration, curveInterpolation);
        return result;
    }

    public uint StopSound()
    {
        uint playingID = AkSoundEngine.AK_INVALID_PLAYING_ID;
        playingID = AkSoundEngine.PostEvent("Stop_All_Sound", _globalObject);
        return playingID;
    }
    
    public void AddDefaultAudioListener(GameObject gameObj)
    {
        if (gameObj != null)
        {
            var listener = gameObj.AddComponent<AkAudioListener>();
            listener.isDefaultListener = true;
        }
    }
}
