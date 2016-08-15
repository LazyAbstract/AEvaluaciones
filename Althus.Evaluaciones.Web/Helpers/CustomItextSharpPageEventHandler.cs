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
            head.TotalWidth = page.Width-(document.LeftMargin+document.RightMargin);
            head.SpacingAfter = 10;

            // add image; PdfPCell() overload sizes image to fit cell
            ImageHeader.ScaleAbsoluteHeight(cellHeight);
            ImageHeader.ScaleAbsoluteWidth(page.Width - (document.LeftMargin + document.RightMargin));
            PdfPCell c = new PdfPCell(ImageHeader, true);
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.VerticalAlignment = Element.ALIGN_BOTTOM;
            c.FixedHeight = cellHeight-35;
            c.Border = PdfPCell.NO_BORDER;
            head.AddCell(c);

            // add the header text
            c = new PdfPCell(new Phrase(
              "Evaluación PSICOLABORAL",
              FontFactory.GetFont("Arial", 12, Font.BOLD, new BaseColor(0, 102, 0))
            ));
            c.Border = PdfPCell.NO_BORDER;
            c.VerticalAlignment = Element.ALIGN_TOP;
            c.HorizontalAlignment = Element.ALIGN_LEFT;
            c.FixedHeight = 20;
            head.AddCell(c);

            c = new PdfPCell(new Phrase(
              "Confidencial",
              FontFactory.GetFont("Arial", 10, Font.BOLD, new BaseColor(0, 102, 0))
            ));
            c.Border = PdfPCell.BOTTOM_BORDER;
            c.VerticalAlignment = Element.ALIGN_TOP;
            c.HorizontalAlignment = Element.ALIGN_LEFT;
            c.FixedHeight = 15;
            head.AddCell(c);

            // since the table header is implemented using a PdfPTable, we call
            // WriteSelectedRows(), which requires absolute positions!
            head.WriteSelectedRows(
              0, -1,  // first/last row; -1 flags all write all rows
              document.LeftMargin,      // left offset
                // ** bottom** yPos of the table
              page.Height - cellHeight + head.TotalHeight,
              writer.DirectContent
            );

            // Footer 
            PdfPTable foot = new PdfPTable(1);
            foot.TotalWidth = page.Width - (document.LeftMargin + document.RightMargin);
            foot.SpacingBefore = 10;
            c = new PdfPCell(new Phrase(
              document.PageNumber.ToString(),
              FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)
            ));
            c.Border = PdfPCell.TOP_BORDER;
            c.VerticalAlignment = Element.ALIGN_MIDDLE;
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.FixedHeight = 20;
            foot.AddCell(c);
            foot.WriteSelectedRows(
              0, -1,  // first/last row; -1 flags all write all rows
              document.LeftMargin,      // left offset
                // ** bottom** yPos of the table
               document.Bottom-foot.TotalHeight,
              writer.DirectContent
            );
        }
    }
}