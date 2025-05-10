// For ease of discovery, resource types should be placed in
// the Aspire.Hosting.ApplicationModel namespace. If there is
// likelihood of a conflict on the resource name consider using
// an alternative namespace.
using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

public class TdEngineResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
    public const string TdEngineEndpointName = "TdEngine";
    // An EndpointReference is a core .NET Aspire type used for keeping
    // track of endpoint details in expressions. Simple literal values cannot
    // be used because endpoints are not known until containers are launched.
    private EndpointReference? _tdEngineReference;

    public EndpointReference TdEngineEndpoint
        => _tdEngineReference ??= new(this, TdEngineEndpointName);

    // Required property on IResourceWithConnectionString. Represents a connection
    // string that applications can use to access the tdEngine server. In this case
    // the connection string is composed of the TdEngineEndpoint endpoint reference.
    public ReferenceExpression ConnectionStringExpression
        => ReferenceExpression.Create($"{TdEngineEndpoint.Property(EndpointProperty.HostAndPort)};username=root;password=taosdata");
}