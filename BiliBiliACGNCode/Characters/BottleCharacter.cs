//****************** 代码文件申明 ***********************
//* 文件：BottleCharacter
//* 作者：wheat
//* 创建时间：2026/03/31 12:47:22 星期二
//* 描述：瓶子君152（牛子豪）角色
//*******************************************************

using BaseLib.Abstracts;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Potions.PotionPool;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Characters;

public sealed class BottleCharacter : CustomCharacterModel
{
    public override Color NameColor => new Color("789ccd");

    public override CharacterGender Gender => CharacterGender.Masculine;

    public override int StartingHp => 80;

    public override CardPoolModel CardPool => ModelDb.CardPool<BottleCardPool>();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<BottleRelicPool>();

    public override PotionPoolModel PotionPool => ModelDb.PotionPool<BottlePotionPool>();

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<BottleStrike>(),
        ModelDb.Card<BottleStrike>(),
        ModelDb.Card<BottleStrike>(),
        ModelDb.Card<BottleStrike>(),
        ModelDb.Card<BottleStrike>(),
        ModelDb.Card<BottleBlock>(),
        ModelDb.Card<BottleBlock>(),
        ModelDb.Card<BottleBlock>(),
        ModelDb.Card<BottleBlock>(),
        ModelDb.Card<BottleBlock>(),
        ModelDb.Card<AngryStrike>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [
    ];

    public override List<string> GetArchitectAttackVfx()
    {
        throw new NotImplementedException();
    }
}   