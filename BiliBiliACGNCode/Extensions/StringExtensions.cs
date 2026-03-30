//****************** 代码文件申明 ***********************
//* 文件：StringExtensions
//* 作者：wheat
//* 创建时间：2026/03/26 10:49:02 星期四
//* 描述：String拓展工具，用来方便路径获取
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Core;

namespace BiliBiliACGN.BiliBiliACGNCode.Extensions;

/// <summary>
/// 用来方便路径获取
/// </summary>
public static class StringExtensions
{
    public static string ImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", path);
    }
    
    public static string CardImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "card_portraits", path);
    }
    public static string BigCardImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "card_portraits", "big", path);
    }

    public static string PowerImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "powers", path);
    }
    
    public static string BigPowerImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "powers", "big", path);
    }
    public static string EnergyIconImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "energy", path);
    }

    public static string RelicImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "relics", path);
    }

    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "relics", "big", path);
    }
}