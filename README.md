# Yıldız - Tavşan Deliği

Ormanda kaybolan tavşan "Yıldız"ı, klavyeden girilen bir senaryo ile tavşan deliğine ulaştıran .NET 8 C# Console Application.

## Çalıştırma

```bash
dotnet run --project src/Tavsan.ConsoleApp
dotnet test
```

Gereksinim: .NET 8 SDK.

## Kurallar

- Orman satranç tahtası gibidir: `4`, `8` veya `16` (tek sayı girilir). Sütunlar `A..`, satırlar yukarıdan aşağı büyüklükten `1`'e doğru numaralanır.
- Tavşan sol üst köşeden (örn. `A8`) başlar, yönü **S** (güney)dir. Tavşan deliği sağ alt köşededir (örn. `H1`).
- Engeller rastgele yerleşir. Her türden en fazla 4 adet bulunur, aynı kareye iki engel konmaz, başlangıç ve delik karesi boştur:
  - Kurt, Tilki: çarpışırsa tavşan ölür.
  - Dikenli tel: altından geçilir (Eğil).
  - Çit: üzerinden atlanır (Zıpla).
  - Tel ve çite yanlış hareketle çarpmak tavşanı öldürmez, tavşan yerinde kalır.
- Komutlar: İleri `N`, Geri `P`, Sağ `R`, Sol `L`, Zıpla `J`, Eğil `İ`. Virgülle ayrılır, büyük/küçük harf fark etmez (`i`, `I`, `ı` da Eğil olarak kabul edilir). Örnek: `N,N,L,J,N,N,İ,P,J`.

## Varsayımlar

Case metninde net olmayan noktalar için alınan kararlar:

| Konu | Karar |
| --- | --- |
| İleri / Geri | Tavşan güneye baktığı için ileri tahtada aşağı, geri yukarı gider. |
| Sağ / Sol | Tavşanın kendi yönüne göredir. Güneye bakarken sol = doğu (B sütunu yönü), sağ = batı. Örnek senaryodaki `N,N,L` bu sayede `A8 -> A6 -> B6` olarak geçerli kalır. |
| Zıpla (`J`) | İleriye 2 kare gider, aradaki çit aşılır. Aradaki karede kurt/tilki varsa çarpışma sayılır. İniş karesi kurt/tilki ise ölür, tel/çit ise yerinde kalır. |
| Eğil (`İ`) | İleriye 1 kare gider, o karedeki dikenli telin altından geçer. Çit varsa geçemez. |
| Normal hareket | Tel veya çite girilemez, yerinde kalır. Kurt/tilki karesine girilirse ölür. |
| Orman sınırı | Sınırı aşan komut yok sayılır, tavşan yerinde kalır. |
| Oyun sonu | Tavşan ölünce veya deliğe ulaşınca kalan komutlar uygulanmaz. |
| Rastgele üretim | Her tür için 0-4 adet engel konur. Delik her zaman ulaşılabilir kalacak şekilde (BFS ile kontrol edilerek) üretilir. |

## Proje yapısı

```
src/
  Tavsan.Core/          Oyun kuralları (arayüzden bağımsız)
    Domain/             Board, Position, Direction, Command, ObstacleType, StepResult
    Services/           GameEngine, MoveRules, ScenarioParser, RandomObstaclePlacer, BoardSolver
  Tavsan.ConsoleApp/    Konsol arayüzü (giriş, tahta çizimi, mesajlar)
tests/
  Tavsan.Tests/         xUnit birim testleri
```

Kurallar `Tavsan.Core` içinde tutulduğu için aynı çekirdek farklı bir arayüzle (web API, masaüstü vb.) kullanılabilir.

## Örnek çıktı

```
     A    B    C    D    E    F    G    H
 8 TAV   .    .   KRT  KRT  TEL   .    .
 ...
  1. İleri  A8 -> A7
  2. İleri  A7 -> A6
  3. Sol    A6 -> B6
  4. Zıpla  B6 -> B4
```
