using Hubtel.PosProxy.Models;
using Hubtel.PosProxyData.EntityModels;
using Hubtel.PosProxyData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Services
{
    public class SalesOrderZipFileService: ISalesOrderZipFileService
    {
        private readonly ISalesOrderZipFileRepository _salesOrderZipFileRepository;
        private readonly ILogger _logger;

        public SalesOrderZipFileService(ISalesOrderZipFileRepository salesOrderZipFileRepository, ILogger<SalesOrderZipFileService> logger)
        {
            _salesOrderZipFileRepository = salesOrderZipFileRepository;
            _logger = logger;
        }

        public SalesOrderList Extract(string fileName, string outFile)
        {
            SalesOrderList salesOrderList = null;

            JsonSerializer serializer = new JsonSerializer();
            
            //FileStream outStream = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            using (MemoryStream outStream = new MemoryStream())
            using (FileStream inStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                // Read the decoder properties
                byte[] properties = new byte[5];
                if (inStream.Read(properties, 0, 5) != 5)
                    throw (new Exception("input .lzma is too short"));
                PosProxyZip.Compression.LZMA.Decoder decoder = new PosProxyZip.Compression.LZMA.Decoder();
                decoder.SetDecoderProperties(properties);
                // Read in the decompress file size.
                long outSize = 0;
                for (int i = 0; i < 8; i++)
                {
                    int v = inStream.ReadByte();
                    if (v < 0)
                        throw (new Exception("Can't Read 1"));
                    outSize |= ((long)(byte)v) << (8 * i);
                }
                long compressedSize = inStream.Length - inStream.Position;

                //decoder.Code(inStream, outStream, outSize, compressedSize, null);
                decoder.Code(inStream, outStream, -1, -1, null);
                //inStream.CopyTo(outStream);

                using (StreamReader jsonData = new StreamReader(outStream, Encoding.ASCII))
                {
                    salesOrderList = (SalesOrderList)serializer.Deserialize(jsonData, typeof(SalesOrderList));
                }
            }
            //outStream.Close();
            return salesOrderList;   
        }

        public void SaveOrders()
        {

        }

        private static void CompressFileLZMA(string inFile, string outFile)
        {
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            // Write the encoder properties
            coder.WriteCoderProperties(output);

            // Write the decompressed file size.
            output.Write(BitConverter.GetBytes(input.Length), 0, 8);

            // Encode the file.
            coder.Code(input, output, input.Length, -1, null);
            output.Flush();
            output.Close();
        }

        private static void DecompressFileLZMA(string inFile, string outFile)
        {
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            // Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
            output.Flush();
            output.Close();
        }

        public async Task<SalesOrderZipFile> SaveFileAsync(IFormFile file, string path, string mimeType)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    var fileName = Path.GetFileName(path);
                    var salesOrderZipFile = new SalesOrderZipFile
                    {
                        MimeType = mimeType,
                        Bucketname = $"{fileName}",
                        Filename = path,
                        Processed = false
                    };
                    return await _salesOrderZipFileRepository.AddAsync(salesOrderZipFile);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return null;
        }
    }

    public interface ISalesOrderZipFileService
    {
        Task<SalesOrderZipFile> SaveFileAsync(IFormFile file, string path, string mimeType);
        SalesOrderList Extract(string fileName, string outFile);
    }
}
