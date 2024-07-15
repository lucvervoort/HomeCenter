﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Kernel;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;

namespace LiveChartsCore.UnitTesting.OtherTests;

[TestClass]
public class LabelsMeasureTest
{
    [TestMethod]
    public void BasicCase()
    {
        var visuls = new List<ChartElement<SkiaSharpDrawingContext>>();

        var y = 10;
        var h = 20;
        for (var i = 0; i < 10; i++)
        {
            visuls.Add(new LabelVisual
            {
                X = 10,
                Y = y,
                LocationUnit = Measure.MeasureUnit.Pixels,
                VerticalAlignment = Drawing.Align.Start,
                HorizontalAlignment = Drawing.Align.Start,
                TextSize = h,
                Text = "X-█-X",
                Paint = new SolidColorPaint(SKColors.Black),
                BackgroundColor = new Drawing.LvcColor(0, 0, 0, 50)
            });

            y += h + 10;
            h += 20;
        }

        var chart = new SKCartesianChart
        {
            Width = 600,
            Height = y + 10,
            Series = Array.Empty<ISeries>(),
            XAxes = new[] { new Axis { IsVisible = false } },
            YAxes = new[] { new Axis { IsVisible = false } },
            VisualElements = visuls
        };

        _ = chart.GetImage();
        //chart.SaveImage("test.png"); // use this method to see the actual tested image
    }

    [TestMethod]
    public void MultiLine()
    {
        var visuls = new List<ChartElement<SkiaSharpDrawingContext>>();

        var y = 10f;
        var h = 25;
        var lines = 5;
        var lineHeight = 0.1f;

        for (var i = 0; i < 20; i++)
        {
            var t = "";
            for (var j = 0; j < lines; j++)
                t += $"{(j % 2 == 0 ? "-" : "")}█-{lineHeight:N2}-█{(j != lines - 1 ? Environment.NewLine : "")}";

            var th = h * lines;

            visuls.Add(new LabelVisual
            {
                X = 10,
                Y = y,
                LocationUnit = Measure.MeasureUnit.Pixels,
                VerticalAlignment = Drawing.Align.Start,
                HorizontalAlignment = Drawing.Align.Start,
                TextSize = 25,
                Text = t,
                Paint = new SolidColorPaint(SKColors.Black),
                BackgroundColor = new Drawing.LvcColor(0, 0, 0, 50),
                LineHeight = lineHeight
            });

            y += th * lineHeight + 10;
            lineHeight += 1f;
        }

        var chart = new SKCartesianChart
        {
            Width = 900,
            Height = (int)(y + 10),
            Series = Array.Empty<ISeries>(),
            XAxes = new[] { new Axis { IsVisible = false } },
            YAxes = new[] { new Axis { IsVisible = false } },
            VisualElements = visuls
        };

        _ = chart.GetImage();
        //chart.SaveImage("multi line labels test.png"); // use this method to see the actual tested image
    }

