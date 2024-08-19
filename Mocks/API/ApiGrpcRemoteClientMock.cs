// Module name: Mocks
// File name: ApiGrpcRemoteClientMock.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Grpc.Core;
using RemoteConnector.Proto;

namespace Mocks.API;

public sealed class ApiGrpcRemoteClientMock : RemoteService.RemoteServiceClient
{
    public Func<CalibrationRequest, CallOptions, AsyncServerStreamingCall<CalibrationResponse>> OnPerformCalibration
    {
        get;
        set;
    } = (_, _) => throw new NotImplementedException();

    public Func<GazeDataRequest, CallOptions, AsyncServerStreamingCall<GazeData>> OnOpenGazeStream { get; set; } =
        (_, _) => throw new NotImplementedException();

    public Func<ObserveEyeTrackerAvailabilityRequest, CallOptions, AsyncServerStreamingCall<EyeTrackerAvailability>>
        OnObserveEyeTrackerAvailability { get; set; } = (_, _) => throw new NotImplementedException();

    public override AsyncServerStreamingCall<CalibrationResponse> PerformCalibration(CalibrationRequest request,
        Metadata? headers = null,
        DateTime? deadline = null, CancellationToken cancellationToken = default)
    {
        return OnPerformCalibration(request, new CallOptions(headers, deadline, cancellationToken));
    }

    public override AsyncServerStreamingCall<CalibrationResponse> PerformCalibration(CalibrationRequest request,
        CallOptions options)
    {
        return OnPerformCalibration(request, options);
    }

    public override AsyncServerStreamingCall<GazeData> OpenGazeStream(GazeDataRequest request, Metadata? headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = default)
    {
        return OnOpenGazeStream(request, new CallOptions(headers, deadline, cancellationToken));
    }

    public override AsyncServerStreamingCall<GazeData> OpenGazeStream(GazeDataRequest request, CallOptions options)
    {
        return OnOpenGazeStream(request, options);
    }

    public override AsyncServerStreamingCall<EyeTrackerAvailability> ObserveEyeTrackerAvailability(
        ObserveEyeTrackerAvailabilityRequest request,
        Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = default)
    {
        return OnObserveEyeTrackerAvailability(request, new CallOptions(headers, deadline, cancellationToken));
    }

    public override AsyncServerStreamingCall<EyeTrackerAvailability> ObserveEyeTrackerAvailability(
        ObserveEyeTrackerAvailabilityRequest request,
        CallOptions options)
    {
        return OnObserveEyeTrackerAvailability(request, options);
    }
}