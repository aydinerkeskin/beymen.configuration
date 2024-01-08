Tüm projelerin referans aldığı ve ayar bilgilerini yönetmek için kullandığı proje [Beymen.Core] dur. Öncelikle bu projenin derlenmesi gerekiyor.
[Beymen.Test] üzerinden foksiyoneliteyi test edebilirsiniz.
2 api içerisinde mongo ve redis ayarlarını appsetting içerisine ekledim, bu değerleri güncellemek isteyebilirsiniz.

[Beymen.Configuration.ReadAPI] projesini değişen eklenen ayarın kullanımını göstermek için ekledim.

[Beymen.Configuration.Managment] projesi UI uygulamasına hizmet edecek ve mongo dan listeleme, ekleme ve güncelleme yapacaktır.

[Beymen.Configuration.UI] react ile geliştirdim. Projeyi indirdikten sonra "npm install" ile paketlerini yükleyip sonrasında "npm start" ile başlatabilirsiniz. Management Api sinin adresi proje içerisinde gömülü ve "http://localhost:5000/" ile başlıyor, isterseniz değiştirebilirsiniz.
