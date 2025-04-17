using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIS.BPC.Core.Services;

namespace BIS.BPC.Core.Calculation;

public class CoreCalcItem
{
    public CostType CostItemType { get; set; } = CostType.Hours;

    /// <summary>
    /// The active amount is determined by the values of AmountInternal and AmountExternal.
    /// </summary>
    public decimal ActiveAmountInternal => AmountInternal != default ? AmountInternal : 0m;
    public decimal ActiveAmountExternal => AmountExternal != default ? AmountExternal : 0m;

    /// <summary>
    /// The active amount for all heads is calculated by multiplying the internal and external amounts by the head count.
    /// This is applicable to CostType.Hours, CostType.ConditionsExclusive, CostType.ConditionsInclusive, CostType.Flight, CostType.Hotel, CostType.Parking, CostType.Transport, CostType.SmallEquipment, and CostType.Tools items.
    /// </summary>
    public decimal ActiveAmountAllHeads => (ActiveAmountInternal + ActiveAmountExternal) * HeadCount;
    
    /// <summary>
    /// The active amount for travel is calculated by multiplying the external amount by the head count and the travel time.
    /// This applicable to CostType.Travel items
    /// </summary>
    public decimal ActiveAmountAllHeadsTravel => 2 * ActiveAmountExternal * HeadCount * TravelTime;

    public decimal HeadCount { get; set; }
    public decimal AmountInternal { get; set; }
    public decimal AmountExternal { get; set; }
    public decimal TravelTime { get; set; }

    public decimal HeadCountNorm { get; set; }
    public decimal AmountInternalNorm { get; set; }
    public decimal AmountExternalNorm { get; set; }

    /// <summary>
    /// The cost of the item SKU is set to 0 by default.
    /// </summary>
    public decimal ItemSkuCost { get; set; } = 0m;

    /// <summary>
    /// The net amount of the item SKU is set to 0 by default.
    /// </summary>
    public decimal ItemSkuNet { get; set; } = 0m;

    /// <summary>
    /// The margin is calculated using the BasicMarginCalculator service.
    /// And return rounded to 2 decimal places.
    /// </summary>
    public decimal Margin => BasicMarginCalculator.CalculateMarginNoDefault(TotalCosts, TotalNet, unrounded: false);

    /// <summary>
    /// The total costs are calculated based on the cost item type.
    /// </summary>
    public decimal TotalCosts =>
            CostItemType == CostType.Travel
            ? ActiveAmountAllHeadsTravel * ItemSkuCost
            : ActiveAmountAllHeads * ItemSkuCost;

    /// <summary>
    /// The total net amount is calculated based on the cost item type.
    /// </summary>
    public decimal TotalNet =>
            CostItemType == CostType.Travel
            ? ActiveAmountAllHeadsTravel * ItemSkuNet
            : ActiveAmountAllHeads * ItemSkuNet;

    /// <summary>
    /// Sets the item SKU net price based on the extrapolated margin.
    /// </summary>
    /// <param name="wishedForTotalAmount"></param>
    public void SetTotalAmountNet(decimal wishedForTotalAmount)
    {
        // Calculate the margin based on the total amount and the cost item type
        var margin = BasicMarginCalculator.CalculateMarginNoDefault(TotalCosts, wishedForTotalAmount, unrounded: false);

        // Set the margin to the calculated value
        ItemSkuNet = BasicMarginCalculator.CalculateNetAmount(ItemSkuNet, margin, unrounded: false);
    }

    /// <summary>
    /// Sets the item SKU net price based on the provided margin value.
    /// NOTE: Uses hardcoded default value of 40.00 if no margin is provided.
    /// </summary>
    /// <param name="margin"></param>
    public void SetMargin(decimal? margin = 40.00m)
    {
        margin ??= 40.00m;
        // Calculate the new net amount based on the margin
        ItemSkuNet = BasicMarginCalculator.CalculateNetAmount(ItemSkuCost, margin.Value, unrounded: false);
    }
}