    [TestMethod]
    public void MaxWidth()
    {
        var maxWidth = 400f;
        var text =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Magnis dis parturient montes nascetur ridiculus mus mauris vitae. Dolor sed viverra ipsum nunc aliquet bibendum. At lectus urna duis convallis convallis. Rutrum quisque non tellus orci ac auctor augue mauris. Id aliquet lectus proin nibh nisl condimentum id. Viverra aliquet eget sit amet tellus cras adipiscing. Volutpat ac tincidunt vitae semper quis. Convallis a cras semper auctor neque. Imperdiet nulla malesuada pellentesque elit.\r\n\r\nViverra nam libero justo laoreet sit amet cursus sit. Sem integer vitae justo eget magna fermentum iaculis. Nulla facilisi etiam dignissim diam quis enim lobortis. At ultrices mi tempus imperdiet nulla. Tellus rutrum tellus pellentesque eu tincidunt tortor. Lacus luctus accumsan tortor posuere ac ut. Viverra nam libero justo laoreet sit amet. In dictum non consectetur a. Odio ut sem nulla pharetra diam sit amet nisl. Porttitor massa id neque aliquam vestibulum morbi blandit. Cras sed felis eget velit aliquet sagittis id consectetur. Mi bibendum neque egestas congue. Massa massa ultricies mi quis hendrerit dolor magna eget est. Leo urna molestie at elementum eu facilisis sed odio. Justo eget magna fermentum iaculis eu non diam. Vitae aliquet nec ullamcorper sit amet risus nullam. Ut tellus elementum sagittis vitae et leo duis ut. Eget nunc scelerisque viverra mauris. Egestas purus viverra accumsan in nisl nisi.\r\n\r\nNisl condimentum id venenatis a condimentum vitae sapien. Ut pharetra sit amet aliquam id diam maecenas ultricies. Hac habitasse platea dictumst quisque sagittis purus. Maecenas pharetra convallis posuere morbi leo urna molestie. Suspendisse ultrices gravida dictum fusce. In est ante in nibh mauris cursus mattis molestie a. Volutpat lacus laoreet non curabitur gravida arcu ac tortor dignissim. Eget lorem dolor sed viverra ipsum nunc aliquet bibendum enim. Sed odio morbi quis commodo odio aenean sed adipiscing. Aliquam eleifend mi in nulla posuere sollicitudin aliquam. Eget lorem dolor sed viverra ipsum nunc aliquet bibendum. Tellus rutrum tellus pellentesque eu tincidunt tortor aliquam nulla facilisi. Dolor sit amet consectetur adipiscing elit ut.\r\n\r\nEst ultricies integer quis auctor elit sed vulputate mi sit. Enim ut tellus elementum sagittis. Donec pretium vulputate sapien nec sagittis aliquam malesuada bibendum arcu. Integer malesuada nunc vel risus commodo viverra maecenas. Praesent tristique magna sit amet. Eget magna fermentum iaculis eu non diam phasellus vestibulum. Netus et malesuada fames ac turpis egestas sed tempus urna. Pellentesque elit eget gravida cum sociis natoque penatibus et magnis. Donec ac odio tempor orci dapibus. Netus et malesuada fames ac turpis. Ultrices in iaculis nunc sed augue lacus viverra. Vulputate mi sit amet mauris. Scelerisque felis imperdiet proin fermentum leo vel orci porta non. Malesuada bibendum arcu vitae elementum curabitur vitae nunc sed.\r\n\r\nQuam viverra orci sagittis eu volutpat odio. Quis ipsum suspendisse ultrices gravida. Ac placerat vestibulum lectus mauris ultrices eros in cursus turpis. Aliquam vestibulum morbi blandit cursus. Mauris cursus mattis molestie a iaculis at erat pellentesque adipiscing. Enim nunc faucibus a pellentesque sit amet porttitor. Urna et pharetra pharetra massa massa. Nisi vitae suscipit tellus mauris a diam maecenas sed. Sit amet purus gravida quis blandit turpis cursus in. Felis eget nunc lobortis mattis aliquam faucibus purus in massa. Consequat id porta nibh venenatis. Tincidunt arcu non sodales neque sodales ut etiam. Fermentum odio eu feugiat pretium nibh ipsum consequat nisl. Tortor at risus viverra adipiscing. Dis parturient montes nascetur ridiculus mus.";

        var canvas = new MotionCanvas<SkiaSharpDrawingContext>();
        var drawingContext = new SkiaSharpDrawingContext(canvas, SKImageInfo.Empty, SKSurface.CreateNull(100, 100), null!);

        var paint = new SolidColorPaint { Color = SKColors.Red };
        paint.InitializeTask(drawingContext);
        paint._skiaPaint.TextSize = 15;

        var label = new LabelGeometry
        {
            Text = text,
            TextSize = 15,
            MaxWidth = maxWidth,
            Padding = new(10, 0),
        };

        var lines = label.GetLines(paint._skiaPaint).ToArray();
        var b = new SKRect();
        var l = 0;
        foreach (var line in lines)
        {
            _ = paint._skiaPaint.MeasureText(line, ref b);
            Assert.IsTrue(b.Width <= maxWidth);
            l++;
        }

        var size = label.Measure(paint);
        Assert.IsTrue(size.Width <= maxWidth);
        Assert.IsTrue(l == label._lines);

        var label2 = new LabelGeometry
        {
            Text =
                "short " +
                "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong " +
                "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong " +
                "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong " +
                "short",
            TextSize = 15,
            MaxWidth = 100
        };
        var size2 = label2.Measure(paint);

        Assert.IsTrue(label2._lines == 5);
        Assert.IsTrue(size2.Width > label2.MaxWidth); // the text is too long, this is allowed.

        var label3 = new LabelGeometry
        {
            Text = $"hello hello hello hello hello {Environment.NewLine} hello hello hello hello hello",
            TextSize = 15,
            MaxWidth = 100
        };
        var size3 = label3.Measure(paint);
        var lines3 = label3.GetLines(paint._skiaPaint).ToArray();

        Assert.IsTrue(label3._lines == 6);
    }
}
