using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResuniqAI.Models;

namespace ResuniqAI.Services
{
    public class PdfService
    {
        public byte[] GenerateResumePdf(Resume r)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Content().Column(col =>
                    {
                        col.Item().Text(r.FullName).FontSize(24).Bold();
                        col.Item().Text(r.Email);
                        col.Item().Text(r.Phone);

                        col.Item().PaddingTop(10).Text("Summary").Bold();
                        col.Item().Text(r.Summary ?? "");

                        col.Item().PaddingTop(10).Text("Skills").Bold();
                        col.Item().Text(r.Skills ?? "");

                        col.Item().PaddingTop(10).Text("Experience").Bold();
                        col.Item().Text(r.Experience ?? "");
                    });
                });
            });

            return doc.GeneratePdf();
        }
    }
}