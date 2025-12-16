# TurnForge

TurnForge es una solución multi-proyecto escrita en C#/.NET (uses .NET 9) que incluye una parte de motor de juego (TurnForge.Engine), reglas (TurnForge.Rules.BarelyAlive) y un proyecto Godot con soporte Mono (BarelyAlive.Godot). El repositorio contiene tests unitarios y el proyecto Godot que actúa como front-end/visualizador.

Este README explica cómo clonar, preparar el entorno (macOS), compilar la solución y ejecutar los tests y el proyecto Godot.

Requisitos (macOS):

- Git
- .NET SDK 9.x (instalar desde https://dotnet.microsoft.com)
- Godot 4.x con soporte Mono (descargar desde https://godotengine.org)

Opcional / recomendado:
- Rider o Visual Studio for Mac para editar los proyectos C#
- IDE/editor que soporte Godot (o usar el editor Godot integrado)

Estructura relevante del repo:

- TurnForge.sln — solución principal
- src/BarelyAlive.Godot — proyecto Godot (contiene `project.godot`, escenas .tscn, Assets/)
- src/TurnForge.Engine — motor principal en C#
- src/TurnForge.Adapters.Godot — adaptadores para Godot
- tests/TurnForge.Engine.Tests — tests unitarios

Pasos rápidos para clonar y compilar

1) Clonar el repositorio

```bash
git clone <repo-url> TurnForge
cd TurnForge
```

2) Restaurar paquetes y compilar la solución

```bash
# Desde la raíz del repo (zsh)
dotnet restore
dotnet build -c Debug
```

3) Ejecutar tests

```bash
dotnet test tests/TurnForge.Engine.Tests/TurnForge.Engine.Tests.csproj -c Debug
```

Abrir y ejecutar el proyecto Godot (Mono)

1. Abre el editor Godot (la versión con Mono). Desde Godot abre el proyecto situado en:

```
/path/to/TurnForge/src/BarelyAlive.Godot/project.godot
```

2. Si es la primera vez, Godot puede generar/importar assets y compilar los assemblies Mono. Deja que termine. Si ves archivos como `.godot/mono/temp/...` o `.godot/imported/...`, esos son artefactos locales y están ignorados por `.gitignore`.

Notas sobre paths y assets (problema frecuente: "no encuentra el fichero")

- Si un componente (p. ej. `MissionLoader.LoadFromFile("Assets/mission01.json")`) no encuentra el fichero en tiempo de ejecución, revisa el path relativo al *working directory* de la aplicación. En tests, las pruebas suelen ejecutarse con el working directory en el directorio del proyecto de test o en `bin/Debug/net9.0/`.
  - Solución rápida para tests: asegúrate de que `Assets/mission01.json` esté copiado en la carpeta de salida de los tests o marca el fichero en el proyecto de tests como "Copy to output directory" (en el csproj: <ItemGroup><None Include="Assets/mission01.json"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None></ItemGroup>).
  - Para la ejecución desde Godot: coloca tus archivos de assets en el árbol de `src/BarelyAlive.Godot/Assets/` y accede con rutas relativas dentro del proyecto Godot.

Control de versiones (.gitignore)

- Este repositorio incluye reglas para ignorar artefactos de compilación (`bin/`, `obj/`), caches de Godot (`.godot/`, `.import/`, `.mono/`) y otros temporales. Los ficheros fuente como `.cs`, `.csproj`, `.sln`, `.tscn`, `project.godot`, `Assets/*` están pensados para estar versionados.

Push / Pull / Flujo de trabajo recomendado

- Trabaja en ramas feature: `git checkout -b feature/mi-cambio`
- Añade cambios: `git add .`
- Haz commits concisos: `git commit -m "Añade X: ..."`
- Empuja tu rama: `git push origin feature/mi-cambio`
- Abre un Pull Request.

Diagnóstico rápido si algo no aparece en "to commit"

- Ejecuta `git status --porcelain` para ver estado. Los ficheros ignorados (por `.gitignore`) no aparecerán como untracked.
- Para ver qué archivos están siendo ignorados: `git ls-files --others --ignored --exclude-standard`.
- Si un fichero debería estar trackeado pero se está ignorando, revisa `.gitignore` y las reglas negativas (`!pattern`) que habiliten ficheros concretos (por ejemplo `!**/*.tscn` o `!src/BarelyAlive.Godot/Assets/**`).

Contacto / Contribución

Si quieres que te ayude a ajustar la configuración de `.gitignore`, a resolver por qué `MissionLoader` no encuentra `mission01.json`, o a commit/push los cambios pendientes, dime y me encargo de:

- Añadir entradas específicas a los csproj para copiar assets a la salida
- Corregir y limpiar archivos trackeados accidentalmente en git (ejecutando `git rm --cached` en bin/obj/artefactos)
- Hacer commit y push de los cambios (si me autorizas a ejecutar esos comandos en tu entorno)

---

Archivo generado automáticamente: `README.md` (añádelo al repo con `git add README.md && git commit -m "docs: add README" && git push`).

