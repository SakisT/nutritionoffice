using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Web;

namespace nutritionoffice.ViewModels
{
    [Serializable()]
    public class BMI
    {
        private double _MinY = 0;
        private double _MaxY = 0;
        public BMI(Sex sex, Size size, Margins margins)
        {
            CustomerSex = sex;
            Size = size;
            Margins = margins;

            _MinY = 10;// Values[sex].Select(r => r.Value.Values.Min()).Min() - 3;
            _MaxY = 35;// Values[sex].Select(r => r.Value.Values.Max()).Max() + 3;

        }
        public double MaxY { get { return _MaxY; } }
        public double MinY { get { return _MinY; } }
        public Size Size { get; set; }
        public Margins Margins { get; set; }
        public double UsedWidth
        {
            get
            {
                return (Size.Width - Margins.Left - Margins.Right);
            }
        }
        public double UsedHeight
        {
            get
            {
                return (Size.Height - Margins.Top - Margins.Bottom);
            }
        }
        public double StepX
        {
            get
            {
                return UsedWidth / (Ages.Count() - 1);
            }

        }
        public double StepY
        {
            get
            {
                return UsedHeight / (MaxY - MinY);
            }
        }
        public string Customer { get; set; }
        public DateTime Date { get; set; }

        public Sex CustomerSex { get; set; }
        public enum Sex
        {
            Boy = 1,
            Girl = 2
        }

        public double[] Ages
        {
            get
            {
                return new double[] { 0, 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7, 7.5, 8, 8.5, 9, 9.5,
                    10, 10.5, 11, 11.5, 12, 12.5, 13, 13.5, 14, 14.5, 15, 15.5, 16, 16.5, 17, 17.5, 18 };
            }
        }

        public class LineData
        {

            public Stroke StrokeData { get; set; }
            public double[] Values { get; set; }

            public class Stroke
            {
                public string Width { get; set; }
                public string WidthValue { get; set; }
                public string DashArray { get; set; }

                public string DashName { get; set; }

                public string Color { get; set; }

            }
        }


        private static Dictionary<Sex, Dictionary<int, LineData>> _Values = null;
        public Dictionary<Sex, Dictionary<int, LineData>> Values
        {
            get
            {
                if (_Values == null) { FillData(); }
                return _Values;
            }
        }

        public int[] GetCoOrdinates(double age, double value)
        {
            double X = Margins.Left + (2 * StepX) * age;
            return new int[] { (int)X, (int)((4 + Margins.Top + ((MaxY - value) * StepY))) }.ToArray();
        }

        public string GetLinePath(Sex sex, int Index)
        {
            var t = new List<string>();
            for (int i = 0; i < Ages.Count(); ++i)
            {
                var values = GetCoOrdinates(Ages[i], Values[sex][Index].Values[i]);
                t.Add(string.Format("{0},{1}", values[0], values[1]));
            }
            return string.Join(" ", t.ToArray());
        }

