// Module name: Mocks
// File name: ApiGrpcRemoteClientMock.cs
// Last edit: 2024-3-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc. - All rights reserved.
// 
// All information contained herein is, and remains the property of
// Inseye Inc. The intellectual and technical concepts contained herein are
// proprietary to Inseye Inc. and may be covered by U.S. and Foreign Patents, patents
// in process, and are protected by trade secret or copyright law. Dissemination
// of this information or reproduction of this material is strictly forbidden
// unless prior written permission is obtained from Inseye Inc. Access to the source
// code contained herein is hereby forbidden to anyone except current Inseye Inc.
// employees, managers or contractors who have executed Confidentiality and
// Non-disclosure agreements explicitly covering such access.

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
        Metadata headers = null,
        DateTime? deadline = null, CancellationToken cancellationToken = default)
    {
        return OnPerformCalibration(request, new CallOptions(headers, deadline, cancellationToken));
    }

    public override AsyncServerStreamingCall<CalibrationResponse> PerformCalibration(CalibrationRequest request,
        CallOptions options)
    {
        return OnPerformCalibration(request, options);
    }

    public override AsyncServerStreamingCall<GazeData> OpenGazeStream(GazeDataRequest request, Metadata headers = null,
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
        Metadata headers = null, DateTime? deadline = null,
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