//****************** 代码文件申明 ***********************
//* PopPipi
//* 作者：wheat
//* 创建时间：2026/04/01 18:44:11 星期三
//* 描述：PopPipi怪物模型
//*******************************************************

using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters.Monsters;

public sealed class PopPipi : MonsterBaseModel
{
        /// <summary>
    /// 单次攻击音效
    /// </summary>
	private const string _attackSingleSfx = "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_attack_single";
    /// <summary>
    /// 多段攻击音效
    /// </summary>
	private const string _attackDoubleSfx = "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_attack_double";
    private const string _attackDoubleTrigger = "DoubleAttack";
	private bool _startsWithStrongPunch;
	private int _startingHpReduction;

    /// <summary>
    /// 加buff音效
    /// </summary>
	private const string _buffSfx = "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_buff";
	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 300, 100);
    public override int MaxInitialHp => MinInitialHp;
    /// <summary>
    /// 重拳伤害
    /// </summary>
	private int StrongPunchDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 16, 14);
    /// <summary>
    /// 快速拳伤害
    /// </summary>
	private int FastPunchDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    /// <summary>
    /// 快速拳重复次数
    /// </summary>
	private int FastPunchRepeat => 2;
    /// <summary>
    /// 受到伤害音效类型
    /// </summary>
    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Magic;
	public bool StartsWithStrongPunch
	{
		get
		{
			return _startsWithStrongPunch;
		}
		set
		{
			AssertMutable();
			_startsWithStrongPunch = value;
		}
	}

	public int StartingHpReduction
	{
		get
		{
			return _startingHpReduction;
		}
		set
		{
			AssertMutable();
			_startingHpReduction = value;
		}
	}

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
		await PowerCmd.Apply<ArtifactPower>(base.Creature, 3m, base.Creature, null);
		if (StartingHpReduction > 0)
		{
			base.Creature.SetCurrentHpInternal(Math.Max(1, base.Creature.CurrentHp - StartingHpReduction));
		}
	}
    /// <summary>
    /// 生成怪物逻辑行为状态机
    /// 防御 -> 重拳出击 -> 多段轻拳 -> 循环 （必须循环）
    /// </summary>
    /// <returns></returns>
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> list = new List<MonsterState>();
        // 防御
		MoveState moveState = new MoveState("READY_MOVE", ReadyMove, new DefendIntent());
        // 重拳出击
		MoveState moveState2 = new MoveState("STRONG_PUNCH_MOVE", StrongPunchMove, new SingleAttackIntent(StrongPunchDamage));
        // 多段轻拳
		MoveState moveState3 = new MoveState("FAST_PUNCH_MOVE", FastPunchMove, new MultiAttackIntent(FastPunchDamage, FastPunchRepeat), new DebuffIntent());
        // 防御 -> 重拳出击 -> 多段轻拳 -> 循环
		moveState.FollowUpState = moveState2;
		moveState2.FollowUpState = moveState3;
		moveState3.FollowUpState = moveState;
		list.Add(moveState);
		list.Add(moveState3);
		list.Add(moveState2);
		return new MonsterMoveStateMachine(list, StartsWithStrongPunch ? moveState2 : moveState);
    }
    /// <summary>
    /// 防御
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
    private async Task ReadyMove(IReadOnlyList<Creature> targets)
	{
		SfxCmd.Play("event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_buff");
		await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.8f);
		await CreatureCmd.GainBlock(base.Creature, 10m, ValueProp.Move, null);
	}

    /// <summary>
    /// 重拳出击
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
	private async Task StrongPunchMove(IReadOnlyList<Creature> targets)
	{
		await DamageCmd.Attack(StrongPunchDamage).FromMonster(this).WithAttackerAnim("Attack", 0.25f)
			.WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_attack_single")
			.WithHitFx("vfx/vfx_attack_blunt")
			.Execute(null);
	}
    /// <summary>
    /// 多段轻拳
    /// 施加虚弱buff
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
	private async Task FastPunchMove(IReadOnlyList<Creature> targets)
	{
		await DamageCmd.Attack(FastPunchDamage).WithHitCount(FastPunchRepeat).FromMonster(this)
			.WithAttackerAnim("DoubleAttack", 0.2f)
			.OnlyPlayAnimOnce()
			.WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_attack_double")
			.WithHitFx("vfx/vfx_attack_blunt")
			.Execute(null);
		await PowerCmd.Apply<WeakPower>(targets, 1m, base.Creature, null);
	}

	public override CreatureAnimator GenerateAnimator(MegaSprite controller)
	{
		AnimState animState = new AnimState("idle_loop", isLooping: true);
		AnimState animState2 = new AnimState("attack_double");
		AnimState animState3 = new AnimState("block");
		AnimState animState4 = new AnimState("attack");
		AnimState animState5 = new AnimState("hurt");
		AnimState state = new AnimState("die");
		animState3.NextState = animState;
		animState4.NextState = animState;
		animState5.NextState = animState;
		animState2.NextState = animState;
		CreatureAnimator creatureAnimator = new CreatureAnimator(animState, controller);
		creatureAnimator.AddAnyState("Cast", animState3);
		creatureAnimator.AddAnyState("Attack", animState4);
		creatureAnimator.AddAnyState("Dead", state);
		creatureAnimator.AddAnyState("Hit", animState5);
		creatureAnimator.AddAnyState("DoubleAttack", animState2);
		return creatureAnimator;
	}
}