using System.Text;
using DiscussionFleet.Application.Common.Providers;
using Net.Codecrete.QrCodeGenerator;

namespace DiscussionFleet.Infrastructure.Providers;

public class QrCodeProvider : IQrCodeProvider
{
    public Task<string> GenerateSvgStringAsync(string input, int sizeInPx,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var qr = QrCode.EncodeText(input, QrCode.Ecc.Medium);
            var path = qr.ToGraphicsPath(1);
            var viewBoxSize = qr.Size + 1 * 2;
            var sb = new StringBuilder();

            sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" viewbox=\"0 0 ")
                .Append(viewBoxSize)
                .Append(' ')
                .Append(viewBoxSize)
                .Append("\" stroke=\"none\" width=\"")
                .Append(sizeInPx)
                .Append("\" height=\"")
                .Append(sizeInPx)
                .Append("\" role=\"img\" aria-label=\"QR Code\">")
                .Append("<rect width=\"100%\" height=\"100%\" fill=\"#ffffff\"></rect>")
                .Append("<path d=\"")
                .Append(path)
                .Append("\" fill=\"#000000\"></path>")
                .Append("</svg>");

            return sb.ToString();
        }, cancellationToken);
    }
}