using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIS.BPC.Core.Services;

public static class BasicMarginCalculator
{
    public static decimal CalculateMarginNoDefault(decimal totalAmountCost, decimal totalAmountNet, bool unrounded = false)
    {
        if (totalAmountNet == 0 && totalAmountCost == 0)
        {
            return 0; // is actually undefined
        }

        if (totalAmountNet == 0)
        {
            return 0;
        }

        if (totalAmountCost == 0)
        {
            return 0;
        }
        var margin = (totalAmountNet - totalAmountCost) / (totalAmountNet / 100);

        return unrounded
            ? margin
            : Math.Round(margin, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal CalculateNetAmount(decimal totalAmountCost, decimal margin, bool unrounded = false)
    {
        var totalAmountNet = totalAmountCost / (1 - (margin / 100));

        return unrounded
            ? totalAmountNet
            : Math.Round(totalAmountNet, 2, MidpointRounding.AwayFromZero);
    }
}
