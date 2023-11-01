using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Xml;

public class Voobrajenie
{
    public string Name { get; set; }
    public int Znachenie1 { get; set; }
    public int Znachenie2 { get; set; }
}

public class TextEditor
{
    private string filePath;
    private List<Voobrajenie> models;

    public TextEditor(string filePath)
    {
        this.filePath = filePath;
        this.models = ReadFile();
    }

    public void Start()
    {
        Console.WriteLine("Загруженные данные:");
        PrintModels();

        while (true)
        {
            Console.WriteLine("Нажмите F1 для сохранения, Escape для выхода, или любую другую клавишу для редактирования текста.");

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.F1)
            {
                SaveFileWithAutoFormat();
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                break;
            }
            else
            {
                EditText();
            }
        }
    }

    private List<Voobrajenie> ReadFile()
    {
        List<Voobrajenie> loadedModels = new List<Voobrajenie>();

        if (File.Exists(filePath))
        {
            string format = Path.GetExtension(filePath).ToLower();

            if (format == ".json")
            {
                loadedModels = ReadFileFromJson(filePath);
            }
            else if (format == ".xml")
            {
                loadedModels = ReadFileFromXml(filePath);
            }
            else if (format == ".txt")
            {
                loadedModels = ReadFileFromTxt(filePath);
            }
            else
            {
                Console.WriteLine("Неподдерживаемый формат файла. Создается новый.");
                loadedModels.Add(new Voobrajenie());
            }
        }
        else
        {
            Console.WriteLine("Файл не найден. Создается новый.");
            loadedModels.Add(new Voobrajenie());
        }

        return loadedModels;
    }

    private List<Voobrajenie> ReadFileFromJson(string filePath)
    {
        List<Voobrajenie> loadedModels = new List<Voobrajenie>();
        string json = File.ReadAllText(filePath);
        loadedModels = JsonConvert.DeserializeObject<List<Voobrajenie>>(json);
        return loadedModels;
    }

    private List<Voobrajenie> ReadFileFromXml(string filePath)
    {
        List<Voobrajenie> loadedModels = new List<Voobrajenie>();
        var xmlDocument = new XmlDocument();
        xmlDocument.Load(filePath);
        var modelNodes = xmlDocument.SelectNodes("/Models/Model");

        foreach (XmlNode modelNode in modelNodes)
        {
            string name = modelNode.SelectSingleNode("Name").InnerText;
            int znachenie1 = int.Parse(modelNode.SelectSingleNode("Znachenie1").InnerText);
            int znachenie2 = int.Parse(modelNode.SelectSingleNode("Znachenie2").InnerText);
            loadedModels.Add(new Voobrajenie { Name = name, Znachenie1 = znachenie1, Znachenie2 = znachenie2 });
        }

        return loadedModels;
    }

    private List<Voobrajenie> ReadFileFromTxt(string filePath)
    {
        List<Voobrajenie> loadedModels = new List<Voobrajenie>();

        string[] fileLines = File.ReadAllLines(filePath);
        for (int i = 0; i < fileLines.Length; i += 3)
        {
            if (i + 2 < fileLines.Length)
            {
                string name = fileLines[i];
                if (int.TryParse(fileLines[i + 1], out int znachenie1) && int.TryParse(fileLines[i + 2], out int znachenie2))
                {
                    loadedModels.Add(new Voobrajenie { Name = name, Znachenie1 = znachenie1, Znachenie2 = znachenie2 });
                }
            }
        }

        return loadedModels;
    }

    private void SaveFileWithAutoFormat()
    {
        Console.Clear();
        Console.WriteLine("Введите путь для сохранения файла:");
        string savePath = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(savePath))
        {
            string format = Path.GetExtension(savePath).ToLower();

            if (format == ".json" || format == ".xml" || format == ".txt")
            {
                SaveFile(savePath, models, format.Replace(".", ""));
                Console.WriteLine($"Файл сохранен в формате {format.Replace(".", "").ToUpper()}.");
            }
            else
            {
                Console.WriteLine("Неподдерживаемый формат файла. Файл не будет сохранен.");
            }
        }
        else
        {
            Console.WriteLine("Путь для сохранения файла не указан.");
        }
    }

    private void SaveFile(string filePath, List<Voobrajenie> data, string format)
    {
        switch (format)
        {
            case "json":
                SaveFileAsJson(filePath, data);
                break;
            case "xml":
                SaveFileAsXml(filePath, data);
                break;
            case "txt":
                SaveFileAsTxt(filePath, data);
                break;
            default:
                Console.WriteLine("Неподдерживаемый формат файла. Файл не будет сохранен.");
                break;
        }
    }

    private void EditText()
    {
        Console.Clear();
        Console.WriteLine("Редактирование:");
        PrintModels();

        Console.WriteLine("Введите номер модели для редактирования или нажмите Escape для выхода из редактирования.");

        ConsoleKeyInfo keyInfo = Console.ReadKey();

        if (keyInfo.Key == ConsoleKey.Escape)
        {
            return;
        }

        if (int.TryParse(keyInfo.KeyChar.ToString(), out int modelNumber) && modelNumber >= 1 && modelNumber <= models.Count)
        {
            Console.WriteLine("Введите новое название:");
            string newName = Console.ReadLine();
            Console.WriteLine("Введите новое значение 1:");
            if (int.TryParse(Console.ReadLine(), out int newZnachenie1))
            {
                Console.WriteLine("Введите новое значение 2:");
                if (int.TryParse(Console.ReadLine(), out int newZnachenie2))
                {
                    models[modelNumber - 1].Name = newName;
                    models[modelNumber - 1].Znachenie1 = newZnachenie1;
                    models[modelNumber - 1].Znachenie2 = newZnachenie2;
                }
                else
                {
                    Console.WriteLine("Неверный ввод для значения 2.");
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод для значения 1.");
            }
        }
        else
        {
            Console.WriteLine("Неверный ввод. Пожалуйста, введите номер корректной модели.");
        }
    }

    private void SaveFileAsJson(string filePath, List<Voobrajenie> data)
    {
        string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    private void SaveFileAsXml(string filePath, List<Voobrajenie> data)
    {
        var xmlDocument = new XmlDocument();
        var root = xmlDocument.CreateElement("Models");
        xmlDocument.AppendChild(root);

        foreach (var model in data)
        {
            var modelElement = xmlDocument.CreateElement("Model");
            root.AppendChild(modelElement);

            var name = xmlDocument.CreateElement("Name");
            name.InnerText = model.Name;
            modelElement.AppendChild(name);

            var znachenie1 = xmlDocument.CreateElement("Znachenie1");
            znachenie1.InnerText = model.Znachenie1.ToString();
            modelElement.AppendChild(znachenie1);

            var znachenie2 = xmlDocument.CreateElement("Znachenie2");
            znachenie2.InnerText = model.Znachenie2.ToString();
            modelElement.AppendChild(znachenie2);
        }

        xmlDocument.Save(filePath);
    }

    private void SaveFileAsTxt(string filePath, List<Voobrajenie> data)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (Voobrajenie model in data)
            {
                writer.WriteLine(model.Name);
                writer.WriteLine(model.Znachenie1);
                writer.WriteLine(model.Znachenie2);
            }
        }
    }

    private void PrintModels()
    {
        for (int i = 0; i < models.Count; i++)
        {
            Console.WriteLine($"Модель {i + 1}:");
            Console.WriteLine("Название: " + models[i].Name);
            Console.WriteLine("Значение 1: " + models[i].Znachenie1);
            Console.WriteLine("Значение 2: " + models[i].Znachenie2);
            Console.WriteLine();
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Введите путь к файлу");
        string filePath = Console.ReadLine();

        TextEditor editor = new TextEditor(filePath);
        editor.Start();
    }
}
