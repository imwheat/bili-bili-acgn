//****************** 代码文件申明 ***********************
//* DesperateDailyEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：绝望日常事件
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class DesperateDaily : EventBaseModel
{
    public override bool IsShared => false;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Eat, "DESPERATE_DAILY.pages.INITIAL.options.EAT", HoverTipFactory.FromCard<DespairSense>()),
            new EventOption(this, Leave, "DESPERATE_DAILY.pages.INITIAL.options.LEAVE", HoverTipFactory.FromCard<EmptyStomach>())
        ];
    }
    /// <summary>
    /// 如果玩家血量都小于等于30%那就可以进入
    /// </summary>
    /// <param name="runState"></param>
    /// <returns></returns>
    public override bool IsAllowed(RunState runState)
    {
        return runState.Players.All(player => player.Creature.CurrentHp <= player.Creature.MaxHp * 0.3m);
    } 

    private async Task Eat()
    {
        // 恢复50%点生命值，但获得诅咒【绝望感】。
        int maxHp = Mathf.Min(base.Owner.Creature.MaxHp - base.Owner.Creature.CurrentHp, (int)(base.Owner.Creature.MaxHp * 0.5m));
        await CreatureCmd.Heal(base.Owner.Creature, maxHp, false);
        await CardPileCmd.Add(base.Owner.RunState.CreateCard<DespairSense>(base.Owner), PileType.Deck);
    }

    private async Task Leave()
    {
        // 获得诅咒【空腹】。获得千户的日记遗物
        await CardPileCmd.Add(base.Owner.RunState.CreateCard<EmptyStomach>(base.Owner), PileType.Deck);
        await RelicCmd.Obtain<QianHuDiary>(base.Owner);
    }
}
