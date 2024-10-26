using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Cysharp.Net.Http;
using Grpc.Core;
using Grpc.Net.Client;

using WaterRing;

public class WaterRingStreamClient
{
    static readonly string Address = "https://localhost:8080";

    public delegate void UserJoinedHandler(UserJoinedNotification notification);
    public delegate void GameEventHandler(GameEventNotification notification);
    public event UserJoinedHandler OnUserJoined;
    public event GameEventHandler OnGameEvent;
    public event Action<Exception> OnError;

    private CancellationTokenSource _cancellationTokenSource = null;

    // 部屋の作成
    public async Task<CreateRoomResponse> CreateRoomAsync()
    {
        try
        {
            using var httpHandler = new YetAnotherHttpHandler() { SkipCertificateVerification = true };
            using var httpClient = new HttpClient(httpHandler);
            using GrpcChannel channel = GrpcChannel.ForAddress(Address, new GrpcChannelOptions() { HttpHandler = httpHandler });
            var gRPCClient = new RoomService.RoomServiceClient(channel);

            var createRoomRequest = new CreateRoomRequest();
            CreateRoomResponse response = await gRPCClient.CreateRoomAsync(createRoomRequest);
            return response;
        }
        catch (RpcException rpcEx)
        {
            Debug.LogError($"gRPC error: {rpcEx.Status.Detail}");
            OnError?.Invoke(rpcEx);
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Create room error: {ex.Message}");
            OnError?.Invoke(ex);
            return null;
        }
    }

    // ユーザーが部屋に参加するのを待つ
    public void WaitForUserJoin(string roomId)
    {
        if (_cancellationTokenSource != null)
        {
            Debug.LogWarning("Stream is already running.");
            return;
        }
        _cancellationTokenSource = new CancellationTokenSource();
        WaitForUserJoinAsync(roomId, _cancellationTokenSource.Token).ConfigureAwait(false);
    }

    private async Task WaitForUserJoinAsync(string roomId, CancellationToken cancellationToken)
    {
        try
        {
            using var httpHandler = new YetAnotherHttpHandler() { SkipCertificateVerification = true };
            using var httpClient = new HttpClient(httpHandler);
            using GrpcChannel channel = GrpcChannel.ForAddress(Address, new GrpcChannelOptions() { HttpHandler = httpHandler });
            var gRPCClient = new RoomService.RoomServiceClient(channel);

            var waitForUserJoinRequest = new WaitForUserJoinRequest { RoomId = roomId };
            AsyncServerStreamingCall<UserJoinedNotification> request = gRPCClient.WaitForUserJoin(waitForUserJoinRequest);

            while (await request.ResponseStream.MoveNext(cancellationToken))
            {
                var notification = request.ResponseStream.Current;
                OnUserJoined?.Invoke(notification);
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
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    // ゲーム開始イベントの受信
    public void StartGame(string roomId)
    {
        if (_cancellationTokenSource != null)
        {
            Debug.LogWarning("Stream is already running.");
            return;
        }
        _cancellationTokenSource = new CancellationTokenSource();
        StartGameAsync(roomId, _cancellationTokenSource.Token).ConfigureAwait(false);
    }

    private async Task StartGameAsync(string roomId, CancellationToken cancellationToken)
    {
        try
        {
            using var httpHandler = new YetAnotherHttpHandler() { SkipCertificateVerification = true };
            using var httpClient = new HttpClient(httpHandler);
            using GrpcChannel channel = GrpcChannel.ForAddress(Address, new GrpcChannelOptions() { HttpHandler = httpHandler });
            var gRPCClient = new RoomService.RoomServiceClient(channel);

            var startGameRequest = new StartGameRequest { RoomId = roomId };
            AsyncServerStreamingCall<GameEventNotification> request = gRPCClient.StartGame(startGameRequest);

            while (await request.ResponseStream.MoveNext(cancellationToken))
            {
                var notification = request.ResponseStream.Current;
                OnGameEvent?.Invoke(notification);
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
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    // ストリームの停止
    public void StopStream()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
