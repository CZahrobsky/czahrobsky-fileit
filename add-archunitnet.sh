#!/usr/bin/env bash
# From the solution root, add the ArchUnitNET NuGet package to the test project so the ArchUnitNET namespaces resolve.
dotnet add FileIt.Module.Services.Test/FileIt.Module.Services.Test.csproj package ArchUnitNET