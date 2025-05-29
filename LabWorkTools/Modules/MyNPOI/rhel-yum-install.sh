#!/bin/bash

sudo yum update -y


sudo yum install -y \
    gcc \
    glibc-devel \
    libicu-devel \
    wget \
    zip \
    unzip

sudo wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
sudo chmod +x dotnet-install.sh
sudo ./dotnet-install.sh -c 8.0 -InstallDir /usr/share/dotnet

echo "export DOTNET_ROOT=/usr/share/dotnet" >> ~/.bashrc
echo "export PATH=\$PATH:\$DOTNET_ROOT" >> ~/.bashrc
source ~/.bashrc

sudo yum install -y git
