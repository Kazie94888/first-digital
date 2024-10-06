# SmartCoin OS Tool

## Preparing the tool

This readme will only show how to pack the tool. To check in details you can follow [this doc link](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create).

We need to:
- Pack the tool
- Install
- Remove
- Use it


### Packging

- Position at the same level as the csproj file
- Build the package `dotnet pack`

The command above should have created the nuget at `\src\Tool\nupkg\Tool.[version].nupkg`

### Installing

Install the tool locally by running `dotnet tool install --add-source .\nupkg\ SmartCoinTool`


If the installation was successful, you should see a note similar to:
> You can invoke the tool from this directory using the following commands: 'dotnet tool run smart-coin' or 'dotnet smart-coin'.

[see dotnet-tool-install](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install)

### Remove

If you feel like you don't have the latest version, you can remove the toll and install again.
Remove the tool by running `dotnet tool uninstall SmartCoinTool`

### Using the tool

From the directory of this project you can invoke the tool by running `dotnet smart-coin`

## Making Changes

When making changes, you also need to publish the new version to the repository.

You can do so by:
- Increasing the version in csproj
- Position on the same level as the csproj
- Run `dotnet pack`
- Run: `dotnet nuget push .\nupkg\SmartCoinTool.<The-New-Version>.nupkg --api-key <The-GH-Key> --source github`
