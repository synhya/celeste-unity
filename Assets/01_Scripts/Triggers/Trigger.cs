
public abstract class Trigger : Entity
{
    public bool Triggered;

    protected override void Start()
    {
        base.Start();
        Room.Triggers.Add(this);
    }

    public abstract void OnEnter(Entity other);
    public abstract void OnStay(Entity other);
    public abstract void OnLeave(Entity other);
}


