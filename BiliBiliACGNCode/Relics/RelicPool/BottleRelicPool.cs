//****************** 代码文件申明 ***********************
//* 文件：BottleRelicPool
//* 作者：wheat
//* 创建时间：2026/03/31 12:45:49 星期二
//* 描述：瓶子君152（牛子豪）遗物池
//*******************************************************

using BaseLib.Abstracts;
using BiliBiliACGN.BiliBiliACGNCode.Extensions;
using Godot;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;

public sealed class BottleRelicPool : CustomRelicPoolModel
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