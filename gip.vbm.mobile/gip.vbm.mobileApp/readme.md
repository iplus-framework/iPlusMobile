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

## 📦 Building Signed Release APKs

This project produces signed APKs for two device families: **Datalogic** and **Zebra**.
The APKs are signed with a team-shared keystore — no Google Play account required.

### Prerequisites (one-time setup)

#### 1. Obtain the keystore file

Ask a colleague for `mobilemaui.keystore`. Place it anywhere on your machine
(e.g., `~/keys/mobilemaui.keystore`).

#### 2. Export environment variables

The `.csproj` resolves the keystore path, key password, and store password from
environment variables. The keystore path is also passed by the VS Code task.
Only the path differs per developer; the alias and passwords are shared across
the team. Add these to your `~/.bashrc` or `~/.zshrc`:

```sh
export MOBILEMAUI_KEYSTORE_PATH="/path/to/mobilemaui.keystore"
export MOBILEMAUI_KEY_PASS="<key password>"
export MOBILEMAUI_STORE_PASS="<store password>"
```

After editing, reload:

```sh
source ~/.bashrc
```

Verify the variables are set:

```sh
echo $MOBILEMAUI_KEYSTORE_PATH
echo $MOBILEMAUI_KEY_PASS
echo $MOBILEMAUI_STORE_PASS
```

All three must output a non-empty value. Otherwise the publish will fail with
a signing error.

### Build commands

#### Using VS Code tasks (recommended)

Press **Ctrl+Shift+P → Tasks: Run Task** and select:

- **`publish-release-datalogic-apk`** → builds Datalogic APK
- **`publish-release-zebra-apk`** → builds Zebra APK

#### From the command line

```sh
# Datalogic
dotnet publish gip.vbm.mobileApp/gip.vbm.mobileApp.csproj \
  -p:TargetFramework=net10.0-android \
  -p:Configuration=ReleaseDatalogic \
  -p:AndroidSigningKeyStore="$MOBILEMAUI_KEYSTORE_PATH" \
  -o:publish-datalogic

# Zebra
dotnet publish gip.vbm.mobileApp/gip.vbm.mobileApp.csproj \
  -p:TargetFramework=net10.0-android \
  -p:Configuration=ReleaseZebra \
  -p:AndroidSigningKeyStore="$MOBILEMAUI_KEYSTORE_PATH" \
  -o:publish-zebra
```

> **Note:** No `RuntimeIdentifiers` flag is needed — MAUI publishes both
> `arm` and `arm64` native libraries by default. Specifying it causes a bash
> parsing error (the semicolon splits the command).

### Where the APKs land

| Configuration       | Output Path                                      |
|--------------------|--------------------------------------------------|
| ReleaseDatalogic   | `publish-datalogic/com.gipsoft.mobilemaui-Signed.apk` |
| ReleaseZebra       | `publish-zebra/com.gipsoft.mobilemaui-Signed.apk`     |

### Distributing to devices

1. Upload the `*-Signed.apk` to your shared drive.
2. Share the download link with testers/customers.
3. They install it directly on the device (sideload).

Because the V5 MAUI app uses package name `com.gipsoft.mobilemaui` (different
from the legacy V4 Xamarin app `com.gipsoft.mobile`), both versions can coexist
on the same device for comparison/testing.

### Troubleshooting

**`NU1101: Unable to find package Microsoft.NETCore.App.Runtime.linux-bionic-arm`**

This is harmless — transitive dependencies pull in Linux runtimes that are not
needed for Android. Ignore or suppress with `-warnaserror:false`.

**`MSB1006: Property is not valid. Switch: android-arm64`**

Caused by `RuntimeIdentifiers=android-arm;android-arm64` — the semicolon is
interpreted as a command separator by bash. **Solution:** don't specify
`RuntimeIdentifiers` at all. MAUI bundles both `arm` and `arm64` by default.

**Signing fails silently / APK is unsigned**

Check that all three environment variables are set and point to valid values:

```sh
ls -la $MOBILEMAUI_KEYSTORE_PATH
echo $MOBILEMAUI_KEY_PASS
echo $MOBILEMAUI_STORE_PASS
```

If the keystore file is not found, update the path in `~/.bashrc` and restart
VS Code (VS Code caches the environment at startup).

If signing fails with `XA4314: $(AndroidSigningKeyPass) is empty` or
`XA4314: $(AndroidSigningStorePass) is empty`, the password environment
variables are missing or empty. Fix them in `~/.bashrc` and restart VS Code.

If signing still fails with `MSB6006: "java" exited with code 2`, ensure the
publish command (or task) uses `-p:AndroidSigningKeyStore=...`.
`AndroidSigningKeyPath` is not used by the Android SDK signing target.

**"Parse error when installing APK"**

Make sure you are installing the correct variant:
- Datalogic scanners → `ReleaseDatalogic` APK
- Zebra scanners → `ReleaseZebra` APK