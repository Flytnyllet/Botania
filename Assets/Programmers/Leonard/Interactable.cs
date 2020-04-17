public interface IInteractable //No it's not a typo
{
    //Gör så att saker kan plockas upp från Interactor.cs
    //Behöver en collider för att bli tillgänglig
    bool Interact();
}
