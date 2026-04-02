//****************** 代码文件申明 ***********************
//* 文件：ElainasBroom
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：伊蕾娜的扫帚 可以在楼层中飞行，每层充能2次
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class ElainasBroom : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 2m)];
    public override bool ShowCounter => true;
    public override int DisplayAmount => Charge;
    private int _charge = 2;
    [SavedProperty]
	public int Charge
	{
		get
		{
			return _charge;
		}
		private set
		{
			AssertMutable();
            _charge = value;
			UpdateDisplay();
		}
	}
    private void UpdateDisplay()
	{
        base.Status = Charge == 0 ? RelicStatus.Disabled : RelicStatus.Normal;
		InvokeDisplayAmountChanged();
	}
}
