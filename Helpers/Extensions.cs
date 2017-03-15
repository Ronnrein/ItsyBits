using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

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
    }
}