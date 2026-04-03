//****************** 代码文件申明 ***********************
//* 文件：NoRightToKnightMePower
//* 作者：wheat
//* 创建时间：2026/04/03 星期五
//* 描述：能力 无权为我授勋
//*******************************************************

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NoRightToKnightMePower : PowerBaseModel
{
    protected override string customIconPath => "no_right_to_knight_me";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if(side == CombatSide.Enemy)
        {
            Flash();
            await CreatureCmd.Kill(base.Owner, true);
        }
    }
    
}