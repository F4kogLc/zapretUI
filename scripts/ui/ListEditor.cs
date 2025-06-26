using ImGuiNET;
using System.Numerics;

internal class ListEditor : IElement
{
    const float minHeight = 50f;
    const float maxHeight = 600f;

    string[] availableFiles = [];
    int selectedFileIndex = -1;
    string currentFilePath = "";
    string fileContent = "";

    public ListEditor()
    {
        LoadAvailableFiles();
    }

    public void Render()
    {
        RenderFileEditor();
    }

    void RenderFileEditor()
    {
        if (availableFiles.Length == 0)
        {
            ImGui.TextColored(new Vector4(1, 1, 0, 1), "No .txt files found in zapret folder");
        }
        else
        {
            if (ImGui.Combo("##FileSelector", ref selectedFileIndex, availableFiles, availableFiles.Length))
                LoadSelectedFile();
        }


        ImGui.SameLine();

        if (ImGui.Button("Save"))
            SaveFileContent();

        ImGui.SameLine();

        if (ImGui.Button("Load"))
            LoadFileContent();

        ImGui.Text($"Lines: {fileContent.Split('\n').Length} | Characters: {fileContent.Length}");

        if (ImGui.BeginChild("##FileEditorContainer", new Vector2(-1, -1), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar))
        {
            ImGui.InputTextMultiline("##FileContent", ref fileContent, 1000000, new Vector2(-1, -1));
        }
        ImGui.EndChild();
    }

    void LoadAvailableFiles()
    {
        try
        {
            if (Directory.Exists(Consts.LISTS_PATH))
            {
                var files = Directory.GetFiles(Consts.LISTS_PATH, "*.txt")
                    .Select(Path.GetFileName)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .OrderBy(name => name)
                    .ToArray();

                availableFiles = files;
            }
            else
            {
                Console.WriteLine($"Zapret folder not found: {Consts.LISTS_PATH}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading files from zapret folder: {ex.Message}");
        }
    }

    void LoadSelectedFile()
    {
        if (selectedFileIndex >= 0 && selectedFileIndex < availableFiles.Length)
        {
            currentFilePath = Path.Combine(Consts.LISTS_PATH, availableFiles[selectedFileIndex]);
            LoadFileContent();
        }
    }

    void LoadFileContent()
    {
        try
        {
            if (!string.IsNullOrEmpty(currentFilePath) && File.Exists(currentFilePath))
            {
                fileContent = File.ReadAllText(currentFilePath);
            }
            else
            {
                Console.WriteLine($"File not found: {currentFilePath}");
                fileContent = "";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading file: {ex.Message}");
            fileContent = "";
        }
    }

    void SaveFileContent()
    {
        try
        {
            File.WriteAllText(currentFilePath, fileContent);
            Console.WriteLine($"File saved: {currentFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }
    }
}
