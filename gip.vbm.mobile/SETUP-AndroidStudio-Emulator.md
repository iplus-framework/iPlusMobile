# Android Studio Emulator Setup for VS Code on Ubuntu 24.04 KDE/Wayland

## Step 1: Install Android Studio

### Option A: Via Snap (Recommended - Easiest)
```bash
sudo snap install android-studio --classic
```

### Option B: Manual Download
1. Download from: https://developer.android.com/studio
2. Extract and run:
```bash
cd ~/Downloads
tar -xzf android-studio-*.tar.gz
sudo mv android-studio /opt/
/opt/android-studio/bin/studio.sh
```

## Step 2: Initial Android Studio Setup

1. **Launch Android Studio:**
   ```bash
   android-studio  # if installed via snap
   # OR
   /opt/android-studio/bin/studio.sh
   ```

2. **Follow the setup wizard:**
   - Choose "Standard" installation
   - Accept licenses
   - Let it download SDK components (this takes 10-15 minutes)

3. **Important paths to note:**
   - SDK Location: `~/Android/Sdk` (or `/home/damir/Android/Sdk`)
   - AVD Location: `~/.android/avd/`

## Step 3: Install Required SDK Components

1. In Android Studio, go to: **Tools → SDK Manager**

2. **SDK Platforms tab:**
   - ✅ Android 14.0 (API 34) - Recommended
   - ✅ Android 13.0 (API 33)
   - ✅ Any other versions your app targets

3. **SDK Tools tab:**
   - ✅ Android SDK Build-Tools
   - ✅ Android SDK Command-line Tools
   - ✅ Android Emulator
   - ✅ Android SDK Platform-Tools
   - ✅ Intel x86 Emulator Accelerator (HAXM) - if on Intel CPU
   - ✅ Google Play Services

4. Click **Apply** and wait for downloads to complete

## Step 4: Configure Environment Variables

Add to your `~/.bashrc` or `~/.zshrc`:

```bash
# Android SDK
export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/emulator
export PATH=$PATH:$ANDROID_HOME/platform-tools
export PATH=$PATH:$ANDROID_HOME/cmdline-tools/latest/bin
```

Then reload:
```bash
source ~/.bashrc
```

Verify:
```bash
adb version
emulator -version
```

## Step 5: Enable KVM for Hardware Acceleration (Critical for Performance)

1. **Check if KVM is supported:**
   ```bash
   egrep -c '(vmx|svm)' /proc/cpuinfo
   # Should return a number > 0
   ```

2. **Install KVM:**
   ```bash
   sudo apt update
   sudo apt install qemu-kvm libvirt-daemon-system libvirt-clients bridge-utils
   ```

3. **Add your user to the kvm group:**
   ```bash
   sudo usermod -aG kvm $USER
   sudo usermod -aG libvirt $USER
   ```

4. **Reboot or log out and back in** for group changes to take effect

5. **Verify KVM access:**
   ```bash
   ls -la /dev/kvm
   # Should show: crw-rw---- 1 root kvm
   ```

## Step 6: Create an Android Virtual Device (AVD)

### Option A: Using Android Studio GUI (Easier)

1. In Android Studio: **Tools → Device Manager**
2. Click **Create Device**
3. Choose a device definition (e.g., "Pixel 6")
4. Select a system image:
   - **Recommended:** API 34 (Android 14)
   - Choose **x86_64** architecture (not ARM)
   - Download the system image if needed
5. Configure AVD:
   - Name: `MAUI_Debug_Device` (or your choice)
   - Graphics: **Hardware - GLES 2.0** (important for Wayland!)
   - Boot option: **Cold boot**
6. Click **Finish**

### Option B: Using Command Line

```bash
# List available system images
sdkmanager --list | grep system-images

# Install a system image (example: Android 14)
sdkmanager "system-images;android-34;google_apis_playstore;x86_64"

# Create AVD
avdmanager create avd \
  -n MAUI_Debug_Device \
  -k "system-images;android-34;google_apis_playstore;x86_64" \
  -d "pixel_6"
```

## Step 7: Fix Wayland Issues (Important for KDE Plasma Wayland)

The emulator may have issues with Wayland. Create a wrapper script:

```bash
nano ~/start-android-emulator.sh
```

