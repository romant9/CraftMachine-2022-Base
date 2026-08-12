using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ChargeMeterModel : TWDModelObject
	{
		public const string chargeMeterValueChanged = "chargeMeterValueChanged";

		public const string ChargeIconFilled = "Ui_Charge_Point_Fill_Yellow";

		public const string ChargeIconAvailable = "Ui_Charge_Point_Fill_Green";

		public const string ChargeIconFilledEX = "Ui_Charge_Point_Fill_Orange";

		public const string ChargeIconEmpty = "Ui_Charge_Point_Bg";

		private int chargeLevel;

		private bool chargeEnabled;

		private int lastChargeConsume;

		[IgnoreModelProperty]
		public ActorModel Actor { get; set; }

		public int ChargeLevel
		{
			get
			{
				return chargeLevel;
			}
			set
			{
				if (value != chargeLevel)
				{
					int num = chargeLevel;
					chargeLevel = value;
					NotifyChange("chargeMeterValueChanged", num);
				}
			}
		}

		[JsonIgnore]
		public bool ChargeAvailable => ChargeLevel >= ChargePointCost;

		[JsonIgnore]
		public int MaxLevel => ChargePointCost + EXMaxLevel;

		public bool ChargeEnabled
		{
			get
			{
				return chargeEnabled;
			}
			set
			{
				if (Actor.GetChargeEquipment() != null && ChargeAvailable)
				{
					chargeEnabled = value;
				}
				else
				{
					chargeEnabled = false;
				}
			}
		}

		[JsonIgnore]
		public int ChargePointCost => Actor.GetChargeEquipment()?.Ability.Definition.ChargePointCost ?? 0;

		[JsonIgnore]
		public int EXMaxLevel
		{
			get
			{
				if (!started)
				{
					return 0;
				}
				FixedPoint value = 0.0;
				if (Actor.HasAnyLevelTrait("LeaderBuffOverload"))
				{
					base.manager.Player.AbilityManager.VisitParameter("LeaderBuffOverload_ChargePointLimitNum", ref value, Actor);
				}
				else if (Actor.HasAnyLevelTrait("BaseOverload"))
				{
					base.manager.Player.AbilityManager.VisitParameter("BaseLeaderBuffOverload_ChargePointLimitNum", ref value, Actor);
				}
				return (int)value;
			}
		}

		public int LastChargeConsume
		{
			get
			{
				return lastChargeConsume;
			}
			set
			{
				int num = value;
				if (value <= 0)
				{
					num = 0;
				}
				if (value > MaxLevel)
				{
					num = MaxLevel;
				}
				lastChargeConsume = num;
			}
		}

		public ChargeMeterModel()
		{
		}

		public ChargeMeterModel(ActorModel ownerActor)
		{
			Actor = ownerActor;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void Reset()
		{
			ChargeLevel = 0;
			chargeEnabled = false;
			lastChargeConsume = 0;
		}

		public void ChangeChargeLevel(int value)
		{
			int num = chargeLevel + value;
			if (num > MaxLevel)
			{
				num = MaxLevel;
			}
			else if (num < 0)
			{
				num = 0;
			}
			ChargeLevel = num;
		}

		private bool IsExLevel(int iconIndex)
		{
			if (EXMaxLevel > 0 && iconIndex >= MaxLevel - EXMaxLevel && iconIndex <= MaxLevel)
			{
				return true;
			}
			return false;
		}

		public string GetLevelSpriteName(int iconIndex)
		{
			if (iconIndex >= ChargeLevel)
			{
				return "Ui_Charge_Point_Bg";
			}
			if (ChargeAvailable)
			{
				if (IsExLevel(iconIndex))
				{
					return "Ui_Charge_Point_Fill_Orange";
				}
				return "Ui_Charge_Point_Fill_Green";
			}
			return "Ui_Charge_Point_Fill_Yellow";
		}
	}
}
