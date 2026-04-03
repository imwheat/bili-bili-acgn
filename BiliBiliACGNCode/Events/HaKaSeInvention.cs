//****************** 代码文件申明 ***********************
//* HaKaSeInventionEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：博士发明事件
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class HaKaSeInvention : EventBaseModel
{
    public override bool IsShared => false;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new GoldVar(50),
    ];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var list = new List<EventOption>();
        list.Add(new EventOption(this, Try, "HA_KA_SE_INVENTION.pages.INITIAL.options.TRY", HoverTipFactory.FromRelic<MildSneezing>()));
        if(base.Owner.Gold < (int)base.DynamicVars["Gold"].BaseValue)
        {
            list.Add(new EventOption(this, Buy, "HA_KA_SE_INVENTION.pages.INITIAL.options.LOCKED", HoverTipFactory.FromRelic<HaKaSeSneezingMachine>()));
        }
        else
        {
            list.Add(new EventOption(this, Buy, "HA_KA_SE_INVENTION.pages.INITIAL.options.BUY", HoverTipFactory.FromRelic<HaKaSeSneezingMachine>()));
        }
        return list;
    }

    private async Task Try()
    {
        // 获得轻微喷嚏遗物
        await RelicCmd.Obtain<MildSneezing>(base.Owner);
    }

    private async Task Buy()
    {
        // 获得博士的喷嚏机遗物
        await RelicCmd.Obtain<HaKaSeSneezingMachine>(base.Owner);
        await PlayerCmd.LoseGold(50, base.Owner);
    }
}
