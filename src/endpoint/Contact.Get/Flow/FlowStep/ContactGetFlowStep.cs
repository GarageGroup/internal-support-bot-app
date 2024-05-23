using System.Web;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

using static ContactGetResource;

internal static partial class ContactGetFlowStep
{
    private const int MaxContactSetCount = 5;

    private const int MaxCustomerSetCount = 6;

    private const string SkipButtonId = "Skip";

    private static string BuildCustomerResultMessage(this IChatFlowContextBase context, string customerTitle)
        =>
        string.Format("{0}: <b>{1}</b>", context.Localizer[CustomerFieldName], HttpUtility.HtmlEncode(customerTitle));

    private static string BuildContactResultMessage(this IChatFlowContextBase context, string contactName)
        =>
        string.Format("{0}: <b>{1}</b>", context.Localizer[ContactFieldName], HttpUtility.HtmlEncode(contactName));
}