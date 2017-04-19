using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Newtonsoft.Json;

namespace ItsyBits.Helpers {

    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions {

        /// <summary>
        /// Clamp value between two other values
        /// </summary>
        /// <typeparam name="T">Type that implments IComparable</typeparam>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <returns>Clamped value</returns>
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T> {
            return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
        }

        /// <summary>
        /// Display description annotation of object, similar to name
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="html">The html object</param>
        /// <param name="expression">The expression object</param>
        /// <returns>The html formatted description</returns>
        public static IHtmlContent DescriptionFor<TModel, TValue>(
            this IHtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression
        ) {
            if (html == null || expression == null) {
                throw new ArgumentNullException();
            }
            ModelExplorer explorer = ExpressionMetadataProvider.FromLambdaExpression(
                expression,
                html.ViewData,
                html.MetadataProvider
            );
            if (explorer == null) {
                throw new InvalidOperationException($"Failed to get model explorer for {ExpressionHelper.GetExpressionText(expression)}");
            }
            return new HtmlString(explorer.Metadata.Description);
        }

        /// <summary>
        /// Returns readable time since the datetime
        /// </summary>
        /// <param name="date">The date to calculate from</param>
        /// <returns>The time passed since the date</returns>
        public static string ReadableAge(this DateTime date) {
            TimeSpan span = DateTime.Now - date;
            string result = span.Seconds + " seconds";
            if (span.TotalDays >= 365) {
                result = ((int)Math.Floor(span.TotalDays / 365)) + " years";
            }
            else if (span.TotalDays >= 31) {
                result = ((int)Math.Floor(span.TotalDays / 31)) + " months";
            }
            else if (span.TotalDays >= 7) {
                result = ((int)Math.Floor(span.TotalDays / 7)) + " weeks";
            }
            else if (span.TotalDays >= 1) {
                result = span.Days + " days";
            }
            else if (span.TotalHours >= 1) {
                result = span.Hours + " hours";
            }
            else if (span.TotalMinutes >= 1) {
                result = span.Minutes + " minutes";
            }
            return int.Parse(result.Split(' ')[0]) == 1 ? result.Remove(result.Length-1) : result;
        }

        /// <summary>
        /// Put object in tempdata by serializing it
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="tempData">The tempdata object</param>
        /// <param name="key">The key it is stored at</param>
        /// <param name="value">The objectt</param>
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Get object in tempdata by deserializing it
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="tempData">The tempdata object</param>
        /// <param name="key">The key it is stored at</param>
        /// <returns>The object</returns>
        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string) o);
        }
    }
}