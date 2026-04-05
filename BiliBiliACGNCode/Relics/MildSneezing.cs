//****************** 代码文件申明 ***********************
//* 文件：MildSneezing
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：轻微喷嚏 下3场战斗，每回合有30%几率跳过敌人回合，并每回合随机丢弃1张牌
//*******************************************************


using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class MildSneezing : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    // 用于UI文案占位符，不在此处实现具体战斗逻辑
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Fights", 3m),
        new DynamicVar("SkipChance", 10m),
        new DynamicVar("DiscardAmount", 1m),
    ];
    public override bool ShowCounter => true;
    public override int DisplayAmount => Fights;

    public override bool IsUsedUp => Fights == 0;

    private int _fights = 3;
    [SavedProperty]
    public int Fights
    {
        get => _fights;
        set
        {
            AssertMutable();
            _fights = value;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        base.Status = _fights == 0 ? RelicStatus.Disabled : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(IsUsedUp)
        {
            return;
        }
        // 玩家回合开始时，判断是否跳过敌人回合
        if(player == base.Owner)
        {
            Flash();
            if(_fights > 0 && base.Owner.RunState.Rng.CombatPotionGeneration.NextInt(0, 100) < (int)base.DynamicVars["SkipChance"].BaseValue)
            {
                // 眩晕所有敌人
                foreach(var enemy in player.Creature.CombatState.HittableEnemies)
                {
                    await CreatureCmd.Stun(enemy);
                }
            }
            // 每回合随机丢弃1张牌
            // 获取手牌
            var hand = PileType.Hand.GetPile(base.Owner);
            // 随机丢弃1张牌
            var randomCard = base.Owner.RunState.Rng.CombatCardSelection.NextItem(hand.Cards);
            if(randomCard != null)
            {
                await CardCmd.Discard(choiceContext, randomCard);
            }
        }
    }
    public override Task AfterCombatVictory(CombatRoom room)
    {
        if(IsUsedUp)
        {
            return Task.CompletedTask;
        }
        if(_fights > 0)
        {
            Fights--;
        }
        return Task.CompletedTask;
    }

}

