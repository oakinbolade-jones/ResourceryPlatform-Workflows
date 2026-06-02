try {
    $client = New-Object System.Net.Sockets.TcpClient('smartserve.ecowas.int', 7600)
    $stream = New-Object System.Net.Security.SslStream($client.GetStream(), $false, { $true })
    $stream.AuthenticateAsClient('smartserve.ecowas.int')
    $x509 = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($stream.RemoteCertificate)
    Write-Output "Subject: $($x509.Subject)"
    Write-Output "Issuer: $($x509.Issuer)"
    Write-Output "Thumbprint: $($x509.Thumbprint)"
    Write-Output "NotBefore: $($x509.NotBefore)"
    Write-Output "NotAfter: $($x509.NotAfter)"
    $san = $x509.Extensions | Where-Object { $_.Oid.FriendlyName -eq 'Subject Alternative Name' }
    if ($san) {
        Write-Output "SAN: $($san[0].Format($false))"
    } else {
        Write-Output "SAN: none"
    }
    $stream.Close()
    $client.Close()
} catch {
    Write-Output "ERROR: $($_.Exception.Message)"
    if ($_.Exception.InnerException) { Write-Output "INNER: $($_.Exception.InnerException.Message)" }
    exit 1
}
