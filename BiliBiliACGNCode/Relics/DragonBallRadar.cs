//****************** 代码文件申明 ***********************
//* 文件：DragonBallRadar
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：龙珠雷达 经过{Amount}个战斗后，下一次问号变成特定事件
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class DragonBallRadar : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 7m)];
    private int _amount = 0;
    public override bool ShowCounter => true;
    public override int DisplayAmount => Amount;
    [SavedProperty]
	public int Amount
	{
		get
		{
			return _amount;
		}
		private set
		{
			AssertMutable();
            _amount = value;
			UpdateDisplay();
		}
	}
    private void UpdateDisplay()
	{
        base.Status = (decimal)Amount < base.DynamicVars["Amount"].BaseValue ? RelicStatus.Disabled : RelicStatus.Normal;
		InvokeDisplayAmountChanged();
	}
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if((decimal)Amount < base.DynamicVars["Amount"].BaseValue)
        {
            Amount++;
            Flash();
        }
    }
}
