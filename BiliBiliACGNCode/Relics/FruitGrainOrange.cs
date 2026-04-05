//****************** 代码文件申明 ***********************
//* 文件：FruitGrainOrange(果粒橙)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：拾起时，选择{Cards:diff()}张牌进行变化。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class FruitGrainOrange : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    /// <summary>
    /// 拾起时，选择{Cards:diff()}张牌进行变化。
    /// </summary>
    /// <returns></returns>
    public override async Task AfterObtained()
    {
        CardModel cardModel = (await CardSelectCmd.FromDeckForTransformation(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, 1))).FirstOrDefault();
		if (cardModel != null)
		{
			await CardCmd.TransformToRandom(cardModel, base.Owner.RunState.Rng.CombatCardGeneration, CardPreviewStyle.EventLayout);
		}
    }

}
