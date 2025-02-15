using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using CheapLoc;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using ImGuiNET;

namespace PlayerTrack
{
    /// <summary>
    /// Player Detail Display Options View.
    /// </summary>
    public partial class Panel
    {
        private readonly List<Vector4> colorPalette = ImGuiHelpers.DefaultColorPalette(36);

        private void PlayerDisplay()
        {
            if (this.SelectedPlayer == null) return;
            const float sameLineOffset = 150f;

            // category
            ImGui.Text(Loc.Localize("PlayerCategory", "Category"));
            ImGuiHelpers.ScaledRelativeSameLine(sameLineOffset);
            var categoryNames = this.plugin.CategoryService.GetCategoryNames().ToArray();
            var categoryIds = this.plugin.CategoryService.GetCategoryIds().ToArray();
            var currentCategory = this.plugin.CategoryService.GetCategory(this.SelectedPlayer.CategoryId);
            var categoryIndex = Array.IndexOf(categoryNames, currentCategory.Name);
            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            if (ImGui.Combo(
                "###PlayerTrack_PlayerCategory_Combo",
                ref categoryIndex,
                categoryNames,
                categoryNames.Length))
            {
                this.SelectedPlayer.CategoryId = categoryIds[categoryIndex];
                this.plugin.PlayerService.UpdatePlayerCategory(this.SelectedPlayer);
            }

            ImGuiHelpers.ScaledDummy(0.5f);
            ImGui.Separator();

            // override warning
            ImGui.TextColored(ImGuiColors.DalamudYellow, Loc.Localize(
                                  "OverrideNote",
                                  "These config will override category config."));
            ImGuiHelpers.ScaledDummy(1f);

            // title
            ImGui.Text(Loc.Localize("Title", "Title"));
            ImGuiHelpers.ScaledRelativeSameLine(sameLineOffset);

            var title = this.SelectedPlayer.Title;
            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            if (ImGui.InputText("###PlayerTrack_PlayerTitle_Input", ref title, 30))
            {
                this.SelectedPlayer.Title = title;
                this.plugin.PlayerService.UpdatePlayerTitle(this.SelectedPlayer);
            }

            ImGui.Spacing();

            // icon
            ImGui.Text(Loc.Localize("Icon", "Icon"));
            ImGuiHelpers.ScaledRelativeSameLine(sameLineOffset);

            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            var iconIndex = this.plugin.IconListIndex(this.SelectedPlayer!.Icon);
            if (ImGui.Combo(
              "###PlayerTrack_PlayerIcon_Combo",
              ref iconIndex,
              this.plugin.IconListNames(),
              this.plugin.IconListNames().Length))
            {
              this.SelectedPlayer.Icon = this.plugin.IconListCodes()[iconIndex];
              this.plugin.PlayerService.UpdatePlayerIcon(this.SelectedPlayer);
            }

            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.TextColored(
                ImGuiColors.DalamudWhite,
                ((FontAwesomeIcon)this.SelectedPlayer.Icon).ToIconString());
            ImGui.PopFont();
            ImGui.Spacing();

            // visibility
            ImGui.Text(Loc.Localize("VisibilityType", "Visibility"));
            ImGuiHelpers.ScaledRelativeSameLine(sameLineOffset);

            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            var visibilityType = (int)this.SelectedPlayer.VisibilityType;
            if (ImGui.Combo(
                "###PlayerTrack_VisibilityType_Combo",
                ref visibilityType,
                Enum.GetNames(typeof(VisibilityType)),
                Enum.GetNames(typeof(VisibilityType)).Length))
            {
                this.SelectedPlayer.VisibilityType = (VisibilityType)visibilityType;
                this.plugin.PlayerService.UpdatePlayerVisibilityType(this.SelectedPlayer);
            }

            ImGui.Spacing();

            // list color
            ImGui.Text(Loc.Localize("List", "List"));
            ImGuiHelpers.ScaledRelativeSameLine(sameLineOffset);
            var listColor = this.SelectedPlayer.EffectiveListColor();
            if (ImGui.ColorButton("List Color###PlayerTrack_PlayerListColor_Button", listColor))
            {
                ImGui.OpenPopup("###PlayerTrack_PlayerListColor_Popup");
            }

            if (ImGui.BeginPopup("###PlayerTrack_PlayerListColor_Popup"))
            {
                if (ImGui.ColorPicker4("List Color###PlayerTrack_PlayerListColor_ColorPicker", ref listColor))
                {
                    this.SelectedPlayer.ListColor = listColor;
                    this.plugin.PlayerService.UpdatePlayerListColor(this.SelectedPlayer);
                }

                this.PlayerOverride_ListColorSwatchRow(0, 8);
                this.PlayerOverride_ListColorSwatchRow(8, 16);
                this.PlayerOverride_ListColorSwatchRow(16, 24);
                this.PlayerOverride_ListColorSwatchRow(24, 32);
                ImGui.EndPopup();
            }

            // fc name color
            ImGui.Spacing();
            ImGui.Text(Loc.Localize("OverrideFCNameColor", "Override FCNameColor"));
            ImGuiHelpers.ScaledRelativeSameLine(sameLineOffset);
            var overrideFCNameColor = this.SelectedPlayer.OverrideFCNameColor;
            if (ImGui.Checkbox(
                "###PlayerTrack_PlayerOverrideFCNameColor_Checkbox",
                ref overrideFCNameColor))
            {
                this.SelectedPlayer.OverrideFCNameColor = overrideFCNameColor;
                this.plugin.PlayerService.UpdatePlayerOverrideFCNameColor(this.SelectedPlayer);
            }

            // alerts
            ImGui.Spacing();
            ImGui.Text(Loc.Localize("Alerts", "Alerts"));
            ImGuiHelpers.ScaledRelativeSameLine(sameLineOffset);
            var isAlertEnabled = this.SelectedPlayer.IsAlertEnabled;
            if (ImGui.Checkbox(
                "###PlayerTrack_PlayerAlerts_Checkbox",
                ref isAlertEnabled))
            {
                this.SelectedPlayer.IsAlertEnabled = isAlertEnabled;
                this.plugin.PlayerService.UpdatePlayerAlert(this.SelectedPlayer);
            }

            // reset
            ImGuiHelpers.ScaledDummy(5f);
            if (ImGui.Button(Loc.Localize("Reset", "Reset") + "###PlayerTrack_PlayerOverrideModalReset_Button"))
            {
                this.SelectedPlayer.Reset();
                this.plugin.PlayerService.ResetPlayerOverrides(this.SelectedPlayer);
            }
        }

        private void PlayerOverride_ListColorSwatchRow(int min, int max)
        {
            ImGui.Spacing();
            for (var i = min; i < max; i++)
            {
                if (ImGui.ColorButton("###PlayerTrack_PlayerListColor_Swatch_" + i, this.colorPalette[i]))
                {
                    this.SelectedPlayer!.ListColor = this.colorPalette[i];
                    this.plugin.PlayerService.UpdatePlayerListColor(this.SelectedPlayer);
                }

                ImGui.SameLine();
            }
        }
    }
}
