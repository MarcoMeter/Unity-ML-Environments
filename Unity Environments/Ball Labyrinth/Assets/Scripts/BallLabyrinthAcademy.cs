using MLAgents;

public class BallLabyrinthAcademy : Academy
{
    #region Member Fields
    private int _ballPositionIndex = 12;
    #endregion

    #region Member Properties
    public int BallPositionIndex
    {
        get { return _ballPositionIndex; }
    }
    #endregion

    #region Academy Overrides
    public override void AcademyReset()
    {
        _ballPositionIndex = (int)resetParameters["positionIndex"];
    }
    #endregion
}
