//****************** 代码文件申明 ***********************
//* 文件：PotionBaseModel
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：药水基类
//*******************************************************

using BaseLib.Abstracts;
using BaseLib.Extensions;
using BiliBiliACGN.BiliBiliACGNCode.Extensions;
using Godot;

namespace BiliBiliACGN.BiliBiliACGNCode.Potions;

public abstract class PotionBaseModel : CustomPotionModel
{
    public override string PackedImagePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
            return ResourceLoader.Exists(path) ? path : "none.png".PotionImagePath();
        }
    }
    public override string PackedOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".PotionImagePath();
            return ResourceLoader.Exists(path) ? path : "none.png".PotionImagePath();
        }
    }
}