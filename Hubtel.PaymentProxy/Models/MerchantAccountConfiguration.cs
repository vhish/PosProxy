using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models
{
    public class MerchantAccountConfiguration : IMerchantAccountConfiguration
    {
        public string PublicBaseUrl { get; set; }
        public string PrivateBaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string Scheme { get; set; }
        public string IpAddressPool { get; set; }
        public string CallbackBaseUrl { get; set; }

        /*string _momoPrimaryCallbackUrl { get; set; }
        string _momoSecondaryCallbackUrl { get; set; }
        string _cardPrimaryCallbackUrl { get; set; }
        string _cardSecondaryCallbackUrl { get; set; }
        string _hubtelMePaymentCallbackUrl { get; set; }*/

        /*
        public string MomoPrimaryCallbackUrl { get { return CallbackBaseUrl + _momoPrimaryCallbackUrl; } set { _momoPrimaryCallbackUrl = value; } }
        public string MomoSecondaryCallbackUrl { get { return CallbackBaseUrl + _momoSecondaryCallbackUrl; } set { _momoSecondaryCallbackUrl = value; } }
        public string CardPrimaryCallbackUrl { get { return CallbackBaseUrl + _cardPrimaryCallbackUrl; } set { _cardPrimaryCallbackUrl = value; } }
        public string CardSecondaryCallbackUrl { get { return CallbackBaseUrl + _cardSecondaryCallbackUrl; } set { _cardSecondaryCallbackUrl = value; } }
        public string HubtelMePaymentCallbackUrl { get { return CallbackBaseUrl + _hubtelMePaymentCallbackUrl; } set { _hubtelMePaymentCallbackUrl = value; } }
        */

    }

    public interface IMerchantAccountConfiguration
    {
        string PublicBaseUrl { get; set; }
        string PrivateBaseUrl { get; set; }
        string ApiKey { get; set; }
        string Scheme { get; set; }
        string CallbackBaseUrl { get; set; }
        string IpAddressPool { get; set; }

        /*string MomoPrimaryCallbackUrl { get; set; }
        string MomoSecondaryCallbackUrl { get; set; }
        string CardPrimaryCallbackUrl { get; set; }
        string CardSecondaryCallbackUrl { get; set; }
        string HubtelMePaymentCallbackUrl { get; set; }*/
    }
}
