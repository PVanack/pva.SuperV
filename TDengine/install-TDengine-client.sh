tar -xf TDengine/tdengine-tsdb-oss-client-3.3.8.8-linux-x64.tar.gz
cd tdengine-tsdb-oss-client-3.3.8.8
./install_client.sh
echo "127.0.0.1 buildkitsandbox localhost" >> /etc/hosts
cd ..