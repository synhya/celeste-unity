
public partial class Player
{

    
    // vars
    private float jumpGraceTimer;

    public int NormalUpdate()
    {
        if (CanDash)
        {
            Speed += LiftBoost;                   
            return StartDash();
        }


        return StNormal;
    }

    private void Jump()
    {
        
    }
    
    private void SuperJump()
    {
        
    }
}


