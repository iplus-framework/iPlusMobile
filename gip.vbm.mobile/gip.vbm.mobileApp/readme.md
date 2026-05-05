# Development Setup

## ✅ Recommended: Waydroid (works with debugger)
```sh
adb connect 192.168.240.112:5555
```
<EmbedAssembliesIntoApk>True</EmbedAssembliesIntoApk> (must be set)

## ✅ USB Device Debugging (Linux + VS Code)

Use this when a physical Android device is connected over USB.

### 1. Prerequisites
1. Enable Developer options on device.
2. Enable USB debugging.
3. Connect with a data-capable USB cable.

### 2. Linux USB permissions (udev rule)
If `adb devices -l` shows `no permissions`, add a udev rule for the device vendor.

Example for this project device (vendor `05e0`):
```sh
sudo tee /etc/udev/rules.d/51-android.rules >/dev/null <<'EOF'
SUBSYSTEM=="usb", ATTR{idVendor}=="05e0", MODE="0666", GROUP="plugdev", TAG+="uaccess"
EOF

sudo chmod 644 /etc/udev/rules.d/51-android.rules
sudo udevadm control --reload-rules
sudo udevadm trigger
adb kill-server
adb start-server
```

### 3. Authorize computer on device
If `adb devices -l` shows `unauthorized`:
1. Keep device unlocked.
2. Revoke USB debugging authorizations on device.
3. Replug cable.
4. Accept `Allow USB debugging` prompt (check `Always allow`).

Verify status:
```sh
adb devices -l
```
Expected: device state is `device`.

### 4. VS Code debug profiles
This workspace has dedicated launch/task pairs:
1. `Debug - Android (USB)` -> uses task `run-debug-android-usb`
2. `Debug - Android (Waydroid)` -> uses task `run-debug-android-waydroid`

USB task uses:
```text
-p:AdbTarget=-d
```
so it always targets the physical USB device.

### 5. Start USB debugging
1. Confirm device is visible: `adb devices -l`
2. In VS Code select launch config: `Debug - Android (USB)`
3. Start debugging (F5)
4. Set breakpoints and run app flow

### 6. Troubleshooting
If multiple devices are connected and target selection is unclear:
```sh
adb devices -l
```
Optionally lock to exact serial in task args:
```text
-p:AdbTarget=-s <device-serial>
```

If RSA prompt does not appear:
```sh
adb kill-server
adb start-server
adb usb
adb reconnect
```

## ⚠️ Android Studio Emulator (debugger attachment unreliable)

Known issue: The Android Studio emulator on Linux has debugger attachment problems when forwarded over network ADB. Use Waydroid instead for development, or use the emulator without debugging.

### If needed for testing without debugger:
```sh
# Option 1: socat
socat TCP-LISTEN:5556,fork,bind=10.40.168.1 TCP:127.0.0.1:5555 &
adb connect 10.40.168.1:5556

# Option 2: SSH tunnel
ssh -L 5556:127.0.0.1:5555 damir@10.40.168.1
adb connect 127.0.0.1:5556
```