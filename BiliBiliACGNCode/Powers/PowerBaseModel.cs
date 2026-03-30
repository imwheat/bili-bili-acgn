//****************** 代码文件申明 ***********************
//* 文件：PowerBaseModel
//* 作者：wheat
//* 创建时间：2026/03/26 10:51:27 星期四
//* 描述：Power基类模型，提供Power图片路径等通用能力
//*******************************************************
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BiliBiliACGN.BiliBiliACGNCode.Extensions;
using Godot;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public abstract class PowerBaseModel : CustomPowerModel
{
    protected virtual string customIconPath=>Id.Entry.RemovePrefix().ToLowerInvariant();
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{customIconPath}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{customIconPath}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}