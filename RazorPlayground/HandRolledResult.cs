using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace RazorPlayground
{
    public class HandRolledResult : IResult
    {
        private readonly int _count;

        private static ReadOnlySpan<byte> HtmlStart => "<html>\r\n"u8;
        private static ReadOnlySpan<byte> HtmlEnd => "</html>\r\n"u8;
        private static ReadOnlySpan<byte> ParagraphStart => "    <p>Hi #"u8;
        private static ReadOnlySpan<byte> ParagraphEnd => "!</p>\r\n"u8;

        public HandRolledResult(int count)
        {
            _count = count;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "text/html";

            var writer = new BufferWriter<PipeWriter>(httpContext.Response.BodyWriter);

            writer.Write(HtmlStart);

            for (int i = 0; i < _count; i++)
            {
                writer.Write(ParagraphStart);
                WriteInt32AsASCII(ref writer, i);
                writer.Write(ParagraphEnd);
            }

            writer.Write(HtmlEnd);
            writer.Commit();

            return httpContext.Response.BodyWriter.FlushAsync().AsTask();
        }

        private static void WriteInt32AsASCII(ref BufferWriter<PipeWriter> writer, int number)
        {
            var asciiLength = (int)Math.Floor(Math.Log10(number)) + 1;
            writer.Ensure(asciiLength);

            if (number == 0)
            {
                writer.Span[0] = (byte)'0';
                writer.Advance(1);
                return;
            }

            var currentIndex = asciiLength;
            while (number > 0)
            {
                var lastDigit = number % 10;
                number /= 10;
                currentIndex--;

                writer.Span[currentIndex] = (byte)(lastDigit + '0');
            }

            writer.Advance(asciiLength);
        }
    }
}