        private static void FillData()
        {
            _Values = new Dictionary<Sex, Dictionary<int, LineData>>();
            var boyslines = new Dictionary<int, LineData>();
            boyslines.Add(1, new LineData { StrokeData = new LineData.Stroke { Color = "black", DashArray = "", DashName = "Solid", Width = "2", WidthValue = "2pt" }, Values = new double[] { 13, 14.2, 14.5, 14.45, 14.25, 14, 13.8, 13.6, 13.6, 13.6, 13.65, 13.65, 13.65, 13.7, 13.9, 14, 14.1, 14.2, 14.3, 14.45, 14.6, 14.8, 15.05, 15.35, 15.55, 15.9, 16.2, 16.5, 16.8, 17, 17.2, 17.5, 17.8, 18, 18.2, 18.3, 18.6 } });
            boyslines.Add(2, new LineData { StrokeData = new LineData.Stroke { Color = "black", DashArray = "4,4", DashName = "Dashed", Width = "2", WidthValue = "2pt" }, Values = new double[] { 14, 14.9, 15.4, 15.35, 15.05, 14.8, 14.55, 14.45, 14.4, 14.3, 14.4, 14.45, 14.5, 14.6, 14.8, 14.9, 15.05, 15.3, 15.4, 15.55, 15.75, 16, 16.25, 16.4, 16.7, 16.9, 17.3, 17.5, 17.95, 18.2, 18.6, 18.8, 19.15, 19.45, 19.7, 20, 20.35 } });
            boyslines.Add(3, new LineData { StrokeData = new LineData.Stroke { Color = "black", DashArray = "4,4", DashName = "Dashed", Width = "2", WidthValue = "2pt" }, Values = new double[] { 15, 16, 16.4, 16.3, 15.95, 15.6, 15.4, 15.2, 15.1, 15.1, 15.2, 15.25, 15.5, 15.6, 15.8, 16, 16.2, 16.5, 16.7, 17, 17.2, 17.4, 17.6, 17.8, 18.1, 18.4, 18.6, 19, 19.4, 19.7, 20.1, 20.5, 20.8, 21.1, 21.4, 21.55, 21.8 } });
            boyslines.Add(4, new LineData { StrokeData = new LineData.Stroke { Color = "green", DashArray = "", DashName = "Solid", Width = "2", WidthValue = "2pt" }, Values = new double[] { 16, 17, 17.5, 17.4, 16.7, 16.5, 16.3, 16.1, 16.1, 16.1, 16.2, 16.45, 16.6, 16.8, 17, 17.4, 17.6, 18, 18.4, 18.7, 19, 19.4, 19.6, 20, 20.2, 20.5, 20.7, 21, 21.4, 21.6, 22, 22.4, 22.7, 23, 23.5, 23.9, 24.3 } });
            boyslines.Add(5, new LineData { StrokeData = new LineData.Stroke { Color = "blue", DashArray = "", DashName = "Solid", Width = "4", WidthValue = "4pt" }, Values = new double[] { 17.2, 17.5, 17.7, 17.6, 17.4, 17, 16.6, 16.5, 16.45, 16.5, 16.8, 17, 17.2, 17.5, 17.8, 18, 18.4, 18.7, 19.2, 19.6, 19.9, 20.3, 20.6, 21, 21.3, 21.6, 21.9, 22.2, 22.4, 22.8, 23.1, 23.4, 23.75, 24.1, 24.45, 24.8, 25.1 } });
            boyslines.Add(6, new LineData { StrokeData = new LineData.Stroke { Color = "black", DashArray = "2,4", DashName = "DashDot", Width = "2", WidthValue = "2pt" }, Values = new double[] { 17, 17.9, 18.4, 18.3, 18, 17.6, 17.4, 17.2, 17.3, 17.4, 17.6, 18, 18.2, 18.5, 18.9, 19.4, 19.8, 20.3, 20.8, 21.2, 21.6, 22, 22.3, 22.5, 23, 23.2, 23.6, 24, 24.25, 24.5, 24.9, 25.2, 25.5, 25.8, 26.2, 26.5, 26.9 } });
            boyslines.Add(7, new LineData { StrokeData = new LineData.Stroke { Color = "navy", DashArray = "", DashName = "Solid", Width = "4", WidthValue = "4pt" }, Values = new double[] { 17.7, 18.7, 19.2, 19.3, 19.1, 18.8, 18.6, 18.65, 18.7, 19, 19.3, 19.8, 20.4, 20.9, 21.4, 22, 22.4, 22.9, 23.4, 23.85, 24.2, 24.55, 24.9, 25.4, 25.9, 26.4, 26.9, 27.1, 27.4, 27.6, 28, 28.35, 28.7, 29, 29.4, 29.7, 30 } });
            boyslines.Add(8, new LineData { StrokeData = new LineData.Stroke { Color = "black", DashArray = "", DashName = "Solid", Width = "2", WidthValue = "2pt" }, Values = new double[] { 18.7, 19.55, 20.2, 20.2, 20, 19.75, 19.7, 19.9, 20.4, 20.65, 21.1, 21.65, 22.5, 23.2, 23.9, 24.5, 25, 25.5, 26, 26.6, 27.1, 27.5, 27.9, 28.3, 28.8, 29.2, 29.6, 29.9, 30.2, 30.6, 31.1, 31.5, 31.8, 32.2, 32.5, 33, 33.3 } });

            var Girllines = new Dictionary<int, LineData>();
            Girllines.Add(1, new LineData { StrokeData = new LineData.Stroke { Color = "black", DashArray = "", DashName = "Solid", Width = "2", WidthValue = "2pt" }, Values = new double[] { 12.9, 13.6, 14.2, 14.1, 13.8, 13.6, 13.5, 13.45, 13.45, 13.4, 13.4, 13.4, 13.45, 13.45, 13.5, 13.6, 13.7, 13.9, 14.15, 14.3, 14.45, 14.6, 14.75, 15, 15.3, 15.5, 15.9, 16.2, 16.5, 16.9, 17.25, 17.5, 17.7, 17.9, 18, 18.1, 18.2 } });
            Girllines.Add(2, new LineData { StrokeData = new LineData.Stroke { Color = "black", DashArray = "4,4", DashName = "Dashed", Width = "2", WidthValue = "2pt" }, Values = new double[] { 13.9, 14.5, 14.95, 14.9, 14.6, 14.4, 14.25, 14.2, 14.2, 14.2, 14.3, 14.35, 14.4, 14.45, 14.55, 14.65, 14.9, 15, 15.25, 15.4, 15.55, 15.75, 16, 16.25, 16.5, 16.85, 17.2, 17.45, 17.75, 18, 18.4, 18.6, 18.9, 19, 19.2, 19.35, 19.4 } });
            Girllines.Add(3, new LineData { StrokeData = new LineData.Stroke { Color = "black", DashArray = "4,4", DashName = "Dashed", Width = "2", WidthValue = "2pt" }, Values = new double[] { 14.1, 15.4, 15.9, 15.9, 15.65, 15.45, 15.2, 15, 15, 15.2, 15.05, 15.2, 15.3, 15.45, 15.65, 15.85, 16.05, 16.2, 16.5, 16.7, 16.9, 17.1, 17.4, 17.75, 18, 18.4, 18.7, 18.9, 19.2, 19.4, 19.6, 19.8, 20, 20.2, 20.3, 20.4, 20.5 } });
            Girllines.Add(4, new LineData { StrokeData = new LineData.Stroke { Color = "green", DashArray = "", DashName = "Solid", Width = "2", WidthValue = "2pt" }, Values = new double[] { 15.4, 16.5, 17.4, 17.35, 16.9, 16.45, 16.2, 16.1, 16.05, 16.1, 16.2, 16.4, 16.55, 16.75, 17, 17.3, 17.6, 17.9, 18.2, 18.5, 18.75, 19, 19.4, 19.7, 20.1, 20.45, 20.75, 21, 21.3, 21.4, 21.55, 21.65, 21.8, 21.9, 22, 22.1, 22.2 } });
            Girllines.Add(5, new LineData { StrokeData = new LineData.Stroke { Color = "#ca6576", DashArray = "2,2", DashName = "Dotted", Width = "2", WidthValue = "2pt" }, Values = new double[] { 16, 17.4, 18, 18.1, 17.6, 17.45, 17.25, 17.2, 17.25, 17.4, 17.6, 17.9, 18.2, 18.5, 18.85, 19.25, 19.75, 20.1, 20.4, 20.9, 21.1, 21.5, 21.95, 22.25, 22.7, 23, 23.2, 23.4, 23.55, 23.6, 23.7, 23.9, 23.95, 24.1, 24.2, 24.4, 24.6 } });
            Girllines.Add(6, new LineData { StrokeData = new LineData.Stroke { Color = "#ca6576", DashArray = "", DashName = "Solid", Width = "4", WidthValue = "4pt" }, Values = new double[] { 16.45, 17.8, 18.4, 18.4, 18.1, 17.9, 17.65, 17.7, 17.9, 18.2, 18.5, 18.8, 19.25, 19.6, 20, 20.5, 20.9, 21.3, 21.5, 21.9, 22.2, 22.5, 22.9, 23.4, 23.7, 24, 24.4, 24.55, 24.7, 24.7, 24.9, 24.95, 25, 25.1, 25.2, 25.25, 25.3 } });
            Girllines.Add(7, new LineData { StrokeData = new LineData.Stroke { Color = "black", DashArray = "4,4", DashName = "Dashed", Width = "2", WidthValue = "2pt" }, Values = new double[] { 16.85, 17.9, 18.7, 18.7, 18.5, 18.2, 18.05, 18.2, 18.5, 18.8, 19.25, 19.5, 20, 20.5, 20.95, 21.5, 22, 22.45, 22.7, 23.2, 23.5, 23.8, 24.1, 24.65, 25, 25.5, 25.75, 26, 26.35, 26.45, 26.6, 26.7, 26.8, 27, 27.15, 27.4, 27.5 } });
            Girllines.Add(8, new LineData { StrokeData = new LineData.Stroke { Color = "#ca6576", DashArray = "", DashName = "Solid", Width = "4", WidthValue = "4pt" }, Values = new double[] { 17.8, 19, 19.75, 19.8, 19.25, 18.8, 18.5, 18.7, 19.4, 19.9, 20.55, 21.1, 21.6, 22.2, 22.9, 23.4, 23.9, 24.4, 24.7, 24.9, 25.2, 25.5, 26.1, 26.5, 27.2, 27.8, 28.1, 28.4, 28.65, 28.9, 29.1, 29.2, 29.35, 29.45, 29.6, 29.7, 29.9 } });
            Girllines.Add(9, new LineData { StrokeData = new LineData.Stroke { Color = "black", DashArray = "", DashName = "Solid", Width = "2", WidthValue = "2pt" }, Values = new double[] { 18.4, 19.6, 20.4, 20.3, 19.65, 19, 18.9, 19.1, 19.9, 20.5, 21.35, 22, 22.45, 23, 23.5, 24, 24.55, 25.2, 25.8, 26.3, 26.7, 27.1, 27.55, 28, 28.4, 28.8, 29.2, 29.5, 29.7, 30.1, 30.45, 30.9, 31.25, 31.75, 32.15, 32.48, 32.6 } });
            _Values.Add(Sex.Boy, boyslines);
            _Values.Add(Sex.Girl, Girllines);
        }

