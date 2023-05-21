using System;

namespace ReelWords.Services;

public interface IConsoleUserInterfaceService 
{
    void ShowTitle(string message);
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowMessage(string message, ConsoleColor? color = null);
    string AskForInputOption(string message, params string[] options);
    string GetInput(string message);
    void NewLine();
}
