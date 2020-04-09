using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CSBEF.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CSBEF.Core.Helpers
{
    public static class Tools {
        public static DateTime ToDateTime (this object value) {
            if (value == null)
                throw new ArgumentNullException (nameof (value));

            return DateTime.TryParse (value.ToString (), out DateTime result) ?
                result :
                DateTime.MinValue;
        }

        public static string ToDateTimeToString (this object value, CustomCultureInfo cultureInfo = null) {
            if (value == null)
                return string.Empty;

            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            return DateTime.TryParse (value.ToString (), out DateTime result) ?
                result > "1900-01-01".ToDateTime () ? result.ToString ("d", cultureInfo.CultureInfo) : string.Empty : string.Empty;
        }

        public static string ToFormatedStr (this object value, CustomCultureInfo cultureInfo = null) {
            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            string request = string.Format (cultureInfo.CultureInfo, "{0:yyyy.MM.dd}", value);
            return request;
        }

        public static string ToFormatedLongStr (this object value, CustomCultureInfo cultureInfo = null) {
            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            string request = string.Format (cultureInfo.CultureInfo, "{0:yyyy.MM.dd HH:mm:ss}", value);
            return request;
        }

        public static int ToInt (this object value, CustomCultureInfo cultureInfo = null) {
            if (value == null)
                return -1;

            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            try {
                return int.Parse (value.ToString (), cultureInfo.CultureInfo);
            } catch (CustomException) {
                return -1;
            }
        }

        public static int ToInt (this object value, int onErrorReturnValue, CustomCultureInfo cultureInfo = null) {
            if (value == null)
                return onErrorReturnValue;

            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            try {
                return int.Parse (value.ToString (), cultureInfo.CultureInfo);
            } catch (CustomException) {
                return onErrorReturnValue;
            }
        }

        public static int ToInt (this string value, CustomCultureInfo cultureInfo = null) {
            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            try {
                return int.Parse (value, cultureInfo.CultureInfo);
            } catch (CustomException) {
                return -1;
            }
        }

        public static decimal ToDecimal (this object value, CustomCultureInfo cultureInfo = null) {
            if (value == null)
                return -1;

            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            try {
                return decimal.Parse (value.ToString ().Replace (",", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (".", ",", cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (" ₺", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (" TL", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo), cultureInfo.CultureInfo);
            } catch (CustomException) {
                return -1;
            }
        }

        public static decimal ToDecimal (this object value, decimal onErrorReturnValue, CustomCultureInfo cultureInfo = null) {
            if (value == null)
                return onErrorReturnValue;

            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            try {
                return decimal.Parse (value.ToString ().Replace (",", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (".", ",", cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (" ₺", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (" TL", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo), cultureInfo.CultureInfo);
            } catch (CustomException) {
                return onErrorReturnValue;
            }
        }

        public static double ToDouble (this object value, CustomCultureInfo cultureInfo = null) {
            if (value == null)
                return -1;

            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            try {
                return double.Parse (value.ToString ().Replace (",", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (".", ",", cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (" ₺", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (" TL", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo), cultureInfo.CultureInfo);
            } catch (CustomException) {
                return -1;
            }
        }

        public static double ToDouble (this object value, double onErrorReturnValue = 0, CustomCultureInfo cultureInfo = null) {
            if (value == null)
                return onErrorReturnValue;

            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            try {
                return double.Parse (value.ToString ().Replace (",", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (".", ",", cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (" ₺", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo).Replace (" TL", string.Empty, cultureInfo.IgnoreCase, cultureInfo.CultureInfo), cultureInfo.CultureInfo);
            } catch (CustomException) {
                return onErrorReturnValue;
            }
        }

        public static bool ToBool (this object value) {
            if (value == null)
                return false;

            return bool.TryParse (value.ToString (), out bool result) && result;
        }

        public static string ToStringNotNull (this object value, string onErrorReturnValue = "") {
            if (value == null)
                return onErrorReturnValue;

            try {
                return value.ToString ();
            } catch (CustomException) {
                return onErrorReturnValue;
            }
        }

        public static bool ToBool2 (this object value) {
            if (value == null)
                return false;

            if (value.GetType () == typeof (int)) {
                return value.ToInt () == 1;
            } else {
                return bool.TryParse (value.ToString (), out bool result) && result;
            }
        }

        public static byte ToByte (this object value, CustomCultureInfo cultureInfo = null) {
            if (value == null)
                return 0;

            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            try {
                return byte.Parse (value.ToString (), cultureInfo.CultureInfo);
            } catch (CustomException) {
                return 0;
            }
        }

        public static Guid ToGuid (this object value) {
            if (value == null)
                return new Guid ();

            try {
                return new Guid (value.ToString ());
            } catch (CustomException) {
                return new Guid ();
            }
        }

        public static bool URLValidate (this object value) {
            if (value == null)
                return false;

            try {
                bool result = Uri.TryCreate (value.ToString (), UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;

                if (uriResult == null)
                    return false;

                return true;
            } catch (CustomException) {
                return false;
            }
        }

        public static string ToCurrencyString (this decimal d, CustomCultureInfo cultureInfo = null) {
            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            decimal t = decimal.Truncate (d);
            if (d.Equals (t)) {
                return d.ToString ("0.##", cultureInfo.CultureInfo);
            } else {
                return d.ToString ("#,##0.00", cultureInfo.CultureInfo);
            }
        }

        public static Expression<Func<T, T>> FuncToExpression<T> (this Func<T, T> f) {
            return x => f (x);
        }

        public static bool In<T> (this T objx, params T[] args) {
            return args.Contains (objx);
        }

        public static string ComputeSha256Hash (string rawData, CustomCultureInfo cultureInfo = null) {
            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            using SHA256 sha256Hash = SHA256.Create ();
            byte[] bytes = sha256Hash.ComputeHash (Encoding.UTF8.GetBytes (rawData));
            StringBuilder builder = new StringBuilder ();
            for (int i = 0; i < bytes.Length; i++) {
                builder.Append (bytes[i].ToString ("x2", cultureInfo.CultureInfo));
            }

            return builder.ToString ();
        }

        public static string GetExceptionDetails (this Exception exception, CustomCultureInfo cultureInfo = null) {
            if (exception == null)
                return string.Empty;

            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            PropertyInfo[] properties = exception.GetType ()
                .GetProperties ();
            List<string> fields = new List<string> ();
            foreach (PropertyInfo property in properties) {
                object value = property.GetValue (exception, null);
                fields.Add (string.Format (
                    cultureInfo.CultureInfo,
                    "{0} = {1}",
                    property.Name,
                    value != null ? value.ToString () : string.Empty
                ));
            }

            return string.Join ("\n", fields.ToArray ());
        }

        public static object GetMemberExpressionValue (this MemberExpression member) {
            var objectMember = Expression.Convert (member, typeof (object));
            var getterLambda = Expression.Lambda<Func<object>> (objectMember);
            var getter = getterLambda.Compile ();

            return getter ();
        }

        public static bool IsValidEmail (this string email) {
            try {
                var addr = new System.Net.Mail.MailAddress (email);
                return addr.Address == email;
            } catch (CustomException) {
                return false;
            }
        }

        public static List<ValidationResult> ModelValidation<T> (this T model)
        where T : class {
            var context = new ValidationContext (model);
            var results = new List<ValidationResult> ();
            Validator.TryValidateObject (model, context, results, true);
            return results;
        }

        public static string FixFileName (this object objx) {
            if (objx == null)
                return string.Empty;

            var fileName = objx.ToString ();
            foreach (char c in Path.GetInvalidFileNameChars ()) {
                fileName = fileName.Replace (c, '_');
            }
            return fileName;
        }

        public static bool CheckExtension (this object objx) {
            if (objx == null)
                return false;

            var ext = Path.GetExtension (objx.ToString ());

            var allowExt = new List<string> {
                ".txt",
                ".pdf",
                ".xls",
                ".xlsx",
                ".doc",
                ".docx",
                ".gif",
                ".png",
                ".jpg",
                ".jpeg",
                ".avi",
                ".mov",
                ".flv"
            };

            return allowExt.Any (i => i == ext);
        }

        public static string GetRandomNumber (int size = 16) {
            var rnd = new Random ();
            var counter = size / 8;
            var rtnString = string.Empty;
            for (var i = 0; i < counter; i++) {
                rtnString += rnd.Next (11111111, 99999999);
            }
            return rtnString;
        }

        public static void CopyValues<T> (this T target, T source) {
            Type t = typeof (T);

            var properties = t.GetProperties ().Where (prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties) {
                var value = prop.GetValue (source, null);
                if (value != null)
                    prop.SetValue (target, value, null);
            }
        }

        public static bool ValidatePassword (this string password) {
            if (password == null)
                throw new ArgumentNullException (nameof (password));

            const int MIN_LENGTH = 8;
            const int MAX_LENGTH = 32;

            bool meetsLengthRequirements = password.Length >= MIN_LENGTH && password.Length <= MAX_LENGTH;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            if (meetsLengthRequirements) {
                foreach (char c in password) {
                    if (char.IsUpper (c))
                        hasUpperCaseLetter = true;
                    else if (char.IsLower (c))
                        hasLowerCaseLetter = true;
                    else if (char.IsDigit (c))
                        hasDecimalDigit = true;
                }
            }

            bool isValid = meetsLengthRequirements &&
                hasUpperCaseLetter &&
                hasLowerCaseLetter &&
                hasDecimalDigit;
            return isValid;
        }

        public static int GetTokenNameClaim (HttpContext context) {
            if (context == null)
                return 0;

            var rtn = 0;

            if (context.User != null) {
                if (context.User.Claims != null) {
                    if (context.User.Claims.Any ()) {
                        var getClaim = context.User.Claims.FirstOrDefault (i => i.Type == ClaimTypes.Name);
                        if (getClaim != null)
                            rtn = getClaim.Value.ToInt (0);
                    }
                }
            }

            return rtn;
        }

        public static int GetTokenNameClaim (HubCallerContext context) {
            if (context == null)
                return 0;

            var rtn = 0;

            if (context.User != null) {
                if (context.User.Claims != null) {
                    if (context.User.Claims.Any ()) {
                        var getClaim = context.User.Claims.FirstOrDefault (i => i.Type == ClaimTypes.Name);
                        if (getClaim != null)
                            rtn = getClaim.Value.ToInt (0);
                    }
                }
            }

            return rtn;
        }

        public static int GetTokenIdClaim (HttpContext context) {
            if (context == null)
                return 0;

            var rtn = 0;

            if (context.User != null) {
                if (context.User.Claims != null) {
                    if (context.User.Claims.Any ()) {
                        var getClaim = context.User.Claims.FirstOrDefault (i => i.Type == "TokenId");
                        if (getClaim != null)
                            rtn = getClaim.Value.ToInt (0);
                    }
                }
            }

            return rtn;
        }

        public static int GetTokenIdClaim (HubCallerContext context) {
            if (context == null)
                return 0;

            var rtn = 0;

            if (context.User != null) {
                if (context.User.Claims != null) {
                    if (context.User.Claims.Any ()) {
                        var getClaim = context.User.Claims.FirstOrDefault (i => i.Type == "TokenId");
                        if (getClaim != null)
                            rtn = getClaim.Value.ToInt (0);
                    }
                }
            }

            return rtn;
        }

        public static Expression<Func<T, bool>> ExpAnd<T> (Expression<Func<T, bool>> expressionOne, Expression<Func<T, bool>> expressionTwo) {
            if (expressionOne == null)
                throw new ArgumentNullException (nameof (expressionOne));

            if (expressionTwo == null)
                throw new ArgumentNullException (nameof (expressionTwo));

            var invokedSecond = Expression.Invoke (expressionTwo, expressionOne.Parameters.Cast<Expression> ());
            return Expression.Lambda<Func<T, bool>> (
                Expression.And (expressionOne.Body, invokedSecond), expressionOne.Parameters
            );
        }

        public static Expression<Func<T, bool>> ExpOr<T> (Expression<Func<T, bool>> expressionOne, Expression<Func<T, bool>> expressionTwo) {
            if (expressionOne == null)
                throw new ArgumentNullException (nameof (expressionOne));

            if (expressionTwo == null)
                throw new ArgumentNullException (nameof (expressionTwo));

            var invokedSecond = Expression.Invoke (expressionTwo, expressionOne.Parameters.Cast<Expression> ());
            return Expression.Lambda<Func<T, bool>> (
                Expression.Or (expressionOne.Body, invokedSecond), expressionOne.Parameters
            );
        }

        public static IList<T> Clone<T> (this IList<T> listToClone) where T : ICloneable {
            return listToClone.Select (item => (T) item.Clone ()).ToList ();
        }

        private static PropertyInfo[] GetProperties (object obj) {
            return obj.GetType ().GetProperties ();
        }

        public static bool HashControl<TParam> (this TParam data, string secureKey, ILogger<IReturnModel<bool>> logger = null, CustomCultureInfo cultureInfo = null) {
            if (cultureInfo == null)
                cultureInfo = new CustomCultureInfo ();

            var properties = GetProperties (data);
            var valueChain = string.Empty;
            var incomingHashProperty = properties.FirstOrDefault (i => i.Name.ToLower (cultureInfo.CultureInfo) == "hash");
            var incomingHashString = string.Empty;

            if (incomingHashProperty == null)
                return false;

            if (string.IsNullOrWhiteSpace (incomingHashProperty.GetValue (data, null).ToStringNotNull (string.Empty)))
                return false;

            incomingHashString = incomingHashProperty.GetValue (data, null).ToStringNotNull (string.Empty);

            foreach (var property in properties) {
                if (property.Name.ToLower (cultureInfo.CultureInfo) != "hash") {
                    if (property.PropertyType.Name == "IFormFile")
                        continue;
                    else if (property.PropertyType == typeof (bool) || property.PropertyType == typeof (bool?)) {
                        if ((bool) property.GetValue (data, null)) {
                            valueChain += "true";
                        } else {
                            valueChain += "false";
                        }
                    } else if (property.PropertyType == typeof (double) || property.PropertyType == typeof (double?)) {
                        valueChain += ((double) property.GetValue (data, null)).ToString ("F", cultureInfo.CultureInfo).Replace (",", ".", cultureInfo.IgnoreCase, cultureInfo.CultureInfo);
                    } else if (property.PropertyType == typeof (DateTime) || property.PropertyType == typeof (DateTime?)) {
                        valueChain += ((DateTime) property.GetValue (data, null)).ToString ("yyyy/MM/dd HH:mm:ss", cultureInfo.CultureInfo);
                    } else if (property.PropertyType == typeof (Array)) {
                        valueChain += string.Join (",", property.GetValue (data, null));
                    } else {
                        valueChain += property.GetValue (data, null);
                    }
                }
            }

            var systemHash = ComputeSha256Hash (valueChain + secureKey);

            var control = systemHash.ToLower (cultureInfo.CultureInfo) == incomingHashString.ToLower (cultureInfo.CultureInfo);

            if (!control && logger != null) {
                var logBuilder = new StringBuilder ();
                logBuilder.AppendLine ("Failed hash code validation!");
                logBuilder.AppendLine ("Incoming Hash Code: " + incomingHashString);
                logBuilder.AppendLine ("Secure Key: " + secureKey);
                logBuilder.AppendLine ("Has Code to System: " + systemHash);
                logBuilder.AppendLine ("Using String Chain for Hash: " + valueChain);
                logger.LogError (logBuilder.ToString ());
            }

            return control;
        }

        public static bool UserNameIsValid (this string input) {
            string pattern = @"^(?=[a-zA-Z])[-\w.]{0,23}([a-zA-Z\d]|(?<![-.])_)$";
            Regex currencyRegex = new Regex (pattern);
            return currencyRegex.IsMatch (input);
        }
    }
}