# Анкета «Путешествие мечты»

Домашнее задание №1 по .NET MAUI. Приложение для заполнения анкеты о путешествии.

## Возможности

- Ввод имени, электронной почты и пожеланий.
- Выбор направления, даты, длительности и транспорта.
- Выбор занятий и необходимости местного гида.
- Проверка обязательных полей и формата электронной почты.
- Вывод сводки всех ответов и очистка анкеты.

## Запуск на Android

Нужны .NET SDK 10, Android SDK, JDK 21 и запущенный Android-эмулятор
или подключённый телефон с включённой отладкой по USB.

```sh
git clone https://github.com/kulickk/form-udv.git
cd form-udv
dotnet workload install maui-android
dotnet build TravelSurvey/TravelSurvey.csproj -t:Run -f net10.0-android -p:TargetFrameworks=net10.0-android
```

Также можно открыть `TravelSurvey.sln` в среде разработки с поддержкой .NET MAUI.

## Файлы проекта

- `TravelSurvey/MainPage.xaml` — разметка анкеты.
- `TravelSurvey/MainPage.xaml.cs` — обработчики событий, проверка и сбор ответов.
- `TravelSurvey/App.xaml` — стили приложения.
- `TravelSurvey/Resources` — изображения и значок.
- `TravelSurvey/Platforms` — файлы запуска для Android, iOS, Mac Catalyst и Windows.
- `Checks` — автоматические проверки формы.

## Проверки

```sh
dotnet run --project Checks/TravelSurvey.Checks.csproj -c Release
```

Проверки выполняются без эмулятора. Сборка и работа интерфейса дополнительно
проверены на Android 11. Другие платформы не проверялись.
