//****************** 代码文件申明 ***********************
//* 文件：EnjoyPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 开始享受
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class EnjoyPower : PowerBaseModel
{
    protected override string customIconPath => "enjoy";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 进入红怒后自动打出 Amount 张手牌中带有一说一的牌
}
