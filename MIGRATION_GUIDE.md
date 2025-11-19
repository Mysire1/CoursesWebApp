# Посібник по міграції

## Останні оновлення (19.11.2025)

### Що змінилося:

1. **LastLoginAt тепер nullable** (`DateTime?`)
   - Дозволяє null значення для користувачів, які ще не заходили в систему

2. **Додано поле Phone до реєстрації**
   - Обов'язкове поле при реєстрації
   - Валідація формату телефону

3. **Виправлено Access Denied для студентів**
   - Тепер використовується `ClaimTypes.Email` замість `User.Identity.Name`

---

## Як мігрувати

### Крок 1: Створіть резервну копію

```bash
pg_dump -U postgres courseswebapp > backup_$(date +%Y%m%d_%H%M%S).sql
```

### Крок 2: Запустіть міграційний скрипт

```bash
psql -U postgres -d courseswebapp -f Migrations/FixLastLoginAtNullable.sql
```

### Крок 3: Перезапустіть додаток

```bash
dotnet build
dotnet run
```

---

## Що робить міграційний скрипт:

1. Дозволяє NULL значення для `LastLoginAt` в Students і Teachers
2. Очищає некоректні дати (менше 2000-01-01)

---

## Перевірка після міграції

Виконайте в psql:

```sql
-- Перевірка Students
SELECT 
    "StudentId", 
    "Email", 
    "Phone",
    "LastLoginAt"
FROM "Students" 
LIMIT 5;

-- Перевірка Teachers
SELECT 
    "TeacherId", 
    "Email", 
    "Phone",
    "LastLoginAt"
FROM "Teachers" 
LIMIT 5;
```

---

## Проблеми та рішення

### Помилка: "Column 'LastLoginAt' is null"

**Рішення:**
```bash
psql -U postgres -d courseswebapp -f Migrations/FixLastLoginAtNullable.sql
```

### Помилка: "Access Denied" для студента

**Причина:** Старий код використовував `User.Identity.Name`  
**Рішення:** Оновлено код, перезапустіть додаток

### Помилка: "Phone is required" при реєстрації

**Причина:** Поле Phone тепер обов'язкове  
**Рішення:** Вкажіть номер телефону при реєстрації

---

## Контакти

Якщо виникли проблеми після міграції:
1. Перевірте логи додатку
2. Відновіть з резервної копії якщо потрібно
3. Створіть issue в GitHub