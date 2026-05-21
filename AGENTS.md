# AGENTS.md

## Repo Shape
- The git repo root is `proyectocompi/`, not the parent `proyecto_compii/` directory.
- This is a single .NET 8 Avalonia desktop app: `EditorTexto.sln` contains only `EditorTexto/EditorTexto.csproj`.
- The assignment requirements live in `proyecto.md`; `plan.md` is implementation notes and may be less current than code.

## Commands
- Build from repo root: `dotnet build EditorTexto.sln`.
- Run the GUI app: `dotnet run --project EditorTexto/EditorTexto.csproj`.
- There is no test project or CI config in the repo; use `dotnet build EditorTexto.sln` as the basic verification step.

## App Wiring
- Entry point: `EditorTexto/Program.cs`; it starts Avalonia with `App` via `StartWithClassicDesktopLifetime`.
- Main window is created in `EditorTexto/App.axaml.cs` with `DataContext = new MainWindowViewModel()`.
- Main UI is `EditorTexto/Views/MainWindow.axaml`; its code-behind handles F5 explicitly so the shortcut still works when the `TextBox` has focus.
- Text analysis logic is centralized in `EditorTexto/Models/TextAnalyzer.cs`; keep counting behavior there rather than duplicating it in the view model or XAML.

## Avalonia/MVVM Gotchas
- `EditorTexto.csproj` enables `AvaloniaUseCompiledBindingsByDefault`; keep `x:DataType` accurate in XAML bindings and data templates.
- `CommunityToolkit.Mvvm` source generators create bindable properties from `[ObservableProperty]` fields and `EjecutarAnalisisCommand` from `[RelayCommand] private void EjecutarAnalisis()`.
- `ViewLocator` maps `*ViewModel` to `*View` by name; avoid renaming view/view-model pairs without updating the convention.

## Assignment Constraints To Preserve
- The app must remain a GUI, single-window editor controlled by F5; console-only behavior would violate `proyecto.md`.
- The main screen should not use a global scrollbar or multiple windows; the text editor may have its own internal scroll.
- Show only vowels that appear; the `Entradas` table counts unique vowels, not total vowel occurrences.
- Punctuation `.`, `,`, `;`, and `:` must not count as words.
- Colors are only for titles/highlights and should stay limited to at most two highlight colors.