        public int[][][] VerticalLines
        {
            get
            {
                List<int[][]> Lines = new List<int[][]>();
                for (double i = Margins.Left; i <= (Size.Width - Margins.Right); i += StepX)//for (int i = 0; i < Ages.Count(); ++i) //
                {
                    Lines.Add(new int[][] { new int[] { Convert.ToInt32(i), Margins.Top }, new int[] { Convert.ToInt32(i), Size.Height - Margins.Bottom } });
                }
                return Lines.ToArray();
            }
        }
        public int[][][] HorizontalLines
        {
            get
            {
                List<int[][]> Lines = new List<int[][]>();
                //for (int i = (Size.Height - Margins.Bottom); i >= Convert.ToInt32(StepY); i -= Convert.ToInt32(StepY))
                //{
                //    Lines.Add(new int[][] { new int[] { Margins.Left, i }, new int[] { Size.Width - Margins.Right, i } });
                //}
                for (double i = (Size.Height - Margins.Bottom); i >= Margins.Top; i -= StepY)
                {
                    Lines.Add(new int[][] { new int[] { Margins.Left, (int)i }, new int[] { Size.Width - Margins.Right, (int)i } });
                }
                return Lines.ToArray();
            }
        }

        public AxisLabel[] AgesScale
        {
            get
            {
                var _AgesScale = new List<AxisLabel>();
                for (int i = 0; i < Ages.Count(); i += 2)
                {
                    _AgesScale.Add(new AxisLabel { Text = Ages[i].ToString(), LocationX = Convert.ToInt32(Margins.Left + i * StepX), LocationY = Size.Height });
                }
                return _AgesScale.ToArray();
            }
        }
        public AxisLabel[] BMIScale
        {
            get
            {
                var _BMIScale = new List<AxisLabel>();
                var counter = 10;
                for (double i = (Size.Height - Margins.Bottom); i >= Margins.Top; i -= StepY)
                {
                    _BMIScale.Add(new AxisLabel { Text = counter.ToString(), LocationX = Convert.ToInt32(Margins.Left / 2), LocationY = (int)i });
                    ++counter;
                }
                return _BMIScale.ToArray();
            }
        }

