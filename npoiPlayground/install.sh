#!/bin/bash

# 更新系統套件
sudo apt-get update -y
sudo apt-get upgrade -y

# 安裝必要的相依套件
sudo apt-get install -y \
    build-essential \
    libc6-dev \
    libicu-dev \
    wget \
    zip \
    unzip

# 安裝 .NET SDK
# 從 Microsoft 取得最新的 .NET 版本
DOTNET_VERSION=8.0 # 或您需要的版本
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh -c $DOTNET_VERSION -InstallDir /usr/share/dotnet

# 設定環境變數 (為了讓 dotnet 指令可以在任何地方執行)
echo "export DOTNET_ROOT=/usr/share/dotnet" >> ~/.bashrc
echo "export PATH=\$PATH:/usr/share/dotnet" >> ~/.bashrc
source ~/.bashrc

# 驗證 .NET 安裝
dotnet --info

# 安裝 Git
sudo apt-get install -y git

# 範例：下載您的專案 (假設放在 GitHub)
# 如果您已經將程式碼複製到 EC2 執行個體，則不需要執行此步驟。
# git clone <您的 Git 儲存庫 URL> /home/ubuntu/my-npoi-project

# 範例：建置專案
# cd /home/ubuntu/my-npoi-project # 導覽到您的專案目錄
# dotnet build

echo "EC2 環境設置完成！"