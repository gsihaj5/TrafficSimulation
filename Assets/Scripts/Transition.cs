
public class Transition {
    public State state;
    public int action;
    public float reward;
    public State newState;

    public Transition(State state, int action, float reward, State newState)
    {
        this.state = state;
        this.action = action;
        this.reward = reward;
        this.newState = newState;
    }
}
