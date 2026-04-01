//****************** 代码文件申明 ***********************
//* 文件：MCardSelectorPrefs
//* 作者：wheat
//* 创建时间：2026/04/01 10:00:00 星期二
//* 描述：自定义卡牌选择器偏好
//*******************************************************

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

public static class MCardSelectorPrefs
{
    public static LocString TO_ADD_YYSY = new LocString("card_selection", "TO_ADD_YYSY");
    public static LocString TO_YYSY = new LocString("card_selection", "TO_YYSY");
    public static Func<CardModel, bool> YYSYFilter = (card) => card.Keywords.Contains(CustomKeyWords.YYSY);
    public static Func<CardModel, bool> NoYYSYFilter = (card) => !card.Keywords.Contains(CustomKeyWords.YYSY);

}