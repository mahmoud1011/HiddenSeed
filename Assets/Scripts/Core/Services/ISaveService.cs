public interface ISaveService
{
    void Save(GameStateDto state);
    GameStateDto Load();
}