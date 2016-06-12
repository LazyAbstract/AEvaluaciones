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
        //private Evaluacion _Evaluacion { get; set; }
        private byte[] _File { get; set; }

        public GeneraEvaluacionPDF()//(Evaluacion evaluacion)
        {
            //_Evaluacion = evaluacion;
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


                //Exportar archivo
                document.Close();
                writer.Close();
                ms.Close();
                _File = ms.GetBuffer();
            }
        }

    }
}