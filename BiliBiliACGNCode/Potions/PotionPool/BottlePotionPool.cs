//* 文件：BottlePotionPool
//* 作者：wheat
//* 创建时间：2026/03/31 12:45:42 星期二
//* 描述：瓶子君152（牛子豪）药水池
//*******************************************************

using BaseLib.Abstracts;
using BiliBiliACGN.BiliBiliACGNCode.Extensions;
using Godot;

namespace BiliBiliACGN.BiliBiliACGNCode.Potions.PotionPool;

public sealed class BottlePotionPool : CustomPotionPoolModel
{
    public override string? TextEnergyIconPath{
        get
        {
            var path = $"bottle152_energy_icon.png".EnergyIconImagePath();
            return ResourceLoader.Exists(path) ? path : "colorless_energy_icon.png".EnergyIconImagePath();
        }
    }
    public override string? BigEnergyIconPath {
        get
        {
            var path = $"bottle152_energy_big.png".EnergyIconImagePath();
            return ResourceLoader.Exists(path) ? path : "colorless_energy_icon.png".EnergyIconImagePath();
        }
    }
}   