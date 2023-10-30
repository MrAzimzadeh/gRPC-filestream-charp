

using Google.Protobuf;
using Grpc.Net.Client;
using grpcFileTransportServer;

Console.BackgroundColor = ConsoleColor.Magenta;
Console.WriteLine("====== Upload ======");
Console.BackgroundColor = ConsoleColor.Black;


var chanel = GrpcChannel.ForAddress("http://localhost:5194");
var client = new FileService.FileServiceClient(chanel);

string file = @"C:\Users\azimz\Videos\WhatsApp Video 2023-10-30 at 12.27.21_97bc64c7.mp4";
using FileStream fileStream = new FileStream(file, FileMode.Open);
var content = new BytesContent
{

    FileSize = fileStream.Length,
    ReadedByte = 0,
    Info = new grpcFileTransportServer.FileInfo
    {
        FileName = Path.GetFileNameWithoutExtension($"{Guid.NewGuid() + fileStream.Name}"),
        FileExtension = Path.GetExtension(fileStream.Name)

    },
};
var upload = client.FileUplad();
byte[] buffer = new byte[2048];
while ((content.ReadedByte = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
{
    content.Buffer = ByteString.CopyFrom(buffer);
    await upload.RequestStream.WriteAsync(content);
}

await upload.RequestStream.CompleteAsync();
fileStream.Close();
 