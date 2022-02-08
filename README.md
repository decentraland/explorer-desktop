# Decentraland Explorer Desktop

This repository contains the project to compile [Unity Renderer](https://github.com/decentraland/unity-renderer) to Windows/Mac/Linux and distribute it with the [kernel](https://github.com/decentraland/kernel).

## Before you start

1. [Contribution Guidelines](https://github.com/decentraland/unity-renderer/.github/CONTRIBUTING.md)
2. [Coding Guidelines](https://github.com/decentraland/unity-renderer/docs/style-guidelines.md)
3. [Code Review Standards](https://github.com/decentraland/unity-renderer/docs/code-review-standards.md)

# Running the explorer in desktop

## Debug using remote Unity Renderer

Take this path if you **are not going** to change [Unity Renderer](https://github.com/decentraland/unity-renderer) continuously.

When we clone this repo, we're going to use the code inside the [Unity Renderer](https://github.com/decentraland/unity-renderer), and the version will depend on what version we locked.

To update the Unity Renderer version, you can use:

```
./update-unity-renderer.sh <branch>
```

**The repository must always have `master` reference to unity-renderer. Only the commit hash may change.**

## Debug using local Unity Renderer

Take this path if you **are going** to change [Unity Renderer](https://github.com/decentraland/unity-renderer) continuously.
### Using the script

To update the Unity Renderer version locally, you can use:

```
./update-unity-renderer.sh <unity-renderer-path>
```
### Manually

You need to change the manifest.json in `explorer-desktop/unity-renderer-desktop/Packages/manifest.json`.

Inside it, you need to change the remote path of the package for the absolute path of the local one.

```
{
  "dependencies": {
    "com.coffee.git-dependency-resolver": "https://github.com/mob-sakai/GitDependencyResolverForUnity.git",
    "com.decentraland.unity-renderer": "file:/unity-renderer/unity-renderer/Assets"
  }
}
```

Please change "`file:/unity-renderer/unity-renderer/Assets`" to the corresponding location (absolute path).

After these steps, you can open **Unity Renderer** and **Unity Renderer Desktop** project at the same time, and the changes in **Unity Renderer** project will be reflected in the **Unity Renderer Desktop** immediately.

## Downloading artifacts to test

### Windows:
Execute the following commands in `Windows PowerShell`
```bash
# Download
curl -o unity-renderer-windows.zip https://renderer-artifacts.decentraland.org/desktop/main/unity-renderer-windows.zip

# Unzip
Expand-Archive -Path unity-renderer-windows.zip -DestinationPath unity-renderer-windows -Force
```
### Mac:
Execute the following commands:
```bash
# Download
curl -o unity-renderer-mac.zip https://renderer-artifacts.decentraland.org/desktop/main/unity-renderer-mac.zip

# Unzip
unzip unity-renderer-mac.zip -d unity-renderer-mac
```
### Linux: 
Execute the following commands:
```
# Download
curl -o unity-renderer-linux.zip https://renderer-artifacts.decentraland.org/desktop/main/unity-renderer-linux.zip

# Unzip
unzip unity-renderer-linux.zip -d unity-renderer-linux
```

## Copyright info

This repository is protected with a standard Apache 2 license. See the terms and conditions in the [LICENSE](LICENSE) file.
