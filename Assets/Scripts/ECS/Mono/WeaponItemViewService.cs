// using System;
// using Entitas;
// using UnityEngine;
//
// public class WeaponItemViewService :MonoBehaviour, IGameViewService, IEventListener, IWeaponColliderListener
// {
//     new private CapsuleCollider collider;
//     new private Rigidbody rigidbody;
//
//     private void Awake()
//     {
//         collider = GetComponent<CapsuleCollider>();
//         collider.isTrigger = true;
//         rigidbody = GetComponent<Rigidbody>();
//         rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
//     }
//
//     public Vector3 GetCameraPos()
//     {
//         return Position;
//     }
//
//     public Vector3 LocalizeVectorXY(Vector2 vector, bool isFocus = false)
//     {
//         return transform.right * vector.x + transform.forward * vector.y;
//     }
//
//     public Transform Model => transform;
//     public Vector3 Position => transform.position;
//     public Vector2 PlanarPosition { get; }
//
//     public GameEntity GetEntity()
//     {
//         throw new NotImplementedException();
//     }
//
//     public void Destroy()
//     {
//         
//     }
//
//     public void OnDead()
//     {
//         throw new NotImplementedException();
//     }
//
//     public void OnAlive()
//     {
//         throw new NotImplementedException();
//     }
//
//     public float Height => 0;
//     public Vector3 HeadPos => Vector3.zero;
//     public void BeFocused(bool value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public void Thrust()
//     {
//         throw new NotImplementedException();
//     }
//
//     public void OnUpdateMove(float deltaTime)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Transform FocusPoint => null;
//     public Transform RightHand { get; }
//
//     public void RegisterEventListener(Contexts contexts, IEntity entity)
//     {
//         var e = entity as GameEntity;
//         e.AddWeaponColliderListener(this);
//     }
//     
//     public void OnWeaponCollider(GameEntity entity, bool isOpen, bool isCCD)
//     {
//         collider.enabled = isOpen;
//         rigidbody.collisionDetectionMode = isCCD ? CollisionDetectionMode.ContinuousDynamic : CollisionDetectionMode.Discrete;
//     }
//     
// }
