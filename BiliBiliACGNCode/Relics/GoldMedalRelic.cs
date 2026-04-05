//****************** 代码文件申明 ***********************
//* 文件：GoldMedalRelic(金牌)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：你的回合结束时，手牌中每有一张牌，对所有敌人造成{Damage:diff()}点伤害。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class GoldMedalRelic : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1m, ValueProp.Unpowered)
    ];
    /// <summary>
    /// 回合结束时，手牌中每有一张牌，对所有敌人造成{Damage:diff()}点伤害。
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if(side == base.Owner.Creature.Side)
        {
            var combatState = base.Owner.Creature.CombatState;
            if(combatState != null){
                int handCount = PileType.Hand.GetPile(base.Owner).Cards.Count;
                if(handCount > 0){
                    Flash();
                    var damage = base.DynamicVars.Damage.BaseValue * handCount;
                    foreach(var enemy in combatState.HittableEnemies)
                    {
                        await CreatureCmd.Damage(choiceContext, enemy, damage, ValueProp.Unpowered, base.Owner.Creature);
                    }
                }
            }
        }
    }

}
