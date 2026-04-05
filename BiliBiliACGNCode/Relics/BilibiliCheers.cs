//****************** 代码文件申明 ***********************
//* 文件：BilibiliCheers(bilibili干杯！)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：每当你抽牌堆洗牌时，获得{Block:diff()}点格挡。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class BilibiliCheers : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];


    //每当你抽牌堆洗牌时，获得{Block:diff()}点格挡。
    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        if(shuffler == base.Owner)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block.BaseValue, ValueProp.Unpowered, null);
        }
    }

}
