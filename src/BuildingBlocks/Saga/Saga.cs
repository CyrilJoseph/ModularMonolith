namespace CompanyName.MyProjectName.BuildingBlocks.Saga;

public abstract class Saga : ISaga
{
    public SagaId Id { get; private set; }

    public SagaStates State { get; protected set; }

    public virtual void Initialize(SagaId id, SagaStates state)
        => (Id, State) = (id, state);

    public virtual SagaId ResolveId(object message, ISagaContext context)
        => context.SagaId;

    public virtual void Complete()
        => State = SagaStates.Completed;

    public virtual Task CompleteAsync()
    {
        Complete();
        return Task.CompletedTask;
    }

    public virtual void Reject(Exception innerException = null)
    {
        State = SagaStates.Rejected;
        throw new ChronicleException("Saga rejection called by method", innerException);
    }

    public virtual Task RejectAsync(Exception innerException = null)
    {
        Reject(innerException);
        return Task.CompletedTask;
    }
}

public abstract class Saga<TData> : Saga, ISaga<TData>
    where TData : class, new()
{
    public TData Data { get; protected set; }

    public virtual void Initialize(SagaId id, SagaStates state, TData data)
    {
#pragma warning disable SA1100 // Do not prefix calls with base unless local implementation exists
        base.Initialize(id, state);
#pragma warning restore SA1100 // Do not prefix calls with base unless local implementation exists
        Data = data;
    }
}
