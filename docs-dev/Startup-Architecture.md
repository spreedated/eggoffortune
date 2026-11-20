# Startup Architecture

```
MauiProgram.CreateMauiApp()
    │
    ├── Configure logger (Serilog)
    ├── Register services
    └── Build()
    │
    ▼
App.App()
    │
    ▼
App.CreateWindow()
    │
    ▼
LoadingScreen.xaml (Minimum load time 2500ms)
    │
    ├── Load configuration
    ├── Initialize database
    ├── Load application data
    └── Initialize services
    │
    ▼
AppShell.xaml
    │
    └── MainPage.xaml
         ├── DevScreen.xaml
         └── EndPagePopup.xaml
```