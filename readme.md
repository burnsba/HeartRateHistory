CSharp project, dotnet core 3.1 windows executable.

Receieves data from a bluetooth heart rate sensor and creates a scrolling bar chart of heartrate.

Note: this uses a forked version of https://github.com/XamlAnimatedGif/WpfAnimatedGif found in the lib/ folder.

Note: Bluetooth support pulls in a dependency from the Windows SDK, a file called "windows.winmd". After installing the SDK, you can find it (depending on the actual version) at

    C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.17763.0\Windows.winmd
