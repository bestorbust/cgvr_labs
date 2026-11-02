# AutoVerse — Manual Build & Deployment Guide

This guide provides step-by-step instructions for generating manual builds for **Windows Standalone** and **Mobile WebGL**, testing locally, and deploying to **Vercel**.

---

## 1. Prerequisites

- **Unity Version**: Unity `6000.6.7f1` (Unity 6.6)
- **Render Pipeline**: Universal Render Pipeline (URP 17.x)
- **Target Platforms**:
  - Windows Standalone (x86_64)
  - WebGL (WebGL 2.0)

---

## 2. Fast Manual Builds via Unity Menu

We have pre-configured one-click automated build pipelines directly in your Unity Editor menu toolbar:

### A. Windows Standalone Build
1. Open the project in Unity Editor.
2. In the top menu bar, click:  
   **`AutoVerse` > `Build` > `Build Windows Standalone`**
3. The build script automatically:
   - Configures Graphics API to **Direct3D 11** (avoids the known Unity 6 DX12 PSO variant compile hang).
   - Adds `Assets/Scenes/AutoVerse_Showwroom.unity` to build scenes.
   - Outputs the standalone executable to: `Builds/Windows/AutoVerse.exe`

### B. Mobile WebGL Build
1. In the top menu bar, click:  
   **`AutoVerse` > `Build` > `Build WebGL`**
2. The build script automatically:
   - Sets WebGL Memory Size to **512 MB** (optimal for both desktop browsers and mobile devices).
   - Sets WebGL 2.0 graphics API.
   - Sets Data Caching and compression options.
   - Outputs the WebGL bundle to: `Builds/WebGL/`

### C. Run Verification Tests
Before building, you can verify all 8 vehicles, 18 colors, finishes, accessories, and camera presets at any time:
- Click: **`AutoVerse` > `Test` > `Run Full AutoVerse Verification Suite`**
- View the console or inspect `test_execution_report.txt` in the project root.

---

## 3. Standard Unity Build Settings (Alternative Method)

If you prefer building through Unity's default dialog:
1. Go to **File > Build Settings...** (`Ctrl + Shift + B`).
2. Ensure `Assets/Scenes/AutoVerse_Showwroom.unity` is checked in **Scenes In Build**.
3. **For Windows**:
   - Platform: **Windows, Mac, Linux**
   - Target Platform: **Windows**
   - Architecture: **Intel 64-bit**
   - In **Player Settings > Other Settings > Auto Graphics API for Windows**: Uncheck this and ensure **Direct3D11** is at the top of the list (remove Direct3D12 if builds hang).
   - Click **Build** and select the folder `Builds/Windows/`.
4. **For WebGL**:
   - Platform: **WebGL** (click **Switch Platform** if not active).
   - In **Player Settings > Publishing Settings**:
     - WebGL Memory Size: `512`
     - Compression Format: `Disabled` (for fastest local testing) or `Gzip`/`Brotli` (for production).
   - Click **Build** and select the folder `Builds/WebGL/`.

---

## 4. Local Testing of WebGL Build

Browsers restrict WebGL and WebAssembly (`.wasm`) from running directly via `file:///` protocols due to browser CORS and security policies. You cannot simply double-click `index.html`.

### Option 1: 1-Click Batch Launcher (Easiest)
We have added a pre-configured local server with full Brotli (`.br`) decompression and MIME-type support:
1. Open the folder: `Builds/WebGL/`
2. Double-click **`run_local_webgl.bat`**
3. It starts a local server on port 8080 and automatically opens `http://localhost:8080/index.html` in your default browser!

### Option 2: Unity Editor "Build and Run"
In Unity Editor:
1. Go to **File > Build Settings...** (`Ctrl + Shift + B`)
2. Select **WebGL**
3. Click **Build and Run** (`Ctrl + B`)
4. Unity will automatically spin up its internal development web server and launch your browser.

### Option 3: Deploy to Vercel (Cloud Production)
Since you already have `autoverse-...vercel.app`:
1. Open PowerShell in `Builds/WebGL`
2. Run: `vercel --prod`
3. Vercel automatically applies [`vercel.json`](file:///c:/Users/priya/OneDrive/Desktop/Sem7/CGVR/cgvr_labs/CaseStudy/AutoVerse/Builds/WebGL/vercel.json) with proper `Content-Encoding: br` and WASM MIME types.

---

## 5. Deploying to Vercel

The repository includes a ready-to-use [`vercel.json`](file:///c:/Users/priya/OneDrive/Desktop/Sem7/CGVR/cgvr_labs/CaseStudy/AutoVerse/vercel.json) configured with appropriate HTTP headers and MIME types for Unity WebGL files (`.wasm`, `.data`, `.framework.js`, and `.br`/`.gz` compression).

### Deploy via Vercel CLI:
1. Install Vercel CLI (if not already installed):
   ```bash
   npm i -g vercel
   ```
2. Navigate to the WebGL build folder:
   ```powershell
   cd "c:\Users\priya\OneDrive\Desktop\Sem7\CGVR\cgvr_labs\CaseStudy\AutoVerse\Builds\WebGL"
   ```
3. Copy `vercel.json` into this folder:
   ```powershell
   Copy-Item "..\..\vercel.json" -Destination .
   ```
4. Deploy to production:
   ```bash
   vercel --prod
   ```

### Deploy via GitHub / GitLab / Bitbucket:
1. Push your repository to GitHub.
2. In the Vercel Dashboard, import the repository.
3. In Project Settings:
   - **Framework Preset**: Other
   - **Root Directory**: `Builds/WebGL`
4. Click **Deploy**. Vercel will automatically read `vercel.json` and serve the WebAssembly showroom at your custom URL.

---

## 6. Testing Responsive Design on Mobile Devices

When testing WebGL on mobile devices:
1. Ensure your mobile device is on the same local Wi-Fi network as your PC.
2. Find your PC's local IP address (e.g. `ipconfig` -> `192.168.1.xxx`).
3. Start the local server:
   ```powershell
   python -m http.server 8080 --bind 0.0.0.0
   ```
4. On your mobile phone browser (Safari / Chrome), navigate to:
   ```
   http://192.168.1.xxx:8080
   ```
5. Test both **Portrait** and **Landscape** orientations:
   - **Portrait**: Customization drawer slides out as a bottom-sheet; top navigation bar is compact; keyboard HUD is hidden.
   - **Landscape**: Customization drawer docks to the right; vehicle viewport remains wide and unobstructed.
   - **Touch Controls**: 1-finger orbit rotates camera; 2-finger pinch zooms in/out; touches over UI buttons do not accidentally rotate the 3D car.

