namespace IdentityServer;

public class Worker : IHostedService
{
  // TODO: register clients
  // TODO: register scopes
  public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
