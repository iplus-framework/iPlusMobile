#!/bin/bash

DEVICE="192.168.240.112:5555"
PACKAGE="com.gipsoft.mobile"

echo "=== Android MAUI Debugging Setup ==="
echo ""

# Step 1: Forward debugger port
echo "1. Setting up port forwarding..."
adb -s $DEVICE forward tcp:10000 tcp:10000
echo "   ✓ Port 10000 forwarded"

# Step 2: Get process ID
echo ""
echo "2. Finding app process..."
PID=$(adb -s $DEVICE shell pidof $PACKAGE)
if [ -z "$PID" ]; then
    echo "   ✗ App is not running! Please start it first."
    exit 1
fi
echo "   ✓ App is running (PID: $PID)"

# Step 3: Check if debugger is listening
echo ""
echo "3. Checking debugger status..."
DEBUG_PORT=$(adb -s $DEVICE shell "cat /proc/$PID/cmdline" | tr '\0' '\n' | grep -o 'debug=[0-9]*' | cut -d= -f2)
if [ -z "$DEBUG_PORT" ]; then
    echo "   ✗ App was not started with debugging enabled"
    echo ""
    echo "To enable debugging, rebuild and run with:"
    echo "  dotnet build gip.vbm.mobileApp/gip.vbm.mobileApp.csproj -t:Run -f net10.0-android -c Debug"
    exit 1
fi
echo "   ✓ Debugger is available on port $DEBUG_PORT"

# Step 4: Forward the mono debugger port
echo ""
echo "4. Forwarding Mono debugger port..."
adb -s $DEVICE forward tcp:10000 tcp:$DEBUG_PORT
echo "   ✓ Debugger port forwarded (localhost:10000 -> device:$DEBUG_PORT)"

echo ""
echo "=== Ready to debug! ==="
echo "Now press F5 in VS Code and select 'Android Attach'"
