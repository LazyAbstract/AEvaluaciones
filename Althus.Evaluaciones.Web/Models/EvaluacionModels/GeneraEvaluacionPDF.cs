﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Althus.Evaluaciones.Core;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Althus.Evaluaciones.Web.Helpers;

namespace Althus.Evaluaciones.Web.Models.EvaluacionModels
{
    public class GeneraEvaluacionPDF
    {
        private Evaluacion _Evaluacion { get; set; }
        private byte[] _File { get; set; }
        private byte[] _imageEmpresa { get; set; }
        private Font headerFont = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.WHITE);
        private Font normalFont = FontFactory.GetFont("Arial", 9, BaseColor.BLACK);
        private Font diagnosticoFont = FontFactory.GetFont("Arial", 12, BaseColor.BLACK);
        private Font Title1Font = FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.BLACK);
        private Font Title1FontUnderline = FontFactory.GetFont("Arial", 12, Font.UNDERLINE, BaseColor.BLACK);
        private Font Title2Font = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);

        public GeneraEvaluacionPDF(Evaluacion evaluacion, byte[] imageEmpresa)
        {
            _Evaluacion = evaluacion;
            _imageEmpresa = imageEmpresa;
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
                document.SetMargins(document.LeftMargin+20, document.RightMargin+20, document.TopMargin+65, document.BottomMargin+20);
                //Header
                // the image we're using for the page header    
                if (_imageEmpresa != null)
                {
                    Image imageHeader = Image.GetInstance(_imageEmpresa);
                    MyPageEventHandler e = new MyPageEventHandler()
                    {
                        ImageHeader = imageHeader
                    };
                    
                    writer.PageEvent = e;
                }
                document.Open();
                //Generación del contenido
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
                firstTable.AddCell(GetNormalCell(_Evaluacion.FechaEvaluacion.ToString("dd-MM-yyyy")));
                document.Add(firstTable);
                //Resumen Estudios y Trayectoria Laboral
                PdfPTable secondTable = GetPdfTable(new float[] { 1 });
                secondTable.AddCell(GetHeaderCell("Resumen Estudios y Trayectoria Laboral"));
                PdfPCell celdaContenido = GetNormalCellRA(_Evaluacion.EvaluacionAbiertas
                    .FirstOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.ResumenEstudiosTrayectoriaLaboral)
                    .EvaluacionAbierta1);
                //celdaContenido.PaddingTop = 10f;
                //celdaContenido.PaddingBottom = 10f;
                secondTable.AddCell(celdaContenido);
                document.Add(secondTable);
                //Motivación
                PdfPTable thirdTable = GetPdfTable(new float[] { 1 });
                thirdTable.AddCell(GetHeaderCell("Motivación Por Cargo"));
                celdaContenido = GetNormalCellRA(_Evaluacion.EvaluacionAbiertas
                    .FirstOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.MotivacionCargo)
                    .EvaluacionAbierta1);
                thirdTable.AddCell(celdaContenido);
                document.Add(thirdTable);
                //GRÁFICO DE COMPETENCIAS:
                document.NewPage();
                Paragraph titulo2 = new Paragraph("GRÁFICO DE COMPETENCIAS:", Title1Font);
                document.Add(titulo2);
                PdfPTable chartTable = GetPdfTable(1);
                //chartTable.AddCell(GetHeaderCell("GRÁFICO DE COMPETENCIAS"));
                GeneraGraficoEvaluacion graficEvaluacion = new GeneraGraficoEvaluacion();
                Image grafico = Image.GetInstance(graficEvaluacion.GenerarGrafico(_Evaluacion));
                grafico.WidthPercentage = 100;
                chartTable.AddCell(grafico);
                chartTable.SpacingBefore = 20f;
                chartTable.SpacingAfter = 20f;
                document.Add(chartTable);

                // Competencias
                PdfPTable fourthTable = GetPdfTable(new float[] { 3, 12, 2, 2 });
                fourthTable.AddCell(GetHeaderCellCenter("Competencia"));
                fourthTable.AddCell(GetHeaderCellCenter("Observaciones"));
                fourthTable.AddCell(GetHeaderCellCenter("Valor Esperado"));
                fourthTable.AddCell(GetHeaderCellCenter("Valor Obtenido"));
                
                foreach (var competencia in  _Evaluacion.Cargo.Competencias.OrderByDescending(x=> x.IdCompetencia))
                {
                    EvaluacionCompetencia evaluacion = _Evaluacion.EvaluacionCompetencias.SingleOrDefault(x => x.IdCompetencia == competencia.IdCompetencia);
                    fourthTable.AddCell(GetHeaderLeftCellCenter(competencia.Competencia1));
                    fourthTable.AddCell(GetNormalCell(evaluacion != null ? evaluacion.Observacion : String.Empty));
                    fourthTable.AddCell(GetNormalCellCenter(competencia.ValorEsperado.ToString()));
                    fourthTable.AddCell(GetNormalCellCenter(evaluacion != null ? evaluacion.ValorObtenido.ToString() : String.Empty));
                }
                document.Add(fourthTable);

                // Conclusiones
                PdfPTable fifthTable = GetPdfTable(new float[] { 1 });
                fifthTable.AddCell(GetHeaderCell("Conclusión y Sugerencias"));
                fifthTable.KeepTogether = true;
                fifthTable.AddCell(GetNormalCellRA(_Evaluacion.EvaluacionAbiertas
                    .FirstOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.ConclusionSugerencias)
                    .EvaluacionAbierta1));
                document.Add(fifthTable);

                // Diagnóstico
                PdfPTable sixthTable = GetPdfTable(new float[] { 1 });
                sixthTable.KeepTogether = true;
                sixthTable.AddCell(GetHeaderCell("Diagnóstico Psicolaboral"));
                sixthTable.AddCell(GetDiagnosticoCell(_Evaluacion.TipoDiagnostico.TipoDiagnostico1));
                document.Add(sixthTable);

                //GLosario
                document.NewPage();
                
                document.Add(new Paragraph("CATEGORÍAS DE EVALUACIÓN PARA SELECCIÓN", Title1FontUnderline)
                { 
                    Alignment = Rectangle.ALIGN_CENTER, 
                    SpacingBefore=10f, 
                    SpacingAfter=10f 
                });
                document.Add(new Paragraph("RECOMENDABLE:", Title2Font) { SpacingBefore = 10f });
                document.Add(new Paragraph("El postulante cumple cabalmente todas las exigencias y presenta un potencial de nivel sobresaliente.", normalFont) { Alignment = Rectangle.ALIGN_JUSTIFIED });
                document.Add(new Paragraph("ADECUADO:", Title2Font) { SpacingBefore = 10f });
                document.Add(new Paragraph("Satisface las condiciones exigidas presentando un nivel de potencial adecuado.", normalFont) { Alignment = Rectangle.ALIGN_JUSTIFIED });
                document.Add(new Paragraph("ADECUADO CON OBSERVACIONES:", Title2Font) { SpacingBefore = 10f });
                document.Add(new Paragraph("Cumple con condiciones básicas para el cargo, presentando debilidades específicas que pueden evolucionar positivamente.", normalFont) { Alignment = Rectangle.ALIGN_JUSTIFIED });
                document.Add(new Paragraph("NO RECOMENDABLE:", Title2Font) { SpacingBefore=10f});
                document.Add(new Paragraph("Satisface algunas condiciones, pero presenta debilidades significativas, características intelectuales, de personalidad o motivacionales que limitarían su nivel de desempeño.", normalFont) { Alignment = Rectangle.ALIGN_JUSTIFIED });

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
            table.SpacingBefore = 10;
            table.WidthPercentage = 100;
            return table;
        }

        private PdfPTable GetPdfTable(float[] arreglo)
        {
            PdfPTable table = new PdfPTable(arreglo);
            table.HorizontalAlignment = Rectangle.ALIGN_LEFT;
            table.SpacingBefore = 10;
            table.WidthPercentage = 100;
            return table;
        }

        private PdfPCell GetHeaderCell(string texto)
        {
            Phrase frase = new Phrase(texto ?? String.Empty, headerFont);
            PdfPCell cel = new PdfPCell(frase);
            cel.BackgroundColor = new BaseColor(0, 102, 0);
            cel.BorderColor = BaseColor.LIGHT_GRAY;
            cel.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            cel.BorderWidth = 0.5f;
            cel.Padding = 5;
            return cel;
        }

        private PdfPCell GetHeaderCellCenter(string texto)
        {
            Phrase frase = new Phrase(texto ?? String.Empty, headerFont);
            PdfPCell cel = new PdfPCell(frase);
            cel.BackgroundColor = new BaseColor(0, 102, 0);
            cel.BorderColor = BaseColor.LIGHT_GRAY;
            cel.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            cel.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            cel.BorderWidth = 0.5f;
            cel.Padding = 5;            
            return cel;
        }

        private PdfPCell GetDiagnosticoCell(string texto)
        {
            Phrase frase = new Phrase(texto ?? String.Empty, diagnosticoFont);
            PdfPCell cel = new PdfPCell(frase);
            cel.BorderColor = BaseColor.LIGHT_GRAY;
            cel.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            cel.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            cel.BorderWidth = 0.5f;
            cel.Padding = 5;
            return cel;
        }

        private PdfPCell GetHeaderLeftCellCenter(string texto)
        {
            Phrase frase = new Phrase(texto ?? String.Empty, headerFont);
            frase.MultipliedLeading = 2f;
            PdfPCell cel = new PdfPCell(frase);
            cel.BackgroundColor = new BaseColor(51, 153, 51);
            cel.BorderColor = BaseColor.LIGHT_GRAY;
            cel.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            cel.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            cel.BorderWidth = 0.5f;
            cel.Padding = 5;
            return cel;
        }

        private PdfPCell GetNormalCell(string texto)
        {
            Phrase frase = new Phrase(texto ?? String.Empty, normalFont);
            frase.SetLeading(0.0f, 2.0f);
            PdfPCell cel = new PdfPCell(frase);
            cel.BorderColor = BaseColor.LIGHT_GRAY;
            cel.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            cel.HorizontalAlignment = Rectangle.ALIGN_JUSTIFIED;
            cel.BorderWidth = 0.5f;
            cel.Padding = 5;
            cel.SetLeading(0, 1.2f);
            return cel;
        }

        private PdfPCell GetNormalCellRA(string texto)
        {
            Phrase frase = new Phrase(texto ?? String.Empty, normalFont);
            frase.SetLeading(0.0f, 2.0f);
            PdfPCell cel = new PdfPCell(frase);
            cel.BorderColor = BaseColor.LIGHT_GRAY;
            cel.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            cel.HorizontalAlignment = Rectangle.ALIGN_JUSTIFIED;
            cel.BorderWidth = 0.5f;
            cel.Padding = 5;
            cel.PaddingTop = 10;
            cel.PaddingBottom = 12;
            cel.SetLeading(0, 1.5f);
            return cel;
        }

        private PdfPCell GetNormalCellCenter(string texto)
        {
            Phrase frase = new Phrase(texto ?? String.Empty, normalFont);
            frase.SetLeading(0.0f, 2.0f);
            PdfPCell cel = new PdfPCell(frase);
            cel.BorderColor = BaseColor.LIGHT_GRAY;
            cel.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            cel.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            cel.BorderWidth = 0.5f;
            cel.Padding = 5;
            return cel;
        }

        public byte[] GetFile()
        {
            return _File;
        }
    }
}