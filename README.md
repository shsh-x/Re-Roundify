# Re-Roundify

A feather-light utility to add beautiful, customizable rounded corners to your displays built from the ground up to be the most efficient and robust screen-rounding Windows app available (at the moment i write this).

## ✨ Features

*   **Multi-Monitor Ready:** Automatically detects and rounds the corners of all your displays, regardless of their arrangement, resolution, or scaling.
*   **Resolution Independent:** Whether you're on a 1080p laptop or a 4K ultrawide, the corners scale perfectly for a crisp, clean look every time.
*   **Extremely Lightweight:** Rests at 0% CPU usage when idle. The intelligent event-driven design only uses resources when absolutely necessary.
*   **Overlay & Game Compatible:** The corners are designed to win the "war for topmost," staying cleanly overlaid on top of stubborn UI like the taskbar, NVIDIA/AMD overlays, and borderless full-screen games.
*   **Fully Configurable:** Easily change the corner radius via a simple shortcut flag — no need to edit any files or recompile.
*   **Zero-Install & Portable:** Runs as a single `.exe` file with no installation required.
*   **Completely safe:** No administrator priveleges needed to run.
  
## 🚀 How to Use

Follow these steps to get Re-Roundify up and running.

### 1. Initial Setup

1.  **Download the App:** Download the [latest release](https://github.com/shsh-x/Re-Roundify/releases/latest).
2.  **Place the File:** Move `ReRoundify.exe` to a permanent location on your computer, for example, your `Documents` folder.
3.  **Run It:** Double-click `ReRoundify.exe` to run it. Your screen corners should now be rounded with the default 16px radius!

### 2. Configuration (Changing the Radius)

You can easily change the corner radius by using a command-line flag in a shortcut.

1.  **Create a Shortcut:** Navigate to where you placed `ReRoundify.exe`, right-click on it, and select **"Create shortcut"**.
2.  **Open Properties:** Right-click the newly created shortcut and choose **"Properties"**.
3.  **Modify the Target Field:** In the "Shortcut" tab, you will see a **Target** field. Add the `--radius X` flag at its' very end (where X is your radius in pixels).

    **Examples:**
    *   For a **larger** 32px radius:
        `"C:\Program Files\ReRoundify.exe" --radius 32`
    *   For a **smaller** 8px radius:
        `"C:\Program Files\ReRoundify.exe" --radius 8`

4.  Click **OK** to save. Now, when you use this shortcut, the app will launch with your custom radius.

### 3. Running on Startup

To have ReRoundify launch automatically when you turn on your computer:

1.  **Open the Startup Folder:** Press `Win + R` to open the Run dialog, type `shell:startup`, and press Enter.
2.  **Move the Shortcut:** Drag and drop your **configured shortcut** into the opened Startup folder.

## 📋 Requirements

* **OS:** Windows 10/11 (64-bit)
* **Software:** [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime?cid=getdotnetcore_runtime_windowshosting_x64_hero)
    *   *Make sure to get the installer for the **"Desktop Apps"** runtime (x64 version).*

## 🔧 Building from Source

If you wish to compile the project yourself:

1.  Install the **.NET 8 SDK**.
2.  Clone this repository.
3.  Open a terminal in the project's root directory.
4.  To create a single, portable `.exe`, run the following command:
    ```powershell
    dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained false
    ```
5.  The final `ReRoundify.exe` will be located in the `bin\Release\net8.0-windows\win-x64\publish` folder.
