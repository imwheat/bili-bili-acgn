//****************** 代码文件申明 ***********************
//* SherryEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：啊~雪莉事件
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public sealed class SherryEvents : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("CurseTitle", ModelDb.Card<Normality>().Title),
    ];

    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Try, "SHERRY_EVENTS.pages.INITIAL.options.TRY", HoverTipFactory.FromCard<Normality>()),
            new EventOption(this, No, "SHERRY_EVENTS.pages.INITIAL.options.NO")
        ];
    }

    /// <summary>
    /// 获得受伤的雪莉，获得诅咒
    /// </summary>
    /// <returns></returns>
    private async Task Try()
    {
        CardModel card = base.Owner.RunState.CreateCard<Normality>(base.Owner);
		CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 2f);
        await RelicCmd.Obtain<SherryHurt>(base.Owner);
        SetEventFinished(L10NLookup("SHERRY_EVENTS.pages.TRYLEAVE.description"));
    }
    /// <summary>
    /// 离开什么都没有
    /// </summary>
    /// <returns></returns>
    private Task No()
    {
        SetEventFinished(L10NLookup("SHERRY_EVENTS.pages.LEAVE.description"));
        return Task.CompletedTask;
    }
}
