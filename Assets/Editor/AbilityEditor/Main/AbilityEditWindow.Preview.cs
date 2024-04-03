using Animancer;
using UnityEditor;
using UnityEngine;
using WGame.Editor;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow
    {
        private float lastTime = 0f;
        private float deltaTime = 1f / 60;

        private AbilityStatusCharacter _abilityStatus;
        private GameObject _playerObject;
        private AnimationClip _animClip;
        private AnimancerComponent _anim;
        private float _animStartTime = 0f;
        private GameObject _camObj;
        private Camera _camera;
        private Vector3 _camPos = new Vector3(0,100,0);

        private void OnEnable()
        {
            _camObj = new GameObject("AbilityCamera");
            _camera = _camObj.AddComponent<Camera>();
            _camObj.transform.localPosition = _camPos;
            _camObj.transform.localRotation = Quaternion.Euler(new Vector3(18f, 0, 0));
            _camera.depth = 100;
            EditorApplication.update += OnEditorUpdate;
            GameAssetsMgr.Inst.InitInstance();
            var obj = GameAssetsMgr.Inst.LoadObject<GameObject>("/Character/BaseCharacter/BaseCharacter.prefab");
            _playerObject = Instantiate(obj).transform.GetChild(1).gameObject;
            _playerObject.transform.parent.localPosition = _camPos + new Vector3(-0.8f, -1.5f, 2.2f);
            _playerObject.transform.parent.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            _anim = _playerObject.GetComponentInChildren<AnimancerComponent>();
            _anim.Playable.PauseGraph();
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            if(_playerObject)
            {
                _anim = null;
                _animClip = null;
                DestroyImmediate(_playerObject.transform.parent.gameObject);
            }
            if(_camObj)
            {
                DestroyImmediate(_camObj);
            }
            DestroyImmediate(_camera);
        }

        private void OnEditorUpdate()
        {
            if (!Application.isPlaying && _playState == PlayState.Play)
            {
                Preview(Time.realtimeSinceStartup - lastTime);
                Repaint();
            }

            lastTime = Time.realtimeSinceStartup;
        }

        private void Preview(float fTick)
        {
            fTick *= _playSpeed;
            
            Tick(fTick);

            CurrentTime += fTick;

            if (CurrentTime >= Length)
            {
                CurrentTime = 0f;
                _playState = PlayState.Stop;
                ResetPreview(fTick);
            }

            // if (_animClip)
            // {
            //     _animClip.SampleAnimation(_playerObject, CurrentTime - _animStartTime);
            //     // _playerObject.transform.parent.
            // }
            _anim.Evaluate(fTick);
        }

        private void Tick(float fTick)
        {
            if (Ability == null)
            {
                return;
            }
            
            using (var itrGroup = itemTreeView.children.GetEnumerator())
            {
                while (itrGroup.MoveNext())
                {
                    using (var itrTrack = itrGroup.Current.children.GetEnumerator())
                    {
                        while (itrTrack.MoveNext())
                        {
                            var track = itrTrack.Current as TrackItem;
                            using (var itrEvent = track.eventList.GetEnumerator())
                            {
                                while (itrEvent.MoveNext())
                                {
                                    var ae = itrEvent.Current;
                                    if (!ae.hasExecute && ToMillisecond(CurrentTime) >= ae.eventProperty.TriggerTime)
                                    {
                                        ExecuteEvent(ae, fTick);
                                    }
                                    else if (ae.hasExecute)
                                    {
                                        if (ToMillisecond(CurrentTime) < ae.eventProperty.TriggerTime)
                                        {
                                            ae.hasExecute = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private void ExecuteEvent(ActorEvent evt, float fTick)
        {
            evt.hasExecute = true;
            switch (evt.eventProperty.EventType)
            {
                case EventDataType.PlayAnim:
                    {
                        var epa = evt.eventProperty.EventData as EventPlayAnim;
                        _animClip = GameAssetsMgr.Inst.LoadAnimClip(epa.AnimName);
                        var state = _anim.Play(_animClip, epa.TransDuration*0.001f, FadeMode.FromStart);
                        state.Time = epa.PlayOffsetStart*0.001f;
                        // _animStartTime = (epa.PlayOffsetStart + evt.eventProperty.TriggerTime ) * 0.001f;
                    }
                    break;
            }
        }

        private void ResetPreview(float fTick)
        {
            // reset event
            using (var itrGroup = itemTreeView.children.GetEnumerator())
            {
                while (itrGroup.MoveNext())
                {
                    using (var itrTrack = itrGroup.Current.children.GetEnumerator())
                    {
                        while (itrTrack.MoveNext())
                        {
                            var track = itrTrack.Current as TrackItem;
                            using (var itrEvent = track.eventList.GetEnumerator())
                            {
                                while (itrEvent.MoveNext())
                                {
                                    itrEvent.Current.hasExecute = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}