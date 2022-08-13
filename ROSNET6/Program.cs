using ROSNET6;
using Serilog;
using Uml.Robotics.Ros;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();


var host = Host.CreateDefaultBuilder(args).UseSerilog()
    .ConfigureServices(services => { services.AddSingleton<ROSWorker>(); })
    .Build();

ROS.Init(Array.Empty<string>(), "ROSNET6");
var spinner = new AsyncSpinner();
spinner.Start();

var rosWorker = host.Services.GetService<ROSWorker>();

IObservable<Messages.std_msgs.String> observable = rosWorker.GetObservable();
IDisposable disposable = observable.Subscribe(msg =>
{
    Log.Information("Received message. \n" + $"MessageType: {msg.MessageType} \n" + $"Data: {msg.data} \n");
});

NodeHandle node = new NodeHandle();
await rosWorker.InitWorker(node);

await host.RunAsync();
