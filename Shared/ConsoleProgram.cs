// Module name: Shared.DependencyInjection
// File name: ConsoleProgram.cs
// Last edit: 2024-2-13 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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


namespace EyeTrackerStreaming.Shared;

/// <summary>
/// Helper class that runs console main body in safe context 
/// </summary>
public class ConsoleProgram
{
    public static async Task Run(Func<CancellationToken, Task> main, CancellationToken token)
    {
        try
        {
            await main(token);
        }
        catch (OperationCanceledException operationCanceledException)
        {
           if (operationCanceledException.CancellationToken == token)
               return;
           HandleException(operationCanceledException);
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
    }

    public static async Task Run(Task task)
    {
        try
        {
            await task;
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
    }

    public static void Run(Action main)
    {
        try
        {
            main();
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
    }

    private static void HandleException(Exception exception)
    {
        Console.Clear();
        var stringifiedException = exception.ToString();
        var split = stringifiedException
            .Split(Environment.NewLine)
            .Select(s => s.Chunk(Console.LargestWindowWidth))
            .SelectMany(s => s)
            .Select(s => new string(s))
            .ToArray();
        var width = split.Max(s => s.Length);
        var height = split.Length + 3;
        if (height > Console.LargestWindowHeight)
        {
            Console.SetWindowSize(width, Console.LargestWindowHeight);
            var currentLineInWindow = 0;
            foreach (var line in split)
            {
                Console.Write(line);
                Console.Write('\n');
                if (++currentLineInWindow != Console.LargestWindowHeight - 1) continue;
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                Console.Clear();
                currentLineInWindow = 0;
            }
            
        }
        else
            Console.SetWindowSize(width, height);
        
        Console.Write(stringifiedException);
        Console.WriteLine("\nPress enter to close the window...");
        Console.ReadLine();
    }
}