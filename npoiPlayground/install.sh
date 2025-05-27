#!/bin/bash


sudo yum update -y


sudo yum install -y \
    gcc \
    glibc-devel \
    libicu-devel \
    wget \
    zip \
    unzip

# 安裝 .NET SDK
# 從 Microsoft 取得最新的 .NET 版本
DOTNET_VERSION=8.0 # 或您需要的版本
sudo wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
sudo chmod +x dotnet-install.sh
sudo ./dotnet-install.sh -c 8.0 -InstallDir /usr/share/dotnet

# 設定環境變數 (為了讓 dotnet 指令可以在任何地方執行)
echo "export DOTNET_ROOT=/usr/share/dotnet" >> ~/.bashrc
echo "export PATH=\$PATH:\$DOTNET_ROOT" >> ~/.bashrc
source ~/.bashrc

# 驗證 .NET 安裝
dotnet --info

# 安裝 Git (使用 yum)
sudo yum install -y git

# 範例：下載您的專案 (假設放在 GitHub)
# 如果您已經將程式碼複製到 EC2 執行個體，則不需要執行此步驟。
# git clone <您的 Git 儲存庫 URL> /home/ec2-user/my-npoi-project

# 範例：建置專案
# cd /home/ec2-user/my-npoi-project # 導覽到您的專案目錄
# dotnet build

echo "EC2 環境設置完成！"



C:\Users\wits\Downloads\hap\may_pk.pem
scp -i "may_pk.pem"
ec2-user@ec2-44-201-205-141.compute-1.amazonaws.com:
/home/ec2-user/copy_to_ec2_npoi/npoiPlayground/Certificates
Student_A_Cert.docx

ssh -i "may_pk.pem" ec2-user@ec2-44-201-205-141.compute-1.amazonaws.com


scp -i "may_pk.pem" ec2-user@ec2-44-201-205-141.compute-1.amazonaws.com:/home/ec2-user/copy_to_ec2_npoi/npoiPlayground/Certificates/Student_A_Cert.docx C:\Users\wits\Downloads\hap\