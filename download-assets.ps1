$ErrorActionPreference = "Continue"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path

$images = Join-Path $root "wwwroot\images"
$products = Join-Path $images "products"
$banners = Join-Path $images "banners"
$brands = Join-Path $images "brands"
$categories = Join-Path $images "categories"

New-Item -ItemType Directory -Force -Path $products | Out-Null
New-Item -ItemType Directory -Force -Path $banners | Out-Null
New-Item -ItemType Directory -Force -Path $brands | Out-Null
New-Item -ItemType Directory -Force -Path $categories | Out-Null


$userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/151 Safari/537.36"


function Download-Url
{
    param
    (
        [string]$Url,
        [string]$Destination
    )

    try
    {
        if (Test-Path $Destination)
        {
            $existing = Get-Item $Destination

            if ($existing.Length -gt 1000)
            {
                return $true
            }
        }

        & curl.exe `
            -L `
            -sS `
            --retry 2 `
            --connect-timeout 15 `
            --max-time 40 `
            -A $userAgent `
            -o $Destination `
            $Url

        if (Test-Path $Destination)
        {
            $file = Get-Item $Destination

            if ($file.Length -gt 1000)
            {
                return $true
            }
        }

        if (Test-Path $Destination)
        {
            Remove-Item $Destination -Force
        }

        return $false
    }
    catch
    {
        return $false
    }
}


