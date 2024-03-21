// 数据缓存，数据逻辑操作

using WGame.Attribute;
using WGame.Runtime;

namespace WGame.UI
{
	public class CharacterModel : Runtime.Singleton<CharacterModel>
	{
		private int _currentControlledCharacterID = 0;

		public int currentControlledCharacterID
		{
			get => _currentControlledCharacterID;
			set
			{
				beforeControlledCharacterID = currentControlledCharacterID;
				_currentControlledCharacterID = value;
			}
		}
		public int beforeControlledCharacterID { get; private set; }
		
		public CharacterModel()
		{
			EventCenter.AddListener(EventDefine.OnControlCharacterChanged, OnCharacterChanged);	
		}

		private void OnCharacterChanged(TAny ctx)
		{
			int id = ctx.AsInt();
			var entity = Contexts.sharedInstance.game.GetEntityWithEntityID(beforeControlledCharacterID);
			if (entity != null && entity.hasAttribute)
			{
				var attribute = entity.attribute.value;
				attribute.CancelEvent(WAttrType.CurHP, OnHPChanged);
				attribute.CancelEvent(WAttrType.MaxHP, OnHPChanged);
				attribute.CancelEvent(WAttrType.MaxHP, OnMPChanged);
				attribute.CancelEvent(WAttrType.CurHP, OnMPChanged);
				attribute.CancelEvent(WAttrType.ATK, OnATKChanged);
				attribute.CancelEvent(WAttrType.DEF, OnDEFChanged);
			}
			entity = Contexts.sharedInstance.game.GetEntityWithEntityID(currentControlledCharacterID);
			if (entity != null && entity.hasAttribute)
			{
				var attribute = entity.attribute.value;
				attribute.RegisterEvent(WAttrType.CurHP, OnHPChanged);
				attribute.RegisterEvent(WAttrType.MaxHP, OnHPChanged);
				attribute.RegisterEvent(WAttrType.MaxHP, OnMPChanged);
				attribute.RegisterEvent(WAttrType.CurHP, OnMPChanged);
				attribute.RegisterEvent(WAttrType.ATK, OnATKChanged);
				attribute.RegisterEvent(WAttrType.DEF, OnDEFChanged);
			}
		}

		private void OnHPChanged(WaEventContext context)
		{
			
		}

		private void OnMPChanged(WaEventContext context)
		{
			
		}

		private void OnATKChanged(WaEventContext context)
		{
			
		}

		private void OnDEFChanged(WaEventContext context)
		{
			
		}

		public void SetCharacterAttr(GameEntity entity, int attrID, int value)
		{
			if (entity.hasAttribute)
			{
				entity.attribute.value.Set(attrID, value);
			}
		}

		public GameEntity GetCurControllerCharacter()
		{
			var entity = Contexts.sharedInstance.game.GetEntityWithEntityID(currentControlledCharacterID);
			return entity;
		}

		public void AddCharacterHP(GameEntity entity ,int value)
		{
			if (entity.hasAttribute == false)
				return;
			var attr = entity.attribute.value;
		}
	}
}
