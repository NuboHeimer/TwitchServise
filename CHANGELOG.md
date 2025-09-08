# Changelog - История изменений TwitchService

Все значимые изменения в проекте документируются в этом файле.

Формат основан на [Keep a Changelog](https://keepachangelog.com/ru/1.0.0/),
и проект следует [Semantic Versioning](https://semver.org/lang/ru/).

## [Unreleased]

### Добавлено

- Функция `GetPresentViewersNameList`
- Заготовка инструкции по установке `docs/INSTALLATION.md`

### Изменено

- Рефактор `GetNewViewers`; переименование `GetViewersCount` → `GetPresentViewersCount`
- Рефакторинг нейминга переменной
- Обновлён `README.md`
- Обновлён файл экспорта для streamer.bot

### Удалено

- Удалены лишние методы

## [1.1.0] - 2025-01-09

### Добавлено

- Новый модуль.

## [1.0.3] - 2024-09-09

### Добавлено

- Файл для импорта в streamer.bot

### Изменено

- Обновлён `README.md`

## Формат версионирования

- **MAJOR.MINOR.PATCH**
- **MAJOR** - несовместимые изменения API
- **MINOR** - новая функциональность (обратно совместимо)
- **PATCH** - исправления багов (обратно совместимо)