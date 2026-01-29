# Development Setup

## ✅ Recommended: Waydroid (works with debugger)
```sh
adb connect 192.168.240.112:5555
```
<EmbedAssembliesIntoApk>True</EmbedAssembliesIntoApk> (must be set)

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