//****************** 代码文件申明 ***********************
//* EHeiJiangEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：诶嘿酱事件
//*******************************************************
using MegaCrit.Sts2.Core.Events;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public sealed class EHeiJiangEvents : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Leave, "E_HEI_JIANG.pages.INITIAL.options.LEAVE")
        ];
    }

    private Task Leave()
    {
        // TODO: 实现离开分支逻辑
        return Task.CompletedTask;
    }
}
