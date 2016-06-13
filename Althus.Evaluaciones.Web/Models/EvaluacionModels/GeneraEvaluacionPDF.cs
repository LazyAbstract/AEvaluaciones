using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Althus.Evaluaciones.Core;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Althus.Evaluaciones.Web.Models.EvaluacionModels
{
    public class GeneraEvaluacionPDF
    {
        private Evaluacion _Evaluacion { get; set; }
        private byte[] _File { get; set; }
        private Font headerFont = new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD, BaseColor.WHITE);
        private Font normalFont = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.NORMAL, BaseColor.BLACK);
        private Font Title1Font = new Font(Font.FontFamily.TIMES_ROMAN, 14, Font.BOLD, BaseColor.BLACK);
        private Font Title2Font = new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD, BaseColor.BLACK);

        public GeneraEvaluacionPDF(Evaluacion evaluacion)
        {
            _Evaluacion = evaluacion;
            GenerateFile();
        }

        private void GenerateFile()
        {
            Rectangle rectangle = new Rectangle(PageSize.LETTER);
            using (MemoryStream ms = new MemoryStream())
            using (Document document = new Document(rectangle))
            using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
            {
                //Configuración y propiedades el archivo
                document.AddTitle("EVALUACIÓN PSICOLABORAL");
                document.AddSubject("EVALUACIÓN PSICOLABORAL");
                document.AddKeywords("Evaluación, Althus, Partners");
                document.AddCreator("Plataforma de Evaluación");
                document.AddAuthor("Plataforma de Evaluación");
                //doc.AddHeader();

                //Generación del contenido
                document.Open();

                // Título 
                Paragraph titulo = new Paragraph("IDENTIFICACION DEL CANDIDATO:", Title1Font);
                document.Add(titulo);
                // EVALUACIÓN PSICOLABORAL
                // Confidencial 
                PdfPTable firstTable = GetPdfTable(new float[] { 1, 2 });
                firstTable.AddCell(GetHeaderCell("Nombre"));
                firstTable.AddCell(GetNormalCell(_Evaluacion.Evaluado.NombreCompleto));
                firstTable.AddCell(GetHeaderCell("Cargo al cual postula"));
                firstTable.AddCell(GetNormalCell(_Evaluacion.Cargo.Cargo1));
                firstTable.AddCell(GetHeaderCell("Empresa"));
                firstTable.AddCell(GetNormalCell(_Evaluacion.Cargo.Empresa.Empresa1));
                firstTable.AddCell(GetHeaderCell("Profesión"));
                firstTable.AddCell(GetNormalCell(_Evaluacion.Evaluado.Profesion));
                DateTime today = DateTime.Today;
                int age = today.Year - _Evaluacion.Evaluado.FechaNacimiento.Year;
                if (_Evaluacion.Evaluado.FechaNacimiento > today.AddYears(-age))
                    age--;
                firstTable.AddCell(GetHeaderCell("Edad"));
                firstTable.AddCell(GetNormalCell(age.ToString()));
                firstTable.AddCell(GetHeaderCell("Rut"));
                firstTable.AddCell(GetNormalCell(new Rut(_Evaluacion.Evaluado.Rut).ToStringConGuion()));
                firstTable.AddCell(GetHeaderCell("Teléfonos"));
                firstTable.AddCell(GetNormalCell(_Evaluacion.Evaluado.Celular.ToString()));
                firstTable.AddCell(GetHeaderCell("E-mail"));
                firstTable.AddCell(GetNormalCell(_Evaluacion.Evaluado.Correo));
                firstTable.AddCell(GetHeaderCell("Fecha de Evaluación"));
                firstTable.AddCell(GetNormalCell(_Evaluacion.FechaEvaluacion.ToShortDateString()));
                document.Add(firstTable);
                //Resumen Estudios y Trayectoria Laboral
                PdfPTable secondTable = GetPdfTable(new float[] { 1 });
                secondTable.AddCell(GetHeaderCell("Resumen Estudios y Trayectoria Laboral"));
                secondTable.AddCell(GetNormalCell(_Evaluacion.EvaluacionAbiertas
                    .FirstOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.ResumenEstudiosTrayectoriaLaboral)
                    .EvaluacionAbierta1));
                document.Add(secondTable);
                //Motivación
                PdfPTable thirdTable = GetPdfTable(new float[] { 1 });
                thirdTable.AddCell(GetHeaderCell("Motivación Por Cargo"));
                thirdTable.AddCell(GetNormalCell(_Evaluacion.EvaluacionAbiertas
                    .FirstOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.MotivacionCargo)
                    .EvaluacionAbierta1));
                document.Add(thirdTable);
                //GRÁFICO DE COMPETENCIAS:
                Paragraph titulo2 = new Paragraph("GRÁFICO DE COMPETENCIAS:", Title1Font);
                document.Add(titulo2);
                GeneraGraficoEvaluacion grafico = new GeneraGraficoEvaluacion();
                document.Add(Image.GetInstance(grafico.GenerarGrafico(_Evaluacion)));

                // Competencias
                
                PdfPTable fourthTable = GetPdfTable(new float[] { 4, 1, 1, 6 });
                fourthTable.AddCell(GetHeaderCell("Competencia"));
                fourthTable.AddCell(GetHeaderCell("Observaciones"));
                fourthTable.AddCell(GetHeaderCell("Valor Obtenido"));
                fourthTable.AddCell(GetHeaderCell("Valor Esperado"));
                foreach (var competencia in _Evaluacion.EvaluacionCompetencias)
                {
                    fourthTable.AddCell(GetNormalCell(competencia.Competencia.Competencia1));
                    fourthTable.AddCell(GetNormalCell(competencia.Competencia.ValorEsperado.ToString()));
                    fourthTable.AddCell(GetNormalCell(competencia.ValorObtenido.ToString()));
                    fourthTable.AddCell(GetNormalCell(competencia.Observacion));
                }
                document.Add(fourthTable);

                // Conclusiones
                PdfPTable fifthTable = GetPdfTable(new float[] { 1 });
                fifthTable.AddCell(GetHeaderCell("Conclusión y Sugerencias"));
                fifthTable.AddCell(GetNormalCell(_Evaluacion.EvaluacionAbiertas
                    .FirstOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.ConclusionSugerencias)
                    .EvaluacionAbierta1));
                document.Add(fifthTable);

                // Conclusiones
                PdfPTable sixthTable = GetPdfTable(new float[] { 1 });
                sixthTable.AddCell(GetHeaderCell("Diagnóstico Psicolaboral"));
                sixthTable.AddCell(GetNormalCell(_Evaluacion.TipoDiagnostico.TipoDiagnostico1));
                document.Add(sixthTable);

                //GLosario
                document.Add(new Paragraph("CATEGORÍAS DE EVALUACIÓN PARA SELECCIÓN", Title1Font));
                document.Add(new Paragraph("RECOMENDABLE:", Title2Font));
                document.Add(new Paragraph("El postulante cumple cabalmente todas las exigencias y presenta un potencial de nivel sobresaliente.", normalFont));
                document.Add(new Paragraph("ADECUADO:", Title2Font));
                document.Add(new Paragraph("Satisface las condiciones exigidas presentando un nivel de potencial adecuado.", normalFont));
                document.Add(new Paragraph("ADECUADO CON OBSERVACIONES:", Title2Font));
                document.Add(new Paragraph("Cumple con condiciones básicas para el cargo, presentando debilidades específicas que pueden evolucionar positivamente.", normalFont));
                document.Add(new Paragraph("NO RECOMENDABLE", Title2Font));
                document.Add(new Paragraph("Satisface algunas condiciones, pero presenta debilidades significativas, características intelectuales, de personalidad o motivacionales que limitarían su nivel de desempeño.", normalFont));

                //Exportar archivo                                    
                document.Close();
                writer.Close();
                ms.Close();
                _File = ms.GetBuffer();
            }
        }

        private PdfPTable GetPdfTable(int numColumns)
        {
            PdfPTable table = new PdfPTable(numColumns);
            table.HorizontalAlignment = Rectangle.ALIGN_LEFT;
            return table;
        }

        private PdfPTable GetPdfTable(float[] arreglo)
        {
            PdfPTable table = new PdfPTable(arreglo);
            table.HorizontalAlignment = Rectangle.ALIGN_LEFT;
            table.SpacingBefore = 10;
            return table;
        }

        private PdfPCell GetHeaderCell(string texto)
        {
            Phrase frase = new Phrase(texto ?? String.Empty, headerFont);
            PdfPCell cel = new PdfPCell(frase);
            cel.BackgroundColor = new BaseColor(0, 102, 0);
            return cel;
        }

        private PdfPCell GetNormalCell(string texto)
        {
            Phrase frase = new Phrase(texto ?? String.Empty, normalFont);
            PdfPCell cel = new PdfPCell(frase);
            return cel;
        }

        public byte[] GetFile()
        {
            return _File;
        }
    }
}