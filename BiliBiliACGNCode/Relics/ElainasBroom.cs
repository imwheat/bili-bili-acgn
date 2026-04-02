//****************** 代码文件申明 ***********************
//* 文件：ElainasBroom
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：伊蕾娜的扫帚 可以在楼层中飞行，每层充能2次
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Patches;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class ElainasBroom : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override bool ShowCounter => true;
    public override int DisplayAmount => Active;
    private int _active = 1;
    [SavedProperty]
	public int Active
	{
		get
		{
			return _active;
		}
		private set
		{
			AssertMutable();
            _active = value;
			UpdateDisplay();
		}
	}
    private void UpdateDisplay()
	{
        // 开启飞行
        NMapScreenRecalculateTravelabilityPatch.UseFlightStyleNextRow = true;
	}


}
