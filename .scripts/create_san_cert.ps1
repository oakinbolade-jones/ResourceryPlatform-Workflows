try {
    $PasswordPlain = 'dev_cert_2026!'
    Write-Output "Generating self-signed certificate for smartserve.ecowas.int..."
    $pwd = ConvertTo-SecureString $PasswordPlain -Force -AsPlainText

    $cert = New-SelfSignedCertificate -DnsName 'smartserve.ecowas.int','localhost' -CertStoreLocation 'Cert:\CurrentUser\My' -NotAfter (Get-Date).AddYears(5) -KeyExportPolicy Exportable -KeyAlgorithm RSA -KeyLength 2048 -FriendlyName 'aspnetapp-smartserve'

    Write-Output "Exporting PFX to $env:USERPROFILE\\.aspnet\\https\\aspnetapp.pfx"
    $target = Join-Path $env:USERPROFILE ".aspnet\https\aspnetapp.pfx"
    Export-PfxCertificate -Cert "Cert:\CurrentUser\My\$($cert.Thumbprint)" -FilePath $target -Password $pwd -Force

    Write-Output "Dumping exported PFX:"
    certutil -p $PasswordPlain -dump $target

    Write-Output "Done"
} catch {
    Write-Error $_.Exception.Message
    exit 1
}