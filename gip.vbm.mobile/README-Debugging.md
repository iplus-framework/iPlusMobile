# MAUI Android Debugging on Waydroid with VS Code

## Current Status
VS Code debugging of MAUI Android apps on Waydroid is extremely limited because:
- MAUI uses Mono runtime (not .NET CoreCLR)
- Waydroid doesn't support all Android debugging features
- VS Code's Mono debugger has poor Android support

## Alternative Debugging Methods

### 1. Use Android Logcat (Recommended for VS Code)
View real-time logs from your app:

```bash
# View all app logs
adb -s 192.168.240.112:5555 logcat --pid=$(adb -s 192.168.240.112:5555 shell pidof com.gipsoft.mobile)

# Filter for specific tags
adb -s 192.168.240.112:5555 logcat -s "DOTNET:D" "mono:D" "mono-rt:D"
```

Add logging in your code:
```csharp
using System.Diagnostics;

Debug.WriteLine($"My variable: {myVar}");
Console.WriteLine($"Important info: {info}");
```

### 2. Use Visual Studio (Full debugging support)
If debugging is critical, use Visual Studio on Windows/Mac which has complete MAUI debugging support.

### 3. Remote Debugging Alternative
For advanced users, manually enable Mono soft debugger:

1. Set environment variable before app starts:
   ```bash
   adb -s 192.168.240.112:5555 shell "setprop debug.mono.log default,debugger-agent"
   ```

2. Start app with debug parameters in AndroidManifest.xml:
   ```xml
   <application android:debuggable="true">
   ```

This is complex and not recommended for regular development.

## Current Setup Summary
- ✅ App builds and deploys successfully
- ✅ Can view logs via `adb logcat`
- ✅ Can use `Debug.WriteLine()` for diagnostics
- ❌ Step-through debugging not available in VS Code
- ❌ Breakpoints not supported in current setup

## Recommendation
For this project, use **logging and logcat** for debugging on Linux, or switch to **Visual Studio** on Windows/Mac for full debugging capabilities.
