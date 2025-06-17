using System.Drawing;
using System.Drawing.Imaging;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Image = iText.Layout.Element.Image;
// <-- necesario para Any(), Where(), OrderBy(), FirstOrDefault()

namespace CargaImagenes.Core
{
    public static class GeneradorCatalogoPdf
    {
        public static void GenerarCatálogo(List<Producto> productos, string rutaSalida, IProgress<int>? progress = null)
        {
            if (productos == null) throw new ArgumentNullException(nameof(productos));
            if (string.IsNullOrWhiteSpace(rutaSalida)) throw new ArgumentException("Debe indicar ruta de salida.", nameof(rutaSalida));

            // ──────────── 1. Constantes de formato ────────────
            const float altoCeldaPt = 398f;
            const float bordePt = 2f;
            const int columnas = 2;
            const int fontDescPx = 15;
            const int fontSecPx = 12;
            const int fontSmallPx = 15;
            const float imgHScale = 0.95f;
            const float imgWScale = 0.95f;
            const int itemsPorPag = 4;    // 2×2
            const float margenPt = 10f;
            const float paddingCeldaPt = 5f;

            // ──────────── 2. Filtrar y ordenar ────────────
            List<Producto> lista;
            if (productos.Any(p => p.Seleccionado))
            {
                // hay seleccionados → sólo esos
                lista = productos
                    .Where(p => p.Seleccionado)
                    .OrderBy(p => p.Descripcion, StringComparer.CurrentCultureIgnoreCase)
                    .ToList();
            }
            else
            {
                // no hay ninguno marcado → todos
                lista = productos
                    .OrderBy(p => p.Descripcion, StringComparer.CurrentCultureIgnoreCase)
                    .ToList();
            }

            // ──────────── 3. Crear documento ────────────
            using var writer = new PdfWriter(rutaSalida);
            using var pdf = new PdfDocument(writer);
            using var doc = new Document(pdf, PageSize.A4);

            doc.SetMargins(margenPt, margenPt, margenPt, margenPt);

            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            float usableWidth = PageSize.A4.GetWidth() - 2 * margenPt;

            int índice = 0, total = lista.Count;
            int procesados = 0;
            progress?.Report(0);

            while (índice < total)
            {
                var tabla = new Table(UnitValue.CreatePercentArray(Enumerable.Repeat(1f, columnas).ToArray()))
                                .UseAllAvailableWidth()
                                .SetFixedLayout();

                int itemsEstaPágina = 0;

                // ──── 4. Añadir hasta `itemsPorPag` productos ────
                while (itemsEstaPágina < itemsPorPag && índice < total)
                {
                    var prod = lista[índice++];
                    itemsEstaPágina++;

                    var celdaExt = new Cell()
                        .SetHeight(altoCeldaPt)
                        .SetPadding(paddingCeldaPt)
                        .SetBorder(new SolidBorder(bordePt));

                    var interna = new Table(1).UseAllAvailableWidth();

                    // A) Imagen
                    var cImg = new Cell()
                        .SetBorder(Border.NO_BORDER)
                        .SetHeight(usableWidth / columnas)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        .SetPaddingLeft(0)
                        .SetPaddingRight(0);

                    byte[]? imgBytes = null;

                    if (prod.Imagen != null)
                    {
                        using var bmp = new Bitmap(prod.Imagen);
                        using var ms = new MemoryStream();
                        bmp.Save(ms,
                                 GetEncoderInfo("image/jpeg"),
                                 new EncoderParameters(1)
                                 {
                                     Param = { [0] = new EncoderParameter(Encoder.Quality, 90L) }
                                 });
                        imgBytes = ms.ToArray();
                    }
                    else if (!string.IsNullOrEmpty(prod.RutaImagen) && File.Exists(prod.RutaImagen))
                    {
                        imgBytes = File.ReadAllBytes(prod.RutaImagen);
                    }


                    if (imgBytes != null)
                    {
                        var img = new Image(ImageDataFactory.Create(imgBytes))
                            .ScaleToFit((usableWidth / columnas) * imgWScale,
                                        (usableWidth / columnas) * imgHScale)
                            .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                            .SetMarginLeft(0)
                            .SetMarginRight(0);
                        cImg.Add(img);
                    }
                    else
                    {
                        cImg.Add(new Paragraph("Sin imagen")
                            .SetFont(fontRegular)
                            .SetFontSize(fontSmallPx));
                    }

                    interna.AddCell(cImg);

                    // B) Texto
                    var cTxt = new Cell().SetBorder(Border.NO_BORDER);
                    var parag = new Paragraph()
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Text(prod.Descripcion + "\n")
                            .SetFont(fontBold).SetFontSize(fontDescPx))
                        .Add(new Text($"CÓDIGO: {prod.Codigo}\n")
                            .SetFont(fontRegular).SetFontSize(fontSecPx))
                        .Add(new Text($"PRECIO: L. {prod.Precio:F2}")
                            .SetFont(fontRegular).SetFontSize(fontSecPx))
                        .SetPaddingTop(12f);
                    cTxt.Add(parag);
                    interna.AddCell(cTxt);

                    celdaExt.Add(interna);
                    tabla.AddCell(celdaExt);
                    procesados++;
                    progress?.Report(procesados * 100 / total);
                }

                // ──── 5. Rellenar huecos de la fila si quedó incompleta ────
                int resto = itemsEstaPágina % columnas;
                if (resto != 0)
                    for (int i = 0; i < columnas - resto; i++)
                        tabla.AddCell(new Cell()
                            .SetBorder(Border.NO_BORDER)
                            .SetHeight(altoCeldaPt));

                doc.Add(tabla);

                if (índice < total)
                    doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            }

            // ──────────── 6. Cerrar ────────────
            doc.Close();
            progress?.Report(100);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
#pragma warning disable CA1416
            => ImageCodecInfo.GetImageEncoders()
#pragma warning restore CA1416
                   .FirstOrDefault(enc => enc.MimeType == mimeType)
               ?? throw new InvalidOperationException("No se encontró encoder JPEG");
    }
}
