# 🔹 SharpVLESS — Простой VLESS-клиент на C# для Windows

<p align="center">
  <img src="https://img.shields.io/badge/version-1.0.0-blue" alt="Версия" />
  <img src="https://img.shields.io/badge/license-MIT-green" alt="Лицензия" />
  <img src="https://img.shields.io/badge/platform-Windows-blue" alt="Платформа" />
  <img src="https://img.shields.io/badge/.NET-6.0-orange" alt=".NET" />
  <img src="https://img.shields.io/github/actions/workflow/status/Trytonottry/sharpvless/build.yml?branch=main" alt="CI/CD" />
</p>

> **SharpVLESS** — легковесный, open-source VLESS-клиент для Windows с поддержкой **WebSocket, TLS, REALITY**, локальным **SOCKS5-прокси** и интуитивным интерфейсом.  
> Вдохновлён Hideify и InvisibleMan Xray, но написан полностью на чистом C# без внешних зависимостей.

---

## 🌟 Возможности

- ✅ Поддержка `vless://` ссылок (включая параметры)
- ✅ **REALITY** (с `pbk`, `sid`, `fp`)
- ✅ **WebSocket + TLS**
- ✅ Локальный **SOCKS5-прокси** на `127.0.0.1:1080`
- ✅ Сохранение последнего конфига
- ✅ Автозапуск с системой
- ✅ Автоподключение при старте
- ✅ Иконка в системном трее
- ✅ Однофайловый `.exe` (Self-contained)
- ✅ Установщик (MSI)

---

## 🚀 Установка

### Вариант 1: Готовый установщик (рекомендуется)

1. Скачайте последнюю версию с [Releases](https://github.com/Trytonottry/sharpvless/releases)
2. Запустите `SharpVLESS-Setup.exe`
3. Готово! Ярлык появится на рабочем столе.

### Вариант 2: Portable .exe

1. Скачайте `SharpVLESS.exe` из папки `publish/`
2. Запустите — работает без установки
3. Все настройки сохраняются в `config.json`

---

## 📥 Использование

1. Вставьте VLESS-ссылку:

2. Нажмите **"Подключить"**

3. Настройте приложение:
- Браузер / Система → Прокси: `SOCKS5 127.0.0.1:1080`

4. Готово! Трафик идёт через VLESS.

---

## ⚙️ Настройки

| Параметр | Описание |
|--------|--------|
| **Автозапуск подключения** | Подключается автоматически при старте |
| **Запускать с системой** | Добавляет в автозагрузку Windows |
| **Трей-иконка** | Минимизирует в трей, а не закрывает |

---

## 🧩 Поддерживаемые протоколы

| Протокол | Поддержка |
|--------|----------|
| `vless://` | ✅ Полная |
| WebSocket (ws/wss) | ✅ |
| gRPC | ❌ (планируется через xray-core) |
| REALITY | ✅ (`pbk`, `sid`, `fp`) |
| XTLS | ❌ (не поддерживается в C#) |

---

## 🛠 Технологии

- **Язык**: C# 10
- **Фреймворк**: .NET 6
- **UI**: WPF
- **Транспорт**: `ClientWebSocket`, `TcpListener`
- **Платформа**: Только Windows (из-за WPF и автозапуска)

---

## 📂 Структура проекта
SharpVLESS/
├── MainWindow.xaml       # GUI
├── VlessClientHandler.cs # Ядро подключения
├── VlessUrlParser.cs     # Парсинг vless://
├── ConfigManager.cs      # Сохранение настроек
├── AutoStartManager.cs   # Автозапуск
├── NotifyIconManager.cs  # Иконка в трее
└── Resources/            # Иконки, лого

## 📦 Сборка из исходников

```bash
git clone https://github.com/Trytonottry/sharpvless.git
cd sharpvless
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

→ Исполняемый файл:
bin/Release/net6.0-windows/win-x64/publish/SharpVLESS.exe

## 📄 Лицензия 

MIT © [TryToNotTry] 

Свободно используй, модифицируй, распространяй — с указанием авторства. 
 
## 💬 Обратная связь 

Нашли баг? Хотите фичу?
👉 Создайте Issue  или PR! 
 

    🔐 Важно: Используйте только в рамках закона. Не несём ответственности за неправомерное использование. 
     
