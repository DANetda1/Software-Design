
# README

## 1. Общая идея решения

Данный проект реализует консольное приложение &laquo;Учет финансов&raquo; для ВШЭ-банка. Приложение позволяет:

- Создавать, редактировать и удалять:
  - **Счета** (BankAccount) с балансом.
  - **Категории** (Category) для доходов/расходов.
  - **Операции** (Operation) — доход или расход, связанные со счётом и категорией.
- Выполнять **аналитику**:
  - Узнать разницу доходов и расходов.
  - Посмотреть группировку операций по категориям.
- Делать **импорт** и **экспорт** данных в три формата: `CSV`, `JSON`, `YAML`.
  - Экспорт — сохраняем все счета, категории и операции в файл.
  - Импорт — загружаем из файла существующие счета, категории, операции.
- Использовать **меню** консольного приложения, чтобы продемонстрировать весь функционал.

Приложение организовано с учётом:

- **Принципов SOLID** и **GRASP** (High Cohesion, Low Coupling).
- **Шести паттернов GoF** (Фабрика, Фасад, Команда, Декоратор, Шаблонный метод, Посетитель).
- **DI-контейнера** (Microsoft.Extensions.DependencyInjection), чтобы собрать объекты в одном месте.

### Структура папок

- **Domain** — классы доменной модели (BankAccount, Category, Operation, Enums).
- **DomainFactories** — фабрика для создания доменных объектов (DomainFactory).
- **Facades** — фасады для CRUD-операций (BankAccountFacade, CategoryFacade, OperationFacade) и фасад AnalyticsFacade для аналитики.
- **Commands** — паттерн &laquo;Команда&raquo;: команды (CreateBankAccountCommand, CreateCategoryCommand и т.д.), декоратор для замера времени TimedCommandDecorator, и CommandInvoker.
- **Importers** — шаблонный метод для импорта: DataImporter (абстракт) и наследники (CsvDataImporter, JsonDataImporter, YamlDataImporter).
- **Exporters** — посетитель для экспорта: IExportVisitor, CsvExportVisitor, JsonExportVisitor, YamlExportVisitor.
- **Program.cs** — точка входа. Организует DI, запускает консольное меню, где пользователь взаимодействует с системой.

Внутри репозитория также есть папка **`testFiles`**, где можно хранить пример готовых файлов для проверки импорта и экспорта:

- `test.csv`
- `test.json`
- `test.yaml`

## 2. Принципы SOLID и GRASP

### SOLID

1. **Single Responsibility**  
   Каждый класс отвечает только за одну зону. Например, `BankAccountFacade` — только за CRUD по счетам, `CategoryFacade` — только за категории, `AnalyticsFacade` — только за аналитику.
2. **Open/Closed**  
   Для добавления нового формата импорта/экспорта достаточно создать нового наследника `DataImporter` или `IExportVisitor`, не меняя существующий код.
3. **Liskov Substitution**  
   Паттерн &laquo;Команда&raquo; (`ICommand`) и его реализации легко взаимозаменяемы.
4. **Interface Segregation**  
   Интерфейсы не перегружены методами. У `ICommand` есть всего один метод `Execute()`, у `IExportVisitor` — методы `Visit(...)` и `SaveToFile(...)`.
5. **Dependency Inversion**  
   Мы используем DI-контейнер (Microsoft.Extensions.DependencyInjection) и фабрику (`DomainFactory`), чтобы не &laquo;зашивать&raquo; конкретные классы в месте использования.

### GRASP

- **High Cohesion**  
  Классы не перегружены, например, `BankAccount` отвечает только за данные о счёте, а `BankAccountFacade` — за операции со счётом.
- **Low Coupling**  
  Модули разнесены по папкам (Domain, Facades, Commands, Importers, Exporters), взаимодействуют через абстракции.

## 3. Паттерны GoF

В проекте шесть паттернов:

1. **Фабрика (Factory)** — `DomainFactory`.  
   Создаёт объекты `BankAccount`, `Category`, `Operation` с централизованной валидацией.
2. **Фасад (Facade)** — `BankAccountFacade`, `CategoryFacade`, `OperationFacade`, `AnalyticsFacade`.  
   Скрывают детали хранения и операций, предоставляя упрощённый интерфейс (Create, Get, Delete).
3. **Команда (Command)** — интерфейс `ICommand`, его реализации (Create..., List..., Delete...), плюс `CommandInvoker`.  
   Легко добавлять новые сценарии, каждый оформлен как отдельная команда.
4. **Декоратор (Decorator)** — `TimedCommandDecorator`.  
   Оборачивает команды, измеряет время выполнения.
5. **Шаблонный метод (Template Method)** — `DataImporter` (абстракт) и его наследники (CsvDataImporter, JsonDataImporter, YamlDataImporter).  
   Импорт в целом одинаков, отличается только парсинг файла.
6. **Посетитель (Visitor)** — `IExportVisitor` (CsvExportVisitor, JsonExportVisitor, YamlExportVisitor).  
   Обходит объекты и формирует выходные файлы для экспорта.

## 4. Инструкция по запуску

1. **Скачайте** проект в этом репозитории.
2. **Откройте** проект в Visual Studio / Rider / VS Code.
3. **Установите** пакеты NuGet:
   - `Microsoft.Extensions.DependencyInjection`      
   - `Newtonsoft.Json`
   - `YamlDotNet`
  
  
![image](https://github.com/user-attachments/assets/b3a8815b-5a28-4805-9e79-4545b6c49bc3) ![image](https://github.com/user-attachments/assets/a91022fc-8f1e-47bf-a57d-adde8f66c3d8)

4. **Соберите** проект.
5. **Запустите** (Run). Появится консольное меню:
   - Пункты 1, 2, 3 — создание счёта, категории, операции.
   - Пункты 4, 5, 6 — просмотр соответствующих списков.
   - Пункт 7 — аналитика (разница доходов/расходов, группировка по категориям).
   - Пункт 8 — импорт из CSV/JSON/YAML.
   - Пункт 9 — экспорт в CSV/JSON/YAML.
   - Пункт 0 — выход.

### Проверка импорта/экспорта

В папке **`testFiles`** можно найти файлы (например, `example.csv`, `example.json`, `example.yaml`) для теста импорта.

- Создайте в консоли несколько счетов, категорий и операций.
- Выберите пункт 9 (Экспорт) и сохраните, например, `C:\temp\export.csv`.
- Перезапустите программу (или очистите данные).
- Выберите пункт 8 (Импорт) и укажите тот же файл `C:\temp\export.csv` — данные восстановятся.

## 5. Проверка выполнения всех пунктов

По заданию требовалось:

1. **Доменная модель** с соблюдением SOLID, GRASP, паттернов GoF — это реализовано.
2. **Консольное приложение** для демонстрации функционала — есть (меню).
3. **Отчёт** (этот README) с:  
   - Описанием функционала.  
   - Перечислением принципов SOLID/GRASP.  
   - Перечислением паттернов GoF.  
4. **Инструкция по запуску** — представлена выше.

Таким образом, все требования выполнены.
