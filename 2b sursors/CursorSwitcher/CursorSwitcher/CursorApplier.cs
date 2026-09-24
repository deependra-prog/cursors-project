using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace CursorSwitcher
{
    /// <summary>
    /// One cursor pack, described by a manifest.json living in its own
    /// subfolder under Cursors\. "Files" maps a Windows pointer role
    /// (see RoleDisplayNames below) to a .cur or .ani filename in that
    /// same folder.
    /// </summary>
    public class CursorTheme
    {
        public string Name { get; set; } = "";
        public Dictionary<string, string> Files { get; set; } = new();

        // Set at load time, not read from JSON.
        [System.Text.Json.Serialization.JsonIgnore]
        public string Folder { get; set; } = "";
    }

    public static class CursorApplier
    {
        // Windows' internal registry value names under
        // HKCU\Control Panel\Cursors, mapped to friendly labels for the UI.
        // A theme's manifest only needs to list the roles it actually
        // provides a file for — anything omitted is left untouched.
        public static readonly Dictionary<string, string> RoleDisplayNames = new()
        {
            { "Arrow", "Normal Select" },
            { "Help", "Help Select" },
            { "AppStarting", "Working In Background" },
            { "Wait", "Busy" },
            { "Crosshair", "Precision Select" },
            { "IBeam", "Text Select" },
            { "NWPen", "Handwriting" },
            { "No", "Unavailable" },
            { "SizeNS", "Vertical Resize" },
            { "SizeWE", "Horizontal Resize" },
            { "SizeNWSE", "Diagonal Resize 1" },
            { "SizeNESW", "Diagonal Resize 2" },
            { "SizeAll", "Move" },
            { "UpArrow", "Alternate Select" },
            { "Hand", "Link Select" },
            { "Pin", "Location Select" },
            { "Person", "Person Select" },
        };

        /// <summary>
        /// Scans Cursors\*\manifest.json and returns every valid theme found.
        /// A malformed manifest is skipped rather than crashing the picker.
        /// </summary>
        public static List<CursorTheme> LoadThemes(string cursorsRootFolder)
        {
            var themes = new List<CursorTheme>();
            if (!Directory.Exists(cursorsRootFolder))
                return themes;

            foreach (var dir in Directory.GetDirectories(cursorsRootFolder))
            {
                var manifestPath = Path.Combine(dir, "manifest.json");
                if (!File.Exists(manifestPath))
                    continue;

                try
                {
                    var json = File.ReadAllText(manifestPath);
                    var theme = JsonSerializer.Deserialize<CursorTheme>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (theme != null && !string.IsNullOrWhiteSpace(theme.Name))
                    {
                        theme.Folder = dir;
                        themes.Add(theme);
                    }
                }
                catch
                {
                    // Skip this one theme; don't take down the whole picker
                    // over one bad manifest.
                }
            }

            return themes;
        }

        /// <summary>
        /// Writes each role's file path into the registry and asks Windows
        /// to reload cursors immediately. Files must live at a permanent
        /// path (the app's own install folder) — Windows stores the path
        /// directly, so moving or deleting the file later breaks the cursor.
        /// </summary>
        public static void ApplyTheme(CursorTheme theme)
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors", writable: true)
                ?? throw new InvalidOperationException("Could not open the cursor registry key.");

            foreach (var (role, fileName) in theme.Files)
            {
                var fullPath = Path.Combine(theme.Folder, fileName);
                if (!File.Exists(fullPath))
                    continue; // skip a missing role instead of failing the whole apply

                key.SetValue(role, fullPath, RegistryValueKind.String);
            }

            key.SetValue("", theme.Name, RegistryValueKind.String);
            NativeMethods.BroadcastCursorChange();
        }

        /// <summary>
        /// Clears custom cursor entries so Windows falls back to its
        /// built-in system cursors.
        /// </summary>
        public static void RevertToDefault()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors", writable: true);
            if (key == null)
                return;

            foreach (var role in RoleDisplayNames.Keys)
            {
                try { key.DeleteValue(role, throwOnMissingValue: false); }
                catch { /* role wasn't set — nothing to remove */ }
            }

            key.SetValue("", "Windows Default", RegistryValueKind.String);
            NativeMethods.BroadcastCursorChange();
        }
    }
}
