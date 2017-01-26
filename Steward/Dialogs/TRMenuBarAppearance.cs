using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Steward.Dialogs
{

    public enum TRAdinShowsUPp
{
    // 0 value in enums is reserved for unknown values.  Either you can supply an explicit one or start enumeration at 1.
    Yes,
    No
};
public enum TRAdinDisabled
{
    // 0 value in enums is reserved for unknown values.  Either you can supply an explicit one or start enumeration at 1.
    Yes,
    No
};
public enum TRAdinInActive
{
    // 0 value in enums is reserved for unknown values.  Either you can supply an explicit one or start enumeration at 1.
    Yes,
    No
};
public enum UACPoppUp
{
    // 0 value in enums is reserved for unknown values.  Either you can supply an explicit one or start enumeration at 1.
    Yes,
    No
};
public enum TRRibonInComplete
{
    // 0 value in enums is reserved for unknown values.  Either you can supply an explicit one or start enumeration at 1.
    Yes,
    No
};
public enum ReinstallOffice
{
    // 0 value in enums is reserved for unknown values.  Either you can supply an explicit one or start enumeration at 1.
    Yes,
    No
};
    [Serializable]
   public class TRAddIn
    {
        public TRAdinShowsUPp mainQ;
        [Prompt("So you are having TR AddIn Issue? {||}")]
        [Template(TemplateUsage.NotUnderstood, "What does \"{0}\" mean???")]
        [Describe("TR AddInIssue")]
        public TRAdinDisabled disabled;
        public TRAdinInActive inactive;
        public UACPoppUp UACPoppup;
        public TRRibonInComplete RibbonInComplete;
        public ReinstallOffice install;
        public string Address;

        public override string ToString()
        {
            var builder = new StringBuilder();
            //builder.AppendFormat("problem({0}, ", Size);
            //switch (Kind)
            //{
            //    case PizzaOptions.BYOPizza:
            //        builder.AppendFormat("{0}, {1}, {2}, [", Kind, BYO.Crust, BYO.Sauce);
            //        foreach (var topping in BYO.Toppings)
            //        {
            //            builder.AppendFormat("{0} ", topping);
            //        }
            //        builder.AppendFormat("]");
            //        break;
            //    case PizzaOptions.GourmetDelitePizza:
            //        builder.AppendFormat("{0}, {1}", Kind, GourmetDelite);
            //        break;
            //    case PizzaOptions.SignaturePizza:
            //        builder.AppendFormat("{0}, {1}", Kind, Signature);
            //        break;
            //    case PizzaOptions.StuffedPizza:
            //        builder.AppendFormat("{0}, {1}", Kind, Stuffed);
            //        break;
            //}
            builder.AppendFormat("problem solved");
            return builder.ToString();
        }
    };

}