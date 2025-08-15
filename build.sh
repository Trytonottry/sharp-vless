#!/bin/bash

# build.sh — Скрипт для сборки sharp-vless.exe на Linux
# Требует: .NET SDK 6.0 или выше

set -e  # Прерывать выполнение при ошибке

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}🚀 Сборка sharp-vless.exe для Windows${NC}"

# --- Проверка 1: Установлен ли dotnet ---
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ Ошибка: dotnet не установлен.${NC}"
    echo "Установи .NET SDK 6.0 или выше: https://dotnet.microsoft.com/download"
    exit 1
fi

echo -e "${GREEN}✅ dotnet найден$(dotnet --version)${NC}"

# --- Проверка 2: Есть ли VlessClientApp.csproj ---
if [ ! -f "VlessClientApp.csproj" ]; then
    echo -e "${RED}❌ Ошибка: Не найден VlessClientApp.csproj${NC}"
    echo "Убедись, что ты в корне проекта sharp-vless"
    exit 1
fi

echo -e "${GREEN}✅ Найден VlessClientApp.csproj${NC}"

# --- Проверка 3: Есть ли App.xaml ---
if [ ! -f "App.xaml" ]; then
    echo -e "${YELLOW}⚠️  Внимание: Нет App.xaml — возможно, проект не WPF?${NC}"
    read -p "Продолжить сборку? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

# --- Проверка 4: Есть ли MainWindow.xaml ---
if [ ! -f "MainWindow.xaml" ]; then
    echo -e "${YELLOW}⚠️  Внимание: Нет MainWindow.xaml — возможно, GUI отсутствует?${NC}"
    read -p "Продолжить сборку? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

# --- Очистка старых сборок ---
echo -e "${GREEN}🧹 Очищаем старую папку ./publish...${NC}"
rm -rf ./publish

# --- Восстановление зависимостей ---
echo -e "${GREEN}📦 Восстанавливаем зависимости...${NC}"
dotnet restore VlessClientApp.csproj -p:EnableWindowsTargeting=true || {
    echo -e "${RED}❌ Ошибка при восстановлении зависимостей${NC}"
    exit 1
}

# --- Сборка ---
echo -e "${GREEN}🔨 Собираем self-contained .exe для Windows...${NC}"
dotnet publish VlessClientApp.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:EnableWindowsTargeting=true \
  -o ./publish || {
    echo -e "${RED}❌ Ошибка при сборке${NC}"
    exit 1
}

# --- Проверка результата ---
if [ -f "./publish/sharp-vless.exe" ]; then
    echo -e "${GREEN}✅ Сборка успешна!${NC}"
    echo -e "${GREEN}🎉 EXE готов: ./publish/sharp-vless.exe${NC}"
else
    echo -e "${RED}❌ Ошибка: Файл sharp-vless.exe не создан${NC}"
    exit 1
fi

# --- Архивация ---
echo -e "${GREEN}📦 Создаём архив...${NC}"
cd ./publish && zip -r ../sharp-vless-windows-x64.zip ./* && cd ..
echo -e "${GREEN}✅ Архив готов: sharp-vless-windows-x64.zip${NC}"

# --- Готово ---
echo -e "\n${GREEN}🎉 Готово! Можешь отправить sharp-vless-windows-x64.zip на Windows и запустить.${NC}"