# Kata-neo

A local multiplayer FFA game

## Building from source

1. Clone the repository `git clone https://github.com/xMelisek/Kata-neo.git`

2. Build the project using .NET CLI (Ctrl + ` to open console in Visual Studio)

Build for Windows: `dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained`

Build for Linux: `dotnet publish -c Release -r linux-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained`

Build for MacOS: `dotnet publish -c Release -r osx-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained`

The build will be in `bin\Release\net6.0\win-x64\publish`

For more info refer to [this link](https://docs.monogame.net/articles/packaging_games.html)

## Contributing

Create a fork of the repository and then clone it

Code is in the Core folder

Data folder contains for example animation interval info

After making some changes, commit and create a pull request to be merged.
