apt update &&
apt install -y curl &&
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl" &&
apt update &&
install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl &&
alias k=kubectl
