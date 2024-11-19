using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compression_image
{
    public class DatabaseHelper
    {
        private const string ConnectionString = "Host=localhost;Username=postgres;Password=123456;Database=remote_sensing";

        public void SaveToDatabase(byte[] imageBytes, string compressionMethod, string fileName, int originalSize, int compressedSize, int compressionTimeMs, int decompressionTimeMs)
        {
            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            const string query = @"
                    INSERT INTO images (image_data, compression_method, file_name, file_size, compressed_size, compression_time_ms, decompression_time_ms)
                    VALUES (@image_data, @compression_method, @file_name, @file_size, @compressed_size, @compression_time_ms, @decompression_time_ms)";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("image_data", imageBytes);
            cmd.Parameters.AddWithValue("compression_method", compressionMethod);
            cmd.Parameters.AddWithValue("file_name", fileName);
            cmd.Parameters.AddWithValue("file_size", originalSize);
            cmd.Parameters.AddWithValue("compressed_size", compressedSize);
            cmd.Parameters.AddWithValue("compression_time_ms", compressionTimeMs);
            cmd.Parameters.AddWithValue("decompression_time_ms", decompressionTimeMs);

            cmd.ExecuteNonQuery();
        }

        public (byte[] CompressedData, string OriginalFileName) GetCompressedDataAndFileNameFromDatabase(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentException("Compression method cannot be null or empty.");

            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            const string query = @"
                    SELECT image_data, file_name 
                    FROM images 
                    WHERE compression_method = @compression_method 
                    ORDER BY id DESC 
                    LIMIT 1";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("compression_method", method);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var compressedData = reader["image_data"] as byte[];
                var fileName = reader["file_name"] as string;

                if (compressedData == null || string.IsNullOrEmpty(fileName))
                    throw new InvalidOperationException("Invalid data in the database.");

                return (compressedData, fileName);
            }

            throw new InvalidOperationException("No data found for the specified compression method.");
        }

        public void UpdateDecompressionTime(string compressionMethod, int decompressionTimeMs, int decompressedFileSize)
        {
            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            const string query = @"
                    UPDATE images 
                    SET decompression_time_ms = @decompression_time_ms, 
                        decompressed_file_size = @decompressed_file_size
                    WHERE id = (
                        SELECT id 
                        FROM images
                        WHERE compression_method = @compression_method
                        ORDER BY id DESC
                        LIMIT 1
                    )";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("decompression_time_ms", decompressionTimeMs);
            cmd.Parameters.AddWithValue("decompressed_file_size", decompressedFileSize);
            cmd.Parameters.AddWithValue("compression_method", compressionMethod);

            cmd.ExecuteNonQuery();
        }

        public bool ImageExistsInDatabase(string fileName, string compressionMethod)
        {
            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            const string query = @"
                    SELECT COUNT(*) 
                    FROM images 
                    WHERE file_name = @file_name AND compression_method = @compression_method";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("file_name", fileName);
            cmd.Parameters.AddWithValue("compression_method", compressionMethod);

            var count = (long)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
