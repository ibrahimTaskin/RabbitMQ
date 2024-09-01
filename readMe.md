## Message Acknowledgment
RabbitMQ'daki mesaj onayı (acknowledgment), bir mesajın başarıyla alındığını ve işlendiğini RabbitMQ sunucusuna bildirme işlemidir. Bu işlem, mesajın kaybolmasını veya işlenmemesini önlemek amacıyla kullanılır ve RabbitMQ'nun mesaj güvenilirliğini sağlamak için kritik bir rol oynar.

#### Mesaj Onayı (Acknowledgment) Nasıl Çalışır?
Otomatik Onay (Auto-Acknowledgment): Bir tüketici (consumer), mesajı alır almaz RabbitMQ sunucusuna mesajı başarıyla aldığını bildirir. Bu durumda, mesajın işlenip işlenmediği kontrol edilmez. Eğer tüketici mesajı işlerken bir hata oluşursa veya tüketici çökerse, mesaj kaybolabilir.

##### Manuel Onay (Manual Acknowledgment): 
Tüketici, mesajı alır ve işleme sürecini tamamladıktan sonra RabbitMQ sunucusuna mesajı başarıyla işlediğini manuel olarak bildirir. Bu işlem basic.ack komutu ile yapılır. Eğer tüketici mesajı alır fakat işleme sırasında bir hata oluşursa, tüketici mesajı onaylamaz ve RabbitMQ mesajı tekrar kuyruğa geri koyar. Bu sayede, başka bir tüketici mesajı tekrar işleyebilir.

##### Onaylama Türleri
basic.ack: Bu komut, bir mesajın başarıyla işlendiğini belirtmek için kullanılır. RabbitMQ, bu onayı aldıktan sonra mesajı kuyruktan siler.

basic.nack: Bu komut, bir mesajın işlenemediğini ve tekrar işlenmesi gerektiğini belirtir. Tüketici bu komutu kullanarak mesajı kuyruğa geri koyabilir. requeue parametresi ile, mesajın kuyruğa geri eklenip eklenmeyeceği belirlenir.

basic.reject: basic.nack komutuna benzerdir, ancak basic.reject komutu tek bir mesaj için kullanılır ve mesajı tekrar kuyruğa koymadan doğrudan atılmasını sağlar.

##### Neden Önemlidir?
Güvenilir Mesajlaşma: Mesaj onay mekanizması, mesajların güvenilir bir şekilde işlenmesini sağlar. Eğer tüketici işleme sırasında çökerse veya hata oluşursa, mesajlar kaybolmaz; tekrar işlenmek üzere kuyruğa döner.

İşlem Garantisi: Bu mekanizma, "en az bir kez işleme" garantisi sağlar. Yani, bir mesaj en az bir kez işlenmiş olacaktır, ancak aynı mesajın birden fazla kez işlenmesi mümkündür. Bu nedenle, tüketicilerin idempotent olması önemlidir; yani, aynı mesaj birden fazla kez işlendiğinde aynı sonucu vermelidir.

RabbitMQ'da mesaj onayları, sistemin mesajları güvenilir bir şekilde işlemesini sağlamak için hayati öneme sahiptir ve bu mekanizmanın doğru şekilde kullanılması, uygulamanızın sağlamlığını artırır.

## Fair Dispatch

RabbitMQ'da Fair Dispatch, mesajların işçiler (workers) arasında adil bir şekilde dağıtılmasını sağlamak için kullanılan bir tekniktir. Bu teknik, işçilerin aşırı yüklenmesini önleyerek her bir işçinin dengeli bir şekilde çalışmasını amaçlar.

##### 1. Prefetch Count Ayarı:
RabbitMQ'da Fair Dispatch'i etkinleştirmek için kullanılan temel mekanizma prefetch count (önceden alınacak mesaj sayısı) ayarıdır. Bu ayar, bir işçinin aynı anda kaç mesaj alabileceğini belirler. İşçi, bu sayı kadar mesaj aldıktan sonra bu mesajları işleyene kadar yeni bir mesaj almaz.

Prefetch Count = 1: Bu en yaygın kullanılan ayardır. Bu durumda, her bir işçi aynı anda sadece bir mesaj alır ve bu mesajı işleyip tamamlayana kadar yeni bir mesaj almaz. Böylece, bir işçi daha uzun süren bir mesajı işlerken diğer işçiler boşta kalmaz, çünkü kuyruktaki diğer mesajlar başka işçilere gönderilir. Bu, mesajların işçilere adil bir şekilde dağıtılmasını sağlar.
##### 2. Adil Mesaj Dağıtımı:
Varsayılan round-robin dağıtımı, sıradaki işçiye yeni mesajı hemen verir. Ancak bu, bazı işçilerin daha önce aldığı mesajları işleyip bitirmeden yeni mesajlar almasına neden olabilir, bu da iş yükünün dengesiz dağılmasına yol açar.

Fair Dispatch ile, her bir işçi yalnızca işleyebileceği kadar mesaj alır. Yani bir işçi, mevcut mesajını tamamen işleyip RabbitMQ'ya onay (acknowledgment) göndermeden yeni bir mesaj almaz. Bu, daha uzun süren işlemleri olan mesajların, daha hızlı biten mesajların bulunduğu işçiler arasında adil bir şekilde dağıtılmasını sağlar.

##### Fair Dispatch'in Avantajları:
Yük Dengeleme: Mesajlar, işçilerin mevcut yük durumuna göre dağıtıldığı için her işçi benzer bir yük altında çalışır.
Verimlilik: İşçilerin aşırı yüklenmesi önlenir ve sistem genelinde daha yüksek verimlilik sağlanır.
Kuyrukta Bekleme Süresinin Azalması: Mesajların işlenme süresi daha dengeli hale gelir ve kuyrukta bekleyen mesajların işlenme süresi kısalır.
Fair Dispatch'in Etkinleştirilmesi:
Fair Dispatch'i RabbitMQ'da etkinleştirmek için tüketici tarafında basic.qos (Quality of Service) ayarı kullanılır. Örneğin:

```
C#
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
```
Bu ayar, her bir işçinin aynı anda sadece bir mesaj almasını sağlar, böylece Fair Dispatch uygulanır.

Sonuç olarak, RabbitMQ'da Fair Dispatch, mesajların işçiler arasında adil ve dengeli bir şekilde dağıtılmasını sağlayarak, sistem performansını ve verimliliğini artırır.