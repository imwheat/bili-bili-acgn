//****************** 代码文件申明 ***********************
//* BagaMegamiEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：智障女神事件
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class BagaMegami : EventBaseModel
{
    public override bool IsShared => false;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Try, "BAGA_MEGAMI_.pages.INITIAL.options.TRY", HoverTipFactory.FromCard<AquasBlessing>()),
            new EventOption(this, No, "BAGA_MEGAMI_.pages.INITIAL.options.NO", HoverTipFactory.FromRelic<AquasTears>()),
            new EventOption(this, Megami, "BAGA_MEGAMI_.pages.INITIAL.options.MEGAMI", HoverTipFactory.FromRelic<AquaCompanion>())
        ];
    }

    private async Task Try()
    {
        // 让她加buff 获得阿库娅的祝福卡牌
        CardModel card = base.Owner.RunState.CreateCard<AquasBlessing>(base.Owner);
        CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 2f);
    }

    private async Task No()
    {
        // 拒绝她，获得阿库娅的眼泪
        await RelicCmd.Obtain<AquasTears>(base.Owner);
    }

    private async Task Megami()
    {
        // 加入队伍，获得智障女神同行遗物
        await RelicCmd.Obtain<AquaCompanion>(base.Owner);
    }
}
