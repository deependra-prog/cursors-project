using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CursorSwitcher
{
    public class MainForm : Form
    {
        private readonly ListBox themeList = new();
        private readonly Button applyButton = new() { Text = "Apply" };
        private readonly Button revertButton = new() { Text = "Revert to Default" };
        private readonly Button refreshButton = new() { Text = "Refresh List" };
        private readonly Label statusLabel = new() { AutoSize = true, ForeColor = Color.DarkGreen };

        private List<CursorTheme> themes = new();

        public MainForm()
        {
            Text = "Cursor Switcher";
            Width = 480;
            Height = 420;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9);

            themeList.Dock = DockStyle.Top;
            themeList.Height = 280;
            themeList.Font = new Font("Segoe UI", 10);

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, Padding = new Padding(8) };
            buttonPanel.Controls.Add(applyButton);
            buttonPanel.Controls.Add(revertButton);
            buttonPanel.Controls.Add(refreshButton);

            statusLabel.Dock = DockStyle.Top;
            statusLabel.Padding = new Padding(8, 6, 8, 6);

            Controls.Add(statusLabel);
            Controls.Add(buttonPanel);
            Controls.Add(themeList);

            applyButton.Click += (_, _) => ApplySelected();
            revertButton.Click += (_, _) => RevertDefault();
            refreshButton.Click += (_, _) => LoadThemes();

            LoadThemes();
        }

        private void LoadThemes()
        {
            var cursorsFolder = Path.Combine(AppContext.BaseDirectory, "Cursors");
            themes = CursorApplier.LoadThemes(cursorsFolder);

            themeList.Items.Clear();
            foreach (var theme in themes)
                themeList.Items.Add(theme.Name);

            if (themeList.Items.Count == 0)
            {
                statusLabel.Text = $"No cursor themes found in:\n{cursorsFolder}";
                statusLabel.ForeColor = Color.DarkRed;
            }
            else
            {
                themeList.SelectedIndex = 0;
                statusLabel.Text = $"{themes.Count} theme(s) loaded.";
                statusLabel.ForeColor = Color.DarkGreen;
            }
        }

        private void ApplySelected()
        {
            if (themeList.SelectedIndex < 0)
                return;

            var theme = themes[themeList.SelectedIndex];
            try
            {
                CursorApplier.ApplyTheme(theme);
                statusLabel.Text = $"Applied \"{theme.Name}\".";
                statusLabel.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Failed to apply: {ex.Message}";
                statusLabel.ForeColor = Color.DarkRed;
            }
        }

        private void RevertDefault()
        {
            try
            {
                CursorApplier.RevertToDefault();
                statusLabel.Text = "Reverted to Windows default cursors.";
                statusLabel.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Failed to revert: {ex.Message}";
                statusLabel.ForeColor = Color.DarkRed;
            }
        }
    }
}
