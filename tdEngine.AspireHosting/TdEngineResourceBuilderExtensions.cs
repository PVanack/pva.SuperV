using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

public static class TdEngineResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="TdEngineResource"/> to the given
    /// <paramref name="builder"/> instance. Uses the "3.3.0" tag.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="tdEnginePort">The TdEngine native port. Defaults to 6030</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{TdEngineResource}"/> instance that
    /// represents the added TdEngine resource.
    /// </returns>
    public static IResourceBuilder<TdEngineResource> AddTdEngine(
        this IDistributedApplicationBuilder builder,
        string name,
        int? tdEnginePort = 6030)
    {
        // The AddResource method is a core API within .NET Aspire and is
        // used by resource developers to wrap a custom resource in an
        // IResourceBuilder<T> instance. Extension methods to customize
        // the resource (if any exist) target the builder interface.
        var resource = new TdEngineResource(name);

        return builder.AddResource(resource)
                      .WithImage(TdEngineContainerImageTags.Image)
                      .WithImageRegistry(TdEngineContainerImageTags.Registry)
                      .WithImageTag(TdEngineContainerImageTags.Tag)
                      .WithEndpoint(
                          targetPort: 6030,
                          port: tdEnginePort,
                          name: TdEngineResource.TdEngineEndpointName);
    }
}

// This class just contains constant strings that can be updated periodically
// when new versions of the underlying container are released.
internal static class TdEngineContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "tdengine/tdengine";

    internal const string Tag = "3.3.6.0";
}