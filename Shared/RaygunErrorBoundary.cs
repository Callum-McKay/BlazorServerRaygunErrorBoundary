#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Mindscape.Raygun4Net.AspNetCore;

namespace blazor_server.Shared
{
  /// <summary>Captures errors thrown from its child content.</summary>
  public class RaygunErrorBoundary : ErrorBoundaryBase
  {
    [Inject]
    private IErrorBoundaryLogger? ErrorBoundaryLogger { get; set; }

    [Inject]
    private IConfiguration Configuration { get; set; }

    /// <summary>
    /// Invoked by the base class when an error is being handled. The default implementation
    /// logs the error.
    /// </summary>
    /// <param name="exception">The <see cref="T:System.Exception" /> being handled.</param>
    protected override async Task OnErrorAsync(Exception exception)  {
      await this.ErrorBoundaryLogger.LogErrorAsync(exception);
      await new RaygunClient(Configuration["RaygunSettings:ApiKey"]).SendInBackground(exception);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
      if (this.CurrentException == null)
        builder.AddContent(0, this.ChildContent);
      else if (this.ErrorContent != null)
      {
        builder.AddContent(1, this.ErrorContent(this.CurrentException));
      }
      else
      {
        builder.OpenElement(2, "div");
        builder.AddAttribute(3, "class", "blazor-error-boundary");
        builder.CloseElement();
      }
    }
  }
}