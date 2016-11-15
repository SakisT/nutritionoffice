using System;
using System.Web.UI.DataVisualization.Charting;
namespace nutritionoffice.Classes
{
    public class ChartClass
    {
        public Chart chart { get; set; }

        public KeyValue[] KeyValues { get; set; }
        public ChartClass(SeriesChartType ChartType, KeyValue[] KeyValuePairs)
        {
            chart = new Chart();
            Series serie = new Series("Series");
            serie.ChartArea = "Area1";
            serie.ChartType = ChartType;
            foreach (var item in KeyValuePairs)
            {
                DataPoint point = new DataPoint
                {
                    
                    AxisLabel = item.LabelText,
                    LegendText = item.LegentText,
                    YValues = new double[] { item.Value }
                };

                serie.Points.Add(point);
            }
            Legend legent = new Legend()
            {
               IsDockedInsideChartArea=true, LegendStyle=LegendStyle.Table
            };

            chart.Legends.Add(legent);

            chart.Series.Add(serie);
            ChartArea ca1 = new ChartArea("Area1");
            chart.ChartAreas.Add(ca1);
        }

        public class KeyValue
        {
            public string LabelText { get; set; }
            public double Value { get; set; }
            public string LegentText { get; set; }
        }

        public string ImageString
        {
            get
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    chart.SaveImage(ms, ChartImageFormat.Png);
                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    return Convert.ToBase64String(ms.ToArray());
                    //return File(ms.ToArray(), "image/png", "mychart.png");
                }
            }
        }
    }
}