using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IGridModel>().To<GridModel>().AsSingle().WithArguments(5, 5, 5);
        //Container.Bind<GridController>().AsSingle();
    }

}
