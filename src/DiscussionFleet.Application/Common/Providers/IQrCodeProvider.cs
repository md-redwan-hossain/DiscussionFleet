namespace DiscussionFleet.Application.Common.Providers;

public interface IQrCodeProvider
{
    Task<string> GenerateSvgStringAsync(string input, int sizeInPx, CancellationToken cancellationToken= default);
}