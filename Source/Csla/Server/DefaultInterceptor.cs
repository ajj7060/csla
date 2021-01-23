using System;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Server
{
  /// <summary>Default interceptor which will automatically create
  /// a new <see cref="IServiceScope"/> if one is not set and dispose it
  /// when done.</summary>
  public class DefaultInterceptor : IInterceptDataPortal
  {
    private readonly IServiceProvider _defaultServiceProvider;
    private IServiceScope _scope;

    public DefaultInterceptor(IServiceProvider defaultServiceProvider)
    {
      _defaultServiceProvider = defaultServiceProvider;
    }

    /// <summary>Override to perform initialization work; the <see cref="ApplicationContext.CurrentServiceProvider"/>
    /// will be set before this method is called.</summary>
    /// <param name="e"></param>
    public virtual void Initialize(InterceptArgs e) { }

    /// <summary>Override to perform cleanup; the <see cref="ApplicationContext.CurrentServiceProvider"/>
    /// will be disposed and cleared after this method is called.</summary>
    /// <param name="e"></param>
    public virtual void Complete(InterceptArgs e) { }

    void IInterceptDataPortal.Initialize(InterceptArgs e)
    {
      if (ApplicationContext.CurrentServiceProvider == null && _defaultServiceProvider != null)
      {
        var scopeFactory = _defaultServiceProvider.GetRequiredService<IServiceScopeFactory>();
        _scope = scopeFactory.CreateScope();
        ApplicationContext.CurrentServiceProvider = _scope.ServiceProvider;
      }

      Initialize(e);
    }

    void IInterceptDataPortal.Complete(InterceptArgs e)
    {
      Complete(e);

      if (_scope != null)
      {
        ApplicationContext.CurrentServiceProvider = null;
        _scope.Dispose();
      }
    }
  }
}

