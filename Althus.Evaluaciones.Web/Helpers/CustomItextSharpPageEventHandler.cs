using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.Helpers
{
    public class MyPageEventHandler : PdfPageEventHelper
    {
        /*
         * We use a __single__ Image instance that's assigned __once__;
         * the image bytes added **ONCE** to the PDF file. If you create 
         * separate Image instances in OnEndPage()/OnEndPage(), for example,
         * you'll end up with a much bigger file size.
         */
        public Image ImageHeader { get; set; }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            // cell height 
            float cellHeight = document.TopMargin;
            // PDF document size      
            Rectangle page = document.PageSize;

            
            // create two column table
            PdfPTable head = new PdfPTable(1);
            head.TotalWidth = page.Width;
            head.PaddingTop = 20;
            head.SpacingAfter = 10;

            // add image; PdfPCell() overload sizes image to fit cell
            ImageHeader.ScaleAbsoluteHeight(cellHeight);
            ImageHeader.ScaleAbsoluteWidth(page.Width);
            PdfPCell c = new PdfPCell(ImageHeader, true);
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.FixedHeight = cellHeight-20;
            c.Border = PdfPCell.NO_BORDER;
            c.PaddingLeft = 40;
            head.AddCell(c);

            // add the header text
            c = new PdfPCell(new Phrase(
              "Evaluación pSICOLABORAL",
              FontFactory.GetFont("Arial", 14, Font.BOLD, new BaseColor(0, 102, 0))
            ));
            c.Border = PdfPCell.NO_BORDER;
            c.VerticalAlignment = Element.ALIGN_TOP;
            c.HorizontalAlignment = Element.ALIGN_LEFT;
            c.FixedHeight = 20;
            c.PaddingRight = 40;
            head.AddCell(c);

            // since the table header is implemented using a PdfPTable, we call
            // WriteSelectedRows(), which requires absolute positions!
            head.WriteSelectedRows(
              0, -1,  // first/last row; -1 flags all write all rows
              0,      // left offset
                // ** bottom** yPos of the table
              page.Height - cellHeight + head.TotalHeight,
              writer.DirectContent
            );
        }
    }
}