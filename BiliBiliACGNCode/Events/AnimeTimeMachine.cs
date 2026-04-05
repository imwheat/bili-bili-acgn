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
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class AnimeTimeMachine : EventBaseModel
{
    public override bool IsShared => false;
    public override EventLayoutType LayoutType => EventLayoutType.Default;
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
        CardModel card = null;
        switch(base.Owner.RunState.Rng.Niche.NextInt(0, 3)){
            case 0:
                card = base.Owner.RunState.CreateCard<GuguGaga>(base.Owner);
                break;
            case 1:
                card = base.Owner.RunState.CreateCard<SakiChan>(base.Owner);
                break;
            case 2:
                card = base.Owner.RunState.CreateCard<Geass>(base.Owner);
                break;
        }
        if(card != null){
            CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 2f);
        }
        await Leave();
    }

    private async Task Microwave()
    {
        // 随机获得一个特殊遗物
        RelicModel relic = null;
        switch(base.Owner.RunState.Rng.Niche.NextInt(0, 3)){
            case 0:
                relic = ModelDb.Relic<ElainasBroom>();
                break;
            case 1:
                relic = ModelDb.Relic<DeathNote>();
                break;
            case 2:
                relic = ModelDb.Relic<DeathRebirthCycle>();
                break;
        }
        if(relic != null){
            await RelicCmd.Obtain(relic, base.Owner);
        }
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
