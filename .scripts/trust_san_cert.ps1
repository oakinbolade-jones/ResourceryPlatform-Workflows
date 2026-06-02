try {
    $PasswordPlain = 'dev_cert_2026!'
    $pwd = ConvertTo-SecureString $PasswordPlain -Force -AsPlainText
    $pfxPath = Join-Path $env:USERPROFILE '.aspnet\https\aspnetapp.pfx'
    Write-Output "Loading certificate from $pfxPath"
    $pfx = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new($pfxPath, $pwd, [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::DefaultKeySet)
    $cert = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new($pfx.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert))
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root','CurrentUser')
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    $store.Add($cert)
    $store.Close()
    Write-Output "Certificate imported into CurrentUser\Root successfully. Subject: $($cert.Subject)"
} catch {
    Write-Error $_.Exception.Message
    if ($_.Exception.InnerException) { Write-Error $_.Exception.InnerException.Message }
    exit 1
}
