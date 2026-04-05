//****************** 代码文件申明 ***********************
//* 文件：SkyStrikeAttack(天击)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：第{Turn:diff()}回合开始时对所有敌人造成{Damage:diff()}点伤害，每多一名玩家额外{MultiDamage:diff()}点。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class SkyStrikeAttack : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Turn", 7m),
        new DamageVar(50m, ValueProp.Unpowered),
        new DynamicVar("MultiDamage", 20m)
    ];

    /// <summary>
    /// 第 {Turn:diff()} 回合开始时对所有敌人造成 {Damage:diff()} 点伤害，每多一名玩家额外 {MultiDamage:diff()} 点
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner)
        {
            CombatState combatState = player.Creature.CombatState;
            if(combatState.RoundNumber == base.DynamicVars["Turn"].BaseValue)
            {
                Flash();
                var dmg = base.DynamicVars.Damage.BaseValue + base.DynamicVars["MultiDamage"].BaseValue * (combatState.Players.Count - 1);
                foreach(var enemy in combatState.HittableEnemies)
                {
                    await CreatureCmd.Damage(choiceContext, enemy, base.DynamicVars.Damage, base.Owner.Creature);
                }
            }
        }
    }
}
