using Althus.Evaluaciones.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Althus.Evaluaciones.Web.Models.EvaluacionModels
{
    public class GeneraGraficoEvaluacion
    {
        public byte[] GenerarGrafico(Evaluacion evaluacion)
        {
            byte[] result = new byte[]{};
            using (MemoryStream ms = new MemoryStream()){
                Chart chart = new Chart();
                List<string> y = new List<string>();
                List<int> xa = new List<int>();
                List<int> xb = new List<int>();
                foreach (var evalCompetencia in evaluacion.EvaluacionCompetencias.OrderByDescending(x => x.IdCompetencia))
                {
                    y.Add(evalCompetencia.Competencia.Competencia1);
                    xa.Add(evalCompetencia.Competencia.ValorEsperado);
                    xb.Add(evalCompetencia.ValorObtenido);
                }
                ChartArea chartArea = new ChartArea("Eval");
                chartArea.AxisX.MajorGrid.LineWidth = 0;
                chartArea.AxisY.MajorGrid.LineWidth = 0;
                chart.ChartAreas.Add(chartArea);
                Series serieA = new Series();
                serieA.Points.DataBindXY(y,xa);
                serieA.ChartType = SeriesChartType.Bar;
                serieA.ChartArea = "Eval";
                serieA.Color = System.Drawing.ColorTranslator.FromHtml("#4F81BD");
                serieA.IsVisibleInLegend = true;
                serieA.Name = "Valor Esperado";
                chart.Series.Add(serieA);
                Series serieB = new Series();
                serieB.Points.DataBindY(xb);
                serieB.ChartType = SeriesChartType.Bar;
                serieB.ChartArea = "Eval";
                serieB.Color = System.Drawing.ColorTranslator.FromHtml("#B7CCE4");
                serieB.IsVisibleInLegend = true;
                serieB.Name = "Valor Obtenido";
                chart.Series.Add(serieB);
                chart.Width = 800;
                Legend legend = new Legend();
                legend.DockedToChartArea = "Eval";
                legend.Docking = Docking.Right;
                legend.IsDockedInsideChartArea = false;
                chart.Legends.Add(legend);
                Title title = new Title("Evaluación de Competencias");
                chart.SaveImage(ms, ChartImageFormat.Png);
                result = ms.ToArray();
            }
            return result;
        }
    }
}