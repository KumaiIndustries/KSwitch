#!/bin/sh

SCRIPT_DIR=$(dirname "$(realpath "$0")")

if [ -f "$SCRIPT_DIR/KSwitch.Headless.SDL2" ]; then
    RYUJINX_BIN="KSwitch.Headless.SDL2"
fi

if [ -f "$SCRIPT_DIR/KSwitch" ]; then
    RYUJINX_BIN="KSwitch"
fi

if [ -z "$RYUJINX_BIN" ]; then
    exit 1
fi

COMMAND="env DOTNET_EnableAlternateStackCheck=1"

if command -v gamemoderun > /dev/null 2>&1; then
    COMMAND="$COMMAND gamemoderun"
fi

exec $COMMAND "$SCRIPT_DIR/$RYUJINX_BIN" "$@"
