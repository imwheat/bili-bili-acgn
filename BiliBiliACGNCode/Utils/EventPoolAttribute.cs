//****************** 代码文件申明 ***********************
//* 文件：EventPoolAttribute
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：标记事件所属池：共享池或绑定到某一 ActModel
//*******************************************************
using System;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Utils;

/// <summary>
/// 共享事件池标记（与具体 Act 无关，注入 <see cref="EventRegister"/> 的共享列表）。
/// </summary>
public static class SharedEventPool { }

/// <summary>
/// 声明事件类所属事件池：<paramref name="poolType"/> 为 <see cref="SharedEventPool"/> 时表示共享；
/// 否则必须为 <see cref="ActModel"/> 的子类型，表示仅在该 Act 出现。
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EventPoolAttribute : Attribute
{
    public Type PoolType { get; }

    public EventPoolAttribute(Type poolType) => PoolType = poolType;
}
