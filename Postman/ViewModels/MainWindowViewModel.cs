using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Postman.Framework;
using Postman.Interfaces;
using Postman.Models;

namespace Postman.ViewModels
{
    public class MainWindowViewModel : BaseModel
    {
        private readonly IHttpService _httpService;
        private readonly ILogger<MainWindowViewModel> _logger;
        public IConfigurationRoot Configuration { get; }
        public AppSettings Options { get; set; }
        public IAppSettings Settings { get; set; }
        private readonly IOptionsMonitor<AppSettings> _optionsMonitor;
        protected internal virtual IDisposable ChangeListener { get; }

        public MainWindowViewModel(IHttpService httpService, ILogger<MainWindowViewModel> logger, ILoggerFactory loggerFactory, IConfigurationRoot config, IAppSettings settings, IOptions<AppSettings> options, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Configuration = config ?? throw new ArgumentNullException(nameof(config));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));

            Url = _optionsMonitor.CurrentValue.Url;// Options.Url;
            Count = Options.Count;
            UseToken = Options.UseToken;
            UseBasic = Settings.UseBasic;

            Async = true;

            Request = File.ReadAllText(@"Data\Refund3.json");
            Response = string.Empty;
            Status = "Ready";
            this.ChangeListener = _optionsMonitor.OnChange(this.OnMyOptionsChange);
        }

        protected internal virtual void OnMyOptionsChange(AppSettings settings, string name)
        {
            Options = settings;
            Settings = settings;

            Url = Settings.Url;
            Count = Settings.Count;
            UseToken = Settings.UseToken;
            UseBasic = Settings.UseBasic;
        }

        private string _url;
        [Required]
        [Url]
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (_url != value)
                {
                    _url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        private int _count;
        [Required]
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }

        private bool _useToken;
        public bool UseToken
        {
            get
            {
                return _useToken;
            }
            set
            {
                if (_useToken != value)
                {
                    _useToken = value;
                    OnPropertyChanged(nameof(UseToken));
                }
            }
        }

        private bool _useBasic;
        public bool UseBasic
        {
            get
            {
                return _useBasic;
            }
            set
            {
                if (_useBasic != value)
                {
                    _useBasic = value;
                    OnPropertyChanged(nameof(UseBasic));
                }
            }
        }

        private bool _async;
        public bool Async
        {
            get
            {
                return _async;
            }
            set
            {
                if (_async != value)
                {
                    _async = value;
                    OnPropertyChanged(nameof(Async));
                }
            }
        }

        private string _token;
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                if (_token != value)
                {
                    _token = value;
                    OnPropertyChanged(nameof(Token));
                }
            }
        }

        private string _request;
        [Required]
        public string Request
        {
            get
            {
                return _request;
            }
            set
            {
                if (_request != value)
                {
                    _request = value;
                    OnPropertyChanged(nameof(Request));
                }
            }
        }

        private string _response = "";
        public string Response
        {
            get
            {
                return _response;
            }
            set
            {
                if (_response != value)
                {
                    _response = value;
                    OnPropertyChanged(nameof(Response));
                }
            }
        }

        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private AsyncCommand _authAsyncCommand;
        public AsyncCommand AuthAsyncCommand
        {
            get
            {
                if (_authAsyncCommand == null)
                {
                    _authAsyncCommand = new AsyncCommand(async execute => await AuthAsync());
                }
                return _authAsyncCommand;
            }
            set
            {
                _authAsyncCommand = value;
            }
        }

        //[DebuggerStepThrough]
        public async Task AuthAsync()
        {
            if (UseToken)
            {
                Token = await _httpService.AuthTokenAsync(Options.AuthUrl);
            }
            if (UseBasic)
            {
                Token = await _httpService.AuthBasicAsync(Options.AuthUrl);
            }
            _logger.LogInformation($"TOKEN: {Token}");
        }

        private AsyncCommand _sendAsyncCommand;
        public AsyncCommand SendAsyncCommand
        {
            get
            {
                if (_sendAsyncCommand == null)
                {
                    _sendAsyncCommand = new AsyncCommand(async execute => await SendParallelAsync(), canExecute => CanSend());
                }
                return _sendAsyncCommand;
            }
            set
            {
                _sendAsyncCommand = value;
            }
        }

        private bool CanSend()
        {
            return !UseToken || !string.IsNullOrWhiteSpace(Token);
        }

        public async Task SendParallelAsync()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            var url = $"{Url}/Refunds";
            //var url = $"{Url}/admin/orders.json/";
            //if (!Async)
            //{
            //    url += "/Sync";
            //}
            var watchForParallel = Stopwatch.StartNew();

            if (Count == 1)
            {
                Response = await _httpService.PostAsync(url, Request, UseToken, Token, UseBasic);
            }
            else
            {
                var numbers = Enumerable.Range(1, Count).ToList();
                var tasks = new ConcurrentBag<Task>();

                Parallel.ForEach(numbers, number =>
                {
                    Task task = _httpService.PostAsync(url, Request, UseToken, Token, UseBasic);
                    tasks.Add(task);
                });
                await Task.WhenAll(tasks);
            }

            watchForParallel.Stop();
            var time = watchForParallel.Elapsed;
            var mode = Async ? "Async" : "Sync";
            Status = $"URL: {url} MODE: {mode} COUNT: {Count} ELAPSED: {time}";
            _logger.LogInformation(Status);
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }
    }
}
