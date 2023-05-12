// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Options;

using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Rabbit SSL Option.
/// </summary>
[DynamicallyAccessedMembers(
    DynamicallyAccessedMemberTypes.PublicProperties)]
public class RabbitSslOptions
{
    /// <summary>
    /// Gets or sets the acceptable policy errors.
    /// </summary>
    /// <value>The acceptable policy errors.</value>
    public SslPolicyErrors AcceptablePolicyErrors { get; set; }

    /// <summary>
    /// Gets or sets the cert passphrase.
    /// </summary>
    /// <value>The cert passphrase.</value>
    public string? CertPassphrase { get; set; }

    /// <summary>
    /// Gets or sets the cert path.
    /// </summary>
    /// <value>The cert path.</value>
    public string? CertPath { get; set; }

    /// <summary>
    /// Gets or sets the certificate selection callback.
    /// </summary>
    /// <value>The certificate selection callback.</value>
    public LocalCertificateSelectionCallback? CertificateSelectionCallback { get; set; }

    /// <summary>
    /// Gets or sets the certificate validation callback.
    /// </summary>
    /// <value>The certificate validation callback.</value>
    public RemoteCertificateValidationCallback? CertificateValidationCallback { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether check for certificate revocation.
    /// </summary>
    /// <value>The flag if we check certificate revocation.</value>
    public bool CheckCertificateRevocation { get; set; }

    /// <summary>
    /// Gets the certificates.
    /// </summary>
    /// <value>The certificates.</value>
    public X509CertificateCollection? Certificates { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether ssl is enabled.
    /// </summary>
    /// <value>The flag if enabled.</value>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the server name.
    /// </summary>
    /// <value>The server name.</value>
    public string? ServerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    /// <value>The version.</value>
    public SslProtocols Version { get; set; }
}
