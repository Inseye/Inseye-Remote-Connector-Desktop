# About

This project contains source code for Windows application (service) used to obtain gaze data from [Android service](https://github.com/Inseye/Inseye-Android-Service) and provide the data to any desktop application that needs it.

# License

This repository is part of Inseye Software Development Kit.
By using content of this repository you agree to SDK [License](./UnityPackage/LICENSE).

# Dependencies

This project requires [.NET 8.0.x SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

# Project structure

Solution is split into multiple `.csproj` projects.

+ `API` - library with code responsible for communication between desktop service and Android service
+ `API.DependencyInjection` - library with helper classes that register services implemented in `API` in [SimpleInjector](https://www.nuget.org/packages/SimpleInjector) container
+ `ClientCommunication` - library with code responsible for communication between desktop service and other client application on desktop
+ `ClientCommunication.DependencyInjection` - library with helper classes that register services implemented in `ClientCommunication` in [SimpleInjector](https://www.nuget.org/packages/SimpleInjector) container
+ `ClientCommunicationTester` - program used to manual test communication between desktop service and desktop client
+ `EyeTrackerStreamingAvalonia` - program that implements desktop service with UI from `Avalonia`
+ `Mocks` - library with various mock implementation of service interfaces from `Shared`
+ `Mocks.DependencyInjection` - library with helper classes that register services implemented in `Mocks` in [SimpleInjector](https://www.nuget.org/packages/SimpleInjector) container
+ `ServiceTester` - console program used to test connection between service and desktop
+ `Shared` - library with interface definition and shared utility classes used across most other projects
+ `Shared.DependencyInjection` - library that helps integrating with dependency injection framework [SimpleInjector](https://www.nuget.org/packages/SimpleInjector)
+ `ViewModels` - library with view models responsible for application behaviour, deeply based on [reactive programming](https://reactivex.io/) paradigm 
+ `ViewModels.DependencyInjection` - library with helper classes that register services implemented in `ViewModels` in [SimpleInjector](https://www.nuget.org/packages/SimpleInjector) container
+ `VrChatConnector` - library that implements OSC protocol used to stream gaze data to VrChat
+ `VrChatConnector.DependencyInjection` - library with helper classes that register services implemented in `VrChatConnector` in [SimpleInjector](https://www.nuget.org/packages/SimpleInjector) container

## Building project

Update all submodule with `git submodule update --init -r .`.

Run main application with `dotnet run --project ./EyeTrackerStreamingAvalonia`.


## ServiceTester

Service tester logging can be configured in [program entrypoint](./ServiceTester/Program.cs) in logging section.

For now all uncaught exceptions are pushed to the terminating the application and then printed in console, please copy them and send to me (Mateusz Chojnowski). 