Add this content:
```bash
#!/bin/bash
export QT_QPA_PLATFORM=xcb
export ANDROID_EMULATOR_USE_SYSTEM_LIBS=1
$ANDROID_HOME/emulator/emulator -avd MAUI_Debug_Device -gpu host "$@"
```

Make it executable:
```bash
chmod +x ~/start-android-emulator.sh
```

## Step 8: Start the Emulator

### Option 1: Using your script
```bash
~/start-android-emulator.sh
```

### Option 2: Direct command
```bash
export QT_QPA_PLATFORM=xcb
emulator -avd MAUI_Debug_Device -gpu host
```

### Option 3: From Android Studio
- Tools → Device Manager → Click ▶️ on your AVD

**First boot takes 2-5 minutes. Be patient!**

## Step 9: Verify Connection

Once the emulator is running:

```bash
adb devices
```

You should see:
```
List of devices attached
emulator-5554   device
```

## Step 10: Configure VS Code for Emulator Debugging

Your existing configuration should work! The emulator appears as a regular device.

### Update your tasks.json (already done):
```json
{
    "label": "android-run",
    "command": "dotnet",
    "type": "shell",
    "args": [
        "build",
        "${workspaceFolder}/gip.vbm.mobileApp/gip.vbm.mobileApp.csproj",
        "-t:Run",
        "-f",
        "net10.0-android",
        "-c",
        "Debug",
        "-p:AndroidAttachDebugger=true",
        "-p:AndroidSdbHostPort=10000",
        "-p:AndroidSdbTargetPort=10000"
    ],
    "problemMatcher": "$msCompile"
}
```

### Your launch.json is ready:
```json
{
    "name": "Android Attach",
    "type": "mono",
    "request": "attach",
    "address": "localhost",
    "port": 10000
}
```

## Step 11: Test Debugging

1. **Start the emulator:**
   ```bash
   ~/start-android-emulator.sh
   ```

2. **In VS Code, run the task:**
   - `Ctrl+Shift+P` → "Tasks: Run Task" → "android-run"

3. **Wait for the app to start on the emulator**

4. **Press F5** to attach the debugger

5. **Set breakpoints and debug!**

## Troubleshooting

### Emulator won't start
```bash
# Check KVM permissions
ls -la /dev/kvm

# Try software rendering (slower)
emulator -avd MAUI_Debug_Device -gpu swiftshader_indirect
```

### Emulator is very slow
```bash
# Make sure you're using x86_64 image, not ARM
# Verify KVM is working:
kvm-ok

# Install if not present:
sudo apt install cpu-checker
```

### Wayland display issues
```bash
# Use the wrapper script with QT_QPA_PLATFORM=xcb
# Or run Android Studio with:
QT_QPA_PLATFORM=xcb android-studio
```

### Debugger won't attach
```bash
# Check port forwarding
adb -e forward tcp:10000 tcp:10000

# Check if app is running
adb -e shell ps | grep gipsoft

# View logs
adb -e logcat | grep -i mono
```

### Multiple devices connected
```bash
# List all devices
adb devices

# Specify emulator explicitly in tasks.json:
# Add: "-p:AdbTarget=-e"
```

## Performance Tips

1. **Allocate more RAM to emulator:**
   - Edit AVD: Device Manager → ⚙️ → Advanced Settings
   - Set RAM to 4096 MB or higher

2. **Enable hardware keyboard:**
   - AVD Settings → Advanced → Enable keyboard input

3. **Disable animations in Android:**
   - Settings → Developer Options
   - Set all animation scales to 0.5x or off

## Quick Reference Commands

```bash
# List AVDs
emulator -list-avds

# Start emulator in background
emulator -avd MAUI_Debug_Device -no-window -no-audio &

# Take screenshot
adb -e shell screencap /sdcard/screen.png
adb -e pull /sdcard/screen.png

# Install APK manually
adb -e install path/to/app.apk

# Clear app data
adb -e shell pm clear com.gipsoft.mobile

# Restart adb
adb kill-server
adb start-server
```

## Next Steps

Once you've verified the emulator works:
1. Keep Waydroid for quick testing (it's faster to start)
2. Use Android Studio emulator for debugging
3. Consider creating multiple AVDs for different Android versions

The emulator should now work reliably with VS Code debugging! 🎉
