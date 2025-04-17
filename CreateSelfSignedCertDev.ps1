# Variables
$certName = "localhost.dev"
$certPath = "C:\temp\yarp-cert.pfx"
$password = ConvertTo-SecureString -String "P@ssw0rd!" -Force -AsPlainText

# Create a self-signed certificate with SANs
$cert = New-SelfSignedCertificate -DnsName "localhost.dev", "bpc-dev.localhost.dev", "sample-dev.localhost.dev" -CertStoreLocation "Cert:\LocalMachine\My"

# Export the certificate to a PFX file
Export-PfxCertificate -Cert $cert -FilePath $certPath -Password $password

