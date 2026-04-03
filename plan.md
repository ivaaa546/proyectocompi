# Plan de Implementación: Editor de Texto con Conteo (Avalonia UI)

Implementar un editor de texto con interfaz gráfica usando **.NET 8 + Avalonia UI (MVVM)** que cumpla con todos los requisitos de [proyecto.md](file:///d:/Proyectos/proyecto_compi/proyecto.md).

---

## Estructura del Proyecto

```
d:\Proyectos\proyecto_compi\
├── proyecto.md
├── EditorTexto.sln
└── EditorTexto/
    ├── EditorTexto.csproj
    ├── App.axaml
    ├── App.axaml.cs
    ├── Program.cs
    ├── Models/
    │   └── TextAnalyzer.cs          ← Lógica de procesamiento
    ├── ViewModels/
    │   ├── ViewModelBase.cs
    │   └── MainWindowViewModel.cs   ← ViewModel principal
    └── Views/
        └── MainWindow.axaml         ← Interfaz gráfica
        └── MainWindow.axaml.cs
```

---

## Propuesta de Cambios

### Componente 1: Scaffolding del Proyecto

#### [NEW] `EditorTexto.sln` y `EditorTexto/EditorTexto.csproj`

Crear el proyecto usando el template de Avalonia:
```bash
dotnet new install Avalonia.Templates
dotnet new avalonia.mvvm -o EditorTexto --name EditorTexto
```
Esto genera automáticamente `App.axaml`, `Program.cs`, `ViewModelBase.cs`, y la estructura base.

---

### Componente 2: Modelo de Datos

#### [NEW] [TextAnalyzer.cs](file:///d:/Proyectos/proyecto_compi/EditorTexto/Models/TextAnalyzer.cs)

Clase C# responsable de toda la lógica de procesamiento de texto:

- **`AnalyzeText(string text)`**: Método principal que recibe el texto del editor.
- **Conteo de Palabras**: Dividir el texto por espacios/saltos de línea, filtrar signos de puntuación (`. , ; :`), contar tokens válidos.
- **Conteo de Vocales**: Iterar sobre el texto (ignorando mayúsculas/minúsculas), contar ocurrencias de cada vocal (`a, e, i, o, u`).
- **Resultado**: Devuelve un objeto con:
  - `int TotalPalabras`
  - `int TotalVocalesUnicas` (cuántas vocales distintas aparecen)
  - `List<VocalInfo>` con `{ Vocal, Cantidad }` solo para las vocales que aparecen (si no aparece, no se incluye).

---

### Componente 3: ViewModel

#### [MODIFY] [MainWindowViewModel.cs](file:///d:/Proyectos/proyecto_compi/EditorTexto/ViewModels/MainWindowViewModel.cs)

Reescribir el ViewModel generado por el template:

- **Propiedades Observables (bindeables al XAML)**:
  - `string TextoUsuario` → Bindeado al TextBox del editor.
  - `int TotalPalabras` → Resultado del conteo.
  - `int TotalVocalesUnicas` → Cuántas vocales distintas hay.
  - `ObservableCollection<VocalInfo> Vocales` → Lista para la Matriz de Vocales.
  - `bool MostrarResultados` → Para mostrar/ocultar las matrices.

- **Comando `EjecutarAnalisis`**: Invocado al presionar F5.
  1. Instancia `TextAnalyzer`.
  2. Llama a `AnalyzeText(TextoUsuario)`.
  3. Actualiza las propiedades con los resultados.
  4. Pone `MostrarResultados = true`.

---

### Componente 4: Vista (Interfaz Gráfica)

#### [MODIFY] [MainWindow.axaml](file:///d:/Proyectos/proyecto_compi/EditorTexto/Views/MainWindow.axaml)

Diseñar la ventana principal cumpliendo **todas las restricciones**:

```
┌──────────────────────────────────────────────────────────────┐
│  ENTRADA (editor de texto)                          [ F5 ]  │
├──────────────────────────────┬───────────────────────────────┤
│                              │  ┌─────────────┬──────┐      │
│  ┌────────────────────────┐  │  │  Entradas   │  #   │      │
│  │                        │  │  ├─────────────┼──────┤      │
│  │  Los compiladores,     │  │  │  Palabras   │  8   │      │
│  │  son grandes ventajas  │  │  ├─────────────┼──────┤      │
│  │  para los              │  │  │  Vocales    │  4   │      │
│  │  programadores.        │  │  └─────────────┴──────┘      │
│  │                        │  │                               │
│  │                        │  │  ┌─────────────┬──────┐      │
│  │    (TextBox con        │  │  │  Vocales    │  #   │      │
│  │     scroll interno)    │  │  ├─────────────┼──────┤      │
│  │                        │  │  │     a       │  8   │      │
│  │                        │  │  ├─────────────┼──────┤      │
│  │                        │  │  │     e       │  4   │      │
│  └────────────────────────┘  │  ├─────────────┼──────┤      │
│                              │  │     i       │  1   │      │
│                              │  ├─────────────┼──────┤      │
│                              │  │     o       │  7   │      │
│                              │  └─────────────┴──────┘      │
└──────────────────────────────┴───────────────────────────────┘
```

**Layout XAML:**
- `Grid` principal con 2 filas (barra superior + contenido) y 2 columnas (editor | tablas).
- **Columna izquierda**: `TextBox` con `AcceptsReturn="True"`, `TextWrapping="Wrap"`, scroll interno.
- **Columna derecha**: `StackPanel` con las dos tablas (Entradas y Vocales) usando `DataGrid` o `ItemsControl`.
- **Barra superior**: Título "ENTRADA (editor de texto)" a la izquierda, botón **F5** a la derecha.
- El botón F5 también es activable con **KeyBinding** de la tecla F5.

**Restricciones en XAML:**
- `Window`: Tamaño fijo (`CanResize="False"`), sin scrollbar global.
- `TextBox`: `AcceptsReturn="True"`, `TextWrapping="Wrap"`, scroll interno.
- `KeyBinding`: Mapear **F5** → Comando `EjecutarAnalisis`.
- **Colores**: Solo 2 colores para títulos/resaltado. Fondo blanco/neutro.
- Las matrices solo se muestran si `MostrarResultados == true`.

---

## Cumplimiento de Requisitos

| # | Requisito | Implementación |
|---|-----------|---------------|
| 1 | Editor de Texto | `TextBox` con `AcceptsReturn` |
| 2 | Tecla F5 | `KeyBinding` en Window → `EjecutarAnalisis` |
| 3 | Matriz de Entradas | Grid con `TotalPalabras` y `TotalVocalesUnicas` |
| 4 | Matriz de Vocales | `ItemsControl` con solo las vocales presentes |
| 5 | Excluir puntuación | Regex en `TextAnalyzer` |
| 6 | Vista única / sin scroll | `CanResize="False"`, layout fijo |
| 7 | Máximo 2 colores | Estilos XAML limitados |
| 8 | POO | Clases: `TextAnalyzer`, `VocalInfo`, `MainWindowViewModel` |
| 9 | GUI obligatoria | Avalonia UI |

---

## Plan de Verificación

### Verificación Manual

1. **Ejecutar la aplicación:**
   ```bash
   cd d:\Proyectos\proyecto_compi\EditorTexto
   dotnet run
   ```
2. **Escribir texto de prueba:**
   ```
   Hola, mundo. Este es un texto de prueba; con signos: varios.
   ```
3. **Presionar F5** y verificar:
   - ✅ Las matrices aparecen.
   - ✅ El conteo de palabras excluye los signos (`. , ; :`).
   - ✅ La Matriz de Vocales muestra solo las vocales presentes con su frecuencia.
   - ✅ No hay scrollbar en la ventana principal.
   - ✅ La ventana no se puede redimensionar.
   - ✅ Solo 2 colores usados para resaltar.
4. **Prueba con texto sin vocales** (ej: "rhythm"): verificar que la Matriz de Vocales quede vacía o no se muestre.
5. **Prueba con texto vacío**: presionar F5 sin escribir nada y verificar que no haya errores.
