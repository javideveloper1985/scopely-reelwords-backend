using System;
using System.Linq;

namespace ReelWords.Services.Implementations;

public class ConsoleUserInterfaceService : IConsoleUserInterfaceService
{
    public void NewLine() => Console.Write(Environment.NewLine);

    public void ShowMessage(string message, ConsoleColor? color = null)
    {
        if (color.HasValue)
            Console.ForegroundColor = color.Value;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void ShowError(string message) => ShowMessage(message, ConsoleColor.Red);

    public void ShowSuccess(string message) => ShowMessage(message, ConsoleColor.Green);

    public string GetInput(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
            ShowMessage(message);

        Console.Write(" -------> ");
        return Console.ReadLine();
    }

    public string AskForInputOption(string message, params string[] options)
    {
        if (options is null || options.Length == 0)
            throw new ArgumentNullException(nameof(options));

        var input = string.Empty;
        bool wrongOption = true;

        while (wrongOption)
        {
            ShowMessage(message);
            foreach (string option in options)
                ShowMessage($" - {option}");

            input = GetInput(string.Empty);
            if (options.Contains(input))
                wrongOption = false;
            else
                ShowError("You must enter a valid option.");
        }

        return input.ToLower();
    }
}