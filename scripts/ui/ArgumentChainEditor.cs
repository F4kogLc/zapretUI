using ImGuiNET;
using System.Numerics;

internal class ArgumentChainEditor : IElement
{
    readonly ConfigManager configManager;

    int selectedAvailableArgIndex = -1;

    public ArgumentChainEditor(ConfigManager configManager)
    {
        this.configManager = configManager;
    }

    public void Render()
    {
        ImGui.Text("Argument Chain Builder");

        var availableArgs = configManager.Config.AvailableArguments.ToArray();

        if (ImGui.Combo("##Chain", ref selectedAvailableArgIndex, availableArgs, availableArgs.Length))
        {
            // при выборе аргумента
        }

        ImGui.SameLine();

        if (ImGui.Button("Add to Chain") && selectedAvailableArgIndex >= 0)
        {
            var selectedArg = availableArgs[selectedAvailableArgIndex];
            configManager.Config.SelectedArgumentsChain.Add(selectedArg);
        }

        ImGui.BeginChild("ChainList", new Vector2(0, 250));

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

        ImGui.Separator();
    }
}
