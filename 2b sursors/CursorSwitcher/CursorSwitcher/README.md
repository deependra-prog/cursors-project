# Cursor Switcher

A lightweight Windows desktop app for picking from a set of premade
animated (`.ani`) and static (`.cur`) cursor packs and applying them
system-wide with one click — no manual Browse-and-Assign through
Windows Settings required.

## Features

- Lists every cursor theme bundled with the app
- Applies a full theme instantly (writes the registry + hot-reloads
  the cursor scheme — no logoff or restart needed)
- One-click revert to Windows' default cursors
- Add new themes just by dropping a folder in — no code changes

## Requirements

- Windows 10 or 11
- To build from source: [.NET 8 SDK](https://dotnet.microsoft.com/download)
  (end users running the published `.exe` don't need anything installed)

## Building

```
dotnet build -c Release
```

## Publishing a single-file .exe for distribution

```
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

The output `.exe` (in `bin\Release\net8.0-windows\win-x64\publish\`)
runs on any Windows 10/11 machine with no .NET install required.
Ship it together with the `Cursors\` folder sitting next to it.

## Adding your own cursor packs

See `Cursors\README.txt` for the manifest format. In short: one
subfolder per theme, containing your `.cur`/`.ani` files plus a
`manifest.json` that maps Windows pointer roles ("Arrow", "Wait",
"AppStarting", "No", etc.) to filenames.

## How it works

- Reads every `Cursors\*\manifest.json` at startup to build the theme
  list
- On Apply, writes each role's full file path into
  `HKEY_CURRENT_USER\Control Panel\Cursors`, then calls the Win32
  `SystemParametersInfo(SPI_SETCURSORS, ...)` API so Explorer picks up
  the change immediately
- Cursor files must stay at a permanent path (the app's install
  folder) — Windows stores a direct path reference, so moving the
  files afterward breaks the assignment until you re-apply

## License

- me 
