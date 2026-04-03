//****************** 代码文件申明 ***********************
//* 文件：CorrectTeamPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 正确车队
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class CorrectTeamPower : PowerBaseModel
{
    protected override string customIconPath => "correctteam";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 红怒状态下额外抽 Amount 张牌
}
