using Unity.Netcode.Components;

public class NetworkAnimatorClientAndServer : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
