#!/bin/bash

sideloader=$(ls AndroidSideloader*.exe | head -n 1)

echo "$sideloader"
./"$sideloader" --offline