        public object LineValues
        {
            get
            {
                List<BMILineData> Result = new List<BMILineData>();
                foreach (var line in Values[CustomerSex])
                {
                    var dash = line.Value.StrokeData.DashArray.Split(',');
                    var linedata = new BMILineData { Color = line.Value.StrokeData.Color, Width = line.Value.StrokeData.Width,WidthValue = line.Value.StrokeData.WidthValue , DashName = line.Value.StrokeData.DashName, DashArray = (dash.Count() > 1) ? dash.Select(r => Convert.ToInt32(r)).ToArray() : new Int32[] { 0 } };
                    var values = new List<int[]>();

                    for (int i = 0; i < Ages.Count(); ++i)
                    {
                        values.Add(GetCoOrdinates(Ages[i], line.Value.Values[i]));
                    }
                    linedata.Values = values.ToArray();
                    Result.Add(linedata);
                }
                return Result.ToArray();
            }
        }
    }

    public class BMILineData
    {
        public string Width { get; set; }
        public string WidthValue { get; set; }
        public string Color { get; set; }
        public int[] DashArray { get; set; }
        public string DashName { get; set; }
        public int[][] Values { get; set; }
    }

    public class AxisLabel
    {
        public string Text { get; set; }

        public int LocationX { get; set; }
        public int LocationY { get; set; }
    }
}