namespace CardEditor.Shared.Models;

/// <summary>
/// 动态变量数值属性（与运行时 <c>MegaCrit.Sts2.Core.ValueProps.ValueProp</c> 语义对齐，按位组合）。
/// </summary>
[Flags]
public enum ValueProp : uint
{
    None = 0,

    /// <summary>通过卡牌造成的伤害/格挡。</summary>
    Move = 1 << 0,

    /// <summary>不受修正影响（如力量等）。</summary>
    Unpowered = 1 << 1,

    /// <summary>伤害不可被格挡。</summary>
    Unblockable = 1 << 2,

    /// <summary>跳过受伤动画。</summary>
    SkipHurtAnim = 1 << 3
}
