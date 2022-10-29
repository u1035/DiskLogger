[![Build Status](https://img.shields.io/github/workflow/status/u1035/DiskLogger/.NET)](https://img.shields.io/github/workflow/status/u1035/DiskLogger/.NET)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/DiskLogger)](https://www.nuget.org/packages/DiskLogger)
[![Nuget](https://img.shields.io/nuget/dt/DiskLogger)](https://www.nuget.org/packages/DiskLogger)
![GitHub](https://img.shields.io/github/license/u1035/DiskLogger)

# DiskLogger
Lightweight application log library that writes debug files to disk.
Targets .NET Standard 2.0 and can be used with different kinds of projects.
Produces new file for each day, can use file prefix, writes information about log function caller ([CallerMemberName], [CallerFilePath], [CallerLineNumber]), adds timezone info to log record timestamp.

(under slow lazy development)

Inspired by NLog, but I wanted to create simple solution by myself.

## Usage

At first we need to initialize LogManager somewhere at application entry point:
```csharp
LogManager = new LogManager("C:\\Logs", "myApp");
```
This will make logger to create log files like
```
C:\Logs\myApp-2022-10-29.log
C:\Logs\myApp-2022-10-30.log
...
```

Then we need to create logger for class or context we need:
```csharp
var _logger = LogManager.ForContext<WorkerClass>();
//or
var _logger = LogManager.ForContext("SomeContext");
```

In code you can use logger like this:
```csharp
_logger.Trace("Some trace message"); 
_logger.Notice("Some notice message"); 
_logger.Debug("Some debug message"); 
_logger.Info("Some info message"); 
_logger.Warning("Some warning message"); 
_logger.Error($"Some error message: {exception.Message}"); 
_logger.Fatal("Some fatal error message"); 
```
and get the following line in your log file:

> 2022-10-29T22:41:22.5116388+03:00|WorkerClass|PrintInfo|MainViewModel.cs|17|Debug|Message text

Where you will find time of event, context, name of function, source file name and line, severity of the message, and your message.
