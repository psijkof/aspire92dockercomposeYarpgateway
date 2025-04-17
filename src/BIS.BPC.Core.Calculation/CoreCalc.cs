using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIS.BPC.Core.Services;

namespace BIS.BPC.Core.Calculation;

public class CoreCalc
{
    // Each item’s final net is only rounded when read by get-accessors
    // or final usage. Summation should match the user’s requested total exactly.

    public decimal TotalCosts => Math.Round(CalcItems.Sum(x => x.TotalCosts), 2, MidpointRounding.AwayFromZero);
    public decimal TotalNet => Math.Round(CalcItems.Sum(x => x.TotalNet), 2, MidpointRounding.AwayFromZero);
    public decimal Margin => BasicMarginCalculator.CalculateMarginNoDefault(TotalCosts, TotalNet, unrounded: false);


    public IEnumerable<CoreCalcItem> CalcItems { get; set; } = [];

    public void SetTotalNet(decimal wishedForTotalAmount)
    {
        // 1) Calculate margin unrounded.
        var marginUnrounded = BasicMarginCalculator
            .CalculateMarginNoDefault(TotalCosts, wishedForTotalAmount, unrounded: true);

        // 2) Update each item's net without forcing rounding yet.
        foreach (var item in CalcItems)
        {
            item.ItemSkuNet = BasicMarginCalculator
                .CalculateNetAmount(item.ItemSkuCost, marginUnrounded, unrounded: true);
        }

        // 3) Calculate how far we are from the final desired total.
        var sumUnrounded = CalcItems.Sum(i => i.ActiveAmountAllHeads * i.ItemSkuNet);
        var difference = wishedForTotalAmount - sumUnrounded;

        // 4) Distribute that difference (for example, just apply it to the last item).
        if (CalcItems.Any() && Math.Abs(difference) > 0.000001m)
        {
            var lastItem = CalcItems.Last();
            var heads = lastItem.ActiveAmountAllHeads != 0 ? lastItem.ActiveAmountAllHeads : 1;
            lastItem.ItemSkuNet += difference / heads;
        }
    }

    public void SetMargin(decimal? margin = 40.00m)
    {
        margin ??= 40.00m;
        foreach (var item in CalcItems)
        {
            item.SetMargin(margin);
        }
    }

}
