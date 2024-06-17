# Nuget Pack Instructions

Unfortunately, due to a lot of bugs and restrictions, we cannot really pack packages normally. I have scripts to do this internally, but wouldn't feel like open sourcing this since I hardcode a lot of paths. However, I will provide instructions if you wish to build nuget packages yourself.

## UWP
1. Set the configuration as Debug x64 (yes, Debug, not Release. When building as Release, for some reasons files .xbf are not generated)
2. Right Click -> Pack the project while building
3. Extract* that package
4. Copy everything from `lib\uap10.0.22621\` to `.\PackFolderTemp\lib\uap10.0.18362` 
5. Zip the package, rename it as .nupkg instead of .zip

## WinUI 3

1. Copy .csproj to a new empty directory
2. Remove `<Import Project="..\Get.UI\Get.UI.projitems" Label="Shared" />`
3. Right Click -> Pack this project. I will call this a "dummy package"
4. Extract* that dummy package
5. Delete everything in `lib/`
6. Build the real, original `Get.UI.WinUI` project
7. Copy everything in `bin/Release/net6.0-windows10.0.19041.0` into `lib/` folder
8. Zip the package, rename it as .nupkg instead of .zip

## Notes

*Some apps like 7-Zip can extract this directly. If you have trouble finding the way to unzip, rename the file from .nupkg to .zip then extract