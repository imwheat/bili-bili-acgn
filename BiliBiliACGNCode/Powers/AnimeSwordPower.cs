//****************** 代码文件申明 ***********************
//* 文件：AnimeSwordPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 动漫区的剑
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AnimeSwordPower : PowerBaseModel
{
    protected override string customIconPath => "animesword";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 获得红温时对随机敌人造成 Amount 点伤害
}
