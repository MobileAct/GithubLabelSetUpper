#!/bin/sh

dotnet publish GithubLabelSetUpper -c Release -r win10-x64
dotnet publish GithubLabelSetUpper -c Release -r osx.10.12-x64
dotnet publish GithubLabelSetUpper -c Release -r linux-x64
dotnet pack GithubLabelSetUpper -c Release