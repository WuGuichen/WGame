using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using WGame.Res;

namespace Weapon
{
    public class WeaponViewServiceImplementation : MonoBehaviour, IWeaponViewService, Interactable
    {
        private WeaponEntity _entity;
        private GameEntity _character;

        private Transform _transform;

        public float lenUp;
        public float lenDown;
        public int checkNum = 4;
        public bool showLen = false;

        private HashSet<int> hittedList = new();

        private Vector3 pC;
        private Vector3 pD;

        private List<GameEntity> hitTargets;

        private float checkOffset = 0f;
        private Vector3 lastUpDir;
        private Vector3 lastOriginPosition;
        private RaycastHit[] _rayHits = new RaycastHit[12];
        private int hitLayer;
        private StickWeaponTrailEffect _trail;
        private bool hasTrail = false;

        private IFactoryService _factory;

        private float radius;
        private bool isInitted = false;

        private void Awake()
        {
            _transform = transform;
        }

        public IWeaponViewService RegisterEntity(WeaponEntity entity)
        {
            _entity = entity;
            // gameObject.Link(_entity);
            _trail = GetComponent<StickWeaponTrailEffect>();
            radius = GetComponent<SphereCollider>().radius;
            _factory = Contexts.sharedInstance.meta.factoryService.instance;
            // _trail.UseWithSRP = true;
            hasTrail = _trail;
            if(hasTrail)
            {
                _trail.enabled = false;
            }
            return this;
        }

        public Vector3 Position => _transform.position;

        public void Push()
        {
            WeaponMgr.Inst.PushWeaponObj(gameObject);
        }

        public void SetDropState(ref WeaponInfo info)
        {
            var trans = gameObject.transform;
            trans.localPosition = info.position + new Vector3(0,lenDown, 0);
            trans.localRotation = info.rotation;
            trans.localScale = info.scale;
            gameObject.layer = LayerMask.NameToLayer("DropItem");
        }

        private void UnLinkCharacter(GameEntity entity)
        {
            _entity.linkCharacter.Character.weaponService.service.OnDropWeapon(_entity.linkCharacter.Character, _entity);
            _entity.RemoveLinkCharacter();
            _character = null;
        }

        public void LinkToCharacter(GameEntity character)
        {
            if (_character != null)
            {
                Debug.LogError("有问题");
                return;
            }

            _entity.AddLinkCharacter(character);
            character.AddLinkWeapon(_entity);
            _character = character;
            
            if (character.gameViewService.service.Model.parent.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                gameObject.layer = LayerMask.NameToLayer("PlayerWeapon");
                hitLayer = 1 << LayerMask.NameToLayer("EnemyHitSensor");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("EnemyWeapon");
                hitLayer = 1 << LayerMask.NameToLayer("PlayerHitSensor");
            }
            _transform.SetParent(character.weaponService.service.WeaponHandle, false);
            _transform.localPosition = Vector3.zero;
            _transform.localRotation = Quaternion.identity;
            _transform.localScale = Vector3.one;
        }

        public void StartHitTargets()
        {
            hittedList.Clear();
            lastUpDir = transform.up;
            lastOriginPosition = transform.position + lastUpDir * lenUp;
            checkOffset = (-lenUp - lenDown) / (checkNum);
            _entity.isOpenSensor = true;
            if (hasTrail)
            {
                _trail.enabled = true;
            }
        }

        public void EndHitTargets()
        {
            // Debug.Log("关闭武器");
            _entity.isOpenSensor = false;
            if(hasTrail)
            {
                _trail.enabled = false;
            }
        }

        /// <param name="onlyThis">是否只有武器销毁，否则是因为武器持有者entity销毁</param>
        public void Destroy(bool onlyThis = false)
        {
            var link = gameObject.GetEntityLink();
            link?.Unlink();
            if (_entity.hasLinkCharacter)
            {
                var character = _entity.linkCharacter.Character;
                UnLinkCharacter(character);
                _character = null;
            }
            WeaponMgr.Inst.PushWeaponObj(gameObject);

            _entity = null;
        }

        private void OnDrawGizmos()
        {
            // if (showLen)
            // {
            //     pD = transform.position + transform.up * lenUp;
            //     pC = transform.position - transform.up * lenDown;
            //     if (pD != null && pC != null)
            //     {
            //         Gizmos.color = Color.green;
            //         Gizmos.DrawSphere(pC, 0.04f);
            //         Gizmos.color = Color.red;
            //         Gizmos.DrawSphere(pD, 0.04f);
            //     }
            //
            //     lastOriginPosition = transform.position + transform.right * 0.4f;
            //     lastUpDir = transform.up;
            //     checkOffset = -(lenUp + lenDown) / (checkNum);
            //     for (int i = 0; i < checkNum; i++)
            //     {
            //         Gizmos.DrawLine(lastOriginPosition + (i-1) * checkOffset * lastUpDir,
            //             transform.position + lastUpDir * lenUp + i * checkOffset * transform.up);
            //     }
            // }
            //
        }

        // 先用instanceID处理
        public void OnUpdateAttackSensor()
        {
            for (int i = 0; i < checkNum; i++)
            {
                var start = lastOriginPosition + i * checkOffset * lastUpDir;
                var end = transform.position + lastUpDir * lenUp + i * checkOffset * transform.up;
                var dir = end - start;
                var num = Physics.RaycastNonAlloc(new Ray(start, dir), _rayHits, dir.magnitude, hitLayer);
                Oddworm.Framework.DbgDraw.Ray(start, dir, Color.red, 4f);
                for (int j = 0; j < num; j++)
                {
                    var tar = _rayHits[j];
                    if (tar.collider != null)
                    {
                        var colliderID = tar.collider.GetInstanceID();
                        if (!hittedList.Contains(colliderID))
                        {
                            hittedList.Add(colliderID);
                            // var gameView = tar.transform.GetComponentInParent<IGameViewService>();
                            if (EntityUtils.TryGetEntitySensorMono(colliderID, out var sensorMono))
                            {
                                var tarCharacter = sensorMono.Entity;

                                var info = new ContactInfo();
                                info.pos = tar.point;
                                info.part = sensorMono.PartType;
                                info.dir = dir;
                                // 受击处理
                                info.entity = tarCharacter;
                                ActionHelper.DoHitTarget(_character, info);
                                // 击中处理
                                info.entity = _character;
                                ActionHelper.DoGotHit(tarCharacter, info);
                            }
                        }
                    }
                }
            }

            lastUpDir = transform.up;
            lastOriginPosition = transform.position + lastUpDir * lenUp;
        }

        public void Interact(GameEntity entity)
        {
            // var character = entity;
            // if (character.hasLinkWeapon)
            // {
            //     var model = character.gameViewService.service.Model;
            //     _factory.SetWeaponDrop(character.linkWeapon.Weapon
            //         , model.position + model.forward, Quaternion.identity, Vector3.one);
            //     _factory.SetWeaponEquipTo(_entity, character);
            // }
            // else
            // {
            //     _factory.SetWeaponEquipTo(_entity, character);
            // }
        }

        public Vector3 TagPos => _transform.position + new Vector3(0, radius, 0);
        public int UID => _entity.entityID.id;
    }

    public struct ContactInfo
    {
        public Vector3 pos;
        public Vector3 dir;
        public int count;
        public GameEntity entity;
        public EntityPartType part;
    }
}