using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Text;

public class ExamQuestion
{
    public int QuestionNumber { get; set; }
    public string CorrectAnswer { get; set; }
    public string QuestionText { get; set; }
    public Dictionary<string, string> Options { get; set; }
    public string Reference { get; set; }
}

public class ExamParser
{
    public static List<ExamQuestion> ParseExamFile(string filePath)
    {
        // 讀取文件內容
        string examText = File.ReadAllText(filePath, Encoding.UTF8);
        return ParseExamText(examText);
    }

    public static List<ExamQuestion> ParseExamText(string examText)
    {
        var questions = new List<ExamQuestion>();

        // 將文字分割
        var lines = examText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                var question = ParseSingleQuestion(line.Trim());
                if (question != null)
                {
                    questions.Add(question);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解析時發生錯誤: {line}");
                Console.WriteLine($"錯誤訊息: {ex.Message}");
            }
        }

        return questions;
    }

    private static ExamQuestion ParseSingleQuestion(string line)
    {
        var question = new ExamQuestion
        {
            Options = new Dictionary<string, string>()
        };

        // 使用正則表達式解析
        var pattern = @"^\((?<answer>[A-D])\)\s*(?<number>\d+)\.\s*(?<text>[^()]+?)\s*(?<options>\([A-D]\)[^()]+(?:\([A-D]\)[^()]+)*)\s*\(出處：(?<reference>[^()]+)\)$";
        var match = Regex.Match(line.Trim() , pattern);

        if (match.Success)
        {
            // 問題編號
            question.QuestionNumber = int.Parse(match.Groups["number"].Value);

            // 正確答案
            question.CorrectAnswer = match.Groups["answer"].Value;

            // 問題文本
            question.QuestionText = match.Groups["text"].Value.ToString().Trim();

            // 參考出處
            question.Reference = match.Groups["reference"].Value.ToString().Trim();

            // 選項
            var optionsText = match.Groups["options"].Value;
            var optionsPattern = @"\((?<option>[A-D])\)(?<text>[^()]+?)(?=\s*\([A-D]\)|\s*$)";
            var optionsMatches = Regex.Matches(optionsText, optionsPattern);

            foreach (Match optionMatch in optionsMatches)
            {
                var optionLetter = optionMatch.Groups["option"].Value;
                var optionText = optionMatch.Groups["text"].Value.ToString().Trim();
                question.Options[optionLetter] = optionText;
            }

            return question;
        }

        return null;
    }

    public static void SaveToJsonFile(List<ExamQuestion> questions, string outputPath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string json = JsonSerializer.Serialize(questions, options);
        File.WriteAllText(outputPath, json, Encoding.UTF8);
    }
}


public class Program
{
    public static void Main()
    {
        Console.WriteLine("考題轉JSON工具");
        Console.WriteLine("==================");

        string inputFile = GetValidFilePath("請輸入題目文件路徑 (例如: C:\\ExamQuestions\\questions.txt): ", true);
        string outputFile = GetValidFilePath("請輸入要儲存的JSON檔案路徑 (例如: C:\\ExamQuestions\\output.json): ", false);

        try
        {
            Console.WriteLine("\n開始處理...");

            // 解析題目
            var questions = ExamParser.ParseExamFile(inputFile);

            // 輸出統計信息
            Console.WriteLine($"成功解析 {questions.Count} 題");

            // 儲存為JSON檔案
            ExamParser.SaveToJsonFile(questions, outputFile);

            Console.WriteLine($"JSON檔案已儲存至: {outputFile}");
            Console.WriteLine("\n處理完成！按任意鍵結束程式...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n錯誤: {ex.Message}");
            Console.WriteLine("\n程式執行失敗。按任意鍵結束...");
        }

        Console.ReadKey();
    }

    private static string GetValidFilePath(string prompt, bool mustExist)
    {
        string? filePath;
        bool isValid;

        do
        {
            Console.Write(prompt);
            filePath = Console.ReadLine()?.Trim();

            // 檢查是否輸入為空
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("錯誤: 路徑不能為空，請重新輸入。");
                isValid = false;
                continue;
            }

            // 檢查路徑格式
            try
            {
                // 獲取完整路徑
                filePath = Path.GetFullPath(filePath);

                // 如果是輸入文件，檢查文件是否存在
                if (mustExist && !File.Exists(filePath))
                {
                    Console.WriteLine("錯誤: 指定的文件不存在，請重新輸入。");
                    isValid = false;
                    continue;
                }

                // 如果是輸出文件，檢查目錄是否存在
                if (!mustExist)
                {
                    string? directory = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directory))
                    {
                        Console.WriteLine("錯誤: 指定的目錄不存在，請重新輸入。");
                        isValid = false;
                        continue;
                    }
                }

                isValid = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤: {ex.Message}");
                isValid = false;
            }
        } while (!isValid);

        return filePath;
    }
}