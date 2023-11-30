// using UnityEngine;
//
// public class AudioCtrl
// {
//     public delegate void EventCallback();
//
//     public static void SoundCtrl(bool bSwitch)
//     {
//         WAudioMgr.Inst.PlaySound(bSwitch ? WAudioMgr.SoundOn : WAudioMgr.SoundOff);
//     }
//     public static void VoiceCtrl(bool bSwitch)
//     {
//         WAudioMgr.Inst.PlaySound(bSwitch ? WAudioMgr.VoiceOn : WAudioMgr.VoiceOff);
//     }
//     public static void MusicCtrl(bool bSwitch)
//     {
//         WAudioMgr.Inst.PlaySound(bSwitch ? WAudioMgr.MusicOn : WAudioMgr.MusicOff);
//     }
//
//     public static void SetSoundVolume(float volume)
//     {
//         WAudioMgr.Inst.SetSoundVolume(volume);
//     }
//
//     public static void SetVoiceVolume(float volume)
//     {
//         WAudioMgr.Inst.SetVoiceVolume(volume);
//     }
//
//     public static void SetMusicVolume(float volume)
//     {
//         WAudioMgr.Inst.SetMusicVolume(volume);
//     }
//
//     public static uint PlaySound(string eventName, GameObject gameObj)
//     {
//         return WAudioMgr.Inst.PlaySound(eventName, gameObj);
//     }
//
//     public static uint PlaySound(string eventName)
//     {
//         return WAudioMgr.Inst.PlaySound(eventName);
//     }
//
//     public static uint PlaySound(string eventName, EventCallback cb)
//     {
//         return WAudioMgr.Inst.PlaySound(eventName, cb);
//     }
//
//     public static AKRESULT StopSound(string eventName, GameObject gameObj)
//     {
//         return WAudioMgr.Inst.StopSound(eventName, gameObj);
//     }
//
//     public static AKRESULT StopSound(string eventName)
//     {
//         return WAudioMgr.Inst.StopSound(eventName);
//     }
//
//     public static void SetState(string stateGroup, string state)
//     {
//         AkSoundEngine.SetState(stateGroup, state);
//     }
//
//     public static void SetSwitch(string switchGroup, string switchValue, GameObject gameObj)
//     {
//         AkSoundEngine.SetSwitch(switchGroup, switchValue, gameObj);
//     }
//
//     public static void SetRTPCValue(string RTPC, float parameter, GameObject gameObj)
//     {
//         AkSoundEngine.SetRTPCValue(RTPC, parameter, gameObj);
//     }
//
//     public static void SetRTPCValue(string RTPC, float parameter)
//     {
//         AkSoundEngine.SetRTPCValue(RTPC, parameter);
//     }
//
//     public static void SetDefaultAudioListener(GameObject gameObj)
//     {
//         WAudioMgr.Inst.AddDefaultAudioListener(gameObj);
//     }
// }
