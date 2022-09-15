﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WpfVerifyCode
{
    public static class DrawingContextHelper
    {
        /// <summary>
        ///     字体资源
        /// </summary>
        public static FontFamily FontFamily = Application.Current.Resources["NormalFontFamily"] as FontFamily;

        /// <summary>
        ///     默认颜色
        /// </summary>
        public static Brush Brush = Application.Current.Resources["PrimaryNormalSolidColorBrush"] as Brush;

        /// <summary>
        ///     颜色转换
        /// </summary>
        public static BrushConverter BrushConverter = new BrushConverter();

        /// <summary>
        ///     绘制Line
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="pen"></param>
        /// <param name="lineThickness"></param>
        /// <param name="points"></param>
        public static void DrawSnappedLinesBetweenPoints(this DrawingContext dc,
            Pen pen, double lineThickness, params Point[] points)
        {
            var guidelineSet = new GuidelineSet();
            foreach (var point in points)
            {
                guidelineSet.GuidelinesX.Add(point.X);
                guidelineSet.GuidelinesY.Add(point.Y);
            }

            var half = lineThickness / 2;
            points = points.Select(p => new Point(p.X + half, p.Y + half)).ToArray();
            dc.PushGuidelineSet(guidelineSet);
            for (var i = 0; i < points.Length - 1; i = i + 2) dc.DrawLine(pen, points[i], points[i + 1]);
            dc.Pop();
        }

        /// <summary>
        ///     返回FormattedText
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="flowDirection"></param>
        /// <param name="fontWeight"></param>
        /// <returns></returns>
        [Obsolete("GetFormattedText")]
        public static FormattedText GetFormattedText(string text, Brush color = null,
            FlowDirection flowDirection = FlowDirection.RightToLeft, double textSize = 12.0D,
            FontWeight fontWeight = default)
        {
            if (fontWeight == default)
                fontWeight = FontWeights.Thin;

            var fontFamily = new FontFamily("Comic Sans MS");

            return new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                flowDirection,
                new Typeface(fontFamily, FontStyles.Normal, fontWeight, FontStretches.Normal),
                textSize, color ?? Brush)
            {
                MaxLineCount = 1,
                TextAlignment = TextAlignment.Justify,
                Trimming = TextTrimming.CharacterEllipsis
            };
            //textSize, color == null ? Brush : (Brush)brushConverter.ConvertFromString(color))
        }
    }
}