function Find-GameStop-Image
{
    param
    (
        [string]$Query
    )

    try
    {
        $encoded = [Uri]::EscapeDataString($Query)

        $url = "https://www.gamestop.com/search/?q=$encoded"

        $html = & curl.exe `
            -L `
            -sS `
            --retry 2 `
            --connect-timeout 15 `
            --max-time 40 `
            -A $userAgent `
            $url

        $html = ($html -join "`n")

        if ([string]::IsNullOrWhiteSpace($html))
        {
            return $null
        }

        $html = [System.Net.WebUtility]::HtmlDecode($html)

        $html = $html.Replace("\/", "/")


        $pattern =
            'https://media\.gamestop\.com/i/gamestop/[A-Za-z0-9_\-./?=&%]+'

        $matches =
            [regex]::Matches(
                $html,
                $pattern,
                [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
            )


        foreach ($match in $matches)
        {
            $image = $match.Value

            if (
                $image -notmatch "logo" -and
                $image -notmatch "icon" -and
                $image -notmatch "badge"
            )
            {
                return $image
            }
        }

        return $null
    }
    catch
    {
        return $null
    }
}


function Download-SearchProduct
{
    param
    (
        [string]$FileName,
        [string]$Query
    )

    $destination =
        Join-Path $products $FileName


    if (Test-Path $destination)
    {
        $existing =
            Get-Item $destination

        if ($existing.Length -gt 1000)
        {
            Write-Host "YA EXISTE: $FileName" -ForegroundColor DarkGray
            return $true
        }
    }


    Write-Host "BUSCANDO: $Query" -ForegroundColor Yellow


    $image =
        Find-GameStop-Image `
            -Query $Query


    if ([string]::IsNullOrWhiteSpace($image))
    {
        Write-Host "NO ENCONTRADO: $FileName" -ForegroundColor Red
        return $false
    }


    $result =
        Download-Url `
            -Url $image `
            -Destination $destination


    if ($result)
    {
        Write-Host "OK: $FileName" -ForegroundColor Green
        return $true
    }


    Write-Host "ERROR: $FileName" -ForegroundColor Red

    return $false
}


function Get-OgImage
{
    param
    (
        [string]$PageUrl
    )

    try
    {
        $html = & curl.exe `
            -L `
            -sS `
            --retry 2 `
            --connect-timeout 15 `
            --max-time 40 `
            -A $userAgent `
            $PageUrl

        $html = ($html -join "`n")

        if ([string]::IsNullOrWhiteSpace($html))
        {
            return $null
        }

        $html = [System.Net.WebUtility]::HtmlDecode($html)


        $pattern1 =
            '<meta[^>]+property=["'']og:image["''][^>]+content=["'']([^"'']+)["'']'

        $match =
            [regex]::Match(
                $html,
                $pattern1,
                [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
            )


        if ($match.Success)
        {
            return $match.Groups[1].Value
        }


        $pattern2 =
            '<meta[^>]+content=["'']([^"'']+)["''][^>]+property=["'']og:image["'']'

        $match =
            [regex]::Match(
                $html,
                $pattern2,
                [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
            )


        if ($match.Success)
        {
            return $match.Groups[1].Value
        }


        return $null
    }
    catch
    {
        return $null
    }
}


Write-Host ""
Write-Host "========================================================" -ForegroundColor Cyan
Write-Host " BIGGAME - LOTE FINAL DE RECURSOS" -ForegroundColor Cyan
Write-Host "========================================================" -ForegroundColor Cyan
Write-Host ""


# ======================================================
# PRODUCTOS
# ======================================================

$productAssets = @(

    @{
        File = "yugioh-magnificent-monsters.jpg"
        Query = "Yu-Gi-Oh Magnificent Monsters Booster Box"
    },

    @{
        File = "pokemon-destined-rivals.jpg"
        Query = "Pokemon Destined Rivals Elite Trainer Box"
    },

    @{
        File = "zelda-ocarina-switch2.jpg"
        Query = "The Legend of Zelda Nintendo Switch 2"
    },

    @{
        File = "final-fantasy-vii.jpg"
        Query = "Final Fantasy VII Rebirth Deluxe Edition PlayStation 5"
    },

    @{
        File = "gta6-ps5.jpg"
        Query = "Grand Theft Auto VI PlayStation 5"
    },

    @{
        File = "star-wars-zero-company.jpg"
        Query = "Star Wars Zero Company"
    },

    @{
        File = "ps5-slim.jpg"
        Query = "Sony PlayStation 5 Slim Console"
    },

    @{
        File = "dualsense-white.jpg"
        Query = "Sony DualSense Wireless Controller White"
    },

    @{
        File = "dualsense-pink.jpg"
        Query = "Sony DualSense Nova Pink"
    },

    @{
        File = "dualsense-edge.jpg"
        Query = "Sony DualSense Edge Wireless Controller"
    },

    @{
        File = "xbox-series-x.jpg"
        Query = "Xbox Series X Console"
    },

    @{
        File = "xbox-controller.jpg"
        Query = "Microsoft Xbox Wireless Controller"
    },

    @{
        File = "turtle-beach-stealth-700.jpg"
        Query = "Turtle Beach Stealth 700 Gen 3"
    },

    @{
        File = "switch-oled.jpg"
        Query = "Nintendo Switch OLED Console"
    },

    @{
        File = "switch2.jpg"
        Query = "Nintendo Switch 2 Console"
    },

    @{
        File = "gaming-keyboard.jpg"
        Query = "Gaming Mechanical Keyboard RGB"
    },

    @{
        File = "gaming-headset.jpg"
        Query = "Gaming Headset"
    },

    @{
        File = "portable-ssd.jpg"
        Query = "Portable Gaming SSD 1TB"
    },

    @{
        File = "invincible-thragg.jpg"
        Query = "Invincible Thragg Action Figure"
    },

    @{
        File = "marvel-punisher.jpg"
        Query = "Marvel Legends Punisher"
    },

    @{
        File = "funko-woody-buzz.jpg"
        Query = "Funko Pop Woody Buzz Toy Story"
    },

    @{
        File = "pomni-plush.jpg"
        Query = "Amazing Digital Circus Pomni Plush"
    },

    @{
        File = "playstation-gift-card.jpg"
        Query = "PlayStation Store Gift Card"
    },

    @{
        File = "xbox-gift-card.jpg"
        Query = "Xbox Gift Card"
    },

    @{
        File = "nintendo-gift-card.jpg"
        Query = "Nintendo eShop Gift Card"
    }
)


$productSuccess = 0
$productErrors = 0


foreach ($asset in $productAssets)
{
    $result =
        Download-SearchProduct `
            -FileName $asset.File `
            -Query $asset.Query

    if ($result)
    {
        $productSuccess++
    }
    else
    {
        $productErrors++
    }
}


# ======================================================
# BANNERS DE LAS PROMOCIONES ACTUALES
# ======================================================

Write-Host ""
Write-Host "DESCARGANDO BANNERS..." -ForegroundColor Cyan
Write-Host ""


$bannerAssets = @(

    @{
        File = "onimusha-banner.jpg"
        Page = "https://www.gamestop.com/video-games/nintendo-switch-2/products/onimusha-way-of-the-sword---nintendo-switch-2-game-key-card/447762.html"
    },

    @{
        File = "fire-emblem-banner.jpg"
        Page = "https://www.gamestop.com/video-games/nintendo-switch-2/products/fire-emblem-fortunes-weave---nintendo-switch-2/447540.html"
    },

    @{
        File = "wolverine-banner.jpg"
        Page = "https://www.gamestop.com/video-games/playstation-5/products/marvels-wolverine---playstation-5/447197.html"
    },

    @{
        File = "marvel-tokon-banner.jpg"
        Page = "https://www.gamestop.com/links/marvel-tokon-fighting-souls"
    },

    @{
        File = "nba2k27-banner.jpg"
        Page = "https://www.gamestop.com/links/nba-2k27"
    },

    @{
        File = "upcoming-banner.jpg"
        Page = "https://www.gamestop.com/links/microsoft-new-and-upcoming-games"
    }
)


$bannerSuccess = 0
$bannerErrors = 0


foreach ($asset in $bannerAssets)
{
    Write-Host "BANNER: $($asset.File)" -ForegroundColor Yellow

    $destination =
        Join-Path $banners $asset.File


    $image =
        Get-OgImage `
            -PageUrl $asset.Page


    if ([string]::IsNullOrWhiteSpace($image))
    {
        Write-Host "NO SE ENCONTRO IMAGEN" -ForegroundColor Red
        $bannerErrors++
        continue
    }


    $result =
        Download-Url `
            -Url $image `
            -Destination $destination


    if ($result)
    {
        Write-Host "OK" -ForegroundColor Green
        $bannerSuccess++
    }
    else
    {
        Write-Host "ERROR" -ForegroundColor Red
        $bannerErrors++
    }
}


# ======================================================
# IMAGENES REPRESENTATIVAS PARA CATEGORIAS
# ======================================================

Write-Host ""
Write-Host "PREPARANDO CATEGORIAS..." -ForegroundColor Cyan
Write-Host ""


$categoryCopies = @(

    @{
        Source = "pokemon-30th.jpg"
        Target = "trading-cards.jpg"
    },

    @{
        Source = "wolverine-ps5.jpg"
        Target = "video-games.jpg"
    },

    @{
        Source = "switch2.jpg"
        Target = "consoles.jpg"
    },

    @{
        Source = "dualsense-white.jpg"
        Target = "controllers.jpg"
    },

    @{
        Source = "gaming-keyboard.jpg"
        Target = "pc-gaming.jpg"
    },

    @{
        Source = "gaming-headset.jpg"
        Target = "headsets.jpg"
    },

    @{
        Source = "funko-woody-buzz.jpg"
        Target = "collectibles.jpg"
    },

    @{
        Source = "nintendo-gift-card.jpg"
        Target = "digital-store.jpg"
    }
)


$categorySuccess = 0


foreach ($item in $categoryCopies)
{
    $source =
        Join-Path $products $item.Source

    $target =
        Join-Path $categories $item.Target


    if (Test-Path $source)
    {
        Copy-Item `
            -Path $source `
            -Destination $target `
            -Force

        Write-Host "OK: $($item.Target)" -ForegroundColor Green

        $categorySuccess++
    }
}


# ======================================================
# REPORTE FINAL
# ======================================================

$report =
@"

BIGGAME
LOTE FINAL DE RECURSOS

PRODUCTOS
Correctos: $productSuccess
Errores: $productErrors

BANNERS
Correctos: $bannerSuccess
Errores: $bannerErrors

CATEGORIAS
Preparadas: $categorySuccess

Productos:
$products

Banners:
$banners

Categorias:
$categories

"@


$reportPath =
    Join-Path $images "asset-report.txt"


Set-Content `
    -Path $reportPath `
    -Value $report `
    -Encoding UTF8


Write-Host ""
Write-Host "========================================================" -ForegroundColor Cyan
Write-Host " LOTE FINAL TERMINADO" -ForegroundColor Cyan
Write-Host "========================================================" -ForegroundColor Cyan

Write-Host ""

Write-Host "PRODUCTOS" -ForegroundColor White
Write-Host "Correctos: $productSuccess" -ForegroundColor Green
Write-Host "Errores:   $productErrors" -ForegroundColor Red

Write-Host ""

Write-Host "BANNERS" -ForegroundColor White
Write-Host "Correctos: $bannerSuccess" -ForegroundColor Green
Write-Host "Errores:   $bannerErrors" -ForegroundColor Red

Write-Host ""

Write-Host "CATEGORIAS" -ForegroundColor White
Write-Host "Preparadas: $categorySuccess" -ForegroundColor Green

Write-Host ""

Write-Host "Reporte:" -ForegroundColor White
Write-Host $reportPath -ForegroundColor Gray

Write-Host ""