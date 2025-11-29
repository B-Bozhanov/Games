namespace SnakeGame.Services
{
    using System;
    using System.IO;

    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    public static class AiLogger
    {
        // Лог файлът ще е до .exe-то: bin/Debug/.../ai-log.txt
        private static readonly string LogFilePath =
            Path.Combine(AppContext.BaseDirectory, "ai-log.txt");

        private static bool headerWritten;

        private static void EnsureHeader()
        {
            if (headerWritten)
            {
                return;
            }

            var header = "T;Head;Food;Len;Branch;Dir;Next;Note";
            File.WriteAllText(LogFilePath, header + Environment.NewLine);
            headerWritten = true;
        }

        public static void LogDecision(
            int tick,
            Coordinates head,
            Coordinates food,
            int length,
            string branch,
            Direction direction,
            Coordinates nextHead,
            string note = "")
        {
            EnsureHeader();

            var line =
                $"{tick}; " +
                $"{head.Row},{head.Col}; " +
                $"{food.Row},{food.Col}; " +
                $"{length};" +
                $"{branch};" +
                $"{direction};" +
                $"{nextHead.Row},{nextHead.Col};" +
                $"{note}";

            File.AppendAllText(LogFilePath, line + Environment.NewLine);
        }

        public static void LogDeath(
            int tick,
            Coordinates head,
            Coordinates nextHead,
            string reason)
        {
            EnsureHeader();

            var line =
                $"{tick};" +
                $"{head.Row},{head.Col};" +
                "-;" +          // Food няма значение тук
                "-;" +          // Len няма значение тук
                "DEATH;" +
                "-;" +
                $"{nextHead.Row},{nextHead.Col};" +
                $"{reason}";

            File.AppendAllText(LogFilePath, line + Environment.NewLine);
        }
    }
}