//****************** 代码文件申明 ***********************
//* 文件：Hextech
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：海克斯炸弹
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Potions;

[Pool(typeof(EventPotionPool))]
public sealed class HextechBomb : PotionBaseModel
{
    public override PotionRarity Rarity => PotionRarity.Event;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AllEnemies;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(40m, ValueProp.Unpowered),
    ];

    /// <summary>
    /// 对所有敌人造成40点伤害
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
	{
		Creature player = base.Owner.Creature;
		DamageVar damage = base.DynamicVars.Damage;
		IReadOnlyList<Creature> targets = player.CombatState.HittableEnemies;
		foreach (Creature item in targets)
		{
			NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(item));
		}
		await Cmd.CustomScaledWait(0.2f, 0.3f);
		await CreatureCmd.Damage(choiceContext, targets, damage.BaseValue, damage.Props, player, null);
	}
}