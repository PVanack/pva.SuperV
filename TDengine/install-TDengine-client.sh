tar -xf TDengine/tdengine-tsdb-oss-client-3.4.0.2-linux-x64.tar.gz
cd tdengine-tsdb-oss-client-3.4.0.2
./install_client.sh
echo "127.0.0.1 buildkitsandbox localhost" >> /etc/hosts
cd ..