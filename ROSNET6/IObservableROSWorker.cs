namespace ROSNET6;

public interface IObservableROSWorker<in TArg, out T>
{
    IObservable<T> GetObservable();
    Task InitWorker(TArg arg);
}
