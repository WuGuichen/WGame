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
        private RaycastHit[] _rayHits = new RaycastHit[8];
        private int hitLayer;
        private StickWeaponTrailEffect _trail;

        private IFactoryService _factory;

        private float radius;

        public IWeaponViewService RegisterEntity(WeaponEntity entity)
        {
            _entity = entity;
            // gameObject.Link(_entity);
            _trail = GetComponent<StickWeaponTrailEffect>();
            // _trail.UseWithSRP = true;
            _trail.enabled = false;
            radius = GetComponent<SphereCollider>().radius;
            _factory = Contexts.sharedInstance.meta.factoryService.instance;
            return this;
        }

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

        public void UnLinkCharacter(GameEntity entity, bool resetLocalMotion = false)
        {
            if (resetLocalMotion)
            {
                _character.linkMotion.Motion.motionService.service.ResetMotion();
            }
            _entity.linkCharacter.Character.RemoveLinkWeapon();
            _entity.RemoveLinkCharacter();
            _character = null;
            transform.parent = GameSceneMgr.Inst.genItemRoot;
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
            transform.SetParent(character.weaponService.service.WeaponHandle, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public void StartHitTargets()
        {
            hittedList = new HashSet<int>();
            lastUpDir = transform.up;
            lastOriginPosition = transform.position + lastUpDir * lenUp;
            checkOffset = (lenUp - lenDown) / (checkNum-1);
            _entity.isOpenSensor = true;
            _trail.enabled = true;
        }

        public void EndHitTargets()
        {
            // Debug.Log("关闭武器");
            _entity.isOpenSensor = false;
            _trail.enabled = false;
        }

        /// <param name="onlyThis">是否只有武器销毁，否则是因为武器持有者entity销毁</param>
        public void Destroy(bool onlyThis = false)
        {
            this.gameObject.Unlink();
            if (onlyThis)
            {
                // 只有武器entity Destroy
                if(_character.hasLinkWeapon)
                    _character.RemoveLinkWeapon();
                GameObject.Destroy(this.gameObject);
            }

            _entity = null;
        }

        private void OnDrawGizmos()
        {
            if (showLen)
            {
                pD = transform.position + transform.up * lenUp;
                pC = transform.position - transform.up * lenDown;
                if (pD != null && pC != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(pC, 0.04f);
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(pD, 0.04f);
                }

                lastOriginPosition = transform.position + transform.right * 0.4f;
                lastUpDir = transform.up;
                checkOffset = (lenUp - lenDown) / (checkNum - 1);
                for (int i = 0; i < checkNum - 1; i++)
                {
                    Gizmos.DrawLine(lastOriginPosition + i * checkOffset * lastUpDir,
                        transform.position + lastUpDir * lenUp + i * checkOffset * transform.up);
                }
            }

        }

        // todo 用WCollider优化
        // 先用instanceID处理
        public void OnUpdateAttackSensor()
        {
            for (int i = 0; i < checkNum - 1; i++)
            {
                var start = lastOriginPosition + i * checkOffset * lastUpDir;
                var end = transform.position + lastUpDir * lenUp + i * checkOffset * transform.up;
                var dir = end - start;
                _rayHits = new RaycastHit[8];
                Physics.RaycastNonAlloc(new Ray(start, dir), _rayHits, dir.magnitude, hitLayer);
                for (int j = 0; j < _rayHits.Length; j++)
                {
                    var tar = _rayHits[j];
                    if (tar.collider != null)
                    {
                        var colliderID = tar.collider.GetInstanceID();
                        if (!hittedList.Contains(colliderID))
                        {
                            hittedList.Add(colliderID);
                            // var gameView = tar.transform.GetComponentInParent<IGameViewService>();
                            if (EntityUtils.TryGetEntity(colliderID, out var tarCharacter))
                            {
                                // var tarCharacter = gameView.GetEntity();
                                var info = new ContactInfo();
                                info.pos = tar.point;
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

        public void Interact()
        {
            var character = EntityUtils.GetCameraEntity();
            if (character.hasLinkWeapon)
            {
                var model = character.gameViewService.service.Model;
                _factory.SetWeaponDrop(character.linkWeapon.Weapon
                    , model.position + model.forward, Quaternion.identity, Vector3.one);
                _factory.SetWeaponEquipTo(_entity, character);
            }
            else
            {
                _factory.SetWeaponEquipTo(_entity, character);
            }
        }

        public Vector3 TagPos => transform.position + new Vector3(0, radius, 0);
        public int UID => _entity.entityID.id;
    }

    public struct ContactInfo
    {
        public Vector3 pos;
        public Vector3 dir;
        public int count;
        public GameEntity entity;
    }
}