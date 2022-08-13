using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Uml.Robotics.Ros;
using String = Messages.std_msgs.String;

namespace ROSNET6;

public class ROSWorker : IObservableROSWorker<NodeHandle, String>
{
    private readonly ILogger<ROSWorker> _logger;
    private readonly Subject<String> _subject = new();
    private Subscriber? _subscriber;

    public ROSWorker(ILogger<ROSWorker> logger)
    {
        _logger = logger;
        _logger.LogInformation("ROSWorker: constructed.");
    }

    public IObservable<String> GetObservable() => _subject.ObserveOn(Scheduler.NewThread).AsObservable();

    public async Task InitWorker(NodeHandle nodeHandle)
    {
        try
        {
            await nodeHandle.SubscribeAsync<String>(
                "/chatter",
                1,
                cb => _subject.OnNext(cb));
            _logger.LogInformation("ROSWorker: Subscriber initialized.");

            ROS.WaitForShutdown();
        }
        catch (Exception e)
        {
            _logger.LogError("ROSWorker: Subscribe failed.\\n {Message}", e.Message);
            throw;
        }
    }
}
