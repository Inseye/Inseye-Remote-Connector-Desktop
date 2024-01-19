# About

This project contains source code for Windows application (service) used to obtain gaze data from [Android service](https://github.com/Inseye/Inseye-Android-Service) and provide the data to any desktop application that needs it.

# Dependencies

This project requires .NET 8.0 SDK.

# Project structure

Solution is split into multiple `.csproj` projects.

+ `API` - library with code responsible for communication between desktop service and Android service
+ `API.DependencyInjection` - library with helper classes that register services implemented in `API` in [SimpleInjector](https://www.nuget.org/packages/SimpleInjector) container
+ `ClientCommunication` - library with code responsible for communication between desktop service and other client application on desktop
+ `ClientCommunication.DependencyInjection` - library with helper classes that register services implemented in `ClientCommunication` in [SimpleInjector](https://www.nuget.org/packages/SimpleInjector) container
+ `ClientCommunicationTester` - program used to manual testing of communication between desktop service and desktop client
+ `EyeTrackerStreamingConsole` - program that implements desktop service with UI from `TerminalGUI`
+ `Mocks` - library with various mock implementation of service interfaces from `Shared`
+ `Mocks.DependencyInjection` - library with helper classes that register services implemented in `Mocks` in [SimpleInjector](https://www.nuget.org/packages/SimpleInjector) container
+ `ServiceTester` - console program used to test connection between service and desktop
+ `Shared` - library with interface definition and shared utility classes used across most other projects
+ `Shared.DependencyInjection` - library that helps integrating with dependency injection framework [SimpleInjector](https://www.nuget.org/packages/SimpleInjector)
+ `TerminalGUI` - library with UI views made with [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui/) library
+ `TerminalGUI.DependencyInjection` - library with helper classes that register services implemented in `TerminalGUI` in [SimpleInjector](https://www.nuget.org/packages/SimpleInjector) container
+ `TerminalGUI.Mock` - program that implements desktop service with UI from `TerminalGUI` and services from `Mock`
+ `ViewModels` - library with view models responsible for application behaviour, deeply based on [reactive programming](https://reactivex.io/) paradigm 
+ `ViewModels.DependencyInjection` - library with helper classes that register services implemented in `ViewModels` in [SimpleInjector](https://www.nuget.org/packages/SimpleInjector) container

## ServiceTester

Service tester logging can be configured in [program entrypoint](./ServiceTester/Program.cs) in logging section.

For now all uncaught exceptions are pushed to the terminating the application and then printed in console, please copy them and send to me (Mateusz Chojnowski). 