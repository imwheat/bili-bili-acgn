//****************** 代码文件申明 ***********************
//* OKuoDaEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：哦跨哒事件(Pop子与Pipi美)
//*******************************************************
using MegaCrit.Sts2.Core.Events;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public sealed class OKuoDaEvents : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Oguoda, "O_KUO_DA.pages.INITIAL.options.OGUODA"),
            new EventOption(this, Naiyo, "O_KUO_DA.pages.INITIAL.options.NAIYO"),
            new EventOption(this, Combat, "O_KUO_DA.pages.INITIAL.options.COMBAT")
        ];
    }

    private Task Oguoda()
    {
        // TODO: 实现选项 OGUODA 的具体逻辑
        return Task.CompletedTask;
    }

    private Task Naiyo()
    {
        // TODO: 实现选项 NAIYO 的具体逻辑
        return Task.CompletedTask;
    }

    private Task Combat()
    {
        // TODO: 实现选项 COMBAT 的具体逻辑
        return Task.CompletedTask;
    }
}
