HOW TO ADD A CURSOR THEME
==========================

1. Make a new subfolder here, e.g. Cursors\MyTheme\
2. Copy your .cur / .ani files into it.
3. Add a manifest.json in that same folder:

   {
     "Name": "My Theme",
     "Files": {
       "Arrow": "arrow.cur",
       "AppStarting": "working.ani",
       "Wait": "busy.ani",
       "No": "unavailable.ani"
     }
   }

4. The left-hand key ("Arrow", "AppStarting", "Wait", "No", ...) is the
   Windows pointer ROLE, not a filename. Valid roles:

     Arrow        Normal Select
     Help         Help Select
     AppStarting  Working In Background
     Wait         Busy
     Crosshair    Precision Select
     IBeam        Text Select
     NWPen        Handwriting
     No           Unavailable
     SizeNS       Vertical Resize
     SizeWE       Horizontal Resize
     SizeNWSE     Diagonal Resize 1
     SizeNESW     Diagonal Resize 2
     SizeAll      Move
     UpArrow      Alternate Select
     Hand         Link Select
     Pin          Location Select
     Person       Person Select

   You only need to list the roles your theme actually provides —
   anything you leave out is left untouched by that theme.

5. Rebuild (or just re-copy the folder if running from source with
   "dotnet run") and the theme will show up in the app's list
   automatically — no code changes needed.

IMPORTANT: .ani FILE STRUCTURE
-------------------------------
If you hand-build or edit .ani files, the animation frames MUST live
inside a RIFF LIST chunk typed "fram" (not "icon" — that's an easy
byte-level mistake that makes Windows silently ignore the whole file
with no error message).
