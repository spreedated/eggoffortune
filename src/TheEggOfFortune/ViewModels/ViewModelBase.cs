using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Serilog.Extensions.Logging;

namespace TheEggOfFortune.ViewModels
{
    public abstract partial class ViewModelBase<T> : ObservableObject where T : IElement
    {
        protected readonly ILogger _logger;

        [ObservableProperty]
        public partial T Instance { get; set; }

        #region Ctor
        protected ViewModelBase(string loggerName)
        {
            _logger = new SerilogLoggerProvider().CreateLogger(loggerName);
        }
        #endregion
    }
}
