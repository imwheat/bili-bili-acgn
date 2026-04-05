//****************** 代码文件申明 ***********************
//* 文件：DeathNoteH(死亡笔记H)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：所有敌人强制死亡。消耗。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(EventCardPool))]
public sealed class DeathNoteH : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    public DeathNoteH() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有可命中敌人执行即死/强制击杀
        var enemies = base.CombatState.HittableEnemies;
        foreach(var enemy in enemies){
            if(enemy != null && enemy.IsAlive){
                await CreatureCmd.Kill(enemy, true);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.AddKeyword(CardKeyword.Retain);
    }
}
