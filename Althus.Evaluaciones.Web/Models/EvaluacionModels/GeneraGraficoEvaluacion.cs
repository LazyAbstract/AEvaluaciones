using Althus.Evaluaciones.Core;
using System;
using System.Collections.Generic;
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
                List<string> y = evaluacion.EvaluacionCompetencias.Select(x => x.Competencia.Competencia1).ToList();
                List<int> xa = evaluacion.EvaluacionCompetencias.Select(x => x.Competencia.ValorEsperado).ToList();
                List<int> xb = evaluacion.EvaluacionCompetencias.Select(x => x.ValorObtenido).ToList();
                ChartArea chartArea = new ChartArea("Eval");
                chart.ChartAreas.Add(chartArea);
                Series serieA = new Series();
                serieA.Points.DataBindXY(y,xa);
                serieA.ChartType = SeriesChartType.Bar;
                serieA.ChartArea = "Eval";
                chart.Series.Add(serieA);
                Series serieB = new Series();
                serieB.Points.DataBindY(xb);
                serieB.ChartType = SeriesChartType.Bar;
                serieB.ChartArea = "Eval";
                chart.Series.Add(serieB);
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                //ms.Close();
                result = ms.ToArray();
            }
            return result;
        }
    }
}