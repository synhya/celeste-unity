using System;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private float xRemainder;
    public Vector2 Position;
    public Solid solids;
    public float Left { get; set; }
    public float Right { get; set; }

    // works like constructor
    protected virtual void Start()
    {
        Level.AllActors.Add(this);
    }

    public void MoveX(float amount, Action onCollide)
    {
        xRemainder += amount;
        int move = Mathf.RoundToInt(xRemainder);
            

        if (move != 0)
        {
            xRemainder -= move;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideAt(solids, Position + new Vector2(sign, 0)))
                {
                    Position.x += sign;
                    move -= sign;
                }
                else
                {
                    if (onCollide != null)
                        onCollide();
                    break;
                }
            }
        }
    }
    private bool CollideAt(Solid solids, Vector2 position)
    {
        return false;
    }

    public virtual bool IsRiding(Solid solid) { return true; }
    public virtual void Squish() { }
}

