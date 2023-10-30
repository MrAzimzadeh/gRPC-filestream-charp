using System.Net;
using Grpc.Net.Client;
using grpcFileTransportServer;

Console.BackgroundColor = ConsoleColor.DarkYellow;
Console.WriteLine("====== Download ======");
Console.BackgroundColor = ConsoleColor.Black;

var chanel = GrpcChannel.ForAddress("http://localhost:5194");
var client = new FileService.FileServiceClient(chanel);


string downloadPath = @"C:\Users\azimz\OneDrive\Desktop\gRPC_filestream\grpcClientDownload\DownloadFiles";


var fileInfo = new grpcFileTransportServer.FileInfo
{
    FileExtension = ".mp4",
    FileName = "WhatsApp Video 2023-10-30 at 12.27.21_97bc64c7"
};

FileStream fileStream = null;

var reques = client.FileDownload(fileInfo);

CancellationTokenSource cancellationToken = new CancellationTokenSource();
int counter = 0;
decimal chunkSize = 0;

while (await reques.ResponseStream.MoveNext(cancellationToken.Token))
{
    if (counter++ == 0)
    {
        System.Console.WriteLine(counter);
        fileStream = new FileStream($"{downloadPath}/{reques.ResponseStream.Current.Info.FileName}{reques.ResponseStream.Current.Info.FileExtension}", FileMode.CreateNew);
        fileStream.SetLength(reques.ResponseStream.Current.FileSize);
    }
    var buffer = reques.ResponseStream.Current.Buffer.ToByteArray();
    await fileStream.WriteAsync(buffer, 0, reques.ResponseStream.Current.ReadedByte);
    System.Console.WriteLine($"{Math.Round(((chunkSize += reques.ResponseStream.Current.ReadedByte) * 100) / reques.ResponseStream.Current.FileSize)}%");

}
System.Console.WriteLine("Download .... ");
await fileStream.DisposeAsync();
fileStream.Close();
