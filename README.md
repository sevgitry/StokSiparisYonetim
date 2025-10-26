# Stok ve Sipariş Yönetim Sistemi

Bu proje, ASP.NET Core 9 Web API kullanılarak geliştirilmiş bir Stok ve Sipariş Yönetim Sistemidir. Sistem, ürün stok yönetimi, müşteri yönetimi, sipariş işlemleri ve raporlama özelliklerini içermektedir.

## 🛠️ Kullanılan Teknolojiler

- **ASP.NET Core 9** - Web API framework
- **Entity Framework Core** - ORM (Code-First yaklaşımı)
- **SQL Server** - Veritabanı
- **AutoMapper** - DTO ve Entity dönüşümleri
- **FluentValidation** - Request validasyonu
- **JWT Authentication** - Kimlik doğrulama ve yetkilendirme

## 📋 Proje Yapısı

```
StockOrderManagement/
├── BusinessLayer/
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── CartService.cs
│   │   ├── CategoryService.cs
│   │   ├── CustomerService.cs
│   │   ├── OrderService.cs
│   │   ├── ProductService.cs
│   │   └── UserService.cs
├── DataAccessLayer/
│   ├── Migrations/
│   └── ApplicationDbContext.cs
├── EntityLayer/
│   │   ├── Category.cs
│   │   ├── Customer.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   ├── Product.cs
│   │   ├── Role.cs
│   │   ├── Sales.cs
│   │   └── User.cs
│   ├── DTOs/
│   │   ├── Category/
│   │   ├── Customer/
│   │   ├── Order/
│   │   ├── Product/
│   │   └── User/
│   └── Enums/
│   │   ├── StatusHelper.cs
├── WebAPI/
│   ├── Controllers/
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs
│   └── Program.cs
└── StokSiparisYonetim/
    ├── Controllers/
    ├── Models/
    └── Views/
```

## 🚀 Kurulum Adımları

### 1. Gereksinimler
- .NET 9 SDK
- SQL Server 
- Visual Studio 2022 

### 2. Projeyi İndirme ve Derleme
```bash
git clone <https://github.com/sevgitry/StokSiparisYonetim>
cd StokSiparisYonetim
dotnet restore
dotnet build
```

### 3. Veritabanı Yapılandırması

**appsettings.json** dosyasında connection string'i güncelleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-EQOD4HE;Database=StokYonetim;Trusted_Connection=True;TrustServerCertificate=True;"
  },
```



### 4. Migration İşlemleri

```bash
# Migration oluşturma
dotnet ef migrations add yeni --project DataAccessLayer --startup-project DataAccessLayer

# Veritabanı güncelleme
dotnet ef database update --project DataAccessLayer --startup-project DataAccessLayer

```
### 5. Seed Data (Test Verileri)

Proje ilk çalıştırmada otomatik olarak test verileri oluşturacaktır:
- Admin kullanıcı
- Test kullanıcıları
- Örnek kategoriler ve ürünler

### 6. Projeyi Çalıştırma

```bash
dotnet run --project StokYonetim
```


## 🔐 Authentication ve Roller

Sistem iki farklı rolü desteklemektedir:

- **Admin**: Tüm işlemlere erişim
- **User**: Sınırlı işlemlere erişim

### Varsayılan Kullanıcılar

| Email | Password | Role |
|-------|----------|------|
| admin@admin.com | admin123! | Admin |
| sevgi@sevgi.com | sevgi123! | User |

## 📚 API Endpoint'leri

### 🔐 Authentication Endpoints

- `POST /api/auth/register` - Yeni kullanıcı kaydı
- `POST /api/auth/login` - Kullanıcı girişi (JWT token döner)
- `POST /api/auth/refresh-token` - Token yenileme

### 👥 User Management (Admin Only)

- `GET /api/users` - Tüm kullanıcıları listele
- `GET /api/users/{id}` - Kullanıcı detayları
- `POST /api/users` - Yeni kullanıcı oluştur
- `PUT /api/users/{id}` - Kullanıcı güncelle
- `DELETE /api/users/{id}` - Kullanıcı sil

### 📦 Product Management (Admin Only)

- `GET /api/products` - Tüm ürünleri listele 
- `GET /api/products/{id}` - Ürün detayları
- `POST /api/products` - Yeni ürün oluştur
- `PUT /api/products/{id}` - Ürün güncelle
- `DELETE /api/products/{id}` - Ürün sil
- `GET /api/products/search?name={name}` - Ürün arama

### 🏷️ Category Management (Admin Only)

- `GET /api/categories` - Tüm kategorileri listele
- `GET /api/categories/{id}` - Kategori detayları
- `POST /api/categories` - Yeni kategori oluştur
- `PUT /api/categories/{id}` - Kategori güncelle
- `DELETE /api/categories/{id}` - Kategori sil

### 👤 Customer Management (Admin Only)

- `GET /api/customers` - Tüm müşterileri listele 
- `GET /api/customers/{id}` - Müşteri detayları
- `POST /api/customers` - Yeni müşteri oluştur
- `PUT /api/customers/{id}` - Müşteri güncelle
- `DELETE /api/customers/{id}` - Müşteri sil
- `GET /api/customers/search?name={name}` - Müşteri arama

### 🛒 Order Management

- `GET /api/orders` - Siparişleri listele 
- `GET /api/orders/{id}` - Sipariş detayları
- `POST /api/orders` - Yeni sipariş oluştur (User ve Admin)
- `PUT /api/orders/{id}/approve` - Sipariş onayla (Admin Only)
- `PUT /api/orders/{id}/cancel` - Sipariş iptal et (Admin Only)
- `PUT /api/orders/{id}` - Sipariş güncelle

### 🛍️ Cart Management

- `GET /api/cart` - Kullanıcının sepetini getir
- `POST /api/cart` - Sepete ürün ekle
- `PUT /api/cart/{itemId}` - Sepet öğesini güncelle
- `DELETE /api/cart/{itemId}` - Sepetten ürün çıkar

### 📊 Reporting (Admin Only)

- `GET /api/reports/stock` - Stok durumu raporu
- `GET /api/reports/sales?startDate={}&endDate={}` - Satış raporu (tarih aralığına göre)

## ⚙️ Özellikler

### Stok Yönetimi
- Otomatik stok takibi
- Sipariş onaylandığında stok düşme
- Sipariş iptalinde stok iadesi

### Sipariş Durumları
- **Draft**: Taslak sipariş
- **Approved**: Onaylanmış sipariş
- **Cancelled**: İptal edilmiş sipariş

### Güvenlik
- JWT tabanlı kimlik doğrulama
- Rol bazlı erişim kontrolü
- Password hashing

### Validasyon
- FluentValidation ile request validasyonu
- Global exception handling
- Concurrent update kontrolü (RowVersion)



## 🐛 Hata Yönetimi

- Global exception middleware ile merkezi hata yönetimi
- 409 Conflict hatası eşzamanlı güncelleme çakışmalarında
- Detaylı hata mesajları (development ortamında)

## 📝 Notlar

- Proje Code-First yaklaşımı ile geliştirilmiştir
- Tüm entity'lerde soft delete desteği mevcut
- API response'ları standart formatta dönmektedir
- Loglama ve monitoring için gerekli altyapı hazırdır

