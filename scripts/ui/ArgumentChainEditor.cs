using ImGuiNET;
using System.Numerics;

internal class ArgumentChainEditor : IElement
{
    readonly ConfigManager configManager;

    int selectedAvailableArgIndex = -1;
    bool showArgs = false;
    string searchFilter = "";
    float inputHeight = 250f;
    const float minHeight = 50f;
    const float maxHeight = 500f;

    public ArgumentChainEditor(ConfigManager configManager)
    {
        this.configManager = configManager;
    }

    public void Render()
    {
        ImGui.Text("Argument Chain Builder");

        var availableArgs = configManager.Config.AvailableArguments.ToArray();

        if (ImGui.Button("Show Arguments"))
            showArgs = !showArgs;

        if (showArgs)
        {
            ImGui.Begin("Arguments", ImGuiWindowFlags.NoTitleBar);

            ImGui.SetNextItemWidth(-1);
            ImGui.InputTextWithHint("##ArgumentSearch", "Search...", ref searchFilter, 100);

            for (int i = 0; i < availableArgs.Length; i++)
            {
                if (!string.IsNullOrEmpty(searchFilter) && !availableArgs[i].Contains(searchFilter, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (ImGui.Selectable(availableArgs[i]))
                {
                    selectedAvailableArgIndex = i;
                    AddToChain();
                }
            }
            ImGui.End();
        }

        ImGui.SameLine();

        if (ImGui.Button("Save "))
        {
            MakeNewFeature();
        }
        ImGuiUtils.Tooltip("Сохранить как новый обход");

        if (ImGui.BeginChild("##ChainListContainer", new Vector2(-1, inputHeight),
            ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar))
        {
            ImGui.BeginChild("ChainList", new Vector2(0, inputHeight));

            // доступная ширина
            float currentX = 0;
            float currentY = 0;
            var availableWidth = ImGui.GetContentRegionAvail().X;
            var itemSpacing = ImGui.GetStyle().ItemSpacing.X;
            var lineHeight = ImGui.GetTextLineHeightWithSpacing();

            ImGui.BeginGroup();

            for (int i = 0; i < configManager.Config.SelectedArgumentsChain.Count; i++)
            {
                var arg = configManager.Config.SelectedArgumentsChain[i];

                // размер текста
                var textSize = ImGui.CalcTextSize(arg);
                var buttonSize = new Vector2(20, textSize.Y);
                var totalWidth = textSize.X + buttonSize.X + itemSpacing * 2;

                // проверка на необходимость переноса
                if (currentX + totalWidth > availableWidth && currentX > 0)
                {
                    currentX = 0;
                    currentY += lineHeight;
                }

                ImGui.SetCursorPos(new Vector2(currentX, currentY));

                ImGui.Text(arg);

                // позиционирование кнопки удаления
                ImGui.SetCursorPos(new Vector2(
                    currentX + textSize.X + itemSpacing / 2,
                    currentY + (textSize.Y - buttonSize.Y) / 2
                ));

                ImGui.PushID($"remove_{i}");
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.1f, 0.1f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(1.0f, 0.2f, 0.2f, 1.0f));
                if (ImGui.Button("X", buttonSize))
                {
                    configManager.Config.SelectedArgumentsChain.RemoveAt(i);
                    i--;
                }
                ImGui.PopStyleColor(2);
                ImGui.PopID();

                ImGui.SameLine();
                currentX += totalWidth + itemSpacing;
            }
            ImGui.EndGroup();

            ImGui.EndChild();
        }

        ImGui.EndChild();

        ImGui.InvisibleButton("##ResizeChainList", new Vector2(-1, 8));

        if (ImGui.IsItemActive())
        {
            inputHeight += ImGui.GetIO().MouseDelta.Y;
            inputHeight = Math.Clamp(inputHeight, minHeight, maxHeight);
        }

        if (ImGui.IsItemHovered() || ImGui.IsItemActive())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeNS);
        }

        ImGuiUtils.CustomSeparator();
    }

    void AddToChain()
    {
        if (selectedAvailableArgIndex >= 0)
        {
            var availableArgs = configManager.Config.AvailableArguments.ToArray();

            var selectedArg = availableArgs[selectedAvailableArgIndex];

            configManager.Config.SelectedArgumentsChain.Add(selectedArg);
        }
    }

    void MakeNewFeature()
    {
        if (configManager.Config.SelectedArgumentsChain.Count > 0)
        {
            var feature = new Feature()
            {
                Name = "Bypass Method #" + (configManager.Config.Features.Count + 1),
                Arguments = configManager.Config.SelectedArgumentsChain.ToArray(),
            };

            configManager.Config.Features.Add(feature);

            configManager.Save();
        }
    }
}
