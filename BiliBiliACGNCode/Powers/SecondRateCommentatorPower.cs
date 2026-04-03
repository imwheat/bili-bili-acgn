//****************** 代码文件申明 ***********************
//* 文件：SecondRateCommentatorPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 二路解说
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class SecondRateCommentatorPower : PowerBaseModel
{
    protected override string customIconPath => "secondratecommentator";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 每回合获得 Amount 点红温
}
