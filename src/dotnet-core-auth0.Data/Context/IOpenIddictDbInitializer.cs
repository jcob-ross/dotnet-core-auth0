namespace dotnet_core_auth0.Data.Context
{
  using System.Threading.Tasks;

  public interface IOpenIddictDbInitializer
  {
    Task InitializeAsync();
  }
}