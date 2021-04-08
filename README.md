# GpsTerminalEmulator

基于部标协议的终端模拟器的服务端

特色功能：根据基准点、速度重新插值计算出更为真实的设备定位数据，最大化数据模拟的真实性。

<div align="center">
      <img src="./img/1.png">
</div>

前端代码详见[TerminalEmulatorWeb](https://github.com/yedajiang44/TerminalEmulatorWeb)

## 部署使用

为了方便大家使用，已提供 docker 镜像，

> docker 安装请参照官方文档

- `touch docker-compose.yml`

- 复制[docker-compose.yml](./docker-compose.yml)内容至刚才新建的`docker-compose.yml`文件中

- 执行`docker-compose up -d`

- 浏览器打开`localhost:4000`

## 已知问题

- efcore 部分使用跟踪查询导致查询速度较慢

- 定位中无方向数据

- 模拟时未进行注册鉴权

## Rodmap

- 修复已知问题

- 添加压测功能

## License

[Apache License 2.0](LICENSE)
