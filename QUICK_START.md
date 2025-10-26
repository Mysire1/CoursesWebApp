# 🚀 Швидкий запуск CoursesWebApp

## ❗ Вирішення помилки 404 Not Found

Якщо ви бачите помилку "404 Not Found" при заході на сайт, виконайте ці кроки:

### Крок 1: Перевірити PostgreSQL
```bash
# Перевірити чи запущений PostgreSQL
sudo service postgresql status
# або
ps aux | grep postgres

# Якщо не запущений - запустити
sudo service postgresql start
```

### Крок 2: Створити базу даних
```bash
# Увійти в PostgreSQL
psql -U postgres

# Створити базу даних
CREATE DATABASE courseswebapp;

# Вийти
\q
```

### Крок 3: Виконати SQL-скрипт
```bash
# Якщо у вас є файл create-database.sql
psql -U postgres -d courseswebapp -f create-database.sql

# АБО скопіювати весь вміст зі згенерованого SQL файлу і вставити в консоль
```

### Крок 4: Перевірити підключення
Перед запуском додатка перевірте налаштування в `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=courseswebapp;Username=postgres;Password=YOUR_PASSWORD;"
  }
}
```

**ВАЖЛИВО:** Змініть `YOUR_PASSWORD` на ваш справжній пароль PostgreSQL!

### Крок 5: Запустити додаток
```bash
cd CoursesWebApp
dotnet restore
dotnet run
```

## 🔍 Діагностика проблем

Якщо все ще не працює, перейдіть на:
- **http://localhost:5000/Test/Simple** - простий тест
- **http://localhost:5000/Test** - повна діагностика

### Можливі проблеми:

#### 1. PostgreSQL не запущений
**Симптоми:** 
- "Cannot connect to database"
- "Connection timeout"

**Рішення:**
```bash
# Ubuntu/Debian
sudo service postgresql start

# CentOS/RHEL
sudo systemctl start postgresql

# Windows
net start postgresql-x64-13
```

#### 2. Неправильний пароль
**Симптоми:**
- "password authentication failed"

**Рішення:**
```bash
# Змінити пароль postgres користувача
sudo -u postgres psql
\password postgres
# Ввести новий пароль і оновити appsettings.json
```

#### 3. База даних не існує
**Симптоми:**
- "database courseswebapp does not exist"

**Рішення:**
```sql
CREATE DATABASE courseswebapp;
```

#### 4. Порти зайняті
**Симптоми:**
- "Address already in use"

**Рішення:**
```bash
# Перевірити які порти використовуються
netstat -tlnp | grep :5000

# Змінити порт в Properties/launchSettings.json
# або запустити з іншим портом
dotnet run --urls="http://localhost:5001"
```

#### 5. Відсутні пакети
**Симптоми:**
- Build errors
- Missing references

**Рішення:**
```bash
dotnet clean
dotnet restore
dotnet build
```

## 📋 Перевірочний чекліст

- [ ] PostgreSQL запущений і доступний
- [ ] База даних `courseswebapp` створена  
- [ ] Користувач postgres має правильний пароль
- [ ] Порт 5000/5001 вільний
- [ ] `dotnet restore` виконано успішно
- [ ] SQL скрипт заповнення виконано
- [ ] appsettings.json містить правильні дані підключення

## 🎯 Успішний запуск

Після успішного запуску ви побачите:
- Головна сторінка з статистикою (кількість студентів, мов, викладачів)
- Навігація працює
- Дані завантажуються без помилок

## 🆘 Контакти для підтримки

Якщо проблеми не вирішуються:
1. Перевірте логи в консолі `dotnet run`
2. Перейдіть на `/Test` для діагностики
3. Створіть issue в GitHub репозиторії