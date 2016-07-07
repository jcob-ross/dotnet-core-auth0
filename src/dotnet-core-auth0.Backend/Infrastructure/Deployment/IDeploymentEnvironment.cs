namespace dotnet_core_auth0.Backend.Infrastructure.Deployment
{
  /// <summary>
  ///   Represents metadata about deployment
  /// </summary>
  public interface IDeploymentEnvironment
  {
    void Initialize();
    string DeploymentSha { get; }
    string Environment { get; }
    string Application { get; }
    string Framework { get; }
    string OS { get; }
  }
}