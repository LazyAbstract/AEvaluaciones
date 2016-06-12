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

        public GeneraEvaluacionPDF(Evaluacion evaluacion)//(Evaluacion evaluacion)
        {
            _Evaluacion = evaluacion;
            GenerateFile();
        }

        public void GenerateFile(){
            Rectangle rectangle = new Rectangle(PageSize.LETTER);
            using (MemoryStream ms = new MemoryStream())
            using (Document document = new Document(rectangle))
            using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
            {
                //Configuración y propiedades el archivo
                document.AddTitle("Evaluación");
                document.AddSubject("This is an Example 4 of Chapter 1 of Book 'iText in Action'");
                document.AddKeywords("Evaluación, Althus, Partners");
                document.AddCreator("Plataforma de Evaluación");
                document.AddAuthor("Plataforma de Evaluación");
                //doc.AddHeader();

                //Generación del contenido
                document.Open();
                PdfPTable firstTable = GetPdfTable(2);
                firstTable.AddCell(GetHeaderCell("Nombre"));

                firstTable.AddCell(GetHeaderCell("Cargo al cual postula"));
                firstTable.AddCell(GetNormalCell(_Evaluacion.Evaluado.Nombre));                
                firstTable.AddCell(GetHeaderCell("Empresa"));
                firstTable.AddCell(GetNormalCell(""));  
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

                //Exportar archivo
                document.Close();
                writer.Close();
                ms.Close();
                _File = ms.GetBuffer();
            }
        }

        private PdfPTable GetPdfTable(int numColumns) {
            PdfPTable table = new PdfPTable(numColumns);
            table.HorizontalAlignment = Rectangle.ALIGN_LEFT;
            return table;
        }

        private PdfPCell GetHeaderCell(string texto){
            Phrase frase = new Phrase(texto ?? String.Empty, headerFont);
            PdfPCell cel = new PdfPCell(frase);
            cel.BackgroundColor = BaseColor.GREEN;
            return cel;
        }

        private PdfPCell GetNormalCell(string texto)
        {
            Phrase frase = new Phrase(texto ?? String.Empty, normalFont);
            PdfPCell cel = new PdfPCell(frase);
            return cel;
        }

        private const Font headerFont = new Font(Font.FontFamily.TIMES_ROMAN, 14, Font.BOLD, BaseColor.WHITE);
        private const Font normalFont = new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.NORMAL, BaseColor.BLACK);
    }
}