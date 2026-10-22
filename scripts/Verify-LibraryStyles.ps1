param(
    [Parameter(Mandatory)]
    [string] $WebRoot
)

$ErrorActionPreference = 'Stop'
$relative = '_content/Assimalign.Viu.Showcase.Components/Assimalign.Viu.Showcase.Components.viu.css'
$cssPath = Join-Path $WebRoot $relative
$identity = [IO.File]::ReadAllBytes((Resolve-Path -LiteralPath $cssPath))
$css = [Text.Encoding]::UTF8.GetString($identity)
foreach ($selector in @('.card', '.counter', '.toggle', '.progress')) {
    if (-not $css.Contains($selector)) { throw "Missing $selector in $cssPath" }
}
if ($css.Contains('<Project')) { throw "Project XML found in $cssPath" }

foreach ($encoding in @('gz', 'br')) {
    $compressed = [IO.File]::OpenRead((Resolve-Path -LiteralPath "$cssPath.$encoding"))
    $decoded = [IO.MemoryStream]::new()
    $decompressor = if ($encoding -eq 'gz') {
        [IO.Compression.GZipStream]::new($compressed, [IO.Compression.CompressionMode]::Decompress)
    } else {
        [IO.Compression.BrotliStream]::new($compressed, [IO.Compression.CompressionMode]::Decompress)
    }
    try {
        $decompressor.CopyTo($decoded)
        if ([Convert]::ToBase64String($identity) -ne [Convert]::ToBase64String($decoded.ToArray())) {
            throw "$cssPath.$encoding does not decompress to the library CSS"
        }
    } finally {
        $decompressor.Dispose()
        $compressed.Dispose()
        $decoded.Dispose()
    }
}

$html = Get-Content -Raw -LiteralPath (Join-Path $WebRoot 'index.html')
if ([regex]::Matches($html, [regex]::Escape($relative)).Count -ne 1) {
    throw 'Expected exactly one injected library stylesheet link'
}
$applicationStyle = [regex]::Match($html, '<link[^>]+href="[^"/]+\.css"')
if (-not $applicationStyle.Success -or $html.IndexOf($relative) -gt $applicationStyle.Index) {
    throw 'The library stylesheet must precede the application bundle'
}
Write-Output "PASS: $WebRoot — injected library CSS ($($identity.Length) bytes); gzip and Brotli match byte for byte."
