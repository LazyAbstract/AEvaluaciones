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
                Series serie0 = chart.Series.Add("Esperado");
                serie0.ChartType = SeriesChartType.Bar;
                Series serie1 = chart.Series.Add("Obtenido");
                int i = 1;
                foreach (var competencia in evaluacion.EvaluacionCompetencias)
                {
                    chart.Series[0].Points.AddXY(i, competencia.Competencia.ValorEsperado);
                    chart.Series[1].Points.AddXY(i, competencia.ValorObtenido);
                    i++;
                }
                chart.SaveImage(ms, ChartImageFormat.Png);
                ms.Close();
                result = ms.GetBuffer();
            }
            return result;
        }
    }
}