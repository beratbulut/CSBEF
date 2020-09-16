using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CSBEF.Models;
using Microsoft.Extensions.Logging;

namespace CSBEF.Helpers
{
#pragma warning disable CA1305
    public static class Tools
    {
        public static IList<ValidationResult> ModelValidation<T>(this T model)
            where T : class
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        public static void WriteLoggerForService<T>(WriteLoggerForService<T> args)
        {
            args.ThrowIfNull();

            args.LoggerInstance.Log(args.LogLevel, $"{args.ModuleName} - {args.ServiceName} - {args.ActionName} - {Enum.GetName(typeof(T), args.ErrorEnum)} - {args.ErrorDescription}", args.Args == null ? null : new object[] { args.Args });
        }

        public static void WriteLoggerForService<T>(string moduleName, string serviceName, string actionName, T errorEnum, string errorDescription, ILogger loggerInstance, LogLevel logLevel, object args = null)
        {
            WriteLoggerForService(new WriteLoggerForService<T>
            {
                ModuleName = moduleName,
                ServiceName = serviceName,
                ActionName = actionName,
                ErrorEnum = errorEnum,
                ErrorDescription = errorDescription,
                LoggerInstance = loggerInstance,
                LogLevel = logLevel,
                Args = args
            });
        }

        public static int ToInt(this object value)
        {
            if (value == null)
                return -1;

            try
            {
                return int.Parse(value.ToString());
            }
            catch (FormatException)
            {
                return -1;
            }
        }

        public static int ToInt(this object value, int onErrorReturnValue)
        {
            if (value == null)
                return onErrorReturnValue;

            try
            {
                return int.Parse(value.ToString());
            }
            catch (FormatException)
            {
                return onErrorReturnValue;
            }
        }

        public static int ToInt(this string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch (FormatException)
            {
                return -1;
            }
        }
    }
#pragma warning restore CA1305
}