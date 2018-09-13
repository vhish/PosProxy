using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Extensions
{
    public static class StringExtensions
    {
        public static string AsPhoneNumber(this string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
            {
                return string.Empty;
            }

            mobileNumber = mobileNumber.Trim();

            if (mobileNumber.Contains(" "))
            {
                mobileNumber = mobileNumber.Replace(" ", string.Empty);
            }

            if (mobileNumber.IndexOf("o", comparisonType: StringComparison.OrdinalIgnoreCase) >= 0)
            {
                mobileNumber = Regex.Replace(mobileNumber, "o", "0", RegexOptions.IgnoreCase);
            }

            if (mobileNumber.Contains(")"))
            {
                mobileNumber = mobileNumber.Replace(")", "0");
            }

            if (mobileNumber.Contains("."))
            {
                mobileNumber = mobileNumber.Replace(".", "0");
            }

            if (mobileNumber.StartsWith("00"))
            {
                mobileNumber = "+" + mobileNumber.Substring(2, mobileNumber.Length - 2);
            }

            if (mobileNumber.Length == 12)
            {
                mobileNumber = "+" + mobileNumber;
            }

            string msisdn = string.Empty;
            PhoneNumber phoneNumber = null;
            var phoneNumberValidator = PhoneNumberUtil.GetInstance();

            try
            {
                string defaultRegion = mobileNumber.StartsWith("0") ? "GH" : string.Empty;
                phoneNumber = phoneNumberValidator.Parse(mobileNumber, defaultRegion);

                if (phoneNumber != null && phoneNumberValidator.IsValidNumber(phoneNumber))
                {
                    msisdn = $"{phoneNumber.CountryCode}{phoneNumber.NationalNumber}";
                }
            }
            catch (NumberParseException ex)
            {
                Trace.TraceError(ex.ToString());
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return msisdn;
        }
    }
}
