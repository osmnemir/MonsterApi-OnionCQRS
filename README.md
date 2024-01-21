
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
</head>
<body>

<h1>ASP.NET Core MediatR ve JWT ile API Geliştirme Projesi</h1>

<p>Bu proje, ASP.NET Core kullanarak geliştirilmiş bir API örneğidir. Proje, <a href="https://github.com/jbogard/MediatR">MediatR</a> kütüphanesini kullanarak CQRS tasarım desenini uygular ve JWT (JSON Web Tokens) ile kimlik doğrulama ve yetkilendirme sağlar.</p>

<h2>Özellikler</h2>

<ul>
    <li><strong>CQRS Tasarım Deseni:</strong> MediatR kullanılarak Command ve Query işlemleri ayrı sınıflar ile yönetilir.</li>
    <li><strong>JWT ile Kimlik Doğrulama:</strong> JSON Web Tokens kullanılarak güvenli kimlik doğrulama ve yetkilendirme işlemleri gerçekleştirilir.</li>
    <li><strong>FluentValidation ile Veri Doğrulama:</strong> Gelen verilerin doğruluğu FluentValidation kütüphanesi ile kontrol edilir.</li>
    <li><strong>Redis ile Önbellekleme:</strong> Redis kullanılarak veriler önbellekleme (caching) işlemine tabi tutulur.</li>
    <li><strong>Global Exception Handling:</strong> Projenin her seviyesinde oluşan hatalar global bir exception handler ile ele alınır.</li>
</ul>

<h2>Nasıl Kullanılır?</h2>

<ol>
    <li>Proje dizininde terminal veya komut istemcisini açın.</li>
    <li><code>dotnet build</code> komutu ile projeyi derleyin.</li>
    <li><code>dotnet run</code> komutu ile projeyi başlatın.</li>
    <li>API endpoint'lerine <a href="http://localhost:5000">http://localhost:5000</a> üzerinden erişebilirsiniz.</li>
</ol>

<h2>Gereksinimler</h2>

<ul>
    <li><a href="https://dotnet.microsoft.com/download/dotnet/5.0">.NET 7 SDK</a></li>
    <li><a href="https://redis.io/download">Redis Server</a></li>
    <li><a href="https://www.postman.com/">Postman</a></li>
</ul>
<h2>Proje Dosyası Detayları</h2>
<a href="https://dosya.co/ddo7ixn85629/MONSTER.API.docx.html" target="_blank">Download</a>


<h2>Katkı ve Geri Bildirim</h2>

<p>Bu proje açık kaynaklıdır. Her türlü katkı, öneri ve geri bildirimi memnuniyetle karşılarız. Lütfen bir <a href="https://github.com/yourusername/yourrepository/issues">issue</a> açarak veya bir <a href="https://github.com/yourusername/yourrepository/pulls">pull request</a> göndererek katkıda bulunun.</p>

</body>
</html>
