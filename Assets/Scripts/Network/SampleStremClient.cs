using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Grpc.Core;
using Grpc.Net.Client;

using Sample.Grpc;

public class SampleStreamClient
{
    static readonly string Address = "https://localhost:8080";

    public delegate void ResponseReceivedHandler(SampleResponse response);
    public event ResponseReceivedHandler OnResponseReceived;
    public event Action<Exception> OnError;

    CancellationTokenSource cancellationTokenSource = null;

    public void StartStream()
    {
        if (cancellationTokenSource != null)
        {
            Debug.LogWarning("Stream is already running.");
            return;
        }
        cancellationTokenSource = new CancellationTokenSource();
        ReceiveStreamAsync(cancellationTokenSource.Token).ConfigureAwait(false);
    }

    async Task ReceiveStreamAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var httpHandler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            using var httpClient = new HttpClient(httpHandler);

            using GrpcChannel channel = GrpcChannel.ForAddress(Address, new GrpcChannelOptions() { HttpHandler = httpHandler });
            var gRPCClient = new SampleService.SampleServiceClient(channel);

            var sampleRequest = new SampleRequest();
            AsyncServerStreamingCall<SampleResponse> request = gRPCClient.Sample(sampleRequest);

            while (await request.ResponseStream.MoveNext(cancellationToken))
            {
                var response = request.ResponseStream.Current;

                await Task.Run(() => OnResponseReceived?.Invoke(response), cancellationToken);
            }
        }
        catch (RpcException rpcEx)
        {
            Debug.LogError($"gRPC error: {rpcEx.Status.Detail}");
            OnError?.Invoke(rpcEx);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Stream error: {ex.Message}");
            OnError?.Invoke(ex);
        }
        finally
        {
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }

    public void StopStream()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
