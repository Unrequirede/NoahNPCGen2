using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;

namespace NoahNPCGen
{
    public class AddPDF
    {
        public void AddElem(ref PdfDocument doc, string pdfField, string entry)
        {
            PdfTextField currentField = (PdfTextField)(doc.AcroForm.Fields[pdfField]);
            string caseName = entry;
            PdfString caseNamePdfStr = new PdfString(caseName);
            //set the value of this field
            currentField.Value = caseNamePdfStr;
        }

        public void AddElem(ref PdfDocument doc, string pdfField, bool entry)
        {
            PdfCheckBoxField currentField = (PdfCheckBoxField)(doc.AcroForm.Fields[pdfField]);
            //set the value of this field
            currentField.Checked = entry;
        }
    }
}
