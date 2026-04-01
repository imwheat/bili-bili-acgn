//****************** 代码文件申明 ***********************
//* 文件：EventBaseModel
//* 作者：wheat
//* 创建时间：2026/04/01 10:41:02 星期三
//* 描述：事件基类模型
//*******************************************************

using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public abstract class EventBaseModel : EventModel
{
    /// <summary>
    /// 事件归属的 Act 类型
    /// </summary>
    public abstract IReadOnlySet<Type> OwnerActTypes { get; }
}