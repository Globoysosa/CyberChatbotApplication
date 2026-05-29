using System;

namespace CyberChatbotApplication
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var app = new System.Windows.Application();
            var mainWindow = new MainWindow();
            app.Run(mainWindow);
        }
    }
}