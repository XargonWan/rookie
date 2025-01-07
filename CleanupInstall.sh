#!/bin/bash

echo "Killing AndroidSideloader processes..."
pkill -f AndroidSideloader

echo "Killing adb processes..."
pkill -f adb

folderPath="$HOME/.local/share/Rookie.WTF/"
echo "Deleting contents of $folderPath..."
rm -rf "${folderPath:?}"/*

folderPath="$HOME/.local/share/Rookie.AndroidSideloader/"
echo "Deleting contents of $folderPath..."
rm -rf "${folderPath:?}"/*

echo "Cleanup complete."
read -p "Press any key to continue..."
