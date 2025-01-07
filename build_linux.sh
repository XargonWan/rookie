#!/bin/bash

# Default to Release if no argument is provided
CONFIG=Release
if [ ! -z "$1" ]; then
    if [ "$1" == "debug" ] || [ "$1" == "Debug" ]; then
        CONFIG=Debug
    fi
fi

# Check for dotnet CLI
if ! command -v dotnet &> /dev/null; then
    echo "dotnet CLI not found! Please check your installation."
    exit 1
fi

echo "Building in $CONFIG configuration..."
dotnet build AndroidSideloader.sln -c $CONFIG -p:DefineConstants="LINUX"