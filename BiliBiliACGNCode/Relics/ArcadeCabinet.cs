//****************** 代码文件申明 ***********************
//* 文件：ArcadeCabinet(街机主机)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：同一回合内各打出攻击、能力、技能各一次后，对所有敌人造成{Damage:diff()}点伤害。
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
public sealed class ArcadeCabinet : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    private int _attackCountThisTurn;
    private int _skillCountThisTurn;
    private int _powerCountThisTurn;
    private int _activeCountThisTurn;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20m, ValueProp.Unpowered)
    ];
    /// <summary>
    /// 回合开始时重置计数器
    /// </summary>
    /// <param name="side"></param>
    /// <param name="combatState"></param>
    /// <returns></returns>
    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        // 回合开始时重置计数器
        if(side == base.Owner.Creature.Side)
        {
            _attackCountThisTurn = 0;
            _skillCountThisTurn = 0;
            _powerCountThisTurn = 0;
            _activeCountThisTurn = 0;
        }
        return Task.CompletedTask;
    }
    /// <summary>
    /// 回合内三种类型牌各打出一次后 AOE 伤害
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="cardPlay"></param>
    /// <returns></returns>
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner == base.Owner)
        {
            _attackCountThisTurn++;
        }
        else if(cardPlay.Card.Type == CardType.Skill && cardPlay.Card.Owner == base.Owner)
        {
            _skillCountThisTurn++;
        }
        else if(cardPlay.Card.Type == CardType.Power && cardPlay.Card.Owner == base.Owner)
        {
            _powerCountThisTurn++;
        }
        int min = Math.Min(_attackCountThisTurn, Math.Min(_skillCountThisTurn, _powerCountThisTurn));
        if(min > _attackCountThisTurn)
        {
            Flash();
            _activeCountThisTurn = min;
            var combatState = base.Owner.Creature.CombatState;
            if(combatState != null){
                foreach(var enemy in combatState.HittableEnemies)
                {
                    await CreatureCmd.Damage(choiceContext, enemy, base.DynamicVars.Damage, base.Owner.Creature);
                }
            }
        }
    }
}
