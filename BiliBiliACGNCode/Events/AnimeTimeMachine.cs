//****************** 代码文件申明 ***********************
//* AnimeTimeMachineEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：新番时光机事件
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class AnimeTimeMachine : EventBaseModel
{
    public override bool IsShared => false;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;
    /// <summary>
    /// 随机获得的遗物列表
    /// </summary>
    private static readonly RelicModel[] _randomRelics = [
        ModelDb.Relic<ElainasBroom>(),
    ];
    /// <summary>
    /// 随机获得的卡牌列表
    /// </summary>
    private static readonly CardModel[] _randomCards = [
    ];
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Drawer, "ANIME_TIME_MACHINE.pages.INITIAL.options.DRAWER"),
            new EventOption(this, Microwave, "ANIME_TIME_MACHINE.pages.INITIAL.options.MICROWAVE")
        ];
    }

    private async Task Drawer()
    {
        // 随机获得一张特殊卡牌
        //CardModel card = base.Owner.RunState.CreateCard(base.Owner.RunState.Rng.Niche.NextItem(_randomCards));
        //CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 2f);

        await Leave();
    }

    private async Task Microwave()
    {
        // 随机获得一个特殊遗物
        await RelicCmd.Obtain(base.Owner.RunState.Rng.Niche.NextItem(_randomRelics), base.Owner);
        await Leave();
    }
    /// <summary>
    /// 离开
    /// </summary>
    /// <returns></returns>
    private Task Leave()
    {
        SetEventFinished(L10NLookup("ANIME_TIME_MACHINE.pages.LEAVE.description"));
        return Task.CompletedTask;
    }
}
