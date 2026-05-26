# ✨ Космическая ложа
## Салон красоты & Магазин косметики — WPF .NET Framework 4.7.2

---

## 🚀 Быстрый старт

### 1. Создать базу данных

Открыть **SQL Server Management Studio** (или Azure Data Studio), подключиться к `(localdb)\MSSQLLocalDB` и выполнить скрипт:

```
database_script.sql
```

Скрипт создаёт БД `KosmicheskayaLozha` и заполняет её тестовыми данными.

---

### 2. Открыть проект

Открыть файл `KosmicheskayaLozha.sln` в **Visual Studio 2019/2022**.

---

### 3. Строка подключения

По умолчанию используется:
```
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=KosmicheskayaLozha;Integrated Security=True
```

Если ваш SQL Server называется иначе — отредактируйте `App.config`:
```xml
<add name="KosmicheskayaLozha"
     connectionString="Data Source=ВАШ_СЕРВЕР;Initial Catalog=KosmicheskayaLozha;Integrated Security=True"
     providerName="System.Data.SqlClient" />
```

---

### 4. Запустить проект

Нажать **F5** или **Ctrl+F5** в Visual Studio.

---

## 👤 Тестовые аккаунты

| Логин      | Пароль      | Роль          |
|------------|-------------|---------------|
| admin      | admin123    | Администратор |
| manager1   | manager123  | Менеджер      |
| master1    | master123   | Мастер        |
| master2    | master123   | Мастер        |
| client1    | client123   | Клиент        |
| client2    | client123   | Клиент        |

---

## 📋 Возможности по ролям

### 🧑 Клиент
- Записаться на услугу (с подтверждением)
- Заказать косметику через корзину
- Просматривать историю записей и заказов

### 👩‍🎨 Мастер
- Просматривать свои записи
- Отмечать записи как выполненные
- Управлять списком своих услуг

### 🗂 Менеджер
- Создавать / переносить / отменять записи
- Закрывать заказы при выдаче
- Добавлять, изменять, замораживать товары и скидки
- Управлять производителями, типами товаров и услуг

### 🛡 Администратор
- Добавлять и редактировать пользователей
- Изменять роли и замораживать аккаунты

---

## 🗂 Структура проекта

```
KosmicheskayaLozha/
├── App.xaml / App.xaml.cs          — точка входа, ресурсы/стили
├── MainWindow.xaml / .cs           — фрейм навигации
├── Session.cs                      — текущий пользователь
├── App.config                      — строка подключения к БД
│
├── Models/
│   ├── User.cs
│   ├── ServiceType.cs              (+ Manufacturer, ProductType)
│   ├── Appointment.cs
│   └── Product.cs                  (+ Order, OrderItem, CartItem)
│
├── Data/
│   └── DatabaseHelper.cs           — все SQL-запросы
│
├── Converters/
│   └── DiscountColorConverter.cs   — конвертеры для XAML-биндинга
│
├── Pages/
│   ├── StartPage          — главная с фильтрами и слотами
│   ├── LoginPage          — авторизация
│   ├── AccountPage        — история клиента
│   ├── AppointmentsPage   — список слотов с фильтром по дате
│   ├── AppointmentDetailPage — страница записи с подтверждением
│   ├── ProductsPage       — каталог товаров (сетка)
│   ├── CartPage           — корзина
│   ├── MasterPage         — кабинет мастера
│   ├── AppointmentInfoPage — детали записи для мастера
│   ├── ManagerPage        — панель менеджера (6 вкладок)
│   └── AdminPage          — панель администратора
│
└── Windows/
    ├── ProductDetailWindow  — всплывающее окно товара
    ├── OrderWindow          — оформление заказа
    ├── EditProductWindow    — добавить/изменить товар
    ├── EditUserWindow       — добавить/изменить пользователя
    ├── SimpleInputDialog    — универсальный ввод строки
    ├── EditServiceTypeDialog — добавить/изменить тип услуги
    ├── CreateAppointmentDialog — создать запись (менеджер)
    └── RescheduleDateDialog — перенести запись
```

---

## 🎨 Дизайн

Тема «Космос» — фиолетово-тёмная палитра:
- Фон: `#1A0030` (тёмно-фиолетовый)
- Поверхность: `#2D0050`
- Акцент: `#C77DFF` (сиреневый)
- Товары со скидкой >15% подсвечиваются оранжевой рамкой

---

*Проект выполнен по заданию ПР 17 (67). «Косметическая лажа» — ВВГ одобряет* 😄
