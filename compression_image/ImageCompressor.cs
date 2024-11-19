using K4os.Compression.LZ4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compression_image
{
    public class ImageCompressor
    {
        public byte[] Compress(byte[] data, string method, out long elapsedTimeMs)
        {
            var stopwatch = Stopwatch.StartNew();
            byte[] result = method switch
            {
                "LZ4" => LZ4Pickler.Pickle(data),
                "DEFLATE" => CompressDeflate(data),
                _ => throw new NotSupportedException($"The compression method '{method}' is not supported.")
            };
            stopwatch.Stop();
            elapsedTimeMs = stopwatch.ElapsedMilliseconds;
            return result;
        }

        public byte[] Decompress(byte[] data, string method, out long elapsedTimeMs)
        {
            var stopwatch = Stopwatch.StartNew();
            byte[] result = method switch
            {
                "LZ4" => LZ4Pickler.Unpickle(data),
                "DEFLATE" => DecompressDeflate(data),
                _ => throw new NotSupportedException($"The compression method '{method}' is not supported.")
            };
            stopwatch.Stop();
            elapsedTimeMs = stopwatch.ElapsedMilliseconds;
            return result;
        }

        private byte[] CompressDeflate(byte[] data)
        {
            using var ms = new MemoryStream();
            using (var deflateStream = new DeflateStream(ms, CompressionLevel.Optimal))
            {
                deflateStream.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }

        private byte[] DecompressDeflate(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var deflateStream = new DeflateStream(ms, CompressionMode.Decompress);
            using var output = new MemoryStream();
            deflateStream.CopyTo(output);
            return output.ToArray();
        }
    }
}
