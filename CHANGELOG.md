# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.0] - 2024-08-19

### Added 

- new user interface in AvaloniaUI

### Removed

- removed previous implementations using Terminal.Gui

## [0.2.0] - 2024-07-03

### Added

- ability to send gaze data and eye openness to VR Chat through OSC protocol

- Github Action script for releasing new versions of application 

## [0.1.0] - 2024-04-30

### Added

- support for [SSCP](https://github.com/Inseye/Inseye-Remote-Connector-Documentation/blob/main/SCCP.md) 0.1.0

## [0.0.1] - 2024-03-04

### Added
- created general architecture and library setup of the project:
   + MVVM project structure
   + introduced dependency injection (DI) with [SimpleInjector](https://simpleinjector.org/) as DI container
   + introduced logging with [SeriLog](https://serilog.net/)
   + introduced [gui.cs](https://github.com/gui-cs) as temporary UI framework 
- service tester application that allows easy gaze data preview
- gRCP api client implementation 
- zeroconf DNS-SD service discovery
- communication with desktop client application via shared memory and named pipes - implemented [SSCP](https://github.com/Inseye/Inseye-Remote-Connector-Documentation/blob/main/SCCP.md) `0.1.0` 
- all logic required to implement api in version `0.0.1`
- [API Interfaces](./API/proto) in version `0.0.1`