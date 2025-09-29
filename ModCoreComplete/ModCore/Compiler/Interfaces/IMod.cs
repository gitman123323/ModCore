public interface IMod
{
    void OnLoad() {}

    void OnUnload() {}

    void OnUpdate() {}

    void OnFixedUpdate() {}

    void OnLateUpdate() {}

    bool ShouldUnload() {return ShouldUnload();}

    void OnGUI() {}
}