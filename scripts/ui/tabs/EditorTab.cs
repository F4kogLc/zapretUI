using ImGuiNET;
using System.Numerics;

internal class EditorTab : ITab
{
    readonly ConfigManager configManager;

    int selectedFeatureIndex = -1;
    string newFeatureName = "";
    string newFeatureArgs = "";
    string newFeatureTooltip = "";

    public EditorTab(ConfigManager configManager)
    {
        this.configManager = configManager;
    }

    public void Render()
    {
        if (!ImGui.BeginTabItem("Editor")) return;

        RenderFeatureSelector();
        RenderEditors();
        RenderActionButtons();
        RenderValidationMessages();

        ImGui.EndTabItem();
    }

    void RenderFeatureSelector()
    {
        var features = configManager.Config.Features;
        var featureNames = features.Select(f => f.Name).ToArray();

        ImGui.Text("Feature");
        if (ImGui.Combo("##FeaturesList", ref selectedFeatureIndex, featureNames, featureNames.Length))
        {
            LoadSelectedFeature();
        }
    }

    void LoadSelectedFeature()
    {
        if (selectedFeatureIndex >= 0 &&
            selectedFeatureIndex < configManager.Config.Features.Count)
        {
            var feature = configManager.Config.Features[selectedFeatureIndex];
            newFeatureName = feature.Name;
            newFeatureArgs = string.Join("\n", feature.Arguments);
            newFeatureTooltip = feature.Tooltip;
        }
    }

    void RenderEditors()
    {
        ImGui.Text("Name");
        ImGui.InputText("##NameInput", ref newFeatureName, 256);

        ImGui.Text("Arguments");
        ImGui.InputTextMultiline("##ArgumentsInput", ref newFeatureArgs, 65536, new Vector2(-1, 250));

        ImGui.Text("Tooltip");
        ImGui.InputTextMultiline("##TooltipInput", ref newFeatureTooltip, 2048, new Vector2(-1, ImGui.GetTextLineHeight() * 3));
    }

    void RenderActionButtons()
    {
        ImGui.Spacing();

        if (ImGui.Button("Add New"))
        {
            CreateNewFeature();
        }

        ImGui.SameLine();

        if (ImGui.Button("Save Changes") && selectedFeatureIndex != -1)
        {
            UpdateExistingFeature();
        }

        ImGui.SameLine();

        if (ImGui.Button("Delete") && selectedFeatureIndex != -1)
        {
            DeleteSelectedFeature();
        }

        ImGui.SameLine();

        if (ImGui.Button("Clear"))
        {
            ClearEditorFields();
        }
    }

    void CreateNewFeature()
    {
        if (!string.IsNullOrWhiteSpace(newFeatureName) && SplitArguments(newFeatureArgs).Length != 0)
        {
            AddFeature(new Feature
            {
                Name = newFeatureName,
                Arguments = SplitArguments(newFeatureArgs),
                Tooltip = newFeatureTooltip,
                IsEnabled = true
            });
            ClearEditorFields();
        }
    }

    void UpdateExistingFeature()
    {
        var feature = configManager.Config.Features[selectedFeatureIndex];
        feature.Name = newFeatureName;
        feature.Arguments = SplitArguments(newFeatureArgs);
        feature.Tooltip = newFeatureTooltip;
        configManager.Save();
    }

    void DeleteSelectedFeature()
    {
        DeleteFeature(selectedFeatureIndex);
        ClearEditorFields();
        selectedFeatureIndex = -1;
    }

    void ClearEditorFields()
    {
        newFeatureName = "";
        newFeatureArgs = "";
        newFeatureTooltip = "";
    }

    void RenderValidationMessages()
    {
        if (string.IsNullOrWhiteSpace(newFeatureName))
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), "Name is required");
        }

        if (string.IsNullOrWhiteSpace(newFeatureArgs))
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), "At least one argument is required");
        }
    }

    void AddFeature(Feature feature)
    {
        configManager.Config.Features.Add(feature);
        configManager.Save();
    }

    void DeleteFeature(int index)
    {
        if (index >= 0 && index < configManager.Config.Features.Count)
        {
            configManager.Config.Features.RemoveAt(index);
            configManager.Save();
        }
    }

    string[] SplitArguments(string input)
    {
        return input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                   .Select(arg => arg.Trim())
                   .Where(arg => !string.IsNullOrEmpty(arg))
                   .ToArray();
    